#ifndef PARTICLE_KEYWORDS
#define PARTICLE_KEYWORDS

// ---------------------------------------------------------------------------------------------------
// Define Keyword
// ---------------------------------------------------------------------------------------------------

    // NormalTex 
    #ifdef _ACTIVENORMAL_ENABLE
                #define USE_NORMAL
                #define USE_TANGENT
                #define USE_TANGENTSPACE_LIGHTDIR
    #endif

    // Parallax 
    #if defined _ACTIVEPARALLAX_ENABLE
                #define USE_TANGENT
                #define USE_TANGENTSPACE_VIEWDIR
    #elif defined _ACTIVEPARALLAX_STEEP
                #define USE_POSWORLD
    #endif

    // Fresnel
    #ifdef _ACTIVEFRESNEL_ENABLE
                #define USE_NORMAL
                #define USE_POSWORLD
                #define USE_WS_VIEWDIR
    #endif

    // Vertex-Offset
    #ifdef _ACTIVEVERTEXOFFSET_ENABLE
                #define USE_NORMAL
                #define USE_VERTEXOFFSET
    #elif _ACTIVEVERTEXOFFSET_ANIMATIONTEX
                #define USE_NORMAL
                #define USE_VERTEXID
    #endif

    // Light-Illumination
    #ifdef _ACTIVELIGHTSYSTEM_VERTEXLIGHT
                #define USE_EXTERNALCOORD
                #define USE_NORMAL
                #define USE_POSWORLD
    #elif  _ACTIVELIGHTSYSTEM_FRAGMENTLIGHT
                #define USE_NORMAL
                #define USE_POSWORLD
    #endif

    // Shadow Coord
    #ifdef _RECEIVESHADOW_ENABLE
                #define USE_POSWORLD
    #endif

    // Depth
    #if defined (_ACTIVESOFTPARTICLE_ENABLE) || defined (_PARTICLEMODE_SCREENDISTORTION) || defined(_PARTICLEMODE_DECAL)
                #define USE_SCREENUV
                #define USE_CAMERA_DEPTH
    #endif

    // Camera Normal Tex
    #if defined (_PARTICLEMODE_DECAL) && defined (_ACTIVEDECALWORLDNORMALMASK_ENABLE)
                #define USE_DECAL_WORLDNORMALMASK
    #endif
    #if defined (USE_DECAL_WORLDNORMALMASK)
                #define USE_CAMERA_NORMAL
    #endif

// ---------------------------------------------------------------------------------------------------
// Define Marco (UV2)
// ---------------------------------------------------------------------------------------------------

    // MainTex Enable UV2 
    #ifdef _MAINTEXUVCHANNEL_UV2
                #define ENABLE_UV2
                #define MAINTEX_UVCHANNEL 1
    #else
                #define MAINTEX_UVCHANNEL 0
    #endif

    // DistortTex Enable UV2 
    #ifdef _DISTORTTEXUVCHANNEL_UV2
                #define ENABLE_UV2
                #define DISTORTTEX_UVCHANNEL 1
    #else
                #define DISTORTTEX_UVCHANNEL 0
    #endif

    // DissolveTex Enable UV2 
    #ifdef _DISSOLVETEXUVCHANNEL_UV2
                #define ENABLE_UV2
                #define DISSOLVETEX_UVCHANNEL 1
    #else
                #define DISSOLVETEX_UVCHANNEL 0
    #endif

    // NormalTex Enable UV2 
    #ifdef _NORMALTEXUVCHANNEL_UV2
                #define ENABLE_UV2
                #define NORMALTEX_UVCHANNEL 1
    #else
                #define NORMALTEX_UVCHANNEL 0
    #endif

    // ParallaxTex Enable UV2 
    #ifdef _PARALLAXTEXUVCHANNEL_UV2
                #define ENABLE_UV2
                #define PARALLAXTEX_UVCHANNEL 1
    #else
                #define PARALLAXTEX_UVCHANNEL 0
    #endif

    // Vertex-Offset Enable UV2 
    #ifdef _VERTEXOFFSETTEXUVCHANNEL_UV2
                #define ENABLE_UV2
                #define VERTEXOFFSETTEX_UVCHANNEL 1
    #else
                #define VERTEXOFFSETTEX_UVCHANNEL 0
    #endif

    // Emission Enable UV2 
    #ifdef _EMISSIONTEXUVCHANNEL_UV2
                #define ENABLE_UV2
                #define EMISSIONTEX_UVCHANNEL 1
    #else
                #define EMISSIONTEX_UVCHANNEL 0
    #endif

