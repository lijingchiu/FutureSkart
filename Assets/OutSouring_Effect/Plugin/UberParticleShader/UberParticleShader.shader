Shader "UberParticleShader"
{ 
    Properties
    {
        // Workflow Mode
        [KeywordEnum(Normal,Geometry,Decal,ScreenDistortion)] _ParticleMode ("Particle Mode" , Float) = 0
            
        // ---------------------------------------------------------------------------------------------------
        // Normal Particle Shader Properties
        // ---------------------------------------------------------------------------------------------------
        
        // MainTex 
        [KeywordEnum(Normal,MultiChannel,FlipBook,PolarCoord,ScreenUV)] _MainTexMode ("MainTex Mode" , Float) = 0
        _MainTex ("Main Texture" , 2D) = "white" {}
        [HDR] _MainTexColor ("MainTex Color" , Color) = (1,1,1,1)
        [KeywordEnum(UV1,UV2)] _MainTexUVChannel ("MainTex UV Channel" , Float) = 0
        [KeywordEnum(SelfSetting , Linear_Repeat , Linear_Clamp , Point_Repeat , Point_Clamp)] _MainTexWrapMode ("MainTex WrapMode" , Float ) = 0
        [Toggle] _MainTexUVAutoOffset ("MainTex UV Auto Offset" , Float) = 0
        _MainTexUVTileController ("MainTex UV Tile Controller" , Vector) = (0,0,0,0)
        [Toggle] _ClampMainTexUV ("Clamp MainTex UV" , Float) = 0
        // MainTex (MultiChannel)
        _MainTexG_Intensity ("MainTex G Intensity", Range(-1,1)) = 0
        [IntRange] _MainTexGIntensityController ("MainTex G Intensity Controller" , Range(0,8)) = 0
        _MainTexB_Intensity ("MainTex B Intensity", Range(-1,1)) = 0
        [IntRange] _MainTexBIntensityController ("MainTex B Intensity Controller" , Range(0,8)) = 0
        // MainTex (FlipBook)
        _FlipBookSetting ("FlipBook Setting" , Vector) = (1,1,1,0)
        
        // Distort
        [KeywordEnum(Disable,Enable,PolarCoord,FlowMap,ScreenUV)] _ActiveDistort ("Active Distort" , Float) = 0
        _DistortTex ("Distort Texture" , 2D) = "white" {}
        [KeywordEnum(UV1,UV2)] _DistortTexUVChannel ("DistortTex UV Channel" , Float) = 0
        [KeywordEnum(SelfSetting , Linear_Repeat , Linear_Clamp , Point_Repeat , Point_Clamp)] _DistortTexWrapMode ("DistortTex WrapMode" , Float ) = 0
        [Enum(R,0,G,1,B,2,A,3)] _DistortChannel ("Distort Channel" , Float) = 0
        [Toggle] _DistortTexUVAutoOffset ("DistortTex UV Auto Offset" , Float) = 0
        _DistortTexUVTileController ("DistortTex UV Tile Controller" , Vector) = (0,0,0,0)
        _DistortIntensity ("Distort Intensity" , Range(-1,1)) = 0
        [IntRange] _DistortIntensityController ("Distort Intensity Controller" , Range(0,8)) = 0
        _DistortImpactTarget ("Distort Impact Target" , Float) = 1 // FlagsEnum: MainTex , Dissolve , NormalMap
        // Distort (FlowMap)
        [Toggle] _EnableHighPrecisionFlowMap ("Enable High Precision FlowMap" , Float) = 0 
        [Toggle] _ReverseFlowMap_G_Color ("ReverseFlowMap G Color" , Float) = 0 

        // Dissolve
        [KeywordEnum(Disable,Enable,PolarCoord,ScreenUV)] _ActiveDissolve ("Active Dissolve" , Float) = 0
        _DissolveTex ("Dissolve Texture" , 2D) = "white" {}
        [KeywordEnum(UV1,UV2)] _DissolveTexUVChannel ("DissolveTex UV Channel" , Float) = 0
        [KeywordEnum(SelfSetting , Linear_Repeat , Linear_Clamp , Point_Repeat , Point_Clamp)] _DissolveTexWrapMode ("DissolveTex WrapMode" , Float ) = 0
        [Enum(R,0,G,1,B,2,A,3)] _DissolveChannel ("Dissolve Channel" , Float) = 0
        [Toggle] _DissolveTexUVAutoOffset ("DissolveTex UV Auto Offset" , Float) = 0
        _DissolveTexUVTileController ("DissolveTex UV Tile Controller" , Vector) = (0,0,0,0)
        [Enum(Substract,0,Erosion,1)] _DissolveCalculateMode ("Dissolve Calculate Mode" , Float) = 0
        _DissolveIntensity ("Dissolve Intensity" , Range(-1,1)) = 0
        [IntRange] _DissolveIntensityController ("Dissolve Intensity Controller" , Range(0,8)) = 0
        [Toggle] _ActiveHardClipDissolve ("Active Hard Clip Dissolve" , Float) = 0
        // Dissolve Extension - Rim Dissolve
        [KeywordEnum(Disable,Enable)] _ActiveRimDissolve ("Active Rim Dissolve" , Float) = 0
        _RimDissolveRange ("Rim Dissolve Range" , Range(0,1)) = 1
        [IntRange] _RimDissolveRangeController ("Rim Dissolve Range Controller" , Range(0,8)) = 0
        [HDR] _RimDissolveColor ("Rim Dissolve Color" , Color) = (1,1,1,1)
        [Enum(Additive,0,Multiply,1,Replace,2)] _RimDissolveColorMode ("Rim Dissolve Color Mode" , Float) = 0

        // Emission Map
        [KeywordEnum(Disable,Enable,PolarCoord,ScreenUV)] _ActiveEmission ("Active Emission" , Float) = 0
        _EmissionTex ("Emission Texture" , 2D) = "white" {}
        [KeywordEnum(UV1,UV2)] _EmissionTexUVChannel ("EmissionTex UV Channel" , Float) = 0
        [KeywordEnum(SelfSetting , Linear_Repeat , Linear_Clamp , Point_Repeat , Point_Clamp)] _EmissionTexWrapMode ("EmissionTex WrapMode" , Float) = 0
        [Toggle] _EmissionTexUVAutoOffset ("EmissionTex UV Auto Offset" , Float) = 0
        _EmissionTexUVTileController ("EmissionTex UV Tile Controller" , Vector) = (0,0,0,0)
        _EmissionIntensity ("Emission Intensity" , Range(0,1)) = 1
        [IntRange] _EmissionIntensityController ("Emission Intensity Controller" , Range(0,8)) = 0
        [Enum(Additive,0,Multiply,1,Replace,2)] _EmissionColorMode ("Emission Color Mode" , Float) = 0
        [HDR] _EmissionColor ("Emission Color" , Color) = (1,1,1,1)

        // Ramp 
        [KeywordEnum(Disable,Enable)] _ActiveRamp ("Active Ramp" , Float) = 0
        _RampTex ("Ramp Texture" , 2D) = "white" {}
        _EnhanceRampColor ("Enhance Ramp Color" , Range(0,10)) = 1
        
        // Normal
        [KeywordEnum(Disable,Enable)] _ActiveNormal ("Active Normal" , Float) = 0
        [Normal] _NormalTex ("Normal Texture" , 2D) = "bump" {}
        [KeywordEnum(UV1,UV2)] _NormalTexUVChannel ("NormalTex UV Channel" , Float) = 0
        [KeywordEnum(SelfSetting , Linear_Repeat , Linear_Clamp , Point_Repeat , Point_Clamp)] _NormalTexWrapMode ("NormalTex WrapMode" , Float ) = 0
        [Toggle] _NormalTexUVAutoOffset ("NormalTex UV Auto Offset" , Float) = 0
        _NormalTexUVTileController ("NormalTex UV Tile Controller" , Vector) = (0,0,0,0)
        _NormalIntensity ("Normal Intensity" , Range(-10,10)) = 0
        [IntRange] _NormalIntensityController ("Normal Intensity Controller" , Range(0,8)) = 0
        _NormalBounceLightRange("Bounce Light Range" , Range(0,1)) = 1
        
        // Parallax 
        [KeywordEnum(Disable,Enable,Steep)] _ActiveParallax ("Active Parallax" , Float) = 0
        _ParallaxTex ("Parallax Texture" , 2D) = "white" {}
        [KeywordEnum(UV1,UV2)] _ParallaxTexUVChannel ("ParallaxTex UV Channel" , Float) = 0
        [KeywordEnum(SelfSetting , Linear_Repeat , Linear_Clamp , Point_Repeat , Point_Clamp)] _ParallaxTexWrapMode ("ParallaxTex WrapMode" , Float ) = 0
        [Enum(R,0,G,1,B,2,A,3)] _ParallaxTexChannel ("ParallaxTex Channel" , Float) = 0
        [Toggle] _ParallaxTexUVAutoOffset ("Parallax UV Auto Offset" , Float) = 0
        _ParallaxTexUVTileController ("Parallax UV Tile Controller" , Vector) = (0,0,0,0)
        _ParallaxIntensity ("Parallax Intensity" , Range(-1,1)) = 0
        [IntRange] _ParallaxIntensityController ("Parallax Intensity Controller" , Range(0,8)) = 0
        [IntRange] _HeightSampleSteps ("Height Sample Steps" , Range(1,64)) = 32
        _ParallaxNoiseStepTex ("Parallax Noise Step Texture" , 2D) = "white" {}
        [KeywordEnum(SelfSetting , Linear_Repeat , Linear_Clamp , Point_Repeat , Point_Clamp)] _ParallaxNoiseStepTexWrapMode ("Parallax NoiseStepTex WrapMode" , Float ) = 0
        [Enum(R,0,G,1,B,2,A,3)] _ParallaxNoiseTexChannel ("Parallax NoiseTex Channel" , Float) = 0
        _ParallaxNoiseIntensity ("Parallax Noise Intensity" , Range(0,1)) = 1
        _ParallaxImpactTarget ("Parallax Impact Target" , Float) = 1 // FlagsEnum: MainTex , Dissolve , NormalMap , Distort
        
        // Fresnel
        [KeywordEnum(Disable,Enable)] _ActiveFresnel ("Active Fresnel" , Float) = 0
        [IntRange] _FresnelFunction("Fresnel Function" , Range(0,255)) = 1 // FlagsEnum: FresnelColor , FresnelAlpha
        _FresnelRange ("Fresnel Range" , Range(-1,1)) = 1
        [IntRange] _FresnelRangeController ("Fresnel Range Controller" , Range(0,8)) = 0
        _FresnelPower ("Fresnel Power" , Range(0.01,100)) = 1
        [IntRange] _FresnelPowerController ("Fresnel Power Controller" , Range(0,8)) = 0
        [HDR] _FresnelColor ("Fresnel Color" , Color) = (1,1,1,1)
        [Toggle] _FlipFresnelColorRange ("Flip Fresnel Color Range" , Float) = 0
        [Toggle] _FlipFresnelAlphaRange ("Flip Fresnel Alpha Range" , Float) = 0
        [Toggle] _FlipInnerFaceNormal ("Flip Inner Face Normal" , Float) = 0
         
        // Vertex-Offset
        [KeywordEnum(Disable,Enable,AnimationTex,FullScreen,LocalBillboard,CameraZAxisOffset)] _ActiveVertexOffset ("Active Vertex Offset" , Float) = 0
        [KeywordEnum(UV1,UV2)] _VertexOffsetTexUVChannel ("Vertex OffsetTex UV Channel" , Float) = 0
        _VertexOffsetTex ("Vertex Offset Texture" , 2D) = "white" {}
        [KeywordEnum(SelfSetting , Linear_Repeat , Linear_Clamp , Point_Repeat , Point_Clamp)] _VertexOffsetTexWrapMode ("Vertex OffsetTex WrapMode" , Float ) = 0
        [Toggle] _VertexOffsetTexUVAutoOffset ("Vertex OffsetTex UV Auto Offset" , Float) = 0
        _VertexOffsetTexUVTileController ("Vertex OffsetTex UV Tile Controller" , Vector) = (0,0,0,0)
        _VertexOffsetTexSampleChannels ("Vertex OffsetTex Sample Channels" , Vector) = (1,1,1,1)
        _VertexOffsetIntensity ("Vertex Offset Intensity" , Range(-1,1)) = 0
        [IntRange] _VertexOffsetIntensityController ("Vertex Offset Intensity Controller" , Range(0,8)) = 0
        // Vertex-Offset (Vertex Offset Mask) 
        [KeywordEnum(Disable,Enable)] _ActiveVertexOffsetMask ("Active Vertex Offset Mask" , Float) = 0
        _VertexOffsetMask ("Vertex Offset Mask" , 2D) = "black" {}
        [KeywordEnum(SelfSetting , Linear_Repeat , Linear_Clamp , Point_Repeat , Point_Clamp)] _VertexOffsetMaskWrapMode ("Vertex Offset Mask WrapMode" , Float ) = 0
        [Toggle] _VertexOffsetMaskUVAutoOffset ("Vertex Offset Mask UV Auto Offset" , Float) = 0
        _VertexOffsetMaskUVTileController ("Vertex Offset Mask UV Tile Controller" , Vector) = (0,0,0,0)
        _VertexOffsetMaskSampleChannels ("Vertex Offset Mask Sample Channels" , Vector) = (1,1,1,1)
        _VertexOffsetMaskIntensity ("Vertex Offset Mask Intensity" , Range(0,1)) = 1
        [IntRange] _VertexOffsetMaskIntensityController ("Vertex Offset Mask Intensity Controller" , Range(0,8)) = 0
        // Vertex-Offset (Animation Vertex Map) 【Need to enable Mesh-GPU Instance】 .
        _AnimationNormalOffsetTex ("Animation Normal OffsetTex" , 2D) = "bump"{}
        [Enum(VertexID,0,UV1.x,1,UV2.x,2)] _AnimationTexUVChannel ("AnimationTexUVChannel" , Float) = 0
        [Enum(Replace,0,Additive,1)] _AnimationVertexVectorMode ("Animation Vertex Vector Mode" , Float) = 0
        [Toggle] _ReClampAnimationVertexVector ("ReClamp Animation Vertex Vector" , Float) = 0
        _AnimationPlayableSlider ("Animation Playable Slider" , Range(0,1)) = 0
        [IntRange] _AnimationPlayableSliderController ("Animation Playable Slider Controller" , Range(0,8)) = 0
        // Vertex-Offset (Camera Z-Axis Offset)
        _VertexOffsetCameraDistance ("Vertex Offset Camera Distance" , Range(-1,1)) = 0
        [IntRange] _VertexOffsetCameraDistanceController ("Vertex Offset Camera Distance Controller" , Range(0,8)) = 0
        
        // Double-Side Color
        [KeywordEnum(Disable,Replace,Add,Multiply)] _ActiveSideFaceColor ("Active Side Face Color" , Float) = 0
        [HDR] _SideColor ("Side Color" , Color) = (0,0,0,1)
        [Toggle] _ChangeSide ("Change Side" , Float) = 0
        
        // Soft-Particle
        [KeywordEnum(Disable,Enable)] _ActiveSoftParticle ("Active Soft Particle" , Float) = 0
        _SoftRange ("Soft Range" , Range(0,100)) = 1
        
        // Light-Illumination
        [KeywordEnum(Disable,VertexLight,FragmentLight)] _ActiveLightSystem ("Active Light System" , Float) = 0
        _MainLightIntensity ("MainLight Intensity" , Range(0,1)) = 1
        _MainLightBounceRange ("MainLight Bounce Range" , Range(0,1)) = 0.5
        _AdditionalLightIntensity("Additional Light Intensity" , Range(0,1)) = 1
        _AdditionalBounceRange ("AdditionalLight Bounce Range" , Range(0,1)) = 0.5
         
        // External Alpha Template
        [KeywordEnum(Disable,Line,Gradient,Circle,Round)] _ActiveExternalAlpha ("Active External Alpha" , Float) = 0 
        [Enum(Horizon,2,Vertical,3)] _LineAlphaMode ("Line Alpha Mode" , Float) = 2
        [Toggle] _InverseAlphaVal ("Inverse Alpha Value" , Float) = 0
        _ExternalAlphaRange ("External Alpha Range" , Range(-1,1)) = 0
        _ExternalAlphaRangeController ("External Alpha Range Controller" , Float) = 0 
        _ExternalAlphaPower ("External Alpha Power" , Range(0.1,100)) = 1
        _ExternalAlphaPowerController ("External Alpha Power Controller" , Float) = 0 
        _ExternalAlphaIntensity ("External Alpha Intensity" , Range(0,10)) = 1
        _ExternalAlphaIntensityController ("External Alpha Intensity Controller" , Float) = 0 
        
        // Alpha-Clip
        [KeywordEnum(Disable,HardClip,SmoothClip)] _ActiveAlphaClip("Active Alpha Clip" , Float) = 0
        _AlphaClipRange ("Alpha Clip Range" , Range(0,1)) = 0.5
        [IntRange] _AlphaClipRangeController ("Alpha Clip Range Controller" , Range(0,8)) = 0
        
        // Color Adjustment
        [KeywordEnum(Disable,Enable)] _ActiveColorAdjustment ("Active Color Adjustment" , Float) = 0
        [IntRange] _ColorAdjustmentLayerMask("Color Adjustment LayerMask" , Range(0,255)) = 1 // FlagsEnum: HUEShift , Saturation , Contrast
        _HUEShiftVal ("HUE Shift Value" , Range(-1,1)) = 0
        [IntRange] _HUEShiftValController ("HUE Shift Value Contorller" , Range(0,8)) = 0
        _SaturationVal ("Saturation Value" , Range(0,2)) = 1
        [IntRange] _SaturationValController ("Saturation Value Contorller" , Range(0,8)) = 0
        _ContrastVal ("Contrast Value" , Range(0,10)) = 1
        [IntRange] _ContrastValController ("Contrast Value Contorller" , Range(0,8)) = 0
        _AntiShineGlowVal ("Anti ShineGlow Time Value" , Range(-1,1)) = 0
        [IntRange] _AntiShineGlowValController ("Anti ShineGlow Time Value Controller" , Range(0,8)) = 0
        
        // Extra Features
        [KeywordEnum(Disable,Enable)] _ActiveExtraFeatures ("Active Extra Features" , Float) = 0
        
        // FinalColor & FinalAlpha Multiplier
        _FinalColorIntensity ("Final Color Intensity" , Range(0,10)) = 1
        [IntRange] _FinalColorIntensityController ("Final Color Intensity Controller" , Range(0,8)) = 0
        _FinalAlphaIntensity ("Final Alpha Intensity" , Range(0,1)) = 1
        [IntRange] _FinalAlphaIntensityController ("Final Alpha Intensity Controller" , Range(0,8)) = 0
        
        // Blend Setting
        _RenderMode ("RenderMode" , Float) = 0 // FlagsEnum: Normal , Additive , PreMultiple , Custom
        [Enum(UnityEngine.Rendering.BlendMode)] _SourceBlend ("SrcBlend",Float) = 5
        [Enum(UnityEngine.Rendering.BlendMode)] _DestBlend ("DestBlend",Float) = 10
        [Enum(Off,0,Front,1,Back,2)] _Cull("CullMask",Float) = 0
        [Enum(Off,0,On,1)] _ZWrite("Zwrite",Float) = 0
        [Enum(UnityEngine.Rendering.CompareFunction)] _ZTest("ZTest",Float) = 2

        // Stencil Setting
        _Ref ("Ref",Float) = 0
        [Enum(UnityEngine.Rendering.CompareFunction)] _Comp ("Comparison",Float) = 0
        [Enum(UnityEngine.Rendering.StencilOp)] _Pass ("Pass ",Float) = 0
        [Enum(UnityEngine.Rendering.StencilOp)] _Fail ("Fail ",Float) = 0
        [Enum(UnityEngine.Rendering.StencilOp)] _ZFail ("ZFail ",Float) = 0
        
        // ---------------------------------------------------------------------------------------------------
        // Geometry Shader Properties
        // ---------------------------------------------------------------------------------------------------
        [KeywordEnum(Disable,Enable)] _ReceiveShadow ("Receive Shadow" , Float) = 0
        
        // ---------------------------------------------------------------------------------------------------
        // Decal Shader Properties
        // ---------------------------------------------------------------------------------------------------
        [KeywordEnum(Normal)] _DecalRenderMode ("Decal Render Mode" , Float) = 0
        [KeywordEnum(Disable,Enable)] _ActiveDecalWorldNormalMask ("Active Decal World Normal Mask" , Float) = 0
        _DecalWorldNormalMaskAxis ("Decal World Normal Mask Axis" , Vector) = (0,1,0,0)
        
        // ---------------------------------------------------------------------------------------------------
        // Distort Screen Shader Properties (GrabPass)
        // ---------------------------------------------------------------------------------------------------

        [KeywordEnum(Normal,BumpTex)] _GrabPassTexMode ("GrabPass Tex Mode" , Float) = 0
        [Toggle] _DistortCenterUV ("Distort Center UV" , Float) = 0
        _GrabPassIntensity ("Distort Intensity" , Range(-1 , 1)) = 1
        [IntRange] _GrabPassIntensityController ("GrabPass Intensity Controller" , Range(0,8)) = 0
    }
    SubShader
    {
        Name "UberParticle"
        Tags
        {
            "RenderType" = "Transparent"
            "IgnoreProjector" = "True"
            "PreviewType" = "Plane"
            "Queue" = "Transparent"
            "RenderPipeline" = "UniversalPipeline"
        }
        Stencil
        {
            Ref [_Ref]
            Comp [_Comp]
            Pass [_Pass]
            Fail [_Fail]
            ZFail [_ZFail]
        }
        
        Pass
        {
            Tags
            {
                "LightMode" = "UniversalForward"
            }
            
            Blend [_SourceBlend] [_DestBlend]
            ZWrite[_ZWrite]
            ZTest [_ZTest]
            Cull [_Cull]

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 4.5
            
            // Include
            #include "Include/ParticleComponent.hlsl"
            
            // Unity Keywords Define (GPU-Instance)
            //#pragma enable_d3d11_debug_symbols
            #pragma multi_compile_instancing
            #pragma instancing_options procedural:ParticleInstancingSetup

            // Texture WrapMode  
            #pragma shader_feature_local_fragment _ _MAINTEXWRAPMODE_LINEAR_REPEAT _MAINTEXWRAPMODE_LINEAR_CLAMP _MAINTEXWRAPMODE_POINT_REPEAT _MAINTEXWRAPMODE_POINT_CLAMP
            #pragma shader_feature_local_fragment _ _DISTORTTEXWRAPMODE_LINEAR_REPEAT _DISTORTTEXWRAPMODE_LINEAR_CLAMP _DISTORTTEXWRAPMODE_POINT_REPEAT _DISTORTTEXWRAPMODE_POINT_CLAMP
            #pragma shader_feature_local_fragment _ _DISSOLVETEXWRAPMODE_LINEAR_REPEAT _DISSOLVETEXWRAPMODE_LINEAR_CLAMP _DISSOLVETEXWRAPMODE_POINT_REPEAT _DISSOLVETEXWRAPMODE_POINT_CLAMP
            #pragma shader_feature_local_fragment _ _NORMALTEXWRAPMODE_LINEAR_REPEAT _NORMALTEXWRAPMODE_LINEAR_CLAMP _NORMALTEXWRAPMODE_POINT_REPEAT _NORMALTEXWRAPMODE_POINT_CLAMP
            #pragma shader_feature_local_fragment _ _PARALLAXTEXWRAPMODE_LINEAR_REPEAT _PARALLAXTEXWRAPMODE_LINEAR_CLAMP _PARALLAXTEXWRAPMODE_POINT_REPEAT _PARALLAXTEXWRAPMODE_POINT_CLAMP
            #pragma shader_feature_local_fragment _ _PARALLAXNOISESTEPTEXWRAPMODE_LINEAR_REPEAT _PARALLAXNOISESTEPTEXWRAPMODE_LINEAR_CLAMP _PARALLAXNOISESTEPTEXWRAPMODE_POINT_REPEAT _PARALLAXNOISESTEPTEXWRAPMODE_POINT_CLAMP
            #pragma shader_feature_local_vertex _ _VERTEXOFFSETTEXWRAPMODE_LINEAR_REPEAT _VERTEXOFFSETTEXWRAPMODE_LINEAR_CLAMP _VERTEXOFFSETTEXWRAPMODE_POINT_REPEAT _VERTEXOFFSETTEXWRAPMODE_POINT_CLAMP
            #pragma shader_feature_local_vertex _ _VERTEXOFFSETMASKWRAPMODE_LINEAR_REPEAT _VERTEXOFFSETMASKWRAPMODE_LINEAR_CLAMP _VERTEXOFFSETMASKWRAPMODE_POINT_REPEAT _VERTEXOFFSETMASKWRAPMODE_POINT_CLAMP
            #pragma shader_feature_local_fragment _ _EMISSIONTEXWRAPMODE_LINEAR_REPEAT _EMISSIONTEXWRAPMODE_LINEAR_CLAMP _EMISSIONTEXWRAPMODE_POINT_REPEAT _EMISSIONTEXWRAPMODE_POINT_CLAMP

            // Texture UV Channel
            #pragma shader_feature_local _ _MAINTEXUVCHANNEL_UV2
            #pragma shader_feature_local _ _DISTORTTEXUVCHANNEL_UV2
            #pragma shader_feature_local _ _DISSOLVETEXUVCHANNEL_UV2
            #pragma shader_feature_local _ _NORMALTEXUVCHANNEL_UV2
            #pragma shader_feature_local _ _PARALLAXTEXUVCHANNEL_UV2
            #pragma shader_feature_local _ _VERTEXOFFSETTEXUVCHANNEL_UV2
            #pragma shader_feature_local _ _EMISSIONTEXUVCHANNEL_UV2

            // Enable Particle Mode
            #pragma shader_feature_local _ _PARTICLEMODE_GEOMETRY _PARTICLEMODE_DECAL _PARTICLEMODE_SCREENDISTORTION
            
            // MainTex Mode
            #pragma shader_feature_local _MAINTEXMODE_NORMAL _MAINTEXMODE_MULTICHANNEL _MAINTEXMODE_FLIPBOOK _MAINTEXMODE_POLARCOORD _MAINTEXMODE_SCREENUV
            
            // Enable Distort 
            #pragma shader_feature_local _ _ACTIVEDISTORT_ENABLE _ACTIVEDISTORT_POLARCOORD _ACTIVEDISTORT_FLOWMAP _ACTIVEDISTORT_SCREENUV
 
            // Enable Dissolve
            #pragma shader_feature_local _ _ACTIVEDISSOLVE_ENABLE _ACTIVEDISSOLVE_POLARCOORD _ACTIVEDISSOLVE_SCREENUV
            
            // Enable Rim Dissolve HighLight
            #pragma shader_feature_local _ _ACTIVERIMDISSOLVE_ENABLE

            // Enable Ramp
            #pragma shader_feature_local _ _ACTIVERAMP_ENABLE

            // Enable Emission Map
            #pragma shader_feature_local _ _ACTIVEEMISSION_ENABLE _ACTIVEEMISSION_POLARCOORD _ACTIVEEMISSION_SCREENUV
            
            // Enable Normal
            #pragma shader_feature_local _ _ACTIVENORMAL_ENABLE

            // Enable Parallax
            #pragma shader_feature_local _ _ACTIVEPARALLAX_ENABLE _ACTIVEPARALLAX_STEEP

            // Enable Fresnel
            #pragma shader_feature_local _ _ACTIVEFRESNEL_ENABLE

            // Enable Vertex-Offset
            #pragma shader_feature_local _ _ACTIVEVERTEXOFFSET_ENABLE _ACTIVEVERTEXOFFSET_ANIMATIONTEX _ACTIVEVERTEXOFFSET_FULLSCREEN _ACTIVEVERTEXOFFSET_LOCALBILLBOARD _ACTIVEVERTEXOFFSET_CAMERAZAXISOFFSET
            #pragma shader_feature_local _ _ACTIVEVERTEXOFFSETMASK_ENABLE

            // Enable Double Side Color
            #pragma shader_feature_local _ _ACTIVESIDEFACECOLOR_REPLACE _ACTIVESIDEFACECOLOR_ADD _ACTIVESIDEFACECOLOR_MULTIPLY

            // Enable Soft-Particle
            #pragma shader_feature_local_fragment _ _ACTIVESOFTPARTICLE_ENABLE

            // Enable Light-Illumination
            #pragma shader_feature_local _ _ACTIVELIGHTSYSTEM_VERTEXLIGHT _ACTIVELIGHTSYSTEM_FRAGMENTLIGHT

            // Enable External Alpha Mode
            #pragma shader_feature_local _ _ACTIVEEXTERNALALPHA_LINE _ACTIVEEXTERNALALPHA_GRADIENT _ACTIVEEXTERNALALPHA_CIRCLE _ACTIVEEXTERNALALPHA_ROUND

            // Enable Alpha-Clip
            #pragma shader_feature_local_fragment _ _ACTIVEALPHACLIP_HARDCLIP _ACTIVEALPHACLIP_SMOOTHCLIP
            
            // Enable Color Adjustment
            #pragma shader_feature_local _ _ACTIVECOLORADJUSTMENT_ENABLE

            // Enable GrabPassTexMode
            #pragma shader_feature_local _ _GRABPASSTEXMODE_BUMPTEX
            
            // Enable GrabPassTexMode
            #pragma shader_feature_local _ _ACTIVEDECALWORLDNORMALMASK_ENABLE

            // Enable Receive Shadow
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile_fragment _ _ADDITIONAL_LIGHT_SHADOWS
            #pragma multi_compile_fragment _ _SHADOWS_SOFT
            #pragma shader_feature_local _ _RECEIVESHADOW_ENABLE
            
            ENDHLSL
        }
        Pass
        {
            Tags
            {
                "LightMode" = "ShadowCaster"
            }
            Blend One Zero
            ZWrite On
            ZTest LEqual
            ColorMask 0
            Cull [_Cull]

            HLSLPROGRAM
            #pragma vertex vertCommonShared
            #pragma fragment fragCommonShared
            #pragma target 4.5

            #define PARTICLE_SHADOWCASTER
             
            // Include  
            #include "Include/ParticleCommonShared.hlsl"
            
            // Unity Keywords Define (GPU-Instance)
            //#pragma enable_d3d11_debug_symbols
            #pragma multi_compile_instancing
            #pragma instancing_options procedural:ParticleInstancingSetup
            
            // Universal Pipeline keywords
            // This is used during shadow map generation to differentiate between directional and punctual light shadows, as they use different formulas to apply Normal Bias
            #pragma multi_compile_vertex _ _CASTING_PUNCTUAL_LIGHT_SHADOW

            // Texture WrapMode  
            #pragma shader_feature_local_fragment _ _MAINTEXWRAPMODE_LINEAR_REPEAT _MAINTEXWRAPMODE_LINEAR_CLAMP _MAINTEXWRAPMODE_POINT_REPEAT _MAINTEXWRAPMODE_POINT_CLAMP
            #pragma shader_feature_local_fragment _ _DISTORTTEXWRAPMODE_LINEAR_REPEAT _DISTORTTEXWRAPMODE_LINEAR_CLAMP _DISTORTTEXWRAPMODE_POINT_REPEAT _DISTORTTEXWRAPMODE_POINT_CLAMP
            #pragma shader_feature_local_fragment _ _DISSOLVETEXWRAPMODE_LINEAR_REPEAT _DISSOLVETEXWRAPMODE_LINEAR_CLAMP _DISSOLVETEXWRAPMODE_POINT_REPEAT _DISSOLVETEXWRAPMODE_POINT_CLAMP
            #pragma shader_feature_local_fragment _ _NORMALTEXWRAPMODE_LINEAR_REPEAT _NORMALTEXWRAPMODE_LINEAR_CLAMP _NORMALTEXWRAPMODE_POINT_REPEAT _NORMALTEXWRAPMODE_POINT_CLAMP
            #pragma shader_feature_local_fragment _ _PARALLAXTEXWRAPMODE_LINEAR_REPEAT _PARALLAXTEXWRAPMODE_LINEAR_CLAMP _PARALLAXTEXWRAPMODE_POINT_REPEAT _PARALLAXTEXWRAPMODE_POINT_CLAMP
            #pragma shader_feature_local_fragment _ _PARALLAXNOISESTEPTEXWRAPMODE_LINEAR_REPEAT _PARALLAXNOISESTEPTEXWRAPMODE_LINEAR_CLAMP _PARALLAXNOISESTEPTEXWRAPMODE_POINT_REPEAT _PARALLAXNOISESTEPTEXWRAPMODE_POINT_CLAMP
            #pragma shader_feature_local_vertex _ _VERTEXOFFSETTEXWRAPMODE_LINEAR_REPEAT _VERTEXOFFSETTEXWRAPMODE_LINEAR_CLAMP _VERTEXOFFSETTEXWRAPMODE_POINT_REPEAT _VERTEXOFFSETTEXWRAPMODE_POINT_CLAMP
            #pragma shader_feature_local_vertex _ _VERTEXOFFSETMASKWRAPMODE_LINEAR_REPEAT _VERTEXOFFSETMASKWRAPMODE_LINEAR_CLAMP _VERTEXOFFSETMASKWRAPMODE_POINT_REPEAT _VERTEXOFFSETMASKWRAPMODE_POINT_CLAMP
           
            // Texture UV Channel
            #pragma shader_feature_local _ _MAINTEXUVCHANNEL_UV2
            #pragma shader_feature_local _ _DISTORTTEXUVCHANNEL_UV2
            #pragma shader_feature_local _ _DISSOLVETEXUVCHANNEL_UV2
            #pragma shader_feature_local _ _NORMALTEXUVCHANNEL_UV2
            #pragma shader_feature_local _ _PARALLAXTEXUVCHANNEL_UV2
            #pragma shader_feature_local _ _VERTEXOFFSETTEXUVCHANNEL_UV2
             
            // MainTex Mode
            #pragma shader_feature_local _MAINTEXMODE_NORMAL _MAINTEXMODE_MULTICHANNEL _MAINTEXMODE_FLIPBOOK _MAINTEXMODE_POLARCOORD _MAINTEXMODE_SCREENUV
            
            // Enable Distort 
            #pragma shader_feature_local _ _ACTIVEDISTORT_ENABLE _ACTIVEDISTORT_POLARCOORD _ACTIVEDISTORT_FLOWMAP _ACTIVEDISTORT_SCREENUV

            // Enable Dissolve
            #pragma shader_feature_local _ _ACTIVEDISSOLVE_ENABLE _ACTIVEDISSOLVE_POLARCOORD _ACTIVEDISSOLVE_SCREENUV

            // Enable Rim Dissolve HighLight
            #pragma shader_feature_local _ _ACTIVERIMDISSOLVE_ENABLE
            
            // Enable Normal
            #pragma shader_feature_local _ _ACTIVENORMAL_ENABLE

            // Enable Parallax
            #pragma shader_feature_local _ _ACTIVEPARALLAX_ENABLE _ACTIVEPARALLAX_STEEP 

            // Enable Fresnel
            #pragma shader_feature_local _ _ACTIVEFRESNEL_ENABLE

            // Enable Vertex-Offset
            #pragma shader_feature_local _ _ACTIVEVERTEXOFFSET_ENABLE _ACTIVEVERTEXOFFSET_ANIMATIONTEX _ACTIVEVERTEXOFFSET_FULLSCREEN _ACTIVEVERTEXOFFSET_LOCALBILLBOARD _ACTIVEVERTEXOFFSET_CAMERAZAXISOFFSET
            #pragma shader_feature_local _ _ACTIVEVERTEXOFFSETMASK_ENABLE
            
            // Enable External Alpha Mode
            #pragma shader_feature_local _ _ACTIVEEXTERNALALPHA_LINE _ACTIVEEXTERNALALPHA_GRADIENT _ACTIVEEXTERNALALPHA_CIRCLE _ACTIVEEXTERNALALPHA_ROUND

            // Enable Alpha-Clip
            #pragma shader_feature_local_fragment _ _ACTIVEALPHACLIP_HARDCLIP _ACTIVEALPHACLIP_SMOOTHCLIP
            
            ENDHLSL
        }

        Pass
        {
            // Depth Only will execute in DepthPrimeMode = Force , another time will be after Opaque .
            // Which mean when draw after Opaque will also output the depth value in DepthTexture with current object's vertex.z.
            
            Name "DepthOnly"
            Tags{"LightMode" = "DepthOnly"}

            ZWrite On
            ColorMask 0
            Cull[_Cull]

            HLSLPROGRAM

            #define PARTICLE_DEPTH_ONLY
            
            // Include
            #include "Include/ParticleCommonShared.hlsl"
            
            #pragma vertex vertCommonShared
            #pragma fragment fragCommonShared
            #pragma target 4.5
            
            // Unity Keywords Define (GPU-Instance)
            //#pragma enable_d3d11_debug_symbols
            #pragma multi_compile_instancing
            #pragma instancing_options procedural:ParticleInstancingSetup

            // Texture WrapMode  
            #pragma shader_feature_local_fragment _ _MAINTEXWRAPMODE_LINEAR_REPEAT _MAINTEXWRAPMODE_LINEAR_CLAMP _MAINTEXWRAPMODE_POINT_REPEAT _MAINTEXWRAPMODE_POINT_CLAMP
            #pragma shader_feature_local_fragment _ _DISTORTTEXWRAPMODE_LINEAR_REPEAT _DISTORTTEXWRAPMODE_LINEAR_CLAMP _DISTORTTEXWRAPMODE_POINT_REPEAT _DISTORTTEXWRAPMODE_POINT_CLAMP
            #pragma shader_feature_local_fragment _ _DISSOLVETEXWRAPMODE_LINEAR_REPEAT _DISSOLVETEXWRAPMODE_LINEAR_CLAMP _DISSOLVETEXWRAPMODE_POINT_REPEAT _DISSOLVETEXWRAPMODE_POINT_CLAMP
            #pragma shader_feature_local_fragment _ _NORMALTEXWRAPMODE_LINEAR_REPEAT _NORMALTEXWRAPMODE_LINEAR_CLAMP _NORMALTEXWRAPMODE_POINT_REPEAT _NORMALTEXWRAPMODE_POINT_CLAMP
            #pragma shader_feature_local_fragment _ _PARALLAXTEXWRAPMODE_LINEAR_REPEAT _PARALLAXTEXWRAPMODE_LINEAR_CLAMP _PARALLAXTEXWRAPMODE_POINT_REPEAT _PARALLAXTEXWRAPMODE_POINT_CLAMP
            #pragma shader_feature_local_fragment _ _PARALLAXNOISESTEPTEXWRAPMODE_LINEAR_REPEAT _PARALLAXNOISESTEPTEXWRAPMODE_LINEAR_CLAMP _PARALLAXNOISESTEPTEXWRAPMODE_POINT_REPEAT _PARALLAXNOISESTEPTEXWRAPMODE_POINT_CLAMP
            #pragma shader_feature_local_vertex _ _VERTEXOFFSETTEXWRAPMODE_LINEAR_REPEAT _VERTEXOFFSETTEXWRAPMODE_LINEAR_CLAMP _VERTEXOFFSETTEXWRAPMODE_POINT_REPEAT _VERTEXOFFSETTEXWRAPMODE_POINT_CLAMP
            #pragma shader_feature_local_vertex _ _VERTEXOFFSETMASKWRAPMODE_LINEAR_REPEAT _VERTEXOFFSETMASKWRAPMODE_LINEAR_CLAMP _VERTEXOFFSETMASKWRAPMODE_POINT_REPEAT _VERTEXOFFSETMASKWRAPMODE_POINT_CLAMP
           
            // Texture UV Channel
            #pragma shader_feature_local _ _MAINTEXUVCHANNEL_UV2
            #pragma shader_feature_local _ _DISTORTTEXUVCHANNEL_UV2
            #pragma shader_feature_local _ _DISSOLVETEXUVCHANNEL_UV2
            #pragma shader_feature_local _ _NORMALTEXUVCHANNEL_UV2
            #pragma shader_feature_local _ _PARALLAXTEXUVCHANNEL_UV2
            #pragma shader_feature_local _ _VERTEXOFFSETTEXUVCHANNEL_UV2
            
            // MainTex Mode
            #pragma shader_feature_local _MAINTEXMODE_NORMAL _MAINTEXMODE_MULTICHANNEL _MAINTEXMODE_FLIPBOOK _MAINTEXMODE_POLARCOORD _MAINTEXMODE_SCREENUV
            
            // Enable Distort 
            #pragma shader_feature_local _ _ACTIVEDISTORT_ENABLE _ACTIVEDISTORT_POLARCOORD _ACTIVEDISTORT_FLOWMAP _ACTIVEDISTORT_SCREENUV

            // Enable Dissolve
            #pragma shader_feature_local _ _ACTIVEDISSOLVE_ENABLE _ACTIVEDISSOLVE_POLARCOORD _ACTIVEDISSOLVE_SCREENUV

            // Enable Rim Dissolve HighLight
            #pragma shader_feature_local _ _ACTIVERIMDISSOLVE_ENABLE
            
            // Enable Normal
            #pragma shader_feature_local _ _ACTIVENORMAL_ENABLE

            // Enable Parallax
            #pragma shader_feature_local _ _ACTIVEPARALLAX_ENABLE _ACTIVEPARALLAX_STEEP

            // Enable Fresnel
            #pragma shader_feature_local _ _ACTIVEFRESNEL_ENABLE

            // Enable Vertex-Offset
            #pragma shader_feature_local _ _ACTIVEVERTEXOFFSET_ENABLE _ACTIVEVERTEXOFFSET_ANIMATIONTEX _ACTIVEVERTEXOFFSET_FULLSCREEN _ACTIVEVERTEXOFFSET_LOCALBILLBOARD _ACTIVEVERTEXOFFSET_CAMERAZAXISOFFSET
            #pragma shader_feature_local _ _ACTIVEVERTEXOFFSETMASK_ENABLE
            
            // Enable External Alpha Mode
            #pragma shader_feature_local _ _ACTIVEEXTERNALALPHA_LINE _ACTIVEEXTERNALALPHA_GRADIENT _ACTIVEEXTERNALALPHA_CIRCLE _ACTIVEEXTERNALALPHA_ROUND

            // Enable Alpha-Clip
            #pragma shader_feature_local_fragment _ _ACTIVEALPHACLIP_HARDCLIP _ACTIVEALPHACLIP_SMOOTHCLIP
            
            ENDHLSL
        }

        // This pass is used to draw a _CameraNormalsTexture texture
        Pass
        {
            Name "DepthNormals"
            Tags{"LightMode" = "DepthNormals"}

            ZWrite On
            Cull[_Cull]

            HLSLPROGRAM
            #pragma vertex vertCommonShared
            #pragma fragment fragCommonShared
            #pragma exclude_renderers gles gles3 glcore
            #pragma target 4.5

            #define PARTICLE_DEPTH_NORMAL

            // Include
            #include "Include/ParticleCommonShared.hlsl"

            // -------------------------------------
            // Material Keywords
            #pragma shader_feature_local _NORMALMAP
            #pragma shader_feature_local _PARALLAXMAP
            #pragma shader_feature_local _ _DETAIL_MULX2 _DETAIL_SCALED
            #pragma shader_feature_local_fragment _ALPHATEST_ON
            #pragma shader_feature_local_fragment _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A

            //--------------------------------------
            // GPU Instancing
            //#pragma enable_d3d11_debug_symbols
            #pragma multi_compile_instancing
            #pragma instancing_options procedural:ParticleInstancingSetup

            // Texture WrapMode  
            #pragma shader_feature_local_fragment _ _MAINTEXWRAPMODE_LINEAR_REPEAT _MAINTEXWRAPMODE_LINEAR_CLAMP _MAINTEXWRAPMODE_POINT_REPEAT _MAINTEXWRAPMODE_POINT_CLAMP
            #pragma shader_feature_local_fragment _ _DISTORTTEXWRAPMODE_LINEAR_REPEAT _DISTORTTEXWRAPMODE_LINEAR_CLAMP _DISTORTTEXWRAPMODE_POINT_REPEAT _DISTORTTEXWRAPMODE_POINT_CLAMP
            #pragma shader_feature_local_fragment _ _DISSOLVETEXWRAPMODE_LINEAR_REPEAT _DISSOLVETEXWRAPMODE_LINEAR_CLAMP _DISSOLVETEXWRAPMODE_POINT_REPEAT _DISSOLVETEXWRAPMODE_POINT_CLAMP
            #pragma shader_feature_local_fragment _ _NORMALTEXWRAPMODE_LINEAR_REPEAT _NORMALTEXWRAPMODE_LINEAR_CLAMP _NORMALTEXWRAPMODE_POINT_REPEAT _NORMALTEXWRAPMODE_POINT_CLAMP
            #pragma shader_feature_local_fragment _ _PARALLAXTEXWRAPMODE_LINEAR_REPEAT _PARALLAXTEXWRAPMODE_LINEAR_CLAMP _PARALLAXTEXWRAPMODE_POINT_REPEAT _PARALLAXTEXWRAPMODE_POINT_CLAMP
            #pragma shader_feature_local_fragment _ _PARALLAXNOISESTEPTEXWRAPMODE_LINEAR_REPEAT _PARALLAXNOISESTEPTEXWRAPMODE_LINEAR_CLAMP _PARALLAXNOISESTEPTEXWRAPMODE_POINT_REPEAT _PARALLAXNOISESTEPTEXWRAPMODE_POINT_CLAMP
            #pragma shader_feature_local_vertex _ _VERTEXOFFSETTEXWRAPMODE_LINEAR_REPEAT _VERTEXOFFSETTEXWRAPMODE_LINEAR_CLAMP _VERTEXOFFSETTEXWRAPMODE_POINT_REPEAT _VERTEXOFFSETTEXWRAPMODE_POINT_CLAMP
            #pragma shader_feature_local_vertex _ _VERTEXOFFSETMASKWRAPMODE_LINEAR_REPEAT _VERTEXOFFSETMASKWRAPMODE_LINEAR_CLAMP _VERTEXOFFSETMASKWRAPMODE_POINT_REPEAT _VERTEXOFFSETMASKWRAPMODE_POINT_CLAMP
           
            // Texture UV Channel
            #pragma shader_feature_local _ _MAINTEXUVCHANNEL_UV2
            #pragma shader_feature_local _ _DISTORTTEXUVCHANNEL_UV2
            #pragma shader_feature_local _ _DISSOLVETEXUVCHANNEL_UV2
            #pragma shader_feature_local _ _NORMALTEXUVCHANNEL_UV2
            #pragma shader_feature_local _ _PARALLAXTEXUVCHANNEL_UV2
            #pragma shader_feature_local _ _VERTEXOFFSETTEXUVCHANNEL_UV2
            
            // MainTex Mode
            #pragma shader_feature_local _MAINTEXMODE_NORMAL _MAINTEXMODE_MULTICHANNEL _MAINTEXMODE_FLIPBOOK _MAINTEXMODE_POLARCOORD _MAINTEXMODE_SCREENUV
            
            // Enable Distort 
            #pragma shader_feature_local _ _ACTIVEDISTORT_ENABLE _ACTIVEDISTORT_POLARCOORD _ACTIVEDISTORT_FLOWMAP _ACTIVEDISTORT_SCREENUV

            // Enable Dissolve
            #pragma shader_feature_local _ _ACTIVEDISSOLVE_ENABLE _ACTIVEDISSOLVE_POLARCOORD _ACTIVEDISSOLVE_SCREENUV

            // Enable Rim Dissolve HighLight
            #pragma shader_feature_local _ _ACTIVERIMDISSOLVE_ENABLE
            
            // Enable Normal
            #pragma shader_feature_local _ _ACTIVENORMAL_ENABLE

            // Enable Parallax
            #pragma shader_feature_local _ _ACTIVEPARALLAX_ENABLE _ACTIVEPARALLAX_STEEP

            // Enable Fresnel
            #pragma shader_feature_local _ _ACTIVEFRESNEL_ENABLE

            // Enable Vertex-Offset
            #pragma shader_feature_local _ _ACTIVEVERTEXOFFSET_ENABLE _ACTIVEVERTEXOFFSET_ANIMATIONTEX _ACTIVEVERTEXOFFSET_FULLSCREEN _ACTIVEVERTEXOFFSET_LOCALBILLBOARD _ACTIVEVERTEXOFFSET_CAMERAZAXISOFFSET
            #pragma shader_feature_local _ _ACTIVEVERTEXOFFSETMASK_ENABLE
            
            // Enable External Alpha Mode
            #pragma shader_feature_local _ _ACTIVEEXTERNALALPHA_LINE _ACTIVEEXTERNALALPHA_GRADIENT _ACTIVEEXTERNALALPHA_CIRCLE _ACTIVEEXTERNALALPHA_ROUND

            // Enable Alpha-Clip
            #pragma shader_feature_local_fragment _ _ACTIVEALPHACLIP_HARDCLIP _ACTIVEALPHACLIP_SMOOTHCLIP
            
            ENDHLSL
        }
        Pass
        {
            Tags
            {
                "LightMode" = "SceneSelectionPass"
            }

            BlendOp Add
            Blend One Zero
            ZWrite On
            Cull Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment fragSceneHighlight
            #pragma target 3.5
            
            // Include
            #include "Include/ParticleComponent.hlsl" 

            // Unity Keywords Define (GPU-Instance)
            #pragma multi_compile_instancing
            #pragma instancing_options procedural:ParticleInstancingSetup

            // Texture WrapMode  
            #pragma shader_feature_local_vertex _ _VERTEXOFFSETTEXWRAPMODE_LINEAR_REPEAT _VERTEXOFFSETTEXWRAPMODE_LINEAR_CLAMP _VERTEXOFFSETTEXWRAPMODE_POINT_REPEAT _VERTEXOFFSETTEXWRAPMODE_POINT_CLAMP
            #pragma shader_feature_local_vertex _ _VERTEXOFFSETMASKWRAPMODE_LINEAR_REPEAT _VERTEXOFFSETMASKWRAPMODE_LINEAR_CLAMP _VERTEXOFFSETMASKWRAPMODE_POINT_REPEAT _VERTEXOFFSETMASKWRAPMODE_POINT_CLAMP
            
            // Enable Vertex-Offset
            #pragma shader_feature_local _ _ACTIVEVERTEXOFFSET_ENABLE _ACTIVEVERTEXOFFSET_ANIMATIONTEX _ACTIVEVERTEXOFFSET_FULLSCREEN _ACTIVEVERTEXOFFSET_LOCALBILLBOARD _ACTIVEVERTEXOFFSET_CAMERAZAXISOFFSET
            #pragma shader_feature_local _ _VERTEXOFFSETTEXUVCHANNEL_UV2 
            #pragma shader_feature_local _ _ACTIVEVERTEXOFFSETMASK_ENABLE
            
            ENDHLSL
        }
        Pass
        {
            Tags
            {
                "LightMode" = "Picking"
            }

            BlendOp Add
            Blend One Zero
            ZWrite On
            Cull Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment fragScenePicking
            #pragma target 3.5

            // Include
            #include "Include/ParticleComponent.hlsl"
            
            // Unity Keywords Define (GPU-Instance)
            #pragma multi_compile_instancing
            #pragma instancing_options procedural:ParticleInstancingSetup

            // Texture WrapMode  
            #pragma shader_feature_local_vertex _ _VERTEXOFFSETTEXWRAPMODE_LINEAR_REPEAT _VERTEXOFFSETTEXWRAPMODE_LINEAR_CLAMP _VERTEXOFFSETTEXWRAPMODE_POINT_REPEAT _VERTEXOFFSETTEXWRAPMODE_POINT_CLAMP
            #pragma shader_feature_local_vertex _ _VERTEXOFFSETMASKWRAPMODE_LINEAR_REPEAT _VERTEXOFFSETMASKWRAPMODE_LINEAR_CLAMP _VERTEXOFFSETMASKWRAPMODE_POINT_REPEAT _VERTEXOFFSETMASKWRAPMODE_POINT_CLAMP
            
            // Enable Vertex-Offset
            #pragma shader_feature_local _ _ACTIVEVERTEXOFFSET_ENABLE _ACTIVEVERTEXOFFSET_ANIMATIONTEX _ACTIVEVERTEXOFFSET_FULLSCREEN _ACTIVEVERTEXOFFSET_LOCALBILLBOARD _ACTIVEVERTEXOFFSET_CAMERAZAXISOFFSET
            #pragma shader_feature_local _ _VERTEXOFFSETTEXUVCHANNEL_UV2
            #pragma shader_feature_local _ _ACTIVEVERTEXOFFSETMASK_ENABLE
            
            ENDHLSL
        }
    }
    CustomEditor "UberParticleShader.Editor.UberParticleEditor"
}