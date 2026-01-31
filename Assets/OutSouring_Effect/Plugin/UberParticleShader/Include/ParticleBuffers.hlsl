#ifndef PARTICLE_BUFFERS
#define PARTICLE_BUFFERS

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
// ---------------------------------------------------------------------------------------------------
// Declare Properties (for Texture 、 SamplerState)
// ---------------------------------------------------------------------------------------------------
     // Shared SamplerState
     SamplerState sampler_linear_repeat, sampler_linear_clamp, sampler_point_repeat, sampler_point_clamp;

     // MainTex Properties
     Texture2D _MainTex;
     SamplerState sampler_MainTex;

     // Distort Properties 
     Texture2D _DistortTex;
     SamplerState sampler_DistortTex;

     // Dissolve Properties
     Texture2D _DissolveTex;
     SamplerState sampler_DissolveTex;

     // Emission Properties
     Texture2D _EmissionTex;
     SamplerState sampler_EmissionTex;

     // Ramp Properties
     Texture2D _RampTex;
     SamplerState sampler_RampTex;

     // Normal Properties
     Texture2D _NormalTex;
     SamplerState sampler_NormalTex;

     // Parallax Properties
     Texture2D _ParallaxTex, _ParallaxNoiseStepTex;
     SamplerState sampler_ParallaxTex, sampler_ParallaxNoiseStepTex;

     // Vertex-Offset Properties
     Texture2D _VertexOffsetTex , _VertexOffsetMask , _AnimationNormalOffsetTex;
     SamplerState sampler_VertexOffsetTex , sampler_VertexOffsetMask;

     // Global Shared Properties
     TEXTURE2D_X_FLOAT (_CameraOpaqueTexture);
     TEXTURE2D_X_FLOAT (_CameraDepthTexture);
     TEXTURE2D_X_FLOAT (_CameraNormalsTexture);
     SamplerState sampler_CameraDepthTexture;

