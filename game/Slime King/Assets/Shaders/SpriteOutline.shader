Shader "Custom/SpriteOutline"
{
    Properties
    {
        _MainTex ("Textura Principal", 2D) = "white" {}
        _OutlineColor ("Cor do Contorno", Color) = (1,1,1,1)
        _OutlineThickness ("Espessura do Contorno", Range(0, 0.1)) = 0.01
        _AlphaThreshold ("Limite de Transparência", Range(0, 1)) = 0.1
    }

    SubShader
    {
        Tags 
        { 
            "Queue"="Transparent" 
            "RenderType"="Transparent" 
        }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _OutlineColor;
            float _OutlineThickness;
            float _AlphaThreshold;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Amostras nas 8 direções ao redor do pixel
                float2 offsets[8] = {
                    float2(-1,0), float2(1,0),
                    float2(0,1), float2(0,-1),
                    float2(-1,1), float2(1,1),
                    float2(-1,-1), float2(1,-1)
                };

                float alpha = tex2D(_MainTex, i.uv).a;
                float outline = 0;
                
                // Verifica pixels vizinhos para detectar bordas
                for(int j = 0; j < 8; j++)
                {
                    float2 uv = i.uv + offsets[j] * _OutlineThickness;
                    outline += step(_AlphaThreshold, tex2D(_MainTex, uv).a);
                }

                outline = saturate(outline * (1 - alpha));
                fixed4 col = tex2D(_MainTex, i.uv);
                
                // Mistura cor original com contorno
                return lerp(col, _OutlineColor, outline);
            }
            ENDCG
        }
    }
}
