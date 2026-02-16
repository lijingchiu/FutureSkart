Shader "OutSouring_Effect/LitPlusFresnelOutline"
{
    Properties
    {
        // ==== Lit Standard Properties ====
        [Header(Surface Options)]
        [Enum(UnityEngine.Rendering.CullMode)] _Cull ("Cull Mode", Float) = 2
        [Enum(Off, 0, On, 1)] _ZWrite ("ZWrite", Float) = 1
        [Enum(UnityEngine.Rendering.CompareFunction)] _ZTest ("ZTest", Float) = 4
        
        [Header(Surface Inputs)]
        [MainTexture] _BaseMap ("Albedo", 2D) = "white" {}
        [MainColor] _BaseColor ("Color", Color) = (1, 1, 1, 1)
        
        [Space(10)]
        [Toggle(_METALLICSPECGLOSSMAP)] _UseMetallicMap ("Use Metallic Map", Float) = 0
        _MetallicGlossMap ("Metallic Map", 2D) = "white" {}
        _Metallic ("Metallic", Range(0.0, 1.0)) = 0.0
        _Smoothness ("Smoothness", Range(0.0, 1.0)) = 0.5
        
        [Space(10)]
        [Toggle(_NORMALMAP)] _UseNormalMap ("Use Normal Map", Float) = 0
        [Normal] _BumpMap ("Normal Map", 2D) = "bump" {}
        _BumpScale ("Normal Scale", Float) = 1.0
        
        [Space(10)]
        [Toggle(_EMISSION)] _UseEmission ("Use Emission", Float) = 0
        [HDR] _EmissionColor ("Emission Color", Color) = (0, 0, 0, 1)
        _EmissionMap ("Emission Map", 2D) = "white" {}
        
        // ==== Tessellation ====
        [Header(Tessellation)]
        [Toggle(_TESSELLATION_ON)] _EnableTessellation ("Enable Tessellation", Float) = 0
        _TessellationFactor ("Tessellation Factor", Range(1, 64)) = 16
        _TessellationMinDistance ("Min Distance", Float) = 5
        _TessellationMaxDistance ("Max Distance", Float) = 20
        
        // ==== Custom Fresnel Effect ====
        [Header(Fresnel Effect)]
        [Toggle(_FRESNEL_ON)] _EnableFresnel ("Enable Fresnel", Float) = 0
        [HDR] _FresnelColor ("Fresnel Color", Color) = (0, 0.5, 1, 1)
        _FresnelPower ("Fresnel Power", Range(0.1, 10)) = 3
        _FresnelStrength ("Fresnel Strength", Range(0, 5)) = 1
        _FresnelIntensity ("Fresnel Intensity", Range(0, 1)) = 1
        
        // ==== Custom Outline Effect ====
        [Header(Outline Effect)]
        [Toggle(_OUTLINE_ON)] _EnableOutline ("Enable Outline", Float) = 0
        [HDR] _OutlineColor ("Outline Color", Color) = (0, 0, 0, 1)
        _OutlineWidth ("Outline Width", Range(0, 0.1)) = 0.01
        _OutlineIntensity ("Outline Intensity", Range(0, 1)) = 1
        
        // ==== Freeze Effect ====
        [Header(Freeze Effect)]
        [Toggle(_FREEZE_ON)] _EnableFreeze ("Enable Freeze Effect", Float) = 0
        _FreezeAmount ("Freeze Amount", Range(0, 1)) = 0
        _FreezeTexture ("Freeze Texture", 2D) = "white" {}
        _FreezeColor ("Freeze Color", Color) = (0.7, 0.9, 1, 1)
        
        [Space(5)]
        _IcicleNoiseScale ("Icicle Noise Scale", Range(1, 50)) = 10
        _IcicleNoisePower ("Icicle Noise Power", Range(0.1, 5)) = 2
        _IcicleLength ("Icicle Length", Range(0, 1)) = 0.5
        
        [Space(5)]
        _FreezeRange ("Freeze Texture Range", Range(0.1, 10)) = 3
        _FreezeSmoothness ("Freeze Smoothness", Range(0, 1)) = 0.9
        _FreezeSmoothnessRemap ("Freeze Smoothness Remap", Vector) = (0.6, 1, 0, 0)
        
        [Space(5)]
        _FreezeNormalStrength ("Freeze Normal Strength", Range(0, 2)) = 0.5
        _FreezeNormalContrast ("Freeze Normal Contrast", Range(1, 5)) = 2
        
        [Space(5)]
        [HDR] _FreezeEmissionColor ("Freeze Emission Color", Color) = (0.5, 0.8, 1, 1)
        _FreezeFresnelPower ("Freeze Fresnel Power", Range(0.1, 10)) = 2
        _FreezeEmissionStrength ("Freeze Emission Strength", Range(0, 1)) = 0.3
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline" = "UniversalPipeline"
            "RenderType" = "Opaque"
            "Queue" = "Geometry"
        }

        // 1. Outline Pass
        Pass
        {
            Name "Outline"
            Tags { "LightMode" = "SRPDefaultUnlit" }
            Cull Front
            ZWrite On

            HLSLPROGRAM
            #pragma vertex OutlineVert
            #pragma fragment OutlineFrag
            #pragma shader_feature_local _OUTLINE_ON
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            CBUFFER_START(UnityPerMaterial)
                float4 _OutlineColor;
                float _OutlineWidth;
                float _OutlineIntensity;
            CBUFFER_END

            struct Attributes 
            { 
                float4 positionOS : POSITION; 
                float3 normalOS : NORMAL; 
            };
            
            struct Varyings 
            { 
                float4 positionCS : SV_POSITION; 
            };

            Varyings OutlineVert(Attributes input)
            {
                Varyings output;
                #ifdef _OUTLINE_ON
                    float3 posWS = TransformObjectToWorld(input.positionOS.xyz + input.normalOS * _OutlineWidth);
                    output.positionCS = TransformWorldToHClip(posWS);
                #else
                    output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                #endif
                return output;
            }

            half4 OutlineFrag(Varyings input) : SV_Target
            {
                #ifdef _OUTLINE_ON
                    return half4(_OutlineColor.rgb, _OutlineIntensity);
                #else
                    discard; 
                    return 0;
                #endif
            }
            ENDHLSL
        }

        // 2. Forward Lit Pass (with Tessellation)
        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }
            Cull [_Cull]
            ZWrite [_ZWrite]
            Blend One Zero

            HLSLPROGRAM
            #pragma target 4.6
            #pragma require tessellation tessHW
            
            // Tessellation Shaders
            #pragma vertex TessellationVertex
            #pragma hull HullProgram
            #pragma domain DomainProgram
            #pragma fragment LitPassFragment

            // Shader Features
            #pragma shader_feature_local _NORMALMAP
            #pragma shader_feature_local _METALLICSPECGLOSSMAP
            #pragma shader_feature_local _EMISSION
            #pragma shader_feature_local _FRESNEL_ON
            #pragma shader_feature_local _FREEZE_ON
            #pragma shader_feature_local _TESSELLATION_ON

            // URP Keywords
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile_fragment _ _SHADOWS_SOFT

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            
            // 修正 LerpWhiteTo Bug
            #ifndef LERPWHITETO_DEFINED
            #define LERPWHITETO_DEFINED
            half LerpWhiteTo(half b, half t) 
            { 
                return half(1.0) + t * (b - half(1.0)); 
            }
            #endif
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseMap_ST;
                float4 _BaseColor;
                float _Metallic;
                float _Smoothness;
                float _BumpScale;
                float4 _EmissionColor;
                
                // Tessellation
                float _TessellationFactor;
                float _TessellationMinDistance;
                float _TessellationMaxDistance;
                
                // Fresnel
                float4 _FresnelColor;
                float _FresnelPower;
                float _FresnelStrength;
                float _FresnelIntensity;
                
                // Freeze
                float _FreezeAmount;
                float4 _FreezeTexture_ST;
                float4 _FreezeColor;
                float _IcicleNoiseScale;
                float _IcicleNoisePower;
                float _IcicleLength;
                float _FreezeRange;
                float _FreezeSmoothness;
                float4 _FreezeSmoothnessRemap;
                float _FreezeNormalStrength;
                float _FreezeNormalContrast;
                float4 _FreezeEmissionColor;
                float _FreezeFresnelPower;
                float _FreezeEmissionStrength;
            CBUFFER_END

            TEXTURE2D(_BaseMap); SAMPLER(sampler_BaseMap);
            TEXTURE2D(_MetallicGlossMap); SAMPLER(sampler_MetallicGlossMap);
            TEXTURE2D(_BumpMap); SAMPLER(sampler_BumpMap);
            TEXTURE2D(_EmissionMap); SAMPLER(sampler_EmissionMap);
            TEXTURE2D(_FreezeTexture); SAMPLER(sampler_FreezeTexture);

            struct Attributes 
            { 
                float4 positionOS : POSITION; 
                float3 normalOS : NORMAL; 
                float4 tangentOS : TANGENT; 
                float2 uv : TEXCOORD0; 
            };
            
            struct TessellationControlPoint
            {
                float4 positionOS : INTERNALTESSPOS;
                float3 normalOS : NORMAL;
                float4 tangentOS : TANGENT;
                float2 uv : TEXCOORD0;
            };
            
            struct Varyings 
            { 
                float4 positionCS : SV_POSITION; 
                float3 positionWS : TEXCOORD0; 
                float3 normalWS : TEXCOORD1; 
                float3 tangentWS : TEXCOORD2;
                float3 bitangentWS : TEXCOORD3;
                float2 uv : TEXCOORD4; 
                float3 viewDirWS : TEXCOORD5;
                float3 positionOS : TEXCOORD6;
                float3 normalOS : TEXCOORD7;
            };

            // ==================== Gradient Noise 函數 ====================
            float2 GradientNoiseDir(float2 p)
            {
                p = fmod(p, 289);
                float x = fmod((34 * p.x + 1) * p.x, 289) + p.y;
                x = fmod((34 * x + 1) * x, 289);
                x = frac(x / 41) * 2 - 1;
                return normalize(float2(x - floor(x + 0.5), abs(x) - 0.5));
            }

            float GradientNoise(float2 UV, float Scale)
            {
                float2 p = UV * Scale;
                float2 ip = floor(p);
                float2 fp = frac(p);
                float d00 = dot(GradientNoiseDir(ip), fp);
                float d01 = dot(GradientNoiseDir(ip + float2(0, 1)), fp - float2(0, 1));
                float d10 = dot(GradientNoiseDir(ip + float2(1, 0)), fp - float2(1, 0));
                float d11 = dot(GradientNoiseDir(ip + float2(1, 1)), fp - float2(1, 1));
                fp = fp * fp * fp * (fp * (fp * 6 - 15) + 10);
                return lerp(lerp(d00, d01, fp.y), lerp(d10, d11, fp.y), fp.x) + 0.5;
            }

            // ==================== Vertex Shader (實際處理頂點) ====================
            Varyings VertexTransform(Attributes input)
            {
                Varyings output;
                
                // 儲存原始 OS 資料供 Freeze 使用
                output.positionOS = input.positionOS.xyz;
                output.normalOS = input.normalOS;
                
                // Freeze 頂點位移效果
                float3 finalPositionOS = input.positionOS.xyz;
                
                #ifdef _FREEZE_ON
                if (_FreezeAmount > 0.001)
                {
                    float normalY = saturate(input.normalOS.y);
                    float2 noiseUV = input.uv * _IcicleNoiseScale;
                    float noise = GradientNoise(noiseUV, 1.0);
                    noise = pow(noise, _IcicleNoisePower);
                    float displacement = normalY * _FreezeAmount * noise * _IcicleLength * -1.0;
                    finalPositionOS += input.normalOS * displacement;
                }
                #endif
                
                VertexPositionInputs vertexInput = GetVertexPositionInputs(finalPositionOS);
                VertexNormalInputs normalInput = GetVertexNormalInputs(input.normalOS, input.tangentOS);
                
                output.positionWS = vertexInput.positionWS;
                output.positionCS = vertexInput.positionCS;
                output.normalWS = normalInput.normalWS;
                output.tangentWS = normalInput.tangentWS;
                output.bitangentWS = normalInput.bitangentWS;
                output.uv = TRANSFORM_TEX(input.uv, _BaseMap);
                output.viewDirWS = GetWorldSpaceNormalizeViewDir(vertexInput.positionWS);
                
                return output;
            }

            // ==================== Tessellation Vertex Shader ====================
            TessellationControlPoint TessellationVertex(Attributes input)
            {
                TessellationControlPoint output;
                output.positionOS = input.positionOS;
                output.normalOS = input.normalOS;
                output.tangentOS = input.tangentOS;
                output.uv = input.uv;
                return output;
            }

            // ==================== Tessellation Factors ====================
            struct TessellationFactors
            {
                float edge[3] : SV_TessFactor;
                float inside : SV_InsideTessFactor;
            };

            // Distance-based Tessellation
            float CalcDistanceTessFactor(float4 positionOS, float minDist, float maxDist, float tess)
            {
                #ifdef _TESSELLATION_ON
                    float3 positionWS = TransformObjectToWorld(positionOS.xyz);
                    float dist = distance(positionWS, _WorldSpaceCameraPos);
                    float f = saturate((maxDist - dist) / (maxDist - minDist));
                    return lerp(1, tess, f);
                #else
                    return 1;
                #endif
            }

            TessellationFactors PatchConstantFunction(InputPatch<TessellationControlPoint, 3> patch)
            {
                TessellationFactors f;
                
                float edge0 = CalcDistanceTessFactor(patch[0].positionOS, _TessellationMinDistance, _TessellationMaxDistance, _TessellationFactor);
                float edge1 = CalcDistanceTessFactor(patch[1].positionOS, _TessellationMinDistance, _TessellationMaxDistance, _TessellationFactor);
                float edge2 = CalcDistanceTessFactor(patch[2].positionOS, _TessellationMinDistance, _TessellationMaxDistance, _TessellationFactor);
                
                f.edge[0] = edge0;
                f.edge[1] = edge1;
                f.edge[2] = edge2;
                f.inside = (edge0 + edge1 + edge2) / 3.0;
                
                return f;
            }

            // ==================== Hull Shader ====================
            [domain("tri")]
            [outputcontrolpoints(3)]
            [outputtopology("triangle_cw")]
            [partitioning("integer")]
            [patchconstantfunc("PatchConstantFunction")]
            TessellationControlPoint HullProgram(InputPatch<TessellationControlPoint, 3> patch, uint id : SV_OutputControlPointID)
            {
                return patch[id];
            }

            // ==================== Domain Shader ====================
            [domain("tri")]
            Varyings DomainProgram(TessellationFactors factors, OutputPatch<TessellationControlPoint, 3> patch, float3 barycentricCoordinates : SV_DomainLocation)
            {
                Attributes input;
                
                // 插值頂點屬性
                #define DOMAIN_INTERPOLATE(fieldName) input.fieldName = \
                    patch[0].fieldName * barycentricCoordinates.x + \
                    patch[1].fieldName * barycentricCoordinates.y + \
                    patch[2].fieldName * barycentricCoordinates.z;
                
                DOMAIN_INTERPOLATE(positionOS)
                DOMAIN_INTERPOLATE(normalOS)
                DOMAIN_INTERPOLATE(tangentOS)
                DOMAIN_INTERPOLATE(uv)
                
                // 正規化
                input.normalOS = normalize(input.normalOS);
                input.tangentOS.xyz = normalize(input.tangentOS.xyz);
                
                // 調用實際的頂點處理函數
                return VertexTransform(input);
            }

            // ==================== Fragment Shader ====================
            half4 LitPassFragment(Varyings input) : SV_Target
            {
                // 採樣基礎貼圖
                half4 albedoAlpha = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, input.uv);
                half3 albedo = albedoAlpha.rgb * _BaseColor.rgb;
                half alpha = albedoAlpha.a * _BaseColor.a;

                // Freeze 效果計算
                half3 finalAlbedo = albedo;
                half finalSmoothness = _Smoothness;
                half3 freezeNormal = half3(0, 0, 1);
                half3 freezeEmission = 0;
                
                #ifdef _FREEZE_ON
                if (_FreezeAmount > 0.001)
                {
                    float normalYMask = saturate(input.normalOS.y);
                    float freezeMask = normalYMask * _FreezeAmount * _FreezeRange;
                    freezeMask = saturate(freezeMask);
                    
                    float2 freezeUV = TRANSFORM_TEX(input.uv, _FreezeTexture);
                    half3 freezeTex = SAMPLE_TEXTURE2D(_FreezeTexture, sampler_FreezeTexture, freezeUV).rgb;
                    freezeTex *= _FreezeColor.rgb;
                    
                    finalAlbedo = lerp(albedo, freezeTex, freezeMask);
                    
                    float smoothnessMask = saturate(normalYMask);
                    smoothnessMask = smoothstep(_FreezeSmoothnessRemap.x, _FreezeSmoothnessRemap.y, smoothnessMask);
                    finalSmoothness = lerp(_Smoothness, _FreezeSmoothness, smoothnessMask * _FreezeAmount);
                    
                    float2 noiseUV = input.uv * _IcicleNoiseScale;
                    float noise = GradientNoise(noiseUV, 1.0);
                    noise = saturate(noise);
                    noise = pow(noise, _FreezeNormalContrast);
                    
                    float normalStrength = _FreezeNormalStrength * 0.01;
                    float2 offset = float2(0.001, 0);
                    float noiseX = GradientNoise(noiseUV + offset.xy, 1.0);
                    float noiseY = GradientNoise(noiseUV + offset.yx, 1.0);
                    float3 normalFromNoise = float3((noise - noiseX), (noise - noiseY), normalStrength);
                    normalFromNoise = normalize(normalFromNoise);
                    
                    freezeNormal = lerp(half3(0, 0, 1), normalFromNoise, normalYMask * _FreezeAmount);
                    
                    float NdotV = saturate(dot(normalize(input.normalWS), normalize(input.viewDirWS)));
                    float freezeFresnel = pow(1.0 - NdotV, _FreezeFresnelPower);
                    freezeEmission = _FreezeEmissionColor.rgb * freezeFresnel * _FreezeEmissionStrength * _FreezeAmount;
                }
                #endif

                // Metallic & Smoothness
                half metallic = _Metallic;
                #ifdef _METALLICSPECGLOSSMAP
                    half4 metallicGloss = SAMPLE_TEXTURE2D(_MetallicGlossMap, sampler_MetallicGlossMap, input.uv);
                    metallic *= metallicGloss.r;
                    finalSmoothness *= metallicGloss.a;
                #endif

                // Normal Map
                half3 normalWS = normalize(input.normalWS);
                #ifdef _NORMALMAP
                    half3 normalTS = UnpackNormalScale(SAMPLE_TEXTURE2D(_BumpMap, sampler_BumpMap, input.uv), _BumpScale);
                    
                    #ifdef _FREEZE_ON
                    normalTS = normalize(normalTS + freezeNormal);
                    #endif
                    
                    half3x3 tangentToWorld = half3x3(input.tangentWS, input.bitangentWS, input.normalWS);
                    normalWS = normalize(mul(normalTS, tangentToWorld));
                #endif

                // Emission
                half3 emission = 0;
                #ifdef _EMISSION
                    emission = SAMPLE_TEXTURE2D(_EmissionMap, sampler_EmissionMap, input.uv).rgb * _EmissionColor.rgb;
                #endif
                
                emission += freezeEmission;

                // 設定 SurfaceData
                SurfaceData surfaceData = (SurfaceData)0;
                surfaceData.albedo = finalAlbedo;
                surfaceData.alpha = alpha;
                surfaceData.metallic = metallic;
                surfaceData.smoothness = finalSmoothness;
                surfaceData.normalTS = half3(0, 0, 1);
                surfaceData.emission = emission;
                surfaceData.occlusion = 1.0;

                // 設定 InputData
                InputData inputData = (InputData)0;
                inputData.positionWS = input.positionWS;
                inputData.normalWS = normalWS;
                inputData.viewDirectionWS = normalize(input.viewDirWS);
                inputData.shadowCoord = TransformWorldToShadowCoord(input.positionWS);
                inputData.bakedGI = SampleSH(inputData.normalWS);

                // PBR 光照計算
                half4 color = UniversalFragmentPBR(inputData, surfaceData);

                // 附加 Fresnel 效果
                #ifdef _FRESNEL_ON
                    float NdotV = saturate(dot(inputData.normalWS, inputData.viewDirectionWS));
                    float fresnel = pow(1.0 - NdotV, _FresnelPower) * _FresnelStrength;
                    color.rgb += _FresnelColor.rgb * fresnel * _FresnelIntensity;
                #endif

                return color;
            }
            ENDHLSL
        }

        // 3. ShadowCaster Pass
        Pass
        {
            Name "ShadowCaster"
            Tags { "LightMode" = "ShadowCaster" }
            ZWrite On
            ZTest LEqual
            ColorMask 0
            Cull [_Cull]

            HLSLPROGRAM
            #pragma vertex ShadowPassVertex
            #pragma fragment ShadowPassFragment
            #pragma shader_feature_local _FREEZE_ON

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            
            #ifndef LERPWHITETO_DEFINED
            #define LERPWHITETO_DEFINED
            half LerpWhiteTo(half b, half t) { return half(1.0) + t * (b - half(1.0)); }
            #endif
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"

            CBUFFER_START(UnityPerMaterial)
                float _FreezeAmount;
                float _IcicleNoiseScale;
                float _IcicleNoisePower;
                float _IcicleLength;
            CBUFFER_END

            struct Attributes 
            { 
                float4 positionOS : POSITION; 
                float3 normalOS : NORMAL;
                float2 uv : TEXCOORD0;
            };
            
            struct Varyings 
            { 
                float4 positionCS : SV_POSITION; 
            };

            float3 _LightDirection;

            float2 GradientNoiseDir(float2 p)
            {
                p = fmod(p, 289);
                float x = fmod((34 * p.x + 1) * p.x, 289) + p.y;
                x = fmod((34 * x + 1) * x, 289);
                x = frac(x / 41) * 2 - 1;
                return normalize(float2(x - floor(x + 0.5), abs(x) - 0.5));
            }

            float GradientNoise(float2 UV, float Scale)
            {
                float2 p = UV * Scale;
                float2 ip = floor(p);
                float2 fp = frac(p);
                float d00 = dot(GradientNoiseDir(ip), fp);
                float d01 = dot(GradientNoiseDir(ip + float2(0, 1)), fp - float2(0, 1));
                float d10 = dot(GradientNoiseDir(ip + float2(1, 0)), fp - float2(1, 0));
                float d11 = dot(GradientNoiseDir(ip + float2(1, 1)), fp - float2(1, 1));
                fp = fp * fp * fp * (fp * (fp * 6 - 15) + 10);
                return lerp(lerp(d00, d01, fp.y), lerp(d10, d11, fp.y), fp.x) + 0.5;
            }

            Varyings ShadowPassVertex(Attributes input)
            {
                Varyings output;
                
                float3 finalPositionOS = input.positionOS.xyz;
                
                #ifdef _FREEZE_ON
                if (_FreezeAmount > 0.001)
                {
                    float normalY = saturate(input.normalOS.y);
                    float2 noiseUV = input.uv * _IcicleNoiseScale;
                    float noise = GradientNoise(noiseUV, 1.0);
                    noise = pow(noise, _IcicleNoisePower);
                    float displacement = normalY * _FreezeAmount * noise * _IcicleLength * -1.0;
                    finalPositionOS += input.normalOS * displacement;
                }
                #endif
                
                float3 positionWS = TransformObjectToWorld(finalPositionOS);
                float3 normalWS = TransformObjectToWorldNormal(input.normalOS);
                output.positionCS = TransformWorldToHClip(ApplyShadowBias(positionWS, normalWS, _LightDirection));
                
                return output;
            }

            half4 ShadowPassFragment(Varyings input) : SV_TARGET { return 0; }
            ENDHLSL
        }

        // 4. DepthOnly Pass
        Pass
        {
            Name "DepthOnly"
            Tags { "LightMode" = "DepthOnly" }
            ZWrite On
            ColorMask 0

            HLSLPROGRAM
            #pragma vertex DepthOnlyVertex
            #pragma fragment DepthOnlyFragment
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes { float4 positionOS : POSITION; };
            struct Varyings { float4 positionCS : SV_POSITION; };

            Varyings DepthOnlyVertex(Attributes input)
            {
                Varyings output;
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                return output;
            }

            half4 DepthOnlyFragment(Varyings input) : SV_TARGET { return 0; }
            ENDHLSL
        }

        // 5. Meta Pass
        Pass
        {
            Name "Meta"
            Tags { "LightMode" = "Meta" }
            Cull Off

            HLSLPROGRAM
            #pragma vertex UniversalVertexMeta
            #pragma fragment UniversalFragmentMeta
            #pragma shader_feature_local _EMISSION
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/MetaInput.hlsl"

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseMap_ST;
                float4 _BaseColor;
                float4 _EmissionColor;
            CBUFFER_END

            TEXTURE2D(_BaseMap); SAMPLER(sampler_BaseMap);
            TEXTURE2D(_EmissionMap); SAMPLER(sampler_EmissionMap);

            struct Attributes
            {
                float4 positionOS   : POSITION;
                float3 normalOS     : NORMAL;
                float2 uv0          : TEXCOORD0;
                float2 uv1          : TEXCOORD1;
                float2 uv2          : TEXCOORD2;
            };

            struct Varyings
            {
                float4 positionCS   : SV_POSITION;
                float2 uv           : TEXCOORD0;
            };

            Varyings UniversalVertexMeta(Attributes input)
            {
                Varyings output;
                output.positionCS = UnityMetaVertexPosition(input.positionOS.xyz, input.uv1, input.uv2);
                output.uv = TRANSFORM_TEX(input.uv0, _BaseMap);
                return output;
            }

            half4 UniversalFragmentMeta(Varyings input) : SV_Target
            {
                MetaInput metaInput = (MetaInput)0;
                half4 albedo = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, input.uv);
                metaInput.Albedo = albedo.rgb * _BaseColor.rgb;
                
                #ifdef _EMISSION
                    metaInput.Emission = SAMPLE_TEXTURE2D(_EmissionMap, sampler_EmissionMap, input.uv).rgb * _EmissionColor.rgb;
                #else
                    metaInput.Emission = 0;
                #endif
                
                return UnityMetaFragment(metaInput);
            }
            ENDHLSL
        }
    }

    FallBack "Hidden/Universal Render Pipeline/FallbackError"
}
