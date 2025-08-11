Shader "SlimeKing/2D Water Effect"
{
    Properties
    {
        [MainTexture] _MainTex ("Texture", 2D) = "white" {}
        _WaterColor ("Water Color", Color) = (0.2, 0.5, 0.8, 0.7)
        _DetectionColor ("Detection Color (RGB)", Color) = (0, 0, 1, 1)
        _ColorTolerance ("Color Detection Tolerance", Range(0, 1)) = 0.2
        _WaterTransparency ("Water Transparency", Range(0, 1)) = 0.7
        
        [Header(Wave Settings)]
        _WaveSpeed ("Wave Speed", Range(0, 10)) = 2
        _WaveFrequency ("Wave Frequency", Range(0, 50)) = 10
        _WaveAmplitude ("Wave Amplitude", Range(0, 0.1)) = 0.01
        
        [Header(Reflection and Lighting)]
        _ReflectionIntensity ("Reflection Intensity", Range(0, 1)) = 0.5
        _Glossiness ("Glossiness", Range(0, 1)) = 0.7
        _RimPower ("Rim Light Power", Range(0, 10)) = 3
        _RimColor ("Rim Color", Color) = (1, 1, 1, 0.5)
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
        
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off
        
        Pass
        {
            Name "WaterEffect"
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            // For mobile and various platform support
            #pragma multi_compile_fog
            #pragma multi_compile_instancing
            
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
                float2 screenPos : TEXCOORD1;
                float fogCoord : TEXCOORD2;
                UNITY_VERTEX_OUTPUT_STEREO
            };
            
            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            
            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float4 _WaterColor;
                float4 _DetectionColor;
                float _ColorTolerance;
                float _WaterTransparency;
                float _WaveSpeed;
                float _WaveFrequency;
                float _WaveAmplitude;
                float _ReflectionIntensity;
                float _Glossiness;
                float _RimPower;
                float4 _RimColor;
            CBUFFER_END
            
            // Wave calculation function
            float2 CalculateWaveOffset(float2 uv, float time)
            {
                // Combine horizontal and vertical waves with different frequencies
                float2 offset;
                offset.x = sin(time * _WaveSpeed + uv.x * _WaveFrequency) * _WaveAmplitude;
                offset.y = sin(time * _WaveSpeed * 0.8 + uv.y * _WaveFrequency * 1.2) * _WaveAmplitude;
                
                return offset;
            }
            
            Varyings vert(Attributes IN)
            {
                Varyings OUT = (Varyings)0;
                UNITY_SETUP_INSTANCE_ID(IN);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
                
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);
                OUT.color = IN.color;
                OUT.screenPos = ComputeScreenPos(OUT.positionHCS).xy;
                OUT.fogCoord = ComputeFogFactor(OUT.positionHCS.z);
                
                return OUT;
            }
            
            half4 frag(Varyings IN) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);
                
                // Calculate wave distortion
                float2 waveOffset = CalculateWaveOffset(IN.screenPos, _Time.y);
                float2 distortedUV = IN.uv + waveOffset;
                
                // Sample the texture with the distorted UVs
                half4 texColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, distortedUV) * IN.color;
                
                // Calculate color difference from detection color
                float3 colorDiff = abs(texColor.rgb - _DetectionColor.rgb);
                float colorDist = length(colorDiff);
                
                // Check if the sampled color is close to our detection color
                float isWater = step(colorDist, _ColorTolerance);
                
                // Create water effect only for matching pixels
                half4 waterPixel = _WaterColor;
                
                // Add reflection effect - create a simple gradient based on UV
                float reflectionMask = pow(abs(sin(IN.screenPos.y * 8 + _Time.y * 1.5)), _RimPower) * _ReflectionIntensity;
                waterPixel.rgb += reflectionMask * _RimColor.rgb * _RimColor.a;
                
                // Add a glossy highlight
                float glossMask = pow(abs(cos(IN.screenPos.y * 10 + _Time.y * 2)), 8) * _Glossiness;
                waterPixel.rgb += glossMask;
                
                // Mix between original texture and water effect based on detection
                half4 finalColor = lerp(texColor, waterPixel, isWater);
                
                // Apply transparency only to water pixels
                float finalAlpha = lerp(texColor.a, _WaterColor.a * _WaterTransparency, isWater);
                
                // Apply fog
                finalColor.rgb = MixFog(finalColor.rgb, IN.fogCoord);
                
                return half4(finalColor.rgb, finalAlpha);
            }
            ENDHLSL
        }
    }
    
    Fallback "Universal Render Pipeline/2D/Sprite-Lit-Default"
    CustomEditor "UnityEditor.Rendering.Universal.ShaderGUI.WaterEffectShaderGUI"
}