// ---------------------------------------------------------------------------------------------------
// Define Marco (Dependency)
// ---------------------------------------------------------------------------------------------------

    // Sharing PolarCoord
    #if defined (_MAINTEXMODE_POLARCOORD) || defined (_ACTIVEDISTORT_POLARCOORD) || defined (_ACTIVEDISSOLVE_POLARCOORD) || defined (_ACTIVEEMISSION_POLARCOORD)
                #define USE_POLARCOORD
    #endif

    // Sharing Distort 
    #if defined (_ACTIVEDISTORT_ENABLE) || defined (_ACTIVEDISTORT_FLOWMAP) || defined (_ACTIVEDISTORT_POLARCOORD) || defined (_ACTIVEDISTORT_SCREENUV)
                #define USE_DISTORT 
                #define USE_DISTORT_UV 
    #endif

    // Sharing Dissolve
    #if defined (_ACTIVEDISSOLVE_ENABLE) || defined (_ACTIVEDISSOLVE_POLARCOORD) || defined (_ACTIVEDISSOLVE_SCREENUV)
                #define USE_DISSOLVE 
                #define USE_DISSOLVE_UV 
    #endif

    // Sharing Normal UV
    #if defined (_ACTIVENORMAL_ENABLE) 
                #define USE_NORMALMAP_UV
    #endif

    // Sharing Parallax / HeightMap UV
    #if defined (_ACTIVEPARALLAX_ENABLE) || defined (_ACTIVEPARALLAX_STEEP)
                #define USE_PARALLAX
                #define USE_PARALLAX_UV
    #endif

    // Sharing Emission
    #if defined (_ACTIVEEMISSION_ENABLE) || defined (_ACTIVEEMISSION_POLARCOORD) || defined (_ACTIVEEMISSION_SCREENUV)
                #define USE_EMISSION
                #define USE_EMISSION_UV
    #endif

    // Sharing ScreenUV
    #if defined (_MAINTEXMODE_SCREENUV) || defined (_ACTIVEDISTORT_SCREENUV) || defined (_ACTIVEDISSOLVE_SCREENUV) || defined (_ACTIVEEMISSION_SCREENUV)
                #define USE_SCREENUV
    #endif

    // Vertex-Offset Enable Mask
    #if defined (USE_VERTEXOFFSET) && defined (_ACTIVEVERTEXOFFSETMASK_ENABLE)
                #define USE_VERTEXOFFSET_MASK
    #endif

    // Dissolve Rim Clip
    #if defined (USE_DISSOLVE) && defined (_ACTIVERIMDISSOLVE_ENABLE)
                #define USE_DISSOLVE_RIMCLIP
    #endif
 
    // Checking ViewDir value is using TangentSpace-ViewDir or WorldSpace-ViewDir
    #if defined (_ACTIVEFRESNEL_ENABLE) && defined (_ACTIVENORMAL_ENABLE)
                #define USE_TANGENT
                #define USE_TANGENTSPACE_VIEWDIR
    #endif

    // Enable Double Side Color
    #if defined (_ACTIVESIDEFACECOLOR_REPLACE) || defined (_ACTIVESIDEFACECOLOR_ADD) || defined (_ACTIVESIDEFACECOLOR_MULTIPLY)
                #define USE_DOUBLE_SIDE_COLOR
    #endif

// ---------------------------------------------------------------------------------------------------
// Define keyword for ParticleCommonShared.hlsl
// ---------------------------------------------------------------------------------------------------
    #if defined (PARTICLE_DEPTH_NORMAL) || defined (PARTICLE_SHADOWCASTER) || defined (PARTICLE_DEPTH_ONLY)
                #define USE_NORMAL
                #define USE_TANGENT
    #endif

