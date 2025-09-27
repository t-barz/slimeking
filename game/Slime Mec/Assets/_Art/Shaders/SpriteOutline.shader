Shader "SlimeMec/SpriteOutline"
{
    // Optimized Sprite Outline - Based on Efficient 8-Direction Algorithm - v6.0
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _OutlineColor ("Outline Color", Color) = (1,1,1,1)
        _OutlineSize ("Outline Size", Range(0, 1.0)) = 0.03
        _AlphaThreshold ("Alpha Threshold", Range(0, 1)) = 0.5
        [MaterialToggle] _ShowOutline ("Show Outline", Float) = 0
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
            float _AlphaThreshold;
            float _ShowOutline;
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

            fixed4 SampleSpriteTextureWithBorder(float2 uv)
            {
                // Verifica se UV está dentro dos limites da textura
                if (uv.x < 0.0 || uv.x > 1.0 || uv.y < 0.0 || uv.y > 1.0)
                {
                    // Fora dos limites = considera como transparente
                    return fixed4(0,0,0,0);
                }
                
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
                
                // Check both _ShowOutline and _EnableOutline for compatibility
                if (_ShowOutline < 0.5 && _EnableOutline < 0.5)
                {
                    c.rgb *= c.a;
                    return c;
                }
                
                float centerAlpha = c.a;
                
                // If current pixel is already solid, return it normally
                if (centerAlpha >= _AlphaThreshold)
                {
                    c.rgb *= c.a;
                    return c;
                }
                
                // Efficient 8-direction outline algorithm
                float2 texelSize = _MainTex_TexelSize.xy;
                float2 pixelSize = texelSize * _OutlineSize;
                
                // 8-direction offsets for smooth outline
                float2 offsets[8] = {
                    float2(-1, 0),  // Left
                    float2(1, 0),   // Right
                    float2(0, 1),   // Up
                    float2(0, -1),  // Down
                    float2(-1, 1),  // Top-left
                    float2(1, 1),   // Top-right
                    float2(-1, -1), // Bottom-left
                    float2(1, -1)   // Bottom-right
                };
                
                float outline = 0;
                
                // Check all 8 directions
                for (int j = 0; j < 8; j++)
                {
                    float2 sampleUV = IN.texcoord + offsets[j] * pixelSize;
                    float sampleAlpha = SampleSpriteTextureWithBorder(sampleUV).a;
                    outline += step(_AlphaThreshold, sampleAlpha);
                }
                
                // If outline detected, apply outline color
                if (outline > 0)
                {
                    c = _OutlineColor;
                    c.a = _OutlineColor.a;
                }
                else
                {
                    // No outline, return transparent
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