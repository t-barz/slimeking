Shader "Custom/Builtin_SpriteOutline"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _OutlineColor ("Outline Color", Color) = (1,1,1,1)
        _OutlineSize ("Outline Size", Range(0, 0.1)) = 0.03
        _AlphaThreshold ("Alpha Threshold", Range(0, 1)) = 0.5
    }
    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "RenderType"="Transparent"
            "IgnoreProjector"="True"
        }
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        Lighting Off
        ZWrite Off
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
            float _OutlineSize;
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
                // Amostra central
                float4 col = tex2D(_MainTex, i.uv);
                float centerAlpha = col.a;

                // Tamanho do pixel em UV (aproximação)
                float2 pixelSize = float2(_OutlineSize, _OutlineSize);

                // Amostras em 8 direções
                float outline = 0;
                float2 offsets[8] = {
                    float2(-1, 0), float2(1, 0),
                    float2(0, 1), float2(0, -1),
                    float2(-1, 1), float2(1, 1),
                    float2(-1, -1), float2(1, -1)
                };

                for (int j = 0; j < 8; j++)
                {
                    float2 sampleUV = i.uv + offsets[j] * pixelSize;
                    float sampleAlpha = tex2D(_MainTex, sampleUV).a;
                    outline += step(_AlphaThreshold, sampleAlpha);
                }

                // Se o pixel central for transparente e algum vizinho for opaco, desenha o contorno
                float isOutline = (centerAlpha < _AlphaThreshold && outline > 0) ? 1.0 : 0.0;

                // Pixels internos mantêm a cor original, pixels de contorno recebem a cor do outline
                float3 finalColor = lerp(col.rgb, _OutlineColor.rgb, isOutline);
                float finalAlpha = max(col.a, isOutline * _OutlineColor.a);

                return float4(finalColor, finalAlpha);
            }
            ENDCG
        }
    }
}
