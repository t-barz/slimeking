Shader "SlimeKing/DamageEffectShader"
{
    Properties
    {
        [MainTexture] _MainTex ("Texture", 2D) = "white" {}
        [MainColor] _Color ("Color", Color) = (1,1,1,1)
        _FlashColor ("Flash Color", Color) = (1,0,0,1)
        _FlashIntensity ("Flash Intensity", Range(0, 1)) = 0.5
        _SemiTransparency ("Semi Transparency", Range(0, 0.9)) = 0.3
        _HitTime ("Hit Time", Float) = 0
        _FlashDuration ("Flash Duration", Float) = 0.2
        _TransparencyDuration ("Transparency Duration", Float) = 1.0
    }
    
    SubShader
    {
        Tags 
        { 
            "RenderType" = "Transparent" 
            "Queue" = "Transparent" 
            "RenderPipeline" = "UniversalPipeline"
            "IgnoreProjector" = "True"
        }
        
        LOD 100

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            Name "DamageFlash"
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            // Make fog and various mobile platform options work
            #pragma multi_compile_fog
            #pragma multi_compile_instancing
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            
            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };
            
            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
                float fogCoord : TEXCOORD1;
                UNITY_VERTEX_OUTPUT_STEREO
            };
            
            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            
            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float4 _Color;
                float4 _FlashColor;
                float _FlashIntensity;
                float _SemiTransparency;
                float _HitTime;
                float _FlashDuration;
                float _TransparencyDuration;
            CBUFFER_END
            
            Varyings vert(Attributes IN)
            {
                Varyings OUT = (Varyings)0;
                UNITY_SETUP_INSTANCE_ID(IN);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
                
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);
                OUT.color = IN.color;
                OUT.fogCoord = ComputeFogFactor(OUT.positionHCS.z);
                
                return OUT;
            }
            
            half4 frag(Varyings IN) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);
                
                // Sample texture and apply base color
                half4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv) * _Color * IN.color;
                
                // Calculate time since hit
                float timeSinceHit = _Time.y - _HitTime;
                
                // Calculate flash effect (stronger at start, fading out)
                float flashFactor = 0;
                if (timeSinceHit >= 0 && timeSinceHit < _FlashDuration) {
                    // Smooth fade out for the flash
                    flashFactor = (1.0 - (timeSinceHit / _FlashDuration)) * _FlashIntensity;
                }
                
                // Calculate transparency effect
                float alpha = color.a;
                if (timeSinceHit >= _FlashDuration && timeSinceHit < (_FlashDuration + _TransparencyDuration)) {
                    // Semi-transparent phase
                    float transparencyProgress = (timeSinceHit - _FlashDuration) / _TransparencyDuration;
                    // Smooth interpolation from semi-transparent back to full alpha
                    alpha = lerp(color.a * (1.0 - _SemiTransparency), color.a, transparencyProgress);
                }
                
                // Combine flash and original color
                half3 finalColor = lerp(color.rgb, _FlashColor.rgb, flashFactor);
                
                // Apply fog
                finalColor = MixFog(finalColor, IN.fogCoord);
                
                return half4(finalColor, alpha);
            }
            ENDHLSL
        }
    }
    
    // Fallback for older render pipelines
    FallBack "Universal Render Pipeline/2D/Sprite-Lit-Default"
    
    CustomEditor "UnityEditor.Rendering.Universal.ShaderGUI.DamageEffectShaderGUI"
}
