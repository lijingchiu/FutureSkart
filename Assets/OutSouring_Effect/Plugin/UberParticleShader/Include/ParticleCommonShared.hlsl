#ifndef PARTICLE_COMMON_SHARED
#define PARTICLE_COMMON_SHARED
// ---------------------------------------------------------------------------------------------------
// Include Files
// ---------------------------------------------------------------------------------------------------
#include "ParitcleVertexStructure.hlsl"

#if defined (PARTICLE_DEPTH_NORMAL)
        #if defined(_DETAIL_MULX2) || defined(_DETAIL_SCALED)
        #define _DETAIL
        #endif

        // GLES2 has limited amount of interpolators
        #if defined(_PARALLAXMAP) && !defined(SHADER_API_GLES)
        #define REQUIRES_TANGENT_SPACE_VIEW_DIR_INTERPOLATOR
        #endif

        #if (defined(_NORMALMAP) || (defined(_PARALLAXMAP) && !defined(REQUIRES_TANGENT_SPACE_VIEW_DIR_INTERPOLATOR))) || defined(_DETAIL)
        #define REQUIRES_WORLD_SPACE_TANGENT_INTERPOLATOR
        #endif
#endif


// ---------------------------------------------------------------------------------------------------
// Vertex Shader
// ---------------------------------------------------------------------------------------------------
VertexOutput vertCommonShared(VertexInput v)
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

    // Normal 
    #ifdef USE_NORMAL
    o.normal = TransformObjectToWorldNormal(v.normal);
    #endif

    // DepthNormal TangentSpaceToWorldSpace
    #if defined (PARTICLE_DEPTH_NORMAL) && defined (_ACTIVENORMAL_ENABLE)
                float3 tangentWS = TransformObjectToWorldDir(v.tangent.xyz, true);
                float3 binormalWS = cross(o.normal, tangentWS.xyz) * v.tangent.w;
                o.TangentSpaceMatrix = float3x3 (tangentWS, binormalWS, o.normal);
    #endif

    // Tangent & ViewDir(TangentSpace) & LightDir(TangentSpace)
    SetTangentSpace(v, o);

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

    return o;
}

// ---------------------------------------------------------------------------------------------------
// Fragment Shader
// ---------------------------------------------------------------------------------------------------
half4 fragCommonShared(VertexOutput o, half facing : VFACE) : SV_Target
{
    //Early Discard
    #if defined(USE_SCREENUV) && defined(PARTICLE_SHADOWCASTER)
        discard;
    #endif

    // GPU-Instance (Set-up)
    UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(o);
    UNITY_SETUP_INSTANCE_ID(o);
    
    // GPU-Instance (CustomData)
    SETUP_INSTANCEDATA();
    CREATE_CUSTOMDATA_LIST(o);

    // Depth
    ScreenData screenData = (ScreenData)0;
    ApplyScreenData(screenData, o);

    // UV setting (PolarCoord)
    ApplyPolarCoord(o, customData);

    // UV setting (ScreenUV)
    #if defined (PARTICLE_DEPTH_NORMAL) || defined (PARTICLE_DEPTH_ONLY)
    ApplyScreenUV(o, customData, screenData);
    #endif

    // Assign UV
    half2 MainTexUV = o.uv.xy;

    // Pre-Light
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
    
    // Final Alpha
    FinalData finalData;
    finalData.Color = MainTexColor.rgb * o.vertexColor.rgb * _MainTexColor.rgb;
    finalData.Alpha = MainTexColor.a * o.vertexColor.a * _MainTexColor.a;

    // MainTex (Multi-Channel)
    ApplyMainTexMode_MultiChannel(finalData, o, MainTexColor, customData);

    // External Alpha Template
    ApplyExternalAlphaTemplate(finalData, o, customData);

    // Normal
    ApplyNormalMap(finalData, o, customData, vectorData);

    // Dissolve
    ApplyDissolve(finalData, o, customData);

    // Fresnel
    ApplyFresnel(finalData, o, customData, vectorData, facing);

    // Alpha-Clip 
    ApplyAlphaClip(finalData, customData);

    // FinalAlpha Multiplier
    finalData.Alpha *= GetCustomIntensity(_FinalAlphaIntensity, _FinalAlphaIntensityController, customData);
    finalData.Alpha = saturate(finalData.Alpha);

    ApplyDithering(finalData, o);

    // DepthNormal PrePass
    #if defined (PARTICLE_SHADOWCASTER) || defined (PARTICLE_DEPTH_ONLY)
                return 0;
    #elif defined (PARTICLE_DEPTH_NORMAL)
        #ifdef _ACTIVENORMAL_ENABLE
                o.normal = TransformTangentToWorld(o.normal, o.TangentSpaceMatrix);
        #endif
                return half4(NormalizeNormalPerPixel(o.normal), 0.0);
    #endif

    return 0;
}

#endif
