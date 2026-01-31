#ifndef PARTICLE_COMPONENT
#define PARTICLE_COMPONENT
// ---------------------------------------------------------------------------------------------------
// Include Files
// ---------------------------------------------------------------------------------------------------
#include "ParitcleVertexStructure.hlsl"
// ---------------------------------------------------------------------------------------------------
// Vertex Shader
// ---------------------------------------------------------------------------------------------------
VertexOutput vert(VertexInput v)
{
    VertexOutput o = (VertexOutput)0;
    
    // GPU-Instance (Set-up)
    UNITY_SETUP_INSTANCE_ID(v);
    UNITY_TRANSFER_INSTANCE_ID(v, o);

    // GPU-Instance (CustomData)
    SETUP_INSTANCEDATA();
    SETUP_CUSTOMDATA(v, o);
    CREATE_CUSTOMDATA_LIST(o);

    // GPU-Instance (UV Channel)
    CREATE_UV_CHANNEL_LIST(v);

    // GPU-Instance (Color)
    SETUP_COLOR(v, o);

    // PosWorld (PositionWS)
    #ifdef USE_POSWORLD
                o.posWorld = mul(unity_ObjectToWorld, v.vertex).xyz;
    #endif

    // Vertex-Offset
    VertexOffset(v, o, customData, UVChannel);

    // MainTex UV
    o.uv.xy = v.uv.xy;
    o.uv.zw = v.uv.xy;
    SetMainTexUV(o, customData, UVChannel);
    vertTexSheetCoord(o.uv.xy);

    // Distort UV
    SetDistortTexUV(o, customData, UVChannel);

    // Dissolve UV
    SetDissolveTexUV(o, customData, UVChannel);

    // Normal UV
    SetNormalTexUV(o, customData, UVChannel);

    // Parallax UV
    SetParallaxTexUV(o, customData, UVChannel);

    // ShadowCoord
    #ifdef _RECEIVESHADOW_ENABLE
                o.ShadowCoord = TransformWorldToShadowCoord(o.posWorld);
    #endif

    // Normal 
    #ifdef USE_NORMAL
                o.normal = TransformObjectToWorldNormal(v.normal);
    #endif

    // Tangent & ViewDir(TangentSpace) & LightDir(TangentSpace)
    SetTangentSpace(v, o);

    // Light-Illumination
    #ifdef _ACTIVELIGHTSYSTEM_VERTEXLIGHT
            #ifdef _RECEIVESHADOW_ENABLE
                o.ExternalCoord.xyz = CalculateLight(_MainLightIntensity, _AdditionalLightIntensity, o.normal, o.posWorld,o.ShadowCoord);
            #else
                o.ExternalCoord.xyz = CalculateLight(_MainLightIntensity, _AdditionalLightIntensity, o.normal, o.posWorld);
            #endif
    #endif

    // Emission UV
    SetEmissionTexUV(o, customData, UVChannel);

    // Decal
    #ifdef _PARTICLEMODE_DECAL
                half3 PositionVS = TransformWorldToView(TransformObjectToWorld(v.vertex.xyz));
                o.DecalRay.xyz = PositionVS * float3(1,1,-1);
    #endif

    return o;
}

// ---------------------------------------------------------------------------------------------------
// Fragment Shader
// ---------------------------------------------------------------------------------------------------
half4 frag(VertexOutput o, half facing : VFACE) : SV_Target
{
    // GPU-Instance (Set-up)
    UNITY_SETUP_INSTANCE_ID(o);

    // GPU-Instance (CustomData)
    SETUP_INSTANCEDATA();
    CREATE_CUSTOMDATA_LIST(o);

    // Depth
    ScreenData screenData = (ScreenData)0;
    ApplyScreenData(screenData, o);

    // Decal
    ApplyDecalUV(o, customData, screenData);

    // UV setting (PolarCoord)
    ApplyPolarCoord(o, customData);

    // UV setting (ScreenUV)
    ApplyScreenUV(o, customData, screenData);

    // Assign UV
    half2 MainTexUV = o.uv.xy;

    // Prepare Light Data (Normalized)
    VectorData vectorData = (VectorData)0;
    PrepareLightVectorData(o, vectorData);

    // Parallax
    ApplyParallaxUV(o, MainTexUV, customData, vectorData);

    // Distort
    ApplyDistortUV(o, MainTexUV, customData, screenData);

    // MainTex UV Clamp
    if (_ClampMainTexUV)
    {
        if (any(MainTexUV > 1.0f) || any(MainTexUV < 0.0f))
            discard;
    }

    // MainTex
    half4 MainTexColor = SAMPLE_TEXTURE2D(_MainTex, MAINTEX_WRAPMODE, MainTexUV);

    // Final Color & Alpha
    FinalData finalData;
    finalData.Color = MainTexColor.rgb * o.vertexColor.rgb * _MainTexColor.rgb;
    finalData.Alpha = MainTexColor.a * o.vertexColor.a * _MainTexColor.a;

    // Decal
    ApplyWorldNormalMaskToDecal(finalData, screenData);

    // MainTex (Multi-Channel)
    ApplyMainTexMode_MultiChannel(finalData, o, MainTexColor, customData);

    // Emission
    ApplyEmission(finalData, o, customData);

    // Double-Side Color
    ApplyDoubleSideColor(finalData, o, facing, MainTexColor);

    // Normal
    ApplyNormalMap(finalData, o, customData, vectorData);

    // Combine Screen Distortion
    ApplyScreenDistortion(finalData, o, MainTexColor, customData, screenData);
    
    // Fresnel
    ApplyFresnel(finalData, o, customData, vectorData, facing);

    // Dissolve
    ApplyDissolve(finalData, o, customData);

    // External Alpha Template
    ApplyExternalAlphaTemplate(finalData, o, customData);

    // Alpha-Clip
    ApplyAlphaClip(finalData, customData);

    // Soft-Particle
    ApplySoftParticle(finalData, o, screenData);

    // Ramp
    ApplyRampColor(finalData);

    // Color Adjustment
    ApplyColorAdjustment(finalData, customData);

    // Light-Illumination
    #ifdef _ACTIVELIGHTSYSTEM_VERTEXLIGHT
                finalData.Color *= o.ExternalCoord.xyz;
    #elif  _ACTIVELIGHTSYSTEM_FRAGMENTLIGHT
                #ifdef _RECEIVESHADOW_ENABLE
                    finalData.Color *= CalculateLight(_MainLightIntensity, _AdditionalLightIntensity, o.normal, o.posWorld,o.ShadowCoord);
                #else
                    finalData.Color *= CalculateLight(_MainLightIntensity, _AdditionalLightIntensity, o.normal, o.posWorld);
                #endif
    #endif

    // FinalColor & FinalAlpha Multiplier
    finalData.Color *= GetCustomIntensity(_FinalColorIntensity, _FinalColorIntensityController, customData);
    finalData.Alpha *= GetCustomIntensity(_FinalAlphaIntensity, _FinalAlphaIntensityController, customData);
    finalData.Alpha = saturate(finalData.Alpha);

    #ifdef _PARTICLEMODE_GEOMETRY
                ApplyDithering(finalData, o);
                return half4(finalData.Color, 1);
    #endif

    return half4(finalData.Color, finalData.Alpha);
}

// ---------------------------------------------------------------------------------------------------
// Editor Rendering
// ---------------------------------------------------------------------------------------------------

half4 fragSceneHighlight(VertexOutput o) : SV_Target
{
    return float4(_ObjectId, _PassValue, 1, 1);
}

half4 fragScenePicking(VertexOutput o) : SV_Target
{
    return _SelectionID;
}

#endif
