Shader "MienTay/StylizedRiverWater"
{
    Properties
    {
        [Header(Colors)]
        _DeepColor("Deep Water Color", Color) = (0.1, 0.3, 0.5, 1)
        _ShallowColor("Shallow Water Color", Color) = (0.3, 0.7, 0.6, 1)
        _ColorRange("Depth Range", Range(0.1, 10.0)) = 2.0
        
        [Header(Waves)]
        _WaveHeight("Wave Height", Range(0, 1)) = 0.2
        _WaveSpeed("Wave Speed", Range(0, 5)) = 1.0
        _WaveFrequency("Wave Frequency", Range(0, 5)) = 1.0
        
        [Header(Foam and Wake)]
        _FoamTex("Foam Texture", 2D) = "white" {}
        _FoamColor("Foam Color", Color) = (1, 1, 1, 1)
        _FoamAmount("Foam Amount", Range(0, 1)) = 0.5
        _FoamSpeed("Foam Speed", Vector) = (0.1, 0.1, 0, 0)
        _WakeFoamIntensity("Wake Foam Intensity", Range(0, 5)) = 2.0
        
        [Header(Ripples)]
        _NormalMap("Normal Map", 2D) = "bump" {}
        _NormalStrength("Normal Strength", Range(0, 1)) = 0.5
        _RippleSpeed("Ripple Speed", Vector) = (0.05, 0.05, 0, 0)
        
        [Header(Shoreline)]
        _ShoreFade("Shore Fade Distance", Range(0.01, 1.0)) = 0.1
        
        [Header(Mobile Optimization)]
        [Toggle(_USE_DEPTH_FADE)] _UseDepthFade("Use Depth Fade (Expensive)", Float) = 1.0
    }

    SubShader
    {
        Tags { "RenderType" = "Transparent" "Queue" = "Transparent" "RenderPipeline" = "UniversalPipeline" }
        LOD 100

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }

            ZWrite On
            Blend SrcAlpha OneMinusSrcAlpha

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog
            #pragma shader_feature _USE_DEPTH_FADE

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                float3 normalOS : NORMAL;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 screenPos : TEXCOORD1;
                float3 normalWS : TEXCOORD3;
                float3 positionWS : TEXCOORD4;
                float wakeIntensity : TEXCOORD5;
            };

            CBUFFER_START(UnityPerMaterial)
                float4 _DeepColor;
                float4 _ShallowColor;
                float _ColorRange;
                float4 _FoamColor;
                float _FoamAmount;
                float2 _FoamSpeed;
                float2 _RippleSpeed;
                float _NormalStrength;
                float _ShoreFade;
                float _WaveHeight;
                float _WaveSpeed;
                float _WaveFrequency;
                float _WakeFoamIntensity;
                float4 _NormalMap_ST;
                float4 _FoamTex_ST;
            CBUFFER_END

            TEXTURE2D(_NormalMap); SAMPLER(sampler_NormalMap);
            TEXTURE2D(_FoamTex);   SAMPLER(sampler_FoamTex);
            TEXTURE2D(_WakeMap);   SAMPLER(sampler_WakeMap);

            float4 _WakeData; // x,y = pos, z = worldSize

            Varyings vert(Attributes input)
            {
                Varyings output = (Varyings)0;
                float3 worldPos = TransformObjectToWorld(input.positionOS.xyz);
                
                // Wake lookup
                float2 wakeUV = (worldPos.xz - _WakeData.xy) / _WakeData.z + 0.5;
                float wakeSample = SAMPLE_TEXTURE2D_LOD(_WakeMap, sampler_WakeMap, wakeUV, 0).r;
                output.wakeIntensity = wakeSample;

                // Vertex waves
                float wave = sin(_Time.y * _WaveSpeed + worldPos.x * _WaveFrequency + worldPos.z * _WaveFrequency) * _WaveHeight;
                worldPos.y += wave;
                
                // Add wake displacement
                worldPos.y += wakeSample * 0.5;

                output.positionWS = worldPos;
                output.positionCS = TransformWorldToHClip(worldPos);
                output.uv = input.uv;
                output.screenPos = ComputeScreenPos(output.positionCS);
                output.normalWS = TransformObjectToWorldNormal(input.normalOS);
                
                return output;
            }

            float4 frag(Varyings input) : SV_Target
            {
                float2 rippleUV1 = input.uv * _NormalMap_ST.xy + _NormalMap_ST.zw + _Time.y * _RippleSpeed;
                float2 rippleUV2 = input.uv * _NormalMap_ST.xy * 0.5 + _Time.y * _RippleSpeed * -0.7;
                
                float3 normal1 = UnpackNormal(SAMPLE_TEXTURE2D(_NormalMap, sampler_NormalMap, rippleUV1));
                float3 normal2 = UnpackNormal(SAMPLE_TEXTURE2D(_NormalMap, sampler_NormalMap, rippleUV2));
                float3 blendedNormal = normalize(normal1 + normal2 + input.normalWS);
                
                float4 finalColor = _DeepColor;
                float alpha = 1.0;

                #if defined(_USE_DEPTH_FADE)
                    float2 screenUV = input.screenPos.xy / input.screenPos.w;
                    float depth = SampleSceneDepth(screenUV);
                    float linearDepth = LinearEyeDepth(depth, _ZBufferParams);
                    float screenDepth = input.screenPos.w;
                    float depthDiff = linearDepth - screenDepth;
                    
                    finalColor = lerp(_ShallowColor, _DeepColor, saturate(depthDiff / _ColorRange));
                    alpha = saturate(depthDiff / _ShoreFade);
                #endif

                // Foam Logic
                float2 foamUV = input.uv * _FoamTex_ST.xy + _Time.y * _FoamSpeed;
                float foamSample = SAMPLE_TEXTURE2D(_FoamTex, sampler_FoamTex, foamUV).r;
                
                // Base foam (edges)
                float foamMask = 0;
                #if defined(_USE_DEPTH_FADE)
                    float localDepthDiff = linearDepth - screenDepth;
                    foamMask = saturate(1.0 - (localDepthDiff / (_FoamAmount * 2.0)));
                #endif
                
                // Add Wake foam
                foamMask = saturate(foamMask + input.wakeIntensity * _WakeFoamIntensity);
                
                finalColor.rgb = lerp(finalColor.rgb, _FoamColor.rgb, foamMask * foamSample);

                // Lighting
                Light mainLight = GetMainLight();
                float3 viewDir = normalize(GetCameraPositionWS() - input.positionWS);
                float3 halfDir = normalize(mainLight.direction + viewDir);
                float spec = pow(saturate(dot(blendedNormal, halfDir)), 64.0);
                
                finalColor.rgb += spec * mainLight.color * 0.8;

                return float4(finalColor.rgb, alpha * _DeepColor.a);
            }
            ENDHLSL
}
    }
}
