Shader "SlimeMec/SpriteOutline"
{
    // PIXEL ART Hard Edge Outline - v5.0
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _OutlineColor ("Outline Color", Color) = (1,1,1,1)
        _OutlineSize ("Outline Size", Range(0, 0.5)) = 0.01
        [MaterialToggle] _EnableOutline ("Enable Outline", Float) = 0
        [HideInInspector] _RendererColor ("RendererColor", Color) = (1,1,1,1)
        [HideInInspector] _Flip ("Flip", Vector) = (1,1,1,1)
        [PerRendererData] _AlphaTex ("External Alpha", 2D) = "white" {}
        [PerRendererData] _EnableExternalAlpha ("Enable External Alpha", Float) = 0
        [HideInInspector] _ForceUpdate ("Force Update", Float) = 1.0
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend One OneMinusSrcAlpha

        Pass
        {
            Name "SpriteOutline"
            
            CGPROGRAM
            #pragma vertex SpriteVert
            #pragma fragment SpriteFragWithOutline
            #pragma target 2.0
            #pragma multi_compile_instancing
            #pragma multi_compile_local _ PIXELSNAP_ON
            #pragma multi_compile _ ETC1_EXTERNAL_ALPHA
            
            #include "UnityCG.cginc"

            #ifdef UNITY_INSTANCING_ENABLED
                UNITY_INSTANCING_BUFFER_START(PerDrawSprite)
                    UNITY_DEFINE_INSTANCED_PROP(fixed4, unity_SpriteRendererColorArray)
                    UNITY_DEFINE_INSTANCED_PROP(fixed4, unity_SpriteFlipArray)
                UNITY_INSTANCING_BUFFER_END(PerDrawSprite)

                #define _RendererColor  UNITY_ACCESS_INSTANCED_PROP(PerDrawSprite, unity_SpriteRendererColorArray)
                #define _Flip           UNITY_ACCESS_INSTANCED_PROP(PerDrawSprite, unity_SpriteFlipArray)

            #endif

            CBUFFER_START(UnityPerDrawSprite)
            #ifndef UNITY_INSTANCING_ENABLED
                fixed4 _RendererColor;
                fixed4 _Flip;
            #endif
                float _EnableExternalAlpha;
            CBUFFER_END

            sampler2D _MainTex;
            sampler2D _AlphaTex;
            fixed4 _Color;
            fixed4 _OutlineColor;
            float _OutlineSize;
            float _EnableOutline;
            float4 _MainTex_TexelSize;

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            inline float4 UnityFlipSprite(in float3 pos, in fixed2 flip)
            {
                return float4(pos.xy * flip, pos.z, 1.0);
            }

            v2f SpriteVert(appdata_t IN)
            {
                v2f OUT;

                UNITY_SETUP_INSTANCE_ID(IN);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);

                OUT.vertex = UnityFlipSprite(IN.vertex, _Flip);
                OUT.vertex = UnityObjectToClipPos(OUT.vertex);
                OUT.texcoord = IN.texcoord;
                OUT.color = IN.color * _Color * _RendererColor;

                #ifdef PIXELSNAP_ON
                OUT.vertex = UnityPixelSnap(OUT.vertex);
                #endif

                return OUT;
            }

            fixed4 SampleSpriteTexture(float2 uv)
            {
                fixed4 color = tex2D(_MainTex, uv);

                #if ETC1_EXTERNAL_ALPHA
                fixed4 alpha = tex2D(_AlphaTex, uv);
                color.a = lerp(color.a, alpha.r, _EnableExternalAlpha);
                #endif

                return color;
            }

            fixed4 SpriteFragWithOutline(v2f IN) : SV_Target
            {
                fixed4 c = SampleSpriteTexture(IN.texcoord) * IN.color;
                
                if (_EnableOutline < 0.5)
                {
                    c.rgb *= c.a;
                    return c;
                }
                
                float currentAlpha = c.a;
                
                // Se o pixel atual já tem conteúdo, retorna ele normalmente
                if (currentAlpha > 0.01)
                {
                    c.rgb *= c.a;
                    return c;
                }
                
                // PIXEL ART OUTLINE: Hard edges, sem suavização
                float2 texelSize = _MainTex_TexelSize.xy;
                int outlinePixels = max(1, (int)(_OutlineSize * 50.0));
                
                // Verifica se há conteúdo nas 4 direções cardinais
                bool hasOutline = false;
                
                // Para cada pixel de distância até o limite do outline
                [unroll]
                for (int dist = 1; dist <= 25; dist++)
                {
                    if (dist > outlinePixels) break;
                    
                    float2 offset = texelSize * float(dist);
                    
                    // Testa 4 direções cardinais (pixel art style)
                    float rightAlpha = SampleSpriteTexture(IN.texcoord + float2(offset.x, 0)).a;
                    float leftAlpha = SampleSpriteTexture(IN.texcoord + float2(-offset.x, 0)).a;
                    float upAlpha = SampleSpriteTexture(IN.texcoord + float2(0, offset.y)).a;
                    float downAlpha = SampleSpriteTexture(IN.texcoord + float2(0, -offset.y)).a;
                    
                    // Se encontrou qualquer conteúdo, marca para outline
                    if (rightAlpha > 0.01 || leftAlpha > 0.01 || upAlpha > 0.01 || downAlpha > 0.01)
                    {
                        hasOutline = true;
                        break;
                    }
                }
                
                // Outline binário: ou tem ou não tem (pixel art style)
                if (hasOutline)
                {
                    c = _OutlineColor;
                    c.a = 1.0; // Alpha fixo para bordas nítidas
                }
                else
                {
                    c = fixed4(0,0,0,0);
                }
                
                c.rgb *= c.a;
                return c;
            }
            ENDCG
        }
    }
    
    Fallback "Sprites/Default"
}