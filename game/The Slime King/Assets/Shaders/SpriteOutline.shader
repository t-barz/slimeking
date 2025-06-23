Shader "Slime/SpriteOutline"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _OutlineColor ("Outline Color", Color) = (1,1,1,1)
        _OutlineThickness ("Outline Thickness", Range(0,4)) = 1
    }
    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
        }
        
        Cull Off
        Lighting Off
        ZWrite Off
        Blend One OneMinusSrcAlpha
        
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
                fixed4 color : COLOR;
            };
            
            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
            };
            
            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _OutlineColor;
            float _OutlineThickness;
            float4 _MainTex_TexelSize;
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color;
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv) * i.color;
                
                // Amostra os texels adjacentes para criar o contorno
                fixed4 outline = fixed4(0, 0, 0, 0);
                
                if (_OutlineThickness > 0) 
                {
                    // Amostra texels ao redor para criar o efeito de contorno
                    float2 texelSize = _OutlineThickness * _MainTex_TexelSize.xy;
                    
                    // Soma amostras em 8 direções para verificar bordas
                    outline += tex2D(_MainTex, i.uv + float2(-texelSize.x, 0));
                    outline += tex2D(_MainTex, i.uv + float2(texelSize.x, 0));
                    outline += tex2D(_MainTex, i.uv + float2(0, -texelSize.y));
                    outline += tex2D(_MainTex, i.uv + float2(0, texelSize.y));
                    outline += tex2D(_MainTex, i.uv + float2(-texelSize.x, -texelSize.y));
                    outline += tex2D(_MainTex, i.uv + float2(texelSize.x, -texelSize.y));
                    outline += tex2D(_MainTex, i.uv + float2(-texelSize.x, texelSize.y));
                    outline += tex2D(_MainTex, i.uv + float2(texelSize.x, texelSize.y));
                    
                    // Normaliza o resultado
                    outline.a = saturate(outline.a);
                    
                    // Remove áreas já cobertas pela textura original
                    outline.a = saturate(outline.a - col.a);
                }
                
                // Combina o contorno com a textura original
                col.rgb = col.rgb * col.a + _OutlineColor.rgb * outline.a * _OutlineColor.a;
                col.a = saturate(col.a + outline.a * _OutlineColor.a);
                
                return col;
            }
            ENDCG
        }
    }
}