// ---------------------------------------------------------------------------------------------------
// Declare Properties (CBUFFER)
// ---------------------------------------------------------------------------------------------------
     //CBUFFER_START(UnityPerMaterial)

          // MainTex Properties
          float4 _MainTex_ST;
          half4 _FlipBookSetting, _MainTexColor;
          half _MainTexUVAutoOffset, _MainTexG_Intensity, _MainTexB_Intensity, _ClampMainTexUV;
          uint4 _MainTexUVTileController;
          uint _MainTexGIntensityController, _MainTexBIntensityController;

          // Distort Properties
          float4 _DistortTex_ST;
          half _DistortTexUVAutoOffset, _DistortIntensity, _DistortChannel;
          uint4 _DistortTexUVTileController;
          uint _DistortImpactTarget, _DistortIntensityController;

          // Dissolve Properties
          float4 _DissolveTex_ST;
          half _DissolveTexUVAutoOffset, _DissolveIntensity, _DissolveChannel, _ActiveHardClipDissolve; 
          uint4 _DissolveTexUVTileController;
          uint _DissolveIntensityController, _EnableHighPrecisionFlowMap , _ReverseFlowMap_G_Color, _DissolveCalculateMode;

          // Emission Properties
          float4 _EmissionTex_ST;
          half4 _EmissionColor;
          half _EmissionIntensity, _EmissionTexUVAutoOffset;
          uint4 _EmissionTexUVTileController;
          uint _EmissionColorMode, _EmissionIntensityController;

          // Ramp Properties
          half _EnhanceRampColor;

          // Normal Properties
          float4 _NormalTex_ST;
          half _NormalTexUVAutoOffset, _NormalIntensity, _NormalBounceLightRange;
          uint4 _NormalTexUVTileController;
          uint _NormalIntensityController;

          // Parallax Properties
          float4 _ParallaxTex_ST, _ParallaxNoiseStepTex_ST;
          half _ParallaxTexUVAutoOffset, _ParallaxIntensity, _ParallaxTexChannel, _HeightSampleSteps, _ParallaxNoiseTexChannel, _ParallaxNoiseIntensity;
          uint4 _ParallaxTexUVTileController;
          uint _ParallaxImpactTarget, _ParallaxIntensityController;

          // Fresnel Properties
          half4 _FresnelColor;
          half _FresnelRange, _FlipFresnelColorRange, _FlipFresnelAlphaRange, _FresnelPower;
          uint _FresnelFunction, _FlipInnerFaceNormal, _FresnelRangeController, _FresnelPowerController;

          // Vertex-Offset Properties
          float4 _VertexOffsetTex_ST, _VertexOffsetTex_TexelSize, _VertexOffsetMask_ST;
          half4 _VertexOffsetTexSampleChannels, _VertexOffsetMaskSampleChannels;
          half _VertexOffsetTexUVAutoOffset, _VertexOffsetIntensity,_AnimationPlayableSlider, _VertexOffsetMaskUVAutoOffset,
               _AnimationTexUVChannel, _AnimationVertexVectorMode, _ReClampAnimationVertexVector,_VertexOffsetMaskIntensity,
               _VertexOffsetCameraDistance;
          uint4 _VertexOffsetTexUVTileController, _VertexOffsetMaskUVTileController;
          uint _VertexOffsetIntensityController, _AnimationPlayableSliderController, _VertexOffsetMaskIntensityController,
               _VertexOffsetCameraDistanceController;

          // Soft-Particle
          half _SoftRange;

          // Double-Side Color
          half4 _SideColor;
          half _ChangeSide;

          // Light-Illumination
          half _MainLightIntensity, _AdditionalLightIntensity, _MainLightBounceRange, _AdditionalBounceRange;

          // External Alpha Template
          half _LineAlphaMode, _ExternalAlphaRange, _ExternalAlphaPower, _InverseAlphaVal, _ExternalAlphaIntensity;
          uint _ExternalAlphaRangeController, _ExternalAlphaPowerController, _ExternalAlphaIntensityController;

          // Alpha-Clip
          half _AlphaClipRange;
          uint _AlphaClipRangeController;

          // Rim Dissolve
          half4 _RimDissolveColor;
          half _RimDissolveRange;
          uint _RimDissolveColorMode, _RimDissolveRangeController;

          // Color Adjustment
          half _HUEShiftVal, _SaturationVal, _ContrastVal, _AntiShineGlowVal;
          uint _ColorAdjustmentLayerMask, _HUEShiftValController, _SaturationValController, _ContrastValController, _AntiShineGlowValController;

          // Final Color & Alpha Multiplier
          half _FinalColorIntensity, _FinalAlphaIntensity;
          uint _FinalColorIntensityController, _FinalAlphaIntensityController;

          // GrabPass
          half _DistortCenterUV , _GrabPassIntensity;
          uint _GrabPassIntensityController;

          // Decal
          uint4 _DecalWorldNormalMaskAxis;

     //CBUFFER_END

// ---------------------------------------------------------------------------------------------------
// Declare Editor Properties (for 'Picking'、'SceneSelectionPass')
// ---------------------------------------------------------------------------------------------------
     float _ObjectId;
     float _PassValue;
     float4 _SelectionID;

// ---------------------------------------------------------------------------------------------------
// Declare ShadowCaster Pass Properties 
// ---------------------------------------------------------------------------------------------------
// Shadow Casting Light geometric parameters. These variables are used when applying the shadow Normal Bias and are set by UnityEngine.Rendering.Universal.ShadowUtils.SetupShadowCasterConstantBuffer in com.unity.render-pipelines.universal/Runtime/ShadowUtils.cs
// For Directional lights, _LightDirection is used when applying shadow Normal Bias.
// For Spot lights and Point lights, _LightPosition is used to compute the actual light direction because it is different at each shadow caster geometry vertex.
     float3 _LightDirection;
     float3 _LightPosition;

#endif