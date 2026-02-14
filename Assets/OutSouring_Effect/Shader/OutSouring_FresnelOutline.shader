Shader "OutSouring_Effect/OutSouring_FresnelOutline"
{
    Properties
    {
        [Header(Base Color)]
        _BaseMap ("Base Map", 2D) = "white" {}
        [HDR] _BaseColor ("Base Color", Color) = (1, 1, 1, 1)
        _BaseIntensity ("Base Intensity", Range(0, 1)) = 1

        [Header(Fresnel)]
        [HDR] _FresnelColor ("Fresnel Color", Color) = (0, 0.5, 1, 1)
        _FresnelPower ("Fresnel Power", Range(0.1, 10)) = 3
        _FresnelStrength ("Fresnel Strength", Range(0, 5)) = 1
        _FresnelIntensity ("Fresnel Intensity", Range(0, 1)) = 1

        [Header(Outline)]
        [HDR] _OutlineColor ("Outline Color", Color) = (0, 0, 0, 1)
        _OutlineWidth ("Outline Width", Range(0, 0.1)) = 0.01
        _OutlineIntensity ("Outline Intensity", Range(0, 1)) = 1

        [Header(Render Settings)]
        [Enum(UnityEngine.Rendering.CullMode)] _CullMode ("Cull Mode", Float) = 2
        [Enum(Off, 0, On, 1)] _ZWrite ("ZWrite", Float) = 0
        [Enum(UnityEngine.Rendering.CompareFunction)] _ZTest ("ZTest", Float) = 4
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline" = "UniversalPipeline"
            "Queue" = "Transparent"
            "RenderType" = "Transparent"
        }

        Pass
        {
            Name "Outline"
            Tags { "LightMode" = "SRPDefaultUnlit" }

            Cull Front
            ZWrite [_ZWrite]
            ZTest [_ZTest]
            Blend SrcAlpha OneMinusSrcAlpha

            HLSLPROGRAM
            #pragma vertex OutlineVert
            #pragma fragment OutlineFrag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseMap_ST;
                float4 _BaseColor;
                float  _BaseIntensity;
                float4 _FresnelColor;
                float  _FresnelPower;
                float  _FresnelStrength;
                float  _FresnelIntensity;
                float4 _OutlineColor;
                float  _OutlineWidth;
                float  _OutlineIntensity;
                float  _CullMode;
                float  _ZWrite;
                float  _ZTest;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
            };

            Varyings OutlineVert(Attributes input)
            {
                Varyings output;
                float width = _OutlineWidth * _OutlineIntensity;
                float3 expandedPos = input.positionOS.xyz + normalize(input.normalOS) * width;
                output.positionCS = TransformObjectToHClip(expandedPos);
                return output;
            }

            half4 OutlineFrag(Varyings input) : SV_Target
            {
                return half4(_OutlineColor.rgb, _OutlineIntensity);
            }
            ENDHLSL
        }

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }

            Cull [_CullMode]
            ZWrite [_ZWrite]
            ZTest [_ZTest]
            Blend SrcAlpha OneMinusSrcAlpha

            HLSLPROGRAM
            #pragma vertex BaseVert
            #pragma fragment BaseFrag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseMap_ST;
                float4 _BaseColor;
                float  _BaseIntensity;
                float4 _FresnelColor;
                float  _FresnelPower;
                float  _FresnelStrength;
                float  _FresnelIntensity;
                float4 _OutlineColor;
                float  _OutlineWidth;
                float  _OutlineIntensity;
                float  _CullMode;
                float  _ZWrite;
                float  _ZTest;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
                float2 uv         : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS  : SV_POSITION;
                float2 uv          : TEXCOORD0;
                float3 normalWS    : TEXCOORD1;
                float3 viewDirWS   : TEXCOORD2;
            };

            Varyings BaseVert(Attributes input)
            {
                Varyings output;
                VertexPositionInputs posInputs = GetVertexPositionInputs(input.positionOS.xyz);
                VertexNormalInputs   norInputs = GetVertexNormalInputs(input.normalOS);

                output.positionCS = posInputs.positionCS;
                output.uv         = TRANSFORM_TEX(input.uv, _BaseMap);
                output.normalWS   = norInputs.normalWS;
                output.viewDirWS  = GetWorldSpaceNormalizeViewDir(posInputs.positionWS);

                return output;
            }

            half4 BaseFrag(Varyings input) : SV_Target
            {
                half4 texColor = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, input.uv);
                half3 baseColor = texColor.rgb * _BaseColor.rgb;
                half  baseAlpha = texColor.a * _BaseColor.a * _BaseIntensity;

                float NdotV = saturate(dot(normalize(input.normalWS), normalize(input.viewDirWS)));
                float fresnel = pow(1.0 - NdotV, _FresnelPower) * _FresnelStrength;
                half3 fresnelColor = _FresnelColor.rgb * fresnel;
                half  fresnelAlpha = saturate(fresnel) * _FresnelIntensity;

                half3 finalColor = baseColor * _BaseIntensity + fresnelColor * _FresnelIntensity;
                half  finalAlpha = saturate(1.0 - (1.0 - baseAlpha) * (1.0 - fresnelAlpha));

                return half4(finalColor, finalAlpha);
            }
            ENDHLSL
        }
    }

    FallBack "Hidden/Universal Render Pipeline/FallbackError"
}
