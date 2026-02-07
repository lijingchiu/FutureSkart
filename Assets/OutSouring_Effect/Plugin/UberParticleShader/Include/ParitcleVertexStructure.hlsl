#ifndef PARTICLE_VERTEX_STRUCTURE
#define PARTICLE_VERTEX_STRUCTURE

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "ParticleSysInstance.hlsl"
#include "ParticleKeywords.hlsl"
#include "ParticleBuffers.hlsl"
#include "ParticleFunctions.hlsl"

// ---------------------------------------------------------------------------------------------------
// Struct Vertex Data
// ---------------------------------------------------------------------------------------------------
struct VertexInput
{
    float4 vertex : POSITION;
    float4 vertexColor : COLOR;
    float4 uv : TEXCOORD0;
    #ifdef USE_VERTEXID
            uint vertexID : SV_VertexID;
    #endif
    #if defined (ENABLE_UV2) && defined (PARTICLESYS_INSTANCE_ENABLE)
            float2 uv2 : TEXCOORD1;
    #elif defined (PARTICLESYS_INSTANCE_ENABLE)
            // There is no need to pass the customData in when 'Mesh GPU Instance is enabled'.
            // Cause the instance data is store in structured buffer, no more customData store in the mesh.
    #else 
            CUSTOM_DATA(1, 2)
    #endif
    #if defined (PARTICLE_SHADOWCASTER) || defined (USE_NORMAL) || defined (USE_TANGENT)
            float3 normal : NORMAL;
    #endif
    #ifdef USE_TANGENT
            float4 tangent : TANGENT;
    #endif
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct VertexOutput
{
    float4 vertex : SV_POSITION;
    float4 uv : TEXCOORD0; // xy: MainTexUV , zw: Original UV (Clean UV)
    float4 vertexColor : COLOR;
    CUSTOM_DATA(1, 2)
    #if defined (USE_DISTORT_UV) || defined (USE_DISSOLVE_UV)
            float4 DistortTexUVAndDissolveTexUV : TEXCOORD3; // xy: Distortion UV , zw : Dissolve UV
    #endif
    #ifdef USE_EXTERNALCOORD
            float4 ExternalCoord : TEXCOORD4; // xyz: Calculate Light , w: FogFactor 
    #endif
    #ifdef USE_NORMAL
            float3 normal : TEXCOORD5;
    #endif
    #ifdef USE_TANGENT
            #ifdef USE_TANGENTSPACE_VIEWDIR
                        float3 ViewDir : TEXCOORD6;
            #endif
            #ifdef USE_TANGENTSPACE_LIGHTDIR
                        float3 LightDir : TEXCOORD7;
            #endif
    #endif
    #ifdef USE_POSWORLD
            float3 posWorld : TEXCOORD8;
    #endif
    #if defined (USE_NORMALMAP_UV) || defined (USE_PARALLAX_UV)
            float4 NormalUVandParallaxUV : TEXCOORD9; // xy: NormalUV , zw: ParallaxUV
    #endif
    #ifdef USE_EMISSION_UV
            float4 EmissionUV : TEXCOORD10; // xy: Emission , zw: None
    #endif
    #if defined(_RECEIVESHADOW_ENABLE)
            float4 ShadowCoord : TEXCOORD11;
    #endif
    #ifdef _PARTICLEMODE_DECAL
            float3 DecalRay : TEXCOORD12;
    #endif
    #if defined (PARTICLE_DEPTH_NORMAL) && defined (_ACTIVENORMAL_ENABLE)
            float3x3 TangentSpaceMatrix : COLOR2;
    #endif

    UNITY_VERTEX_INPUT_INSTANCE_ID
    UNITY_VERTEX_OUTPUT_STEREO
};

struct FinalData
{
    half3 Color;
    half Alpha;
};

struct VectorData
{
    half3 NormalWS;
    half3 ViewDirWS;
    half3 LightDirTS;
    half3 ViewDirTS;
};

struct ScreenData
{
    half2 ScreenUV;
    real CameraDepthVal;
    float SceneEyeDepth;
    float Scene01Depth;
    real4 CameraNormalVal;
};

// ---------------------------------------------------------------------------------------------------
// Encapsulate Vertex Function (Universal)
// ---------------------------------------------------------------------------------------------------

float4 TransformToShadowCoordOrHClip(in float3 positionOS, inout VertexInput v)
{
        #if defined (PARTICLE_SHADOWCASTER) && defined (USE_NORMAL)
                return GetShadowPositionHClip(positionOS, v.normal);
        #else
                return TransformObjectToHClip(positionOS);
        #endif
}

void VertexOffset(inout VertexInput v, inout VertexOutput o, in half4x2 customData, in float2 UVChannel[2])
{
        #ifdef _ACTIVEVERTEXOFFSET_ENABLE
                float2 VertexOffsetUV = GetCustomUV(UVChannel[VERTEXOFFSETTEX_UVCHANNEL], _VertexOffsetTexUVAutoOffset, _VertexOffsetTex_ST, _VertexOffsetTexUVTileController, customData);
                half4 VertexOffsetTexColor = SAMPLE_TEXTURE2D_LOD(_VertexOffsetTex , VERTEXOFFSETTEX_WRPAMODE , VertexOffsetUV , 0);
                SetTextureChannels(VertexOffsetTexColor , _VertexOffsetTexSampleChannels);
                half3 VertexOffsetTexValue = half3(0, 0, 0);
                #ifdef _VERTEXOFFSETDIRECTION_NORMAL
                        VertexOffsetTexValue = VertexOffsetTexColor.rgb * GetCustomIntensity(_VertexOffsetIntensity , _VertexOffsetIntensityController , customData) * v.normal.xyz;
                #elif  _VERTEXOFFSETDIRECTION_TANGENT
                        float CalU = (v.uv.x - 0.5) * 2;
                        VertexOffsetTexValue = VertexOffsetTexColor.rgb * GetCustomIntensity(_VertexOffsetIntensity , _VertexOffsetIntensityController , customData) * v.tangent.xyz * CalU;
                #elif  _VERTEXOFFSETDIRECTION_BINORMAL
                        half3 Binormal = cross(v.normal, v.tangent.xyz) * v.tangent.w;
                        float CalV = (v.uv.y - 0.5) * 2;
                        VertexOffsetTexValue = VertexOffsetTexColor.rgb * GetCustomIntensity(_VertexOffsetIntensity , _VertexOffsetIntensityController , customData) * Binormal.xyz * CalV;
                #endif
            #ifdef USE_VERTEXOFFSET_MASK
                    float2 VertexOffsetMaskUV = GetCustomUV(UVChannel[VERTEXOFFSETTEX_UVCHANNEL] , _VertexOffsetMaskUVAutoOffset , _VertexOffsetMask_ST , _VertexOffsetMaskUVTileController , customData);
                    half4 VertexOffsetMaskColor = SAMPLE_TEXTURE2D_LOD(_VertexOffsetMask , VERTEXOFFSETMASK_WRPAMODE , VertexOffsetMaskUV , 0);
                    SetTextureChannels(VertexOffsetMaskColor , _VertexOffsetMaskSampleChannels);
                    half VertexOffsetMaskIntensity = GetCustomIntensity(_VertexOffsetMaskIntensity , _VertexOffsetMaskIntensityController , customData);
                    half3 VertexOffsetMaskValue = VertexOffsetMaskColor.rgb * VertexOffsetMaskIntensity;
                    VertexOffsetTexValue *= VertexOffsetMaskValue;
            #endif
        
                v.vertex.xyz += VertexOffsetTexValue;
                o.vertex = TransformToShadowCoordOrHClip(v.vertex.xyz, v);
        #elif _ACTIVEVERTEXOFFSET_ANIMATIONTEX
                float3 AnimationVert = float3((v.vertexID + 0.5f) * _VertexOffsetTex_TexelSize.x , UVChannel[0].x , UVChannel[1].x);
                float AnimationTime = lerp(_VertexOffsetTex_TexelSize.y,1,frac(GetCustomIntensity(_AnimationPlayableSlider, _AnimationPlayableSliderController, customData))); // UV.y
                float4 AnimationVertexOffsetTexColor = SAMPLE_TEXTURE2D_LOD(_VertexOffsetTex, sampler_point_repeat, float2(AnimationVert[_AnimationTexUVChannel], AnimationTime), 0);
                float4 AnimationNormalOffsetTexColor = SAMPLE_TEXTURE2D_LOD(_AnimationNormalOffsetTex, sampler_point_repeat, float2(AnimationVert[_AnimationTexUVChannel], AnimationTime), 0);
                AnimationVertexOffsetTexColor = _ReClampAnimationVertexVector ? (AnimationVertexOffsetTexColor * 2 - 1) : AnimationVertexOffsetTexColor;
                AnimationNormalOffsetTexColor = _ReClampAnimationVertexVector ? (AnimationNormalOffsetTexColor * 2 - 1) : AnimationNormalOffsetTexColor;
                float3 AnimationVertex = AnimationVertexOffsetTexColor.xyz + v.vertex.xyz * _AnimationVertexVectorMode;
                v.normal = AnimationNormalOffsetTexColor.xyz + v.normal * _AnimationVertexVectorMode;
                o.vertex = TransformToShadowCoordOrHClip(AnimationVertex, v);
        #elif _ACTIVEVERTEXOFFSET_FULLSCREEN
                o.vertex = float4((v.uv.xy * 2 - 1), 0, v.vertex.w);
                #ifdef UNITY_UV_STARTS_AT_TOP
                        o.vertex.y *= -1;
                #endif
        #elif _ACTIVEVERTEXOFFSET_LOCALBILLBOARD
                // Note : Using this function need to be careful with some points.
                // 1. Turn Render Alignment to 'Local'.
                // 2. Turn Render Mode to 'Mesh'.
                // 3. Enable 'Mesh GPU Instancing' to make sure Matrix like unity_WorldToObject would have correct value.
                // Reason: Unity’s particle meshes are pre-baked into world space and cause unity_WorldToObject and unity_ObjectToWorld for particle meshes are identity matrices.
                //         To resolve this we need to re-bake the data into these Matrix through 'Mesh GPU Instance' way.
                float3 CameraToLocal = mul(unity_WorldToObject,float4(_WorldSpaceCameraPos,1)).xyz;
                float3 ViewDir = CameraToLocal;
                ViewDir.y *= 0; 
                ViewDir = normalize(ViewDir);
                float3 UpDir = float3(0,1,0);
                float3 rightDir = normalize(cross(UpDir,ViewDir));
                float3 CenterOffset = v.vertex.xyz;
                float3 FinalVertexPos = (CenterOffset.x * rightDir + CenterOffset.y * UpDir + CenterOffset.z * ViewDir);
                o.vertex = TransformToShadowCoordOrHClip(FinalVertexPos.xyz, v);
        #elif _ACTIVEVERTEXOFFSET_CAMERAZAXISOFFSET
                float VertexZ_Offset = GetCustomIntensity(_VertexOffsetCameraDistance,_VertexOffsetCameraDistanceController,customData);
                float3 positionVS = mul(UNITY_MATRIX_MV, v.vertex).xyz;
                positionVS.z += VertexZ_Offset;
                float3 positionOS = mul(float4(positionVS,v.vertex.w), UNITY_MATRIX_IT_MV).xyz;
                o.vertex = TransformToShadowCoordOrHClip(positionOS , v);
        #else
                o.vertex = TransformToShadowCoordOrHClip(v.vertex.xyz, v);

        #endif
}

void SetMainTexUV(inout VertexOutput o, in half4x2 customData, in float2 UVChannel[2])
{
        #if defined (_MAINTEXMODE_NORMAL) || defined (_MAINTEXMODE_MULTICHANNEL)
    o.uv.xy = GetCustomUV(UVChannel[MAINTEX_UVCHANNEL], _MainTexUVAutoOffset, _MainTex_ST, _MainTexUVTileController,
                          customData);
        #elif defined (_MAINTEXMODE_FLIPBOOK)
                o.uv.xy = MainTexFlipBook_Vert(UVChannel[MAINTEX_UVCHANNEL], _FlipBookSetting.x, _FlipBookSetting.y, GetCustomIntensity(_FlipBookSetting.z , _FlipBookSetting.w, customData));
        #endif
}

void SetDistortTexUV(inout VertexOutput o, in half4x2 customData, in float2 UVChannel[2])
{
        #if defined _ACTIVEDISTORT_ENABLE || _ACTIVEDISTORT_FLOWMAP
                o.DistortTexUVAndDissolveTexUV.xy = GetCustomUV(UVChannel[DISTORTTEX_UVCHANNEL], _DistortTexUVAutoOffset, _DistortTex_ST, _DistortTexUVTileController, customData);
                vertTexSheetCoord(o.DistortTexUVAndDissolveTexUV.xy);
        #endif
}

void SetDissolveTexUV(inout VertexOutput o, in half4x2 customData, in float2 UVChannel[2])
{
        #if defined _ACTIVEDISSOLVE_ENABLE
                o.DistortTexUVAndDissolveTexUV.zw = GetCustomUV(UVChannel[DISSOLVETEX_UVCHANNEL], _DissolveTexUVAutoOffset, _DissolveTex_ST, _DissolveTexUVTileController, customData);
                vertTexSheetCoord(o.DistortTexUVAndDissolveTexUV.zw);
        #endif
}

void SetNormalTexUV(inout VertexOutput o, in half4x2 customData, in float2 UVChannel[2])
{
        #ifdef _ACTIVENORMAL_ENABLE
                o.NormalUVandParallaxUV.xy = GetCustomUV(UVChannel[NORMALTEX_UVCHANNEL], _NormalTexUVAutoOffset, _NormalTex_ST, _NormalTexUVTileController, customData);
                vertTexSheetCoord(o.NormalUVandParallaxUV.xy);
        #endif
}

void SetParallaxTexUV(inout VertexOutput o, in half4x2 customData, in float2 UVChannel[2])
{
        #if defined _ACTIVEPARALLAX_ENABLE || _ACTIVEPARALLAX_STEEP
                o.NormalUVandParallaxUV.zw = GetCustomUV(UVChannel[PARALLAXTEX_UVCHANNEL], _ParallaxTexUVAutoOffset, _ParallaxTex_ST, _ParallaxTexUVTileController, customData);
                vertTexSheetCoord(o.NormalUVandParallaxUV.zw);
        #endif
}

void SetEmissionTexUV(inout VertexOutput o, in half4x2 customData, in float2 UVChannel[2])
{
        #ifdef _ACTIVEEMISSION_ENABLE
                o.EmissionUV.xy = GetCustomUV(UVChannel[EMISSIONTEX_UVCHANNEL], _EmissionTexUVAutoOffset, _EmissionTex_ST, _EmissionTexUVTileController, customData);
                vertTexSheetCoord(o.EmissionUV.xy);
        #endif
}

void SetTangentSpace(inout VertexInput v, inout VertexOutput o)
{
        #ifdef USE_TANGENT
                float3 binormal = cross(normalize(v.normal), normalize(v.tangent).xyz) * v.tangent.w;
                float3x3 ObjSpaceToTangentSpace = float3x3(v.tangent.xyz, binormal, v.normal);
            #ifdef USE_TANGENTSPACE_LIGHTDIR
                o.LightDir = mul(ObjSpaceToTangentSpace, ObjSpaceLightDir());
            #endif
            #ifdef USE_TANGENTSPACE_VIEWDIR
                o.ViewDir = mul(ObjSpaceToTangentSpace, ObjSpaceViewDir(v.vertex)); 
            #endif
        #endif
}

// ---------------------------------------------------------------------------------------------------
// Encapsulate Fragment Function (Universal)
// ---------------------------------------------------------------------------------------------------

void ApplyScreenData(inout ScreenData sd, inout VertexOutput o)
{
        #ifdef USE_SCREENUV
                sd.ScreenUV = o.vertex.xy / _ScaledScreenParams.xy;
        #endif

        #ifdef USE_CAMERA_DEPTH
                sd.CameraDepthVal = SAMPLE_TEXTURE2D_X(_CameraDepthTexture, sampler_CameraDepthTexture, sd.ScreenUV).r;
                sd.SceneEyeDepth = LinearEyeDepth(sd.CameraDepthVal, _ZBufferParams);
                sd.Scene01Depth = Linear01Depth(sd.CameraDepthVal, _ZBufferParams);
        #endif

        #ifdef USE_CAMERA_NORMAL
                sd.CameraNormalVal = SAMPLE_TEXTURE2D(_CameraNormalsTexture, sampler_linear_clamp, sd.ScreenUV);
        #endif
}

void ApplyDecalUV(inout VertexOutput o, in half4x2 customData, inout ScreenData sd)
{
        #ifdef _PARTICLEMODE_DECAL
                float viewDepth = sd.Scene01Depth * _ProjectionParams.z;
                float3 viewPos = o.DecalRay.xyz * viewDepth / o.DecalRay.z ;
                float4 worldPos = mul (unity_CameraToWorld , float4(viewPos,1 ) ) ;
                float3 objectPos = mul (unity_WorldToObject , worldPos).xyz;
                clip(float3(0.5,0.5,0.5) - abs(objectPos));
                float3 DecalUV = objectPos + 0.5f;
       
                #ifdef _MAINTEXMODE_FLIPBOOK
                        o.uv.xy = GetCustomUV(DecalUV.xz, _MainTexUVAutoOffset, _MainTex_ST, _MainTexUVTileController,customData);
                        o.uv.xy = MainTexFlipBook_Frag(DecalUV.xz,_FlipBookSetting.x, _FlipBookSetting.y, GetCustomIntensity(_FlipBookSetting.z , _FlipBookSetting.w, customData));
                #else
                        o.uv.xy = GetCustomUV(DecalUV.xz, _MainTexUVAutoOffset, _MainTex_ST, _MainTexUVTileController,customData);
                #endif
                        o.uv.zw = DecalUV.xz; 
                #if defined (_ACTIVEDISTORT_ENABLE) || defined (_ACTIVEDISTORT_FLOWMAP)
                        o.DistortTexUVAndDissolveTexUV.xy = GetCustomUV(DecalUV.xz, _DistortTexUVAutoOffset, _DistortTex_ST, _DistortTexUVTileController, customData);
                #endif
                #ifdef _ACTIVEDISSOLVE_ENABLE
                        o.DistortTexUVAndDissolveTexUV.zw = GetCustomUV(DecalUV.xz, _DissolveTexUVAutoOffset, _DissolveTex_ST, _DissolveTexUVTileController, customData);
                #endif
                #ifdef _ACTIVENORMAL_ENABLE
                        o.NormalUVandParallaxUV.xy = GetCustomUV(DecalUV.xz, _NormalTexUVAutoOffset, _NormalTex_ST, _NormalTexUVTileController, customData);
                #endif
                #if defined (_ACTIVEPARALLAX_ENABLE) || defined (_ACTIVEPARALLAX_STEEP)
                        o.NormalUVandParallaxUV.zw = GetCustomUV(DecalUV.xz, _ParallaxTexUVAutoOffset, _ParallaxTex_ST, _ParallaxTexUVTileController, customData);
                #endif
                #ifdef _ACTIVEEMISSION_ENABLE
                        o.EmissionUV.xy = GetCustomUV(DecalUV.xz, _EmissionTexUVAutoOffset, _EmissionTex_ST, _EmissionTexUVTileController, customData);
                #endif
        #endif
}

void ApplyWorldNormalMaskToDecal(inout FinalData fd, inout ScreenData sd)
{
        #ifdef USE_DECAL_WORLDNORMALMASK
                half3 DecalWorldNormalMask = abs(sd.CameraNormalVal).xyz * _DecalWorldNormalMaskAxis.a + (1-_DecalWorldNormalMaskAxis.a) * sd.CameraNormalVal.xyz;
                DecalWorldNormalMask *= _DecalWorldNormalMaskAxis.xyz;
                half DecalWorldNormalMaskVal = max(DecalWorldNormalMask.z,max(DecalWorldNormalMask.x,DecalWorldNormalMask.y));
                fd.Alpha *= DecalWorldNormalMaskVal;
        #endif
}

void ApplyPolarCoord(inout VertexOutput o, in half4x2 customData)
{
        #ifdef USE_POLARCOORD
                float2 PolarCoordUV = ToPolar(o.uv.zw);
        #endif

        #ifdef _MAINTEXMODE_POLARCOORD
                o.uv.xy = GetCustomUV(PolarCoordUV, _MainTexUVAutoOffset, _MainTex_ST, _MainTexUVTileController, customData);
        #endif

        #ifdef _ACTIVEDISTORT_POLARCOORD
                o.DistortTexUVAndDissolveTexUV.xy = GetCustomUV(PolarCoordUV, _DistortTexUVAutoOffset, _DistortTex_ST, _DistortTexUVTileController, customData);
        #endif

        #ifdef _ACTIVEDISSOLVE_POLARCOORD
                o.DistortTexUVAndDissolveTexUV.zw = GetCustomUV(PolarCoordUV , _DissolveTexUVAutoOffset , _DissolveTex_ST , _DissolveTexUVTileController , customData);
        #endif

        #ifdef _ACTIVEEMISSION_POLARCOORD
                o.EmissionUV.xy = GetCustomUV(PolarCoordUV, _EmissionTexUVAutoOffset, _EmissionTex_ST, _EmissionTexUVTileController, customData);
        #endif
}

void ApplyScreenUV(inout VertexOutput o, in half4x2 customData, in ScreenData sd)
{
        #ifdef _MAINTEXMODE_SCREENUV
                o.uv.xy = GetCustomUV(sd.ScreenUV, _MainTexUVAutoOffset, _MainTex_ST, _MainTexUVTileController, customData);
        #endif

        #ifdef _ACTIVEDISTORT_SCREENUV
                o.DistortTexUVAndDissolveTexUV.xy = GetCustomUV(sd.ScreenUV, _DistortTexUVAutoOffset, _DistortTex_ST, _DistortTexUVTileController, customData);
        #endif

        #ifdef _ACTIVEDISSOLVE_SCREENUV
                o.DistortTexUVAndDissolveTexUV.zw = GetCustomUV(sd.ScreenUV , _DissolveTexUVAutoOffset , _DissolveTex_ST , _DissolveTexUVTileController , customData);
        #endif

        #ifdef _ACTIVEEMISSION_SCREENUV
                o.EmissionUV.xy = GetCustomUV(sd.ScreenUV, _EmissionTexUVAutoOffset, _EmissionTex_ST, _EmissionTexUVTileController, customData);
        #endif
}

void PrepareLightVectorData(inout VertexOutput o, inout VectorData vd)
{
        #ifdef USE_NORMAL
                o.normal = normalize(o.normal);
                vd.NormalWS = o.normal;
        #endif
        #ifdef USE_WS_VIEWDIR
                vd.ViewDirWS = normalize(_WorldSpaceCameraPos.xyz - o.posWorld);
        #endif
        #ifdef USE_TANGENTSPACE_VIEWDIR
                vd.ViewDirTS = normalize(o.ViewDir);
        #endif
        #ifdef USE_TANGENTSPACE_LIGHTDIR
                vd.LightDirTS = normalize(o.LightDir);
        #endif
}

void ApplyParallaxUV(inout VertexOutput o, inout half2 MainTexUV, in half4x2 customData, inout VectorData vd)
{
        #ifdef _ACTIVEPARALLAX_ENABLE
                half4 ParallaxTexColor = SAMPLE_TEXTURE2D(_ParallaxTex, PARALLAXTEX_WRAPMODE, o.NormalUVandParallaxUV.zw);
                half ParallaxTexValue = ParallaxTexColor[_ParallaxTexChannel];
                half ParallaxIntensity = GetCustomIntensity(_ParallaxIntensity, _ParallaxIntensityController,customData);
                half ParallaxScaledHeight = -ParallaxTexValue * ParallaxIntensity;
                half3 ParallaxViewDir = vd.ViewDirTS;
                ParallaxViewDir.z += 0.5f; 
                half2 ParallaxOffsetUV = ParallaxViewDir.xy / ParallaxViewDir.z * ParallaxScaledHeight;
        #elif _ACTIVEPARALLAX_STEEP
                // Ref - https://learnopengl.com/Advanced-Lighting/Parallax-Mapping
                half4 ParallaxTexColor = SAMPLE_TEXTURE2D(_ParallaxTex, PARALLAXTEX_WRAPMODE, o.NormalUVandParallaxUV.zw);
                half ParallaxTexValue = ParallaxTexColor[_ParallaxTexChannel];
                half ParallaxIntensity = GetCustomIntensity(_ParallaxIntensity, _ParallaxIntensityController,customData);
                const float NumLayers = _HeightSampleSteps; 
                float LayerDepth = 1 / NumLayers;
                float CurrentLayerDepth = 0;
                #ifdef _PARTICLEMODE_DECAL
                        half3 ParallaxViewDir = normalize(mul((float3x3)unity_WorldToObject, _WorldSpaceCameraPos.xyz - unity_ObjectToWorld._m03_m13_m23));
                        float2 ParallaxOffsetStepVal = ParallaxViewDir.xz * ParallaxIntensity ;
                #else
                        half3 ParallaxViewDir = normalize(mul((float3x3)unity_WorldToObject, _WorldSpaceCameraPos.xyz - o.posWorld.xyz));
                        float2 ParallaxOffsetStepVal = ParallaxViewDir.xy * ParallaxIntensity ;
                #endif
                float2 DeltaTexCoords = ParallaxOffsetStepVal / NumLayers;
                float2 CurrentTexCoords = GetCustomUV(o.uv.zw , _ParallaxTexUVAutoOffset , _ParallaxTex_ST , _ParallaxTexUVTileController , customData); 
                float NoiseStepTexValue = SAMPLE_TEXTURE2D_LOD(_ParallaxNoiseStepTex , PARALLAX_NOISESTEPTEX_WRAPMODE , TRANSFORM_TEX(o.uv.zw , _ParallaxNoiseStepTex) , 0)[_ParallaxNoiseTexChannel];
                NoiseStepTexValue = lerp(1, NoiseStepTexValue, _ParallaxNoiseIntensity);
                float CurrentDepthMapValue = ParallaxTexValue;
               
                for(int i = 0 ; i < NumLayers ; i++) {
                    if(CurrentLayerDepth < CurrentDepthMapValue) {
                        CurrentTexCoords -= DeltaTexCoords * NoiseStepTexValue;
                        CurrentDepthMapValue = SAMPLE_TEXTURE2D(_ParallaxTex, PARALLAXTEX_WRAPMODE, CurrentTexCoords)[_ParallaxTexChannel];
                        CurrentLayerDepth += LayerDepth;
                    }
                }
    
                float2 PreTexCoords = CurrentTexCoords + DeltaTexCoords;
                float BeforeDepth = SAMPLE_TEXTURE2D(_ParallaxTex, PARALLAXTEX_WRAPMODE, PreTexCoords)[_ParallaxTexChannel] - CurrentLayerDepth + LayerDepth;
                float AfterDepth = CurrentDepthMapValue - CurrentLayerDepth;
                float Weight = AfterDepth / (AfterDepth - BeforeDepth);
                float2 FinalTexCoords = PreTexCoords * Weight + CurrentTexCoords * (1 - Weight);
                float2 Parallax_OutputUV = FinalTexCoords;
        #endif

    //* Set UV
        #ifdef _ACTIVEPARALLAX_ENABLE
                // MainTex UV
                if((_ParallaxImpactTarget & 1 << 0) != 0)
                        MainTexUV += ParallaxOffsetUV;

                #ifdef USE_DISSOLVE_UV
                // Dissolve UV
                if((_ParallaxImpactTarget & 1 << 1) != 0)
                        o.DistortTexUVAndDissolveTexUV.zw += ParallaxOffsetUV;
                #endif

                #ifdef USE_NORMALMAP_UV
                // NormalMap UV
                if((_ParallaxImpactTarget & 1 << 2) != 0)
                        o.NormalUVandParallaxUV.xy += ParallaxOffsetUV;
                #endif

                #ifdef USE_DISTORT_UV
                // Distort UV
                if((_ParallaxImpactTarget & 1 << 3) != 0)
                        o.DistortTexUVAndDissolveTexUV.xy += ParallaxOffsetUV;
                #endif

                #ifdef USE_EMISSION_UV
                // Emission UV
                if((_ParallaxImpactTarget & 1 << 4) != 0)
                        o.EmissionUV.xy += ParallaxOffsetUV;
                #endif
        #elif _ACTIVEPARALLAX_STEEP
                // MainTex UV
                if((_ParallaxImpactTarget & 1 << 0) != 0)
                        MainTexUV = GetCustomUV(Parallax_OutputUV, _MainTexUVAutoOffset, _MainTex_ST, _MainTexUVTileController, customData);
                
                #ifdef USE_DISSOLVE_UV
                // Dissolve UV
                if((_ParallaxImpactTarget & 1 << 1) != 0)
                        o.DistortTexUVAndDissolveTexUV.zw = GetCustomUV(Parallax_OutputUV, _DissolveTexUVAutoOffset , _DissolveTex_ST , _DissolveTexUVTileController , customData);
                #endif

                #ifdef USE_NORMALMAP_UV
                // NormalMap UV
                if((_ParallaxImpactTarget & 1 << 2) != 0)
                        o.NormalUVandParallaxUV.xy = GetCustomUV(Parallax_OutputUV, _NormalTexUVAutoOffset , _NormalTex_ST , _NormalTexUVTileController , customData);
              
                #endif

                #ifdef USE_DISTORT_UV
                // Distort UV
                if((_ParallaxImpactTarget & 1 << 3) != 0)
                        o.DistortTexUVAndDissolveTexUV.xy = GetCustomUV(Parallax_OutputUV, _DistortTexUVAutoOffset , _DistortTex_ST , _DistortTexUVTileController , customData);
             
                #endif

                #ifdef USE_EMISSION_UV
                // Emission UV
                if((_ParallaxImpactTarget & 1 << 4) != 0)
                        o.EmissionUV.xy = GetCustomUV(Parallax_OutputUV, _EmissionTexUVAutoOffset , _EmissionTex_ST , _EmissionTexUVTileController , customData);
             
                #endif

        #endif
}

void ApplyDistortUV(inout VertexOutput o, inout half2 MainTexUV, in half4x2 customData, in ScreenData sd)
{
        #ifdef _ACTIVEDISTORT_ENABLE
                half4 DistortTexColor = SAMPLE_TEXTURE2D(_DistortTex, DISTORTTEX_WRAPMODE, o.DistortTexUVAndDissolveTexUV.xy);
                half2 DistortTexValue = DistortTexColor[_DistortChannel] * (o.uv.zw * 2 - 1);
                DistortTexValue = DistortTexValue * GetCustomIntensity(_DistortIntensity, _DistortIntensityController , customData);
        #elif _ACTIVEDISTORT_FLOWMAP
                half4 DistortTexColor = SAMPLE_TEXTURE2D(_DistortTex, DISTORTTEX_WRAPMODE, o.DistortTexUVAndDissolveTexUV.xy);
                // Cause reClamp [0,1] to [-1,1] is unpredictable on gray color(value) when sample from every texture with various compression.
                // so here decide to minus B channel's gray color.
                DistortTexColor.rg = _EnableHighPrecisionFlowMap ? (DistortTexColor.rg - DistortTexColor.bb) * 2 : (DistortTexColor.rg * 2 - 1);
                // Multiply -0.1f to make more straighter on visualization also decrease the intensity. 
                half2 DistortTexValue = DistortTexColor.rg * -0.1f;
                // Why need to multiply negative value again here?
                // For most common usage , people will use software to create flow-map but there G channel would be invert. (white is down, black is up)  
                DistortTexValue.y *= _ReverseFlowMap_G_Color ? -1 : 1;
                DistortTexValue = DistortTexValue * GetCustomIntensity(_DistortIntensity, _DistortIntensityController , customData);
        #elif _ACTIVEDISTORT_POLARCOORD
                half4 DistortTexColor = SAMPLE_TEXTURE2D(_DistortTex, DISTORTTEX_WRAPMODE, o.DistortTexUVAndDissolveTexUV.xy);
                half2 DistortTexValue = DistortTexColor[_DistortChannel] * (o.uv.zw * 2 - 1);
                DistortTexValue = DistortTexValue * GetCustomIntensity(_DistortIntensity, _DistortIntensityController , customData);
        #elif _ACTIVEDISTORT_SCREENUV
                half4 DistortTexColor = SAMPLE_TEXTURE2D(_DistortTex, DISTORTTEX_WRAPMODE, o.DistortTexUVAndDissolveTexUV.xy);
                half2 DistortTexValue = DistortTexColor[_DistortChannel] * (sd.ScreenUV * 2 - 1);
                DistortTexValue = DistortTexValue * GetCustomIntensity(_DistortIntensity, _DistortIntensityController , customData);
        #endif

    //* Set UV
        #ifdef USE_DISTORT
                // MainTex
                    if((_DistortImpactTarget & 1 << 0) != 0)
                        MainTexUV += DistortTexValue;
            
                // Dissolve
                #ifdef USE_DISSOLVE_UV
                    if((_DistortImpactTarget & 1 << 1) != 0) 
                        o.DistortTexUVAndDissolveTexUV.zw += DistortTexValue;
                #endif

    // NormalMap
                #ifdef USE_NORMALMAP_UV
                    if((_DistortImpactTarget & 1 << 2) != 0)
                        o.NormalUVandParallaxUV.xy += DistortTexValue;
                #endif

    // Emission
                #ifdef USE_EMISSION_UV
                    if((_DistortImpactTarget & 1 << 3) != 0) 
                        o.EmissionUV.xy += DistortTexValue;
                #endif
        #endif
}

void ApplyMainTexMode_MultiChannel(inout FinalData fd, inout VertexOutput o, in half4 MainTexColor, in half4x2 customData)
{
        #ifdef _MAINTEXMODE_MULTICHANNEL
                    fd.Color = o.vertexColor.rgb * _MainTexColor.rgb * MainTexColor.r;
                    fd.Alpha = MainTexColor.a * o.vertexColor.a ;
                    fd.Alpha *= saturate( MainTexColor.g + GetCustomIntensity(_MainTexG_Intensity , _MainTexGIntensityController , customData));
                    fd.Alpha *= saturate( MainTexColor.b + GetCustomIntensity(_MainTexB_Intensity , _MainTexBIntensityController , customData));
        #endif
}

void ApplyEmission(inout FinalData fd, inout VertexOutput o, in half4x2 customData)
{
        #ifdef USE_EMISSION
                    half4 EmissionTexColor = SAMPLE_TEXTURE2D(_EmissionTex, EMISSIONTEX_WRPAMODE, o.EmissionUV.xy);
                    half  EmissionIntensity = GetCustomIntensity(_EmissionIntensity, _EmissionIntensityController , customData);
                    half3 EmissionColor = EmissionTexColor.rgb * _EmissionColor.rgb;
                    switch (_EmissionColorMode)
                    {
                    case 0: // Additive
                                half3 EmissionOutputColor_Additive = fd.Color + EmissionColor * o.vertexColor.rgb * EmissionIntensity;
                                fd.Color = EmissionOutputColor_Additive;
                            break;
                    case 1: // Multiply
                                half3 EmissionOutputColor_Multiply = fd.Color * EmissionColor * o.vertexColor.rgb * EmissionIntensity;
                                fd.Color = EmissionOutputColor_Multiply;
                            break;
                    case 2: // Replace
                                half3 EmissionOutputColor_Replace = lerp(fd.Color, EmissionColor, EmissionTexColor.a * EmissionIntensity);
                                fd.Color = EmissionOutputColor_Replace;
                            break;
                    }
        #endif
}

void ApplyDoubleSideColor(inout FinalData fd, inout VertexOutput o, in half facing, in half4 MainTexColor)
{
        #ifdef USE_DOUBLE_SIDE_COLOR
            #ifdef _ACTIVESIDEFACECOLOR_REPLACE
                    if(_ChangeSide)
                        fd.Color = facing > 0 ? MainTexColor.rgb * _SideColor.rgb * _SideColor.a : MainTexColor.rgb * o.vertexColor.rgb;
                    else
                        fd.Color = facing > 0 ? MainTexColor.rgb * o.vertexColor.rgb : MainTexColor.rgb * _SideColor.rgb * _SideColor.a;
                    fd.Color *= _MainTexColor.rgb.rgb;
            #elif _ACTIVESIDEFACECOLOR_ADD
                    if(_ChangeSide)
                        fd.Color += facing > 0 ? _SideColor.rgb * _SideColor.a : 0;
                    else
                        fd.Color += facing > 0 ? 0 : _SideColor.rgb * _SideColor.a;
            #elif _ACTIVESIDEFACECOLOR_MULTIPLY
                    if(_ChangeSide)
                        fd.Color = facing > 0 ? fd.Color * _SideColor.rgb * _SideColor.a : fd.Color;
                    else
                        fd.Color = facing > 0 ? fd.Color : fd.Color * _SideColor.rgb * _SideColor.a;
            #endif
        #endif
}

void ApplyExternalAlphaTemplate(inout FinalData fd, inout VertexOutput o, in half4x2 customData)
{
        #ifdef _ACTIVEEXTERNALALPHA_LINE
                float LineAlphaRange = sin((o.uv[_LineAlphaMode] + GetCustomIntensity(_ExternalAlphaRange, _ExternalAlphaRangeController, customData)) * PI);
                LineAlphaRange = saturate(LineAlphaRange);
                LineAlphaRange = pow(LineAlphaRange , GetCustomIntensity(_ExternalAlphaPower, _ExternalAlphaPowerController, customData));
                LineAlphaRange = saturate(LineAlphaRange * GetCustomIntensity(_ExternalAlphaIntensity, _ExternalAlphaIntensityController, customData));
                LineAlphaRange = _InverseAlphaVal ? (1-LineAlphaRange) : LineAlphaRange;
                fd.Alpha *= LineAlphaRange;
        #elif _ACTIVEEXTERNALALPHA_GRADIENT
                float GradientAlphaRange = o.uv[_LineAlphaMode] + GetCustomIntensity(_ExternalAlphaRange, _ExternalAlphaRangeController, customData);
                GradientAlphaRange = saturate(GradientAlphaRange);
                GradientAlphaRange = pow(GradientAlphaRange , GetCustomIntensity(_ExternalAlphaPower, _ExternalAlphaPowerController, customData));
                GradientAlphaRange = saturate(GradientAlphaRange * GetCustomIntensity(_ExternalAlphaIntensity, _ExternalAlphaIntensityController, customData));
                GradientAlphaRange = _InverseAlphaVal ? GradientAlphaRange : (1-GradientAlphaRange);
                fd.Alpha *= GradientAlphaRange;
        #elif _ACTIVEEXTERNALALPHA_CIRCLE
                float CircelAlphaDistance = distance(o.uv.zw , float2(0.5f, 0.5f));
                float CircelAlphaRange = sin((CircelAlphaDistance + GetCustomIntensity(_ExternalAlphaRange, _ExternalAlphaRangeController, customData)) * PI);
                CircelAlphaRange = saturate(CircelAlphaRange);
                CircelAlphaRange = pow(CircelAlphaRange , GetCustomIntensity(_ExternalAlphaPower, _ExternalAlphaPowerController, customData));
                CircelAlphaRange = saturate(CircelAlphaRange * GetCustomIntensity(_ExternalAlphaIntensity, _ExternalAlphaIntensityController, customData));
                CircelAlphaRange = _InverseAlphaVal ? (1-CircelAlphaRange) : CircelAlphaRange;
                fd.Alpha *= CircelAlphaRange;
        #elif _ACTIVEEXTERNALALPHA_ROUND
                float RoundAlphaDistance = distance(o.uv.zw , float2(0.5f, 0.5f));
                float RoundAlphaRange = RoundAlphaDistance + GetCustomIntensity(_ExternalAlphaRange, _ExternalAlphaRangeController, customData);
                RoundAlphaRange = saturate(RoundAlphaRange);
                RoundAlphaRange = pow(RoundAlphaRange , GetCustomIntensity(_ExternalAlphaPower, _ExternalAlphaPowerController, customData));
                RoundAlphaRange = saturate(RoundAlphaRange * GetCustomIntensity(_ExternalAlphaIntensity, _ExternalAlphaIntensityController, customData));
                RoundAlphaRange = _InverseAlphaVal ? RoundAlphaRange : (1-RoundAlphaRange);
                fd.Alpha *= RoundAlphaRange;
        #endif
}

void ApplyNormalMap(inout FinalData fd, inout VertexOutput o, in half4x2 customData, in VectorData vd)
{
        #ifdef _ACTIVENORMAL_ENABLE
                half4 SampleNormalTex = SAMPLE_TEXTURE2D(_NormalTex, NORMALTEX_WRAPMODE, o.NormalUVandParallaxUV.xy);
                half3 NormalValue = UnpackNormal(SampleNormalTex);
                NormalValue.xy *= GetCustomIntensity(_NormalIntensity,_NormalIntensityController,customData);
                o.normal = normalize(NormalValue);
                half LdotN = dot(vd.LightDirTS, o.normal);
                LdotN = LdotN * _NormalBounceLightRange + 1 - _NormalBounceLightRange;
                LdotN = saturate(LdotN);
                fd.Color *= LdotN;
        #endif
}

void ApplyDissolve(inout FinalData fd, inout VertexOutput o, in half4x2 customData)
{
         #if defined USE_DISSOLVE
                half4 DissolveTexColor = SAMPLE_TEXTURE2D(_DissolveTex, DISSOLVETEX_WRAPMODE, o.DistortTexUVAndDissolveTexUV.zw);
                half DissolveTexValue = DissolveTexColor[_DissolveChannel];
                half DissolveIntensity = GetCustomIntensity(_DissolveIntensity , _DissolveIntensityController , customData);
                half DissolveRange = DissolveTexValue + DissolveIntensity;
    
                switch (_DissolveCalculateMode) {
                    case 0 : // Subtract (Normal mode)
                        DissolveRange = saturate(DissolveRange);
                        break;
                    case 1 : // Erosion
                        DissolveRange = saturate((DissolveRange-0.99f) / (DissolveTexValue * (1-saturate(DissolveIntensity*0.5f)))); 
                        break;
                }
    
                // Hard Clip
                if(_ActiveHardClipDissolve) 
                    DissolveRange = step(1, DissolveRange);
    
                fd.Alpha *= DissolveRange;
    
                #ifdef USE_DISSOLVE_RIMCLIP
                    half GetRimDissolveRange = GetCustomIntensity(_RimDissolveRange ,_RimDissolveRangeController ,customData );
                    half RimDissolve = _ActiveHardClipDissolve ? step(DissolveTexValue ,saturate(1-saturate(DissolveIntensity) + GetRimDissolveRange)) : 1 - saturate(DissolveTexValue + DissolveIntensity - GetRimDissolveRange);
                     
                    switch (_RimDissolveColorMode) {
                        case 0: // Additive
                            fd.Color += RimDissolve * _RimDissolveColor.rgb * _RimDissolveColor.a;
                            break;
                        case 1: // Multiply
                            fd.Color *= lerp(1 ,RimDissolve * _RimDissolveColor.rgb * _RimDissolveColor.a , RimDissolve);
                            break;
                        case 2: // Replace
                            fd.Color = lerp(fd.Color ,RimDissolve * _RimDissolveColor.rgb * _RimDissolveColor.a , RimDissolve);
                            break;
                    }
                #endif
        #endif
}

void ApplyFresnel(inout FinalData fd, inout VertexOutput o, in half4x2 customData, in VectorData vd, in half facing)
{
        #ifdef _ACTIVEFRESNEL_ENABLE

                half3 FacingNormal = facing > 0 ? o.normal : - o.normal;
                half3 FresnelNormal = lerp(o.normal, FacingNormal, _FlipInnerFaceNormal);
    
                #ifdef USE_TANGENTSPACE_VIEWDIR
                    half FresnelVal = dot(vd.ViewDirTS , FresnelNormal);
                #else
                    half FresnelVal = dot(vd.ViewDirWS , FresnelNormal);
                #endif

                    FresnelVal = FresnelVal + GetCustomIntensity(_FresnelRange , _FresnelRangeController , customData);
                    FresnelVal = saturate(FresnelVal);
                    FresnelVal = pow(FresnelVal, GetCustomIntensity(_FresnelPower , _FresnelPowerController , customData));
        
                    if((_FresnelFunction & (1 << 0)) != 0)
                    {
                        half ColorRange = _FlipFresnelColorRange ? 1-FresnelVal : FresnelVal; 
                        fd.Color = lerp(_FresnelColor.rgb * o.vertexColor.a , fd.Color , ColorRange);
                    }
                    
                    if((_FresnelFunction & (1 << 1)) != 0)
                    {
                        half AlphaRange = _FlipFresnelAlphaRange ? 1-FresnelVal : FresnelVal;
                        fd.Alpha *= AlphaRange * _FresnelColor.a * o.vertexColor.a;
                    }
        #endif
}

void ApplyScreenDistortion(inout FinalData fd, inout VertexOutput o, in half4 MainTexColor, in half4x2 customData, in ScreenData sd)
{
        #ifdef _PARTICLEMODE_SCREENDISTORTION
                float2 RemapGrabPassUV = o.uv.zw;
                RemapGrabPassUV = _DistortCenterUV ? RemapGrabPassUV * 2 - 1 : RemapGrabPassUV ;
                half2 GrabPassUV = fd.Color.rg * RemapGrabPassUV; 

                #ifdef _GRABPASSTEXMODE_BUMPTEX
                    GrabPassUV = (MainTexColor.ga - (fd.Color.rr * 0.5f)) * RemapGrabPassUV;
                    fd.Alpha = o.vertexColor.a;
                #else
                    GrabPassUV *= fd.Alpha;
                #endif

                GrabPassUV = GrabPassUV * GetCustomIntensity(_GrabPassIntensity, _GrabPassIntensityController, customData) * o.vertexColor.a + sd.ScreenUV;
                GrabPassUV = saturate(GrabPassUV);
                float3 GrabPassFinalColor = SAMPLE_TEXTURE2D(_CameraOpaqueTexture, MAINTEX_WRAPMODE, GrabPassUV).rgb;

                fd.Color = GrabPassFinalColor;
        #endif
}

void ApplyRampColor(inout FinalData fd)
{
        #ifdef _ACTIVERAMP_ENABLE
                half3 PreColor = fd.Color * fd.Alpha;
                half SaturateColor = dot(PreColor.rgb, float3(0.3, 0.59, 0.11));
                half4 RampSampleColor = SAMPLE_TEXTURE2D(_RampTex, sampler_linear_clamp, SaturateColor.rr);
                fd.Color = RampSampleColor.rgb;
                fd.Color *= _EnhanceRampColor;
        #endif
}

void ApplyAlphaClip(inout FinalData fd, in half4x2 customData)
{
        #ifdef _ACTIVEALPHACLIP_HARDCLIP
                half CutValue = GetCustomIntensity(_AlphaClipRange, _AlphaClipRangeController, customData);
                fd.Alpha = step(CutValue ,clamp(fd.Alpha,0,0.99f));
                clip(fd.Alpha - CutValue);
        #elif _ACTIVEALPHACLIP_SMOOTHCLIP
                half CutValue = GetCustomIntensity(_AlphaClipRange, _AlphaClipRangeController, customData);
                fd.Alpha = InverseLerp(CutValue,1,fd.Alpha);
                clip(fd.Alpha - CutValue);
        #endif
}

void ApplySoftParticle(inout FinalData fd, inout VertexOutput o, inout ScreenData sd)
{
        #ifdef _ACTIVESOFTPARTICLE_ENABLE
                // Here we take the vertex.z from non-linear to linear.
                float ObjectDepth = LinearEyeDepth(o.vertex.z , _ZBufferParams);
                half SoftAlpha = saturate((sd.SceneEyeDepth - ObjectDepth) / _SoftRange);
                fd.Alpha *= SoftAlpha;
        #endif
}

void ApplyColorAdjustment(inout FinalData fd, in half4x2 customData)
{
        #ifdef _ACTIVECOLORADJUSTMENT_ENABLE
                if((_ColorAdjustmentLayerMask & (1 << 0)) != 0)
                    fd.Color = HueShift(fd.Color , GetCustomIntensity(_HUEShiftVal, _HUEShiftValController, customData)).xyz;
                if((_ColorAdjustmentLayerMask & (1 << 1)) != 0)
                    fd.Color = ColorSaturation(fd.Color , GetCustomIntensity(_SaturationVal, _SaturationValController, customData));
                if((_ColorAdjustmentLayerMask & (1 << 2)) != 0)
                    fd.Color = ColorContrast(fd.Color , GetCustomIntensity(_ContrastVal, _ContrastValController, customData));
                if((_ColorAdjustmentLayerMask & (1 << 3)) != 0)
                    fd.Color = AntiShineGlow(fd.Color , GetCustomIntensity(_AntiShineGlowVal, _AntiShineGlowValController, customData));
        #endif
}

void ApplyDithering(inout FinalData fd, inout VertexOutput o)
{
    float2 uv = o.vertex.xy / _ScaledScreenParams.xy * _ScreenParams.xy;
    float DITHER_THRESHOLDS[16] =
    {
        1.0 / 17.0, 9.0 / 17.0, 3.0 / 17.0, 11.0 / 17.0,
        13.0 / 17.0, 5.0 / 17.0, 15.0 / 17.0, 7.0 / 17.0,
        4.0 / 17.0, 12.0 / 17.0, 2.0 / 17.0, 10.0 / 17.0,
        16.0 / 17.0, 8.0 / 17.0, 14.0 / 17.0, 6.0 / 17.0
    };
    uint index = (uint(uv.x) % 4) * 4 + uint(uv.y) % 4;
    clip(fd.Alpha - DITHER_THRESHOLDS[index]);
}

#endif