// ---------------------------------------------------------------------------------------------------
// Define CustomData & Particle Instance
// ---------------------------------------------------------------------------------------------------

    // CustomData
    #define CUSTOM_DATA(index1,index2) float4 customData1 : TEXCOORD##index1; float4 customData2 : TEXCOORD##index2;

    // CustomData (Particle Instanced)
    #ifdef PARTICLESYS_INSTANCE_ENABLE
                #define SETUP_INSTANCEDATA() PARTICLESYS_INSTANCE_DATA instance_data = unity_ParticleInstanceData[unity_InstanceID];
                #define SETUP_CUSTOMDATA(input,output) output.customData1 = instance_data.customData1; output.customData2 = instance_data.customData2;
                #define CREATE_CUSTOMDATA_LIST(input) half4x2 customData = {instance_data.customData1.x,instance_data.customData1.y,instance_data.customData1.z,instance_data.customData1.w,instance_data.customData2.x,instance_data.customData2.y,instance_data.customData2.z,instance_data.customData2.w};
    #else
                #define SETUP_INSTANCEDATA();
                #define SETUP_CUSTOMDATA(input,output) output.customData1 = input.customData1; output.customData2 = input.customData2;
                #define CREATE_CUSTOMDATA_LIST(input) half4x2 customData = {input.customData1.x,input.customData1.y, input.customData1.z,input.customData1.w,input.customData2.x, input.customData2.y,input.customData2.z,input.customData2.w};
    #endif

    // Color (Particle Instanced)
    #ifdef PARTICLESYS_INSTANCE_ENABLE 
                #define SETUP_COLOR(input,output) output.vertexColor = lerp(1, input.vertexColor, unity_ParticleUseMeshColors); output.vertexColor *= UnpackFromR8G8B8A8(instance_data.color);
    #else
                #define SETUP_COLOR(input,output) output.vertexColor = input.vertexColor;
    #endif

    // UV、UV2 Channel (Particle Instanced)
    #if defined (ENABLE_UV2) && defined (PARTICLESYS_INSTANCE_ENABLE)
                #define CREATE_UV_CHANNEL_LIST(input) float2 UVChannel[2] = {input.uv.xy , input.uv2.xy};
    #else
                #define CREATE_UV_CHANNEL_LIST(input) float2 UVChannel[2] = {input.uv.xy , input.uv.zw};
    #endif

    // TextureSheet (Particle Instanced)
    #if defined (PARTICLESYS_INSTANCE_ENABLE)
            #define vertTexSheetCoord(v) vertInstancingTextureSheetUV(v);
    #else
            #define vertTexSheetCoord(v) vertInstancingTextureSheetUV();
    #endif

// ---------------------------------------------------------------------------------------------------
// Define Texture Wrap-Mode
// ---------------------------------------------------------------------------------------------------

    // MainTex Wrap-Mode
    #ifdef _MAINTEXWRAPMODE_LINEAR_REPEAT
                #define MAINTEX_WRAPMODE sampler_linear_repeat
    #elif _MAINTEXWRAPMODE_LINEAR_CLAMP
                #define MAINTEX_WRAPMODE sampler_linear_clamp
    #elif _MAINTEXWRAPMODE_POINT_REPEAT
                #define MAINTEX_WRAPMODE sampler_point_repeat
    #elif _MAINTEXWRAPMODE_POINT_CLAMP
                #define MAINTEX_WRAPMODE sampler_point_clamp
    #else
                #define MAINTEX_WRAPMODE sampler_MainTex
    #endif

    // DistortTex Wrap-Mode
    #ifdef _DISTORTTEXWRAPMODE_LINEAR_REPEAT
                #define DISTORTTEX_WRAPMODE sampler_linear_repeat
    #elif _DISTORTTEXWRAPMODE_LINEAR_CLAMP
                #define DISTORTTEX_WRAPMODE sampler_linear_clamp
    #elif _DISTORTTEXWRAPMODE_POINT_REPEAT
                #define DISTORTTEX_WRAPMODE sampler_point_repeat
    #elif _DISTORTTEXWRAPMODE_POINT_CLAMP
                #define DISTORTTEX_WRAPMODE sampler_point_clamp
    #else
                #define DISTORTTEX_WRAPMODE sampler_DistortTex
    #endif

    // DissolveTex Wrap-Mode
    #ifdef _DISSOLVETEXWRAPMODE_LINEAR_REPEAT
                #define DISSOLVETEX_WRAPMODE sampler_linear_repeat
    #elif _DISSOLVETEXWRAPMODE_LINEAR_CLAMP
                #define DISSOLVETEX_WRAPMODE sampler_linear_clamp
    #elif _DISSOLVETEXWRAPMODE_POINT_REPEAT
                #define DISSOLVETEX_WRAPMODE sampler_point_repeat
    #elif _DISSOLVETEXWRAPMODE_POINT_CLAMP
                #define DISSOLVETEX_WRAPMODE sampler_point_clamp
    #else
                #define DISSOLVETEX_WRAPMODE sampler_DissolveTex
    #endif

    // NormalTex Wrap-Mode
    #ifdef _NORMALTEXWRAPMODE_LINEAR_REPEAT
                #define NORMALTEX_WRAPMODE sampler_linear_repeat
    #elif _NORMALTEXWRAPMODE_LINEAR_CLAMP
                #define NORMALTEX_WRAPMODE sampler_linear_clamp
    #elif _NORMALTEXWRAPMODE_POINT_REPEAT
                #define NORMALTEX_WRAPMODE sampler_point_repeat
    #elif _NORMALTEXWRAPMODE_POINT_CLAMP
                #define NORMALTEX_WRAPMODE sampler_point_clamp
    #else
                #define NORMALTEX_WRAPMODE sampler_NormalTex
    #endif

    // ParallaxTex Wrap-Mode
    #ifdef _PARALLAXTEXWRAPMODE_LINEAR_REPEAT
                #define PARALLAXTEX_WRAPMODE sampler_linear_repeat
    #elif _PARALLAXTEXWRAPMODE_LINEAR_CLAMP
                #define PARALLAXTEX_WRAPMODE sampler_linear_clamp
    #elif _PARALLAXTEXWRAPMODE_POINT_REPEAT
                #define PARALLAXTEX_WRAPMODE sampler_point_repeat
    #elif _PARALLAXTEXWRAPMODE_POINT_CLAMP
                #define PARALLAXTEX_WRAPMODE sampler_point_clamp
    #else
                #define PARALLAXTEX_WRAPMODE sampler_ParallaxTex
    #endif

    // Parallax-NoiseStepTex Wrap-Mode
    #ifdef _PARALLAXNOISESTEPTEXWRAPMODE_LINEAR_REPEAT
                #define PARALLAX_NOISESTEPTEX_WRAPMODE sampler_linear_repeat
    #elif _PARALLAXNOISESTEPTEXWRAPMODE_LINEAR_CLAMP
                #define PARALLAX_NOISESTEPTEX_WRAPMODE sampler_linear_clamp
    #elif _PARALLAXNOISESTEPTEXWRAPMODE_POINT_REPEAT
                #define PARALLAX_NOISESTEPTEX_WRAPMODE sampler_point_repeat
    #elif _PARALLAXNOISESTEPTEXWRAPMODE_POINT_CLAMP
                #define PARALLAX_NOISESTEPTEX_WRAPMODE sampler_point_clamp
    #else
                #define PARALLAX_NOISESTEPTEX_WRAPMODE sampler_ParallaxNoiseStepTex
    #endif

    // Vertex-Offset Wrap-Mode
    #ifdef _VERTEXOFFSETTEXWRAPMODE_LINEAR_REPEAT
                #define VERTEXOFFSETTEX_WRPAMODE sampler_linear_repeat
    #elif _VERTEXOFFSETTEXWRAPMODE_LINEAR_CLAMP
                #define VERTEXOFFSETTEX_WRPAMODE sampler_linear_clamp
    #elif _VERTEXOFFSETTEXWRAPMODE_POINT_REPEAT
                #define VERTEXOFFSETTEX_WRPAMODE sampler_point_repeat
    #elif _VERTEXOFFSETTEXWRAPMODE_POINT_CLAMP
                #define VERTEXOFFSETTEX_WRPAMODE sampler_point_clamp
    #else
                #define VERTEXOFFSETTEX_WRPAMODE sampler_VertexOffsetTex
    #endif

    // Vertex-Offset Mask Wrap-Mode
    #ifdef _VERTEXOFFSETMASKWRAPMODE_LINEAR_REPEAT
                #define VERTEXOFFSETMASK_WRPAMODE sampler_linear_repeat
    #elif _VERTEXOFFSETMASKWRAPMODE_LINEAR_CLAMP
                #define VERTEXOFFSETMASK_WRPAMODE sampler_linear_clamp
    #elif _VERTEXOFFSETMASKWRAPMODE_POINT_REPEAT
                #define VERTEXOFFSETMASK_WRPAMODE sampler_point_repeat
    #elif _VERTEXOFFSETMASKWRAPMODE_POINT_CLAMP
                #define VERTEXOFFSETMASK_WRPAMODE sampler_point_clamp
    #else
                #define VERTEXOFFSETMASK_WRPAMODE sampler_VertexOffsetMask
    #endif

    // Emission Wrap-Mode
    #ifdef _EMISSIONTEXWRAPMODE_LINEAR_REPEAT
                #define EMISSIONTEX_WRPAMODE sampler_linear_repeat
    #elif _EMISSIONTEXWRAPMODE_LINEAR_CLAMP
                #define EMISSIONTEX_WRPAMODE sampler_linear_clamp
    #elif _EMISSIONTEXWRAPMODE_POINT_REPEAT
                #define EMISSIONTEX_WRPAMODE sampler_point_repeat
    #elif _EMISSIONTEXWRAPMODE_POINT_CLAMP
                #define EMISSIONTEX_WRPAMODE sampler_point_clamp
    #else
                #define EMISSIONTEX_WRPAMODE sampler_EmissionTex
    #endif


#endif