using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace UberParticleShader.Editor
{
    public class UberParticleEditor : UberParticleEditorGUIStyle
    {
        #region User Setting

        internal static bool refreshSetting { get; set; }
        private bool autoFixCustomVertexStreams { get; set; }
        private bool collapseFeatures { get; set; }

        #endregion

        #region Check Register Data

        private ParticleSystemRenderer[] selectedParticleSystemRenderers;
        private ParticleSystem[] selectedParticleSystems;
        private ParticleSystemRenderMode[] temp_RenderModeInstance;
        private bool temp_RenderModeInstance_Initialize;
        private bool[] temp_UsingMeshGPUInstance;
        private bool temp_UsingMeshGPUInstance_Initialize;
        private ParticleMode currentParticleMode;

        #endregion

        public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
        {
            base.OnGUI(materialEditor, properties);

            InitializeData();
            RefreshSetting();
            DrawLogoGUI();
            DrawAddComponentGUI();
            DrawWorkFlowModeGUI();
            FixCustomDataSettingGUI();
        }

        private void RefreshSetting()
        {
            if (refreshSetting)
            {
                // UserPrefs Setting, executing when detected modified.
                refreshSetting = false;
                UserPrefsDataRefresh();
                Check_ShowHideShaderPropertyGUI();
                Check_ShowHideConstantPropertyGUI();
            }

            Check_isEnableMeshGPUInstanceChanged();
        }

        private void InitializeData()
        {
            if (initialize == false)
            {
                initialize = true;

                // Check Enable Mesh GPU Instance 
                selectedParticleSystemRenderers = Selection.GetFiltered<ParticleSystemRenderer>(SelectionMode.Editable);
                selectedParticleSystems = Selection.GetFiltered<ParticleSystem>(SelectionMode.Editable);
                Initialize_EnableMeshGPUInstance(selectedParticleSystemRenderers);

                // Check Show/Hide ShaderProperties
                UserPrefsDataRefresh();
                Check_ShowHideShaderPropertyGUI();
                Check_ShowHideConstantPropertyGUI();
            }
        }

        private void UserPrefsDataRefresh()
        {
            autoFixCustomVertexStreams = UberParticleEditorSetting.Instance.AutoFixVertexStreams;
            collapseFeatures = UberParticleEditorSetting.Instance.CollapseFeatures;
        }

        private void Check_ShowHideShaderPropertyGUI()
        {
            if (collapseFeatures)
            {
                UberParticleFeatures.Distort = particleProperties.ActiveDistort.floatValue != 0;
                UberParticleFeatures.Dissolve = particleProperties.ActiveDissolve.floatValue != 0;
                UberParticleFeatures.Emission = particleProperties.ActiveEmission.floatValue != 0;
                UberParticleFeatures.ColorRamp = particleProperties.ActiveRamp.floatValue != 0;
                UberParticleFeatures.NormalMap = particleProperties.ActiveNormal.floatValue != 0;
                UberParticleFeatures.Parallax = particleProperties.ActiveParallax.floatValue != 0;
                UberParticleFeatures.Fresnel = particleProperties.ActiveFresnel.floatValue != 0;
                UberParticleFeatures.VertexOffset = particleProperties.ActiveVertexOffset.floatValue != 0;
                UberParticleFeatures.DoubleSideColor = particleProperties.ActiveSideFaceColor.floatValue != 0;
                UberParticleFeatures.SoftParticle = particleProperties.ActiveSoftParticle.floatValue != 0;
                UberParticleFeatures.LightSystem = particleProperties.ActiveLightSystem.floatValue != 0;
                UberParticleFeatures.ExternalAlpha = particleProperties.ActiveExternalAlpha.floatValue != 0;
                UberParticleFeatures.AlphaClip = particleProperties.ActiveAlphaClip.floatValue != 0;
                UberParticleFeatures.ColorAdjustment = particleProperties.ActiveColorAdjustment.floatValue != 0;
            }
            else
            {
                UberParticleFeatures.Distort = true;
                UberParticleFeatures.Dissolve = true;
                UberParticleFeatures.Emission = true;
                UberParticleFeatures.ColorRamp = true;
                UberParticleFeatures.NormalMap = true;
                UberParticleFeatures.Parallax = true;
                UberParticleFeatures.Fresnel = true;
                UberParticleFeatures.VertexOffset = true;
                UberParticleFeatures.DoubleSideColor = true;
                UberParticleFeatures.SoftParticle = true;
                UberParticleFeatures.LightSystem = true;
                UberParticleFeatures.ExternalAlpha = true;
                UberParticleFeatures.AlphaClip = true;
                UberParticleFeatures.ColorAdjustment = true;
            }
        }

        private void Check_ShowHideConstantPropertyGUI()
        {
            UberParticleConstantProperties.FinalOutput = UberParticleEditorSetting.Instance.FinalOutput;
            UberParticleConstantProperties.BlendSetting = UberParticleEditorSetting.Instance.BlendSetting;
            UberParticleConstantProperties.StencilSetting = UberParticleEditorSetting.Instance.StencilSetting;
            UberParticleConstantProperties.RenderQueueSetting = UberParticleEditorSetting.Instance.RenderQueueSetting;
            UberParticleConstantProperties.GPUInstancingSetting = UberParticleEditorSetting.Instance.GPUInstancingSetting;
        }

        private void DrawWorkFlowModeGUI()
        {
            SetParticleMode();
            currentParticleMode = (ParticleMode)particleProperties.ParticleMode.floatValue;
            switch (currentParticleMode)
            {
                case ParticleMode.Normal:
                    DrawMainTexGUI(MatEditor);
                    DrawDistortGUI(MatEditor);
                    DrawDissolveGUI(MatEditor);
                    DrawEmissionGUI(MatEditor);
                    DrawRampGUI(MatEditor);
                    DrawNormalGUI(MatEditor);
                    DrawParallaxGUI(MatEditor);
                    DrawFresnelGUI(MatEditor);
                    DrawVertexOffsetGUI(MatEditor);
                    DrawDoubleSideFaceGUI(MatEditor);
                    DrawSoftParticleGUI(MatEditor);
                    DrawLightSystemGUI(MatEditor);
                    DrawExternalAlphaGUI(MatEditor);
                    DrawAlphaClipGUI(MatEditor);
                    DrawColorAdjustmentGUI(MatEditor);
                    break;
                case ParticleMode.Geometry:
                    DrawMainTexGUI(MatEditor);
                    DrawDistortGUI(MatEditor);
                    DrawDissolveGUI(MatEditor);
                    DrawEmissionGUI(MatEditor);
                    DrawRampGUI(MatEditor);
                    DrawNormalGUI(MatEditor);
                    DrawParallaxGUI(MatEditor);
                    DrawFresnelGUI(MatEditor);
                    DrawVertexOffsetGUI(MatEditor);
                    DrawDoubleSideFaceGUI(MatEditor);
                    DrawSoftParticleGUI(MatEditor);
                    DrawLightSystemGUI(MatEditor);
                    DrawExternalAlphaGUI(MatEditor);
                    DrawAlphaClipGUI(MatEditor);
                    DrawColorAdjustmentGUI(MatEditor);
                    break;
                case ParticleMode.Decal:
                    DrawDecalGUI(MatEditor);
                    DrawMainTexGUI(MatEditor);
                    DrawDistortGUI(MatEditor);
                    DrawDissolveGUI(MatEditor);
                    DrawEmissionGUI(MatEditor);
                    DrawRampGUI(MatEditor);
                    DrawNormalGUI(MatEditor);
                    DrawParallaxGUI(MatEditor);
                    DrawFresnelGUI(MatEditor);
                    DrawVertexOffsetGUI(MatEditor);
                    DrawDoubleSideFaceGUI(MatEditor);
                    DrawSoftParticleGUI(MatEditor);
                    DrawLightSystemGUI(MatEditor);
                    DrawExternalAlphaGUI(MatEditor);
                    DrawAlphaClipGUI(MatEditor);
                    DrawColorAdjustmentGUI(MatEditor);
                    break;
                case ParticleMode.DistortionScreen:
                    DrawGrabPassGUI(MatEditor);
                    DrawMainTexGUI(MatEditor);
                    DrawDistortGUI(MatEditor);
                    DrawDissolveGUI(MatEditor);
                    DrawEmissionGUI(MatEditor);
                    DrawRampGUI(MatEditor);
                    DrawNormalGUI(MatEditor);
                    DrawParallaxGUI(MatEditor);
                    DrawFresnelGUI(MatEditor);
                    DrawVertexOffsetGUI(MatEditor);
                    DrawDoubleSideFaceGUI(MatEditor);
                    DrawSoftParticleGUI(MatEditor);
                    DrawLightSystemGUI(MatEditor);
                    DrawExternalAlphaGUI(MatEditor);
                    DrawAlphaClipGUI(MatEditor);
                    DrawColorAdjustmentGUI(MatEditor);
                    break;
            }

            DrawFinalOutputIntensityGUI(MatEditor);
            DrawBlendSettingGUI(MatEditor);
            DrawStencilSettingGUI(MatEditor);
            DrawRenderQueueGUI(MatEditor);
            DrawGPUInstancingGUI(MatEditor);
        }

        #region Draw GUI

        private void DrawLogoGUI()
        {
            var FullRect = EditorGUI.IndentedRect(EditorGUILayout.GetControlRect(false, LogoTex.height));

            var LogoRect = FullRect;
            var UserPrefsRect = FullRect;
            var RefreshRect = FullRect;

            GUI.Label(LogoRect, LogoTex, new GUIStyle { alignment = TextAnchor.MiddleCenter });

            UserPrefsRect.y += 5f;
            UserPrefsRect.height = 25f;
            UserPrefsRect.xMin = UserPrefsRect.xMax - 25f;
            UserPrefsRect.width = 25f;

            RefreshRect.height = 25f;
            RefreshRect.y += UserPrefsRect.height + 5f;
            RefreshRect.xMin = RefreshRect.xMax - 25f;
            RefreshRect.width = 25f;

            if (GUI.Button(UserPrefsRect, EditorGUIUtility.IconContent("CustomTool"))) UserPrefs.ShowWindow();

            if (GUI.Button(RefreshRect, EditorGUIUtility.IconContent("Refresh@2x"))) registerVertexStreams = true;
        }

        private void SetParticleMode()
        {
            switch (currentParticleMode)
            {
                case ParticleMode.Normal:
                    targetMat.SetShaderPassEnabled("UniversalForward", true);
                    targetMat.SetShaderPassEnabled("ShadowCaster", false);
                    targetMat.SetShaderPassEnabled("DepthOnly", false);
                    targetMat.renderQueue = 3000;
                    break;
                case ParticleMode.Geometry:
                    targetMat.SetShaderPassEnabled("UniversalForward", true);
                    targetMat.SetShaderPassEnabled("ShadowCaster", true);
                    targetMat.SetShaderPassEnabled("DepthOnly", true);
                    targetMat.renderQueue = 2000;
                    break;
                case ParticleMode.Decal:
                    targetMat.SetShaderPassEnabled("UniversalForward", true);
                    targetMat.SetShaderPassEnabled("ShadowCaster", false);
                    targetMat.SetShaderPassEnabled("DepthOnly", false);
                    targetMat.renderQueue = 3000;
                    break;
                case ParticleMode.DistortionScreen:
                    targetMat.SetShaderPassEnabled("UniversalForward", true);
                    targetMat.SetShaderPassEnabled("ShadowCaster", false);
                    targetMat.SetShaderPassEnabled("DepthOnly", false);
                    targetMat.renderQueue = 3000;
                    break;
            }
        }

        private void DrawGrabPassGUI(in MaterialEditor editor)
        {
            GUILayout.Space(10);
            ActiveModeShaderPropertyGUI(particleProperties.GrabPassTexMode, "Screen Distortion", FeatureType.Base);

            GUILayout.BeginVertical(boxStyle);
            {
                ToggleShaderPropertyGUI(particleProperties.DistortCenterUV, "Remap Distort Center", 1);
                SliderControllerShaderPropertyGUI(particleProperties.GrabPassIntensity, particleProperties.GrabPassIntensityController, "Intensity ", 1);
            }
            GUILayout.EndVertical();
        }

        private void DrawDecalGUI(in MaterialEditor editor)
        {
            GUILayout.Space(10);
            ActiveModeShaderPropertyGUI(particleProperties.DecalRenderMode, "Decal", FeatureType.Base);
            GUILayout.BeginVertical("box");
            {
                GUILayout.BeginVertical(boxStyle);
                {
                    DrawMessageTypeTitleBoxGUI("Using 'Cube Mesh' to play Decal.", MessageType.Info);
                    GUILayout.BeginVertical("box");
                    {
                        AdditionalTitleGUI("CameraNormal Mask");
                        editor.ShaderProperty(particleProperties.ActiveDecalWorldNormalMask, "World Normal Mask", 1);
                        if ((ActiveMode)particleProperties.ActiveDecalWorldNormalMask.floatValue == ActiveMode.Enable)
                        {
                            DrawMessageTypeTitleHelpBoxGUI("This function need to enable 'Decal' or other RenderFeature which can enable '_CameraNormalsTexture'.", MessageType.Warning);
                            MultiChannelsShaderPropertyGUI(particleProperties.DecalWorldNormalMaskAxis, "Mask Axis", 1, new[] { "X", "Y", "Z", "Abs" });
                        }
                    }
                    GUILayout.EndVertical();
                }
                GUILayout.EndVertical();
            }
            GUILayout.EndVertical();
        }

        private void DrawMainTexGUI(in MaterialEditor editor)
        {
            GUILayout.Space(10);
            var mainTexMode = (MainTexMode)particleProperties.MainTexMode.floatValue;
            ActiveModeShaderPropertyGUI(particleProperties.MainTexMode, "MainTex", FeatureType.Base);
            GUILayout.BeginVertical(boxStyle);
            {
                switch (mainTexMode)
                {
                    default:
                        TextureShaderPropertyGUI(particleProperties.MainTex, particleProperties.MainTexWrapMode, particleProperties.MainTexUVChannel, true, false, true);
                        TileOffsetControllerShaderPropertyGUI(particleProperties.MainTexUVTileController, particleProperties.MainTexUVAutoOffset, "Tile Offset Controller", ref MainTexTileOffsetController, nameof(MainTexTileOffsetController), true);
                        ToggleShaderPropertyGUI(particleProperties.ClampMainTexUV, "Clamp MainTex UV", 1);
                        break;
                    case MainTexMode.FlipBook:
                        TextureShaderPropertyGUI(particleProperties.MainTex, particleProperties.MainTexWrapMode, particleProperties.MainTexUVChannel, true, false, true);
                        FlipBookShaderPropertyGUI(particleProperties.FlipBookSetting, "Flip Book Setting", 1);
                        ToggleShaderPropertyGUI(particleProperties.ClampMainTexUV, "Clamp MainTex UV", 1);
                        break;
                    case MainTexMode.MultiChannel:
                        TextureShaderPropertyGUI(particleProperties.MainTex, particleProperties.MainTexWrapMode, particleProperties.MainTexUVChannel, true, false, true);
                        TileOffsetControllerShaderPropertyGUI(particleProperties.MainTexUVTileController, particleProperties.MainTexUVAutoOffset, "Tile Offset Controller", ref MainTexTileOffsetController, nameof(MainTexTileOffsetController), true);
                        GUILayout.BeginVertical("box");
                    {
                        GUILayout.Label("MultiChannel", upperCenterTitleStyle);
                        SliderControllerShaderPropertyGUI(particleProperties.MainTexG_Intensity, particleProperties.MainTexGIntensityController, "G Intensity", 1);
                        SliderControllerShaderPropertyGUI(particleProperties.MainTexB_Intensity, particleProperties.MainTexBIntensityController, "B Intensity", 1);
                        GUILayout.EndVertical();
                    }
                        ToggleShaderPropertyGUI(particleProperties.ClampMainTexUV, "Clamp MainTex UV", 1);
                        break;
                    case MainTexMode.PolarCoord:
                        TextureShaderPropertyGUI(particleProperties.MainTex, particleProperties.MainTexWrapMode, particleProperties.MainTexUVChannel, true, false, false);
                        TileOffsetControllerShaderPropertyGUI(particleProperties.MainTexUVTileController, particleProperties.MainTexUVAutoOffset, "Tile Offset Controller", ref MainTexTileOffsetController, nameof(MainTexTileOffsetController), true);
                        ToggleShaderPropertyGUI(particleProperties.ClampMainTexUV, "Clamp MainTex UV", 1);
                        break;
                    case MainTexMode.ScreenUV:
                        if (currentParticleMode == ParticleMode.Geometry)
                            DrawMessageTypeTitleBoxGUI("ShadowCaster doesn't support with ScreenUV Mode.", MessageType.Warning);
                        TextureShaderPropertyGUI(particleProperties.MainTex, particleProperties.MainTexWrapMode, particleProperties.MainTexUVChannel, true, false, true);
                        TileOffsetControllerShaderPropertyGUI(particleProperties.MainTexUVTileController, particleProperties.MainTexUVAutoOffset, "Tile Offset Controller", ref MainTexTileOffsetController, nameof(MainTexTileOffsetController), true);
                        ToggleShaderPropertyGUI(particleProperties.ClampMainTexUV, "Clamp MainTex UV", 1);
                        break;
                }

                editor.ShaderProperty(particleProperties.MainTexColor, "Color", 1);
            }
            GUILayout.EndVertical();
        }

        private void DrawDistortGUI(in MaterialEditor editor)
        {
            if (!UberParticleFeatures.Distort)
                return;

            GUILayout.Space(10);
            var distortMode = (DistortMode)particleProperties.ActiveDistort.floatValue;
            ActiveModeShaderPropertyGUI(particleProperties.ActiveDistort, "Distort", FeatureType.Addition);
            GUILayout.BeginVertical(boxStyle);
            switch (distortMode)
            {
                case DistortMode.Disable:
                    break;
                case DistortMode.Enable:
                case DistortMode.PolarCoord:
                    TextureShaderPropertyGUI(particleProperties.DistortTex, particleProperties.DistortTexWrapMode, particleProperties.DistortTexUVChannel, true, false, false);
                    TileOffsetControllerShaderPropertyGUI(particleProperties.DistortTexUVTileController, particleProperties.DistortTexUVAutoOffset, "Tile Offset Controller", ref DistortTexTileOffsetController, nameof(DistortTexTileOffsetController), true);
                    editor.ShaderProperty(particleProperties.DistortChannel, "Sample Channel", 1);
                    SliderControllerShaderPropertyGUI(particleProperties.DistortIntensity, particleProperties.DistortIntensityController, "Intensity", 1);
                    FlagEnumShaderPropertyGUI<DistortImpactTargets>(particleProperties.DistortImpactTarget, "Impact Targets", false, 1);
                    break;
                case DistortMode.ScreenUV:
                    if (currentParticleMode == ParticleMode.Geometry)
                        DrawMessageTypeTitleBoxGUI("ShadowCaster doesn't support with ScreenUV Mode.", MessageType.Warning);

                    TextureShaderPropertyGUI(particleProperties.DistortTex, particleProperties.DistortTexWrapMode, particleProperties.DistortTexUVChannel, true, false, true);
                    TileOffsetControllerShaderPropertyGUI(particleProperties.DistortTexUVTileController, particleProperties.DistortTexUVAutoOffset, "Tile Offset Controller", ref DistortTexTileOffsetController, nameof(DistortTexTileOffsetController), true);
                    editor.ShaderProperty(particleProperties.DistortChannel, "Sample Channel", 1);
                    SliderControllerShaderPropertyGUI(particleProperties.DistortIntensity, particleProperties.DistortIntensityController, "Intensity", 1);
                    FlagEnumShaderPropertyGUI<DistortImpactTargets>(particleProperties.DistortImpactTarget, "Impact Targets", false, 1);
                    break;
                case DistortMode.FlowMap:
                    TextureShaderPropertyGUI(particleProperties.DistortTex, particleProperties.DistortTexWrapMode, particleProperties.DistortTexUVChannel, true, false, true);
                    TileOffsetControllerShaderPropertyGUI(particleProperties.DistortTexUVTileController, particleProperties.DistortTexUVAutoOffset, "Tile Offset Controller", ref DistortTexTileOffsetController, nameof(DistortTexTileOffsetController), true);
                    ToggleShaderPropertyGUI(particleProperties.EnableHighPrecisionFlowMap, "High Precision", 1);
                    ToggleShaderPropertyGUI(particleProperties.ReverseFlowMap_G_Color, "Reverse G Color", 1);
                    SliderControllerShaderPropertyGUI(particleProperties.DistortIntensity, particleProperties.DistortIntensityController, "Intensity", 1);
                    FlagEnumShaderPropertyGUI<DistortImpactTargets>(particleProperties.DistortImpactTarget, "Impact Targets", false, 1);
                    break;
            }

            GUILayout.EndVertical();
        }

        private void DrawDissolveGUI(in MaterialEditor editor)
        {
            if (!UberParticleFeatures.Dissolve)
                return;

            GUILayout.Space(10);
            var dissolveMode = (DissolveMode)particleProperties.ActiveDissolve.floatValue;
            ActiveModeShaderPropertyGUI(particleProperties.ActiveDissolve, "Dissolve", FeatureType.Addition);
            GUILayout.BeginVertical(boxStyle);
            switch (dissolveMode)
            {
                case DissolveMode.Disable:
                    break;
                default:
                    TextureShaderPropertyGUI(particleProperties.DissolveTex, particleProperties.DissolveTexWrapMode, particleProperties.DissolveTexUVChannel, true, false, dissolveMode != DissolveMode.PolarCoord);
                    TileOffsetControllerShaderPropertyGUI(particleProperties.DissolveTexUVTileController, particleProperties.DissolveTexUVAutoOffset, "Tile Offset Controller", ref DissolveTexTileOffsetController, nameof(DissolveTexTileOffsetController), true);
                    editor.ShaderProperty(particleProperties.DissolveChannel, "Sample Channel", 1);
                    editor.ShaderProperty(particleProperties.DissolveCalculateMode, "Dissolve Calculate Mode", 1);
                    SliderControllerShaderPropertyGUI(particleProperties.DissolveIntensity, particleProperties.DissolveIntensityController, "Intensity", 1);
                    ToggleShaderPropertyGUI(particleProperties.ActiveHardClipDissolve, "Hard Clip", 1);
                    DrawRimDissolveGUI(editor);
                    break;

                case DissolveMode.ScreenUV:
                    if (currentParticleMode == ParticleMode.Geometry)
                        DrawMessageTypeTitleBoxGUI("ShadowCaster doesn't support with ScreenUV Mode.", MessageType.Warning);

                    TextureShaderPropertyGUI(particleProperties.DissolveTex, particleProperties.DissolveTexWrapMode, particleProperties.DissolveTexUVChannel, true, false, true);
                    TileOffsetControllerShaderPropertyGUI(particleProperties.DissolveTexUVTileController, particleProperties.DissolveTexUVAutoOffset, "Tile Offset Controller", ref DissolveTexTileOffsetController, nameof(DissolveTexTileOffsetController), true);
                    editor.ShaderProperty(particleProperties.DissolveChannel, "Sample Channel", 1);
                    editor.ShaderProperty(particleProperties.DissolveCalculateMode, "Dissolve Calculate Mode", 1);
                    SliderControllerShaderPropertyGUI(particleProperties.DissolveIntensity, particleProperties.DissolveIntensityController, "Intensity", 1);
                    ToggleShaderPropertyGUI(particleProperties.ActiveHardClipDissolve, "Hard Clip", 1);
                    DrawRimDissolveGUI(editor);

                    break;
            }

            GUILayout.EndVertical();
        }

        private void DrawRimDissolveGUI(in MaterialEditor editor)
        {
            GUILayout.BeginVertical("box");
            {
                AdditionalTitleGUI("Rim Dissolve");
                editor.ShaderProperty(particleProperties.ActiveRimDissolve, "Rim Dissolve", 1);
                if (particleProperties.ActiveRimDissolve.floatValue != 0)
                {
                    SliderControllerShaderPropertyGUI(particleProperties.RimDissolveRange, particleProperties.RimDissolveRangeController, "Range", 1);
                    editor.ShaderProperty(particleProperties.RimDissolveColorMode, "Color Mode", 1);
                    editor.ShaderProperty(particleProperties.RimDissolveColor, "Color", 1);
                }
            }
            GUILayout.EndVertical();
        }

        private void DrawEmissionGUI(in MaterialEditor editor)
        {
            if (!UberParticleFeatures.Emission)
                return;

            GUILayout.Space(10);
            var emissionMode = (EmissionMode)particleProperties.ActiveEmission.floatValue;
            ActiveModeShaderPropertyGUI(particleProperties.ActiveEmission, "Emission", FeatureType.Addition);
            GUILayout.BeginVertical(boxStyle);
            switch (emissionMode)
            {
                case EmissionMode.Disable:
                    break;
                case EmissionMode.Enable:
                case EmissionMode.PolarCoord:
                case EmissionMode.ScreenUV:
                    TextureShaderPropertyGUI(particleProperties.EmissionTex, particleProperties.EmissionTexWrapMode, particleProperties.EmissionTexUVChannel, true, false, emissionMode != EmissionMode.PolarCoord);
                    TileOffsetControllerShaderPropertyGUI(particleProperties.EmissionTexUVTileController, particleProperties.EmissionTexUVAutoOffset, "Tile Offset Controller", ref EmissionTexTileOffsetController, nameof(EmissionTexTileOffsetController), true);
                    SliderControllerShaderPropertyGUI(particleProperties.EmissionIntensity, particleProperties.EmissionIntensityController, "Intensity", 1);
                    editor.ShaderProperty(particleProperties.EmissionColorMode, "Color Mode", 1);
                    editor.ShaderProperty(particleProperties.EmissionColor, "Color", 1);
                    break;
            }

            GUILayout.EndVertical();
        }

        private void DrawRampGUI(in MaterialEditor editor)
        {
            if (!UberParticleFeatures.ColorRamp)
                return;

            GUILayout.Space(10);
            var rampMode = (ActiveMode)particleProperties.ActiveRamp.floatValue;
            ActiveModeShaderPropertyGUI(particleProperties.ActiveRamp, "Color Ramp", FeatureType.Addition);
            GUILayout.BeginVertical(boxStyle);
            {
                if (rampMode == ActiveMode.Enable)
                {
                    TextureShaderPropertyGUI(particleProperties.RampTex, true, true);
                    editor.ShaderProperty(particleProperties.EnhanceRampColor, "Enhance Color", 1);

                    GUILayout.BeginVertical("Box");
                    {
                        AdditionalTitleGUI("Edit GradientMap Tool");
                        GradientMapShaderPropertyGUI(particleProperties.RampTex, "Gradient Color", false, 1);
                    }
                    GUILayout.EndVertical();
                }
            }

            GUILayout.EndVertical();
        }

        private void DrawNormalGUI(in MaterialEditor editor)
        {
            if (!UberParticleFeatures.NormalMap)
                return;

            GUILayout.Space(10);
            var normalMode = (ActiveMode)particleProperties.ActiveNormal.floatValue;
            ActiveModeShaderPropertyGUI(particleProperties.ActiveNormal, "NormalMap", FeatureType.Addition);
            GUILayout.BeginVertical(boxStyle);
            switch (normalMode)
            {
                case ActiveMode.Disable:
                    break;
                case ActiveMode.Enable:
                    TextureShaderPropertyGUI(particleProperties.NormalTex, particleProperties.NormalTexWrapMode,
                        particleProperties.NormalTexUVChannel,
                        true, false, true);
                    TileOffsetControllerShaderPropertyGUI(particleProperties.NormalTexUVTileController, particleProperties.NormalTexUVAutoOffset,
                        "Tile Offset Controller", ref NormalTexTileOffsetController, nameof(NormalTexTileOffsetController), true);
                    SliderControllerShaderPropertyGUI(particleProperties.NormalIntensity, particleProperties.NormalIntensityController, "Intensity",
                        1);
                    editor.ShaderProperty(particleProperties.NormalBounceLightRange, "Bounce Light Range", 1);
                    break;
            }

            GUILayout.EndVertical();
        }

        private void DrawParallaxGUI(in MaterialEditor editor)
        {
            if (!UberParticleFeatures.Parallax)
                return;

            GUILayout.Space(10);
            var parallaxMode = (ParallaxMode)particleProperties.ActiveParallax.floatValue;
            ActiveModeShaderPropertyGUI(particleProperties.ActiveParallax, "Parallax", FeatureType.Addition);
            GUILayout.BeginVertical(boxStyle);
            switch (parallaxMode)
            {
                case ParallaxMode.Disable:
                    break;
                case ParallaxMode.Enable:
                    TextureShaderPropertyGUI(particleProperties.ParallaxTex, particleProperties.ParallaxTexWrapMode,
                        particleProperties.ParallaxTexUVChannel, true, false, true);
                    TileOffsetControllerShaderPropertyGUI(particleProperties.ParallaxTexUVTileController, particleProperties.ParallaxTexUVAutoOffset,
                        "Tile Offset Controller", ref ParallaxTexTileOffsetController, nameof(ParallaxTexTileOffsetController), true);
                    editor.ShaderProperty(particleProperties.ParallaxTexChannel, "Sample Channel", 1);
                    FlagEnumShaderPropertyGUI<ParallaxImpactTargets>(particleProperties.ParallaxImpactTarget, "Impact Targets", false, 1);
                    SliderControllerShaderPropertyGUI(particleProperties.ParallaxIntensity, particleProperties.ParallaxIntensityController,
                        "Intensity",
                        1);
                    break;
                case ParallaxMode.Steep:
                    DrawMessageTypeTitleHelpBoxGUI("1.'Render Mode' must be changed to 'Mesh', and use the 'Quad' model built into Unity.\r\n" +
                                                   "2.Turn on the 'Enable Mesh GPU Instancing'.", MessageType.Info);
                    TextureShaderPropertyGUI(particleProperties.ParallaxTex, particleProperties.ParallaxTexWrapMode,
                        particleProperties.ParallaxTexUVChannel, true, false, true);
                    TileOffsetControllerShaderPropertyGUI(particleProperties.ParallaxTexUVTileController, particleProperties.ParallaxTexUVAutoOffset,
                        "Tile Offset Controller", ref ParallaxTexTileOffsetController, nameof(ParallaxTexTileOffsetController), true);
                    editor.ShaderProperty(particleProperties.ParallaxTexChannel, "Sample Channel", 1);
                    FlagEnumShaderPropertyGUI<ParallaxImpactTargets>(particleProperties.ParallaxImpactTarget, "Impact Targets", false, 1);
                    DrawMessageTypeTitleBoxGUI("Impact Targets will overwrite the original texture mode like polarCoord or ScreenUV.", MessageType.Info);
                    SliderControllerShaderPropertyGUI(particleProperties.ParallaxIntensity, particleProperties.ParallaxIntensityController,
                        "Intensity",
                        1);

                    editor.ShaderProperty(particleProperties.HeightSampleSteps, "Sample Steps", 1);

                    GUILayout.BeginVertical("box");
                    EditorGUI.indentLevel++;
                    var folderRect = EditorGUI.IndentedRect(EditorGUILayout.GetControlRect(false, texturesinglePropertyHeight));
                    EditorGUI.indentLevel--;

                    Parallax_HeightMapNoiseFolder =
                        EditorGUI.Foldout(folderRect, Parallax_HeightMapNoiseFolder, "Ray Step Distance Noise", true, folderStyle);
                    if (Parallax_HeightMapNoiseFolder)
                    {
                        EditorPrefs.SetBool(nameof(Parallax_HeightMapNoiseFolder), true);
                        TextureShaderPropertyGUI(particleProperties.ParallaxNoiseStepTex, particleProperties.ParallaxNoiseStepTexWrapMode, true,
                            false);
                        editor.ShaderProperty(particleProperties.ParallaxNoiseTexChannel, "Sample Channel", 1);
                        editor.ShaderProperty(particleProperties.ParallaxNoiseIntensity, "Intensity", 1);
                    }
                    else
                    {
                        EditorPrefs.SetBool(nameof(Parallax_HeightMapNoiseFolder), false);
                    }

                    GUILayout.EndVertical();

                    break;
            }

            GUILayout.EndVertical();
        }

        private void DrawFresnelGUI(in MaterialEditor editor)
        {
            if (!UberParticleFeatures.Fresnel)
                return;

            GUILayout.Space(10);
            var fresnelMode = (ActiveMode)particleProperties.ActiveFresnel.floatValue;
            ActiveModeShaderPropertyGUI(particleProperties.ActiveFresnel, "Fresnel", FeatureType.Addition);
            GUILayout.BeginVertical(boxStyle);
            switch (fresnelMode)
            {
                case ActiveMode.Disable:
                    break;
                case ActiveMode.Enable:
                    FlagEnumShaderPropertyGUI<FresnelFunction>(particleProperties.FresnelFunction, "Functions", true, 1);
                    var fresnelFunction = (FresnelFunction)particleProperties.FresnelFunction.floatValue;
                    if ((fresnelFunction & FresnelFunction.FresnelColor) == FresnelFunction.FresnelColor ||
                        (fresnelFunction & FresnelFunction.FresnelAlpha) == FresnelFunction.FresnelAlpha)
                    {
                        SliderControllerShaderPropertyGUI(particleProperties.FresnelRange, particleProperties.FresnelRangeController, "Range", 1);
                        SliderControllerShaderPropertyGUI(particleProperties.FresnelPower, particleProperties.FresnelPowerController, "Power", 1);
                        ToggleShaderPropertyGUI(particleProperties.FlipInnerFaceNormal, "Flip Inner Face Normal", 1);
                    }

                    if ((fresnelFunction & FresnelFunction.FresnelColor) == FresnelFunction.FresnelColor)
                    {
                        editor.ShaderProperty(particleProperties.FresnelColor, "Color", 1);
                        ToggleShaderPropertyGUI(particleProperties.FlipFresnelColorRange, "Flip Color Range", 1);
                    }

                    if ((fresnelFunction & FresnelFunction.FresnelAlpha) == FresnelFunction.FresnelAlpha)
                        ToggleShaderPropertyGUI(particleProperties.FlipFresnelAlphaRange, "Flip Alpha Range", 1);


                    break;
            }

            GUILayout.EndVertical();
        }

        private void DrawVertexOffsetGUI(in MaterialEditor editor)
        {
            if (!UberParticleFeatures.VertexOffset)
                return;

            GUILayout.Space(10);
            var vertexOffsetMode = (VertexOffsetMode)particleProperties.ActiveVertexOffset.floatValue;
            ActiveModeShaderPropertyGUI(particleProperties.ActiveVertexOffset, "Vertex Offset", FeatureType.Addition);
            GUILayout.BeginVertical(boxStyle);
            switch (vertexOffsetMode)
            {
                case VertexOffsetMode.Disable:
                    break;
                case VertexOffsetMode.Enable:
                    TextureShaderPropertyGUI(particleProperties.VertexOffsetTex, particleProperties.VertexOffsetTexWrapMode,
                        particleProperties.VertexOffsetTexUVChannel, true, false, true);
                    TileOffsetControllerShaderPropertyGUI(particleProperties.VertexOffsetTexUVTileController,
                        particleProperties.VertexOffsetTexUVAutoOffset, "Tile Offset Controller", ref VertexOffsetTexTileOffsetController,
                        nameof(VertexOffsetTexTileOffsetController), true);
                    editor.ShaderProperty(particleProperties.VertexOffsetDirection, "Direction", 1);
                    MultiChannelsShaderPropertyGUI(particleProperties.VertexOffsetTexSampleChannels, "Sample Channels", 1);
                    SliderControllerShaderPropertyGUI(particleProperties.VertexOffsetIntensity, particleProperties.VertexOffsetIntensityController,
                        "Intensity", 1);

                    GUILayout.BeginVertical("box");
                    AdditionalTitleGUI("Vertex Offset Mask");
                    editor.ShaderProperty(particleProperties.ActiveVertexOffsetMask, "Vertex Offset Mask", 1);
                    if (particleProperties.ActiveVertexOffsetMask.floatValue != 0)
                    {
                        TextureShaderPropertyGUI(particleProperties.VertexOffsetMask, particleProperties.VertexOffsetMaskWrapMode, true,
                            false);
                        TileOffsetControllerShaderPropertyGUI(particleProperties.VertexOffsetMaskUVTileController,
                            particleProperties.VertexOffsetMaskUVAutoOffset, "Tile Offset Controller", ref VertexOffsetMaskTileOffsetController,
                            nameof(VertexOffsetMaskTileOffsetController), true);
                        MultiChannelsShaderPropertyGUI(particleProperties.VertexOffsetMaskSampleChannels, "Sample Channels", 1);
                        SliderControllerShaderPropertyGUI(particleProperties.VertexOffsetMaskIntensity,
                            particleProperties.VertexOffsetMaskIntensityController, "Intensity", 1);
                    }

                    GUILayout.EndVertical();
                    break;
                case VertexOffsetMode.AnimationTex:
                    EditorGUILayout.HelpBox("Need to use 'Mesh' renderMode and turn on 'Enable Mesh Gpu Instance'.", MessageType.Info);
                    DrawBoxTitleGUI("Vertex Offset Map", true);
                    TextureShaderPropertyGUI(particleProperties.VertexOffsetTex, true, false);
                    DrawBoxTitleGUI("Normal Offset Map", true);
                    TextureShaderPropertyGUI(particleProperties.AnimationNormalOffsetTex, true, false);
                    ToggleShaderPropertyGUI(particleProperties.ReClampAnimationVertexVector, "ReClamp Vector [0,1] to [-1,1]", 1);
                    editor.ShaderProperty(particleProperties.AnimationTexUVChannel, "Sample Channel", 1);
                    editor.ShaderProperty(particleProperties.AnimationVertexVectorMode, "Vector Mode", 1);
                    SliderControllerShaderPropertyGUI(particleProperties.AnimationPlayableSlider,
                        particleProperties.AnimationPlayableSliderController,
                        "Current Frame", 1);
                    break;
                case VertexOffsetMode.FullScreen:
                    EditorGUILayout.HelpBox("Enable FullScreen need to turn the 'ZTest' to 'Always'.", MessageType.Info);
                    break;
                case VertexOffsetMode.LocalBillboard:
                    EditorGUILayout.HelpBox("'Render Mode' must be changed to 'Mesh', and use the 'Quad' model built into Unity.", MessageType.Info);
                    break;
                case VertexOffsetMode.CameraZAxisOffset:
                    SliderControllerShaderPropertyGUI(particleProperties.VertexOffsetCameraDistance,
                        particleProperties.VertexOffsetCameraDistanceController, "Camera Z-Axis Offset", 1);
                    break;
            }

            GUILayout.EndVertical();
        }

        private void DrawDoubleSideFaceGUI(in MaterialEditor editor)
        {
            if (!UberParticleFeatures.DoubleSideColor)
                return;

            GUILayout.Space(10);
            var sideFaceColorMode = (DoubleSideColorMode)particleProperties.ActiveSideFaceColor.floatValue;
            ActiveModeShaderPropertyGUI(particleProperties.ActiveSideFaceColor, "Double-Side Color", FeatureType.Addition);
            GUILayout.BeginVertical(boxStyle);
            switch (sideFaceColorMode)
            {
                case DoubleSideColorMode.Disable:
                    break;
                case DoubleSideColorMode.Replace:
                case DoubleSideColorMode.Add:
                case DoubleSideColorMode.Multiply:
                    editor.ShaderProperty(particleProperties.SideColor, "Color", 1);
                    editor.ShaderProperty(particleProperties.ChangeSide, "Change Side", 1);
                    break;
            }

            GUILayout.EndVertical();
        }

        private void DrawSoftParticleGUI(in MaterialEditor editor)
        {
            if (!UberParticleFeatures.SoftParticle)
                return;

            GUILayout.Space(10);
            var softParticleMode = (ActiveMode)particleProperties.ActiveSoftParticle.floatValue;
            ActiveModeShaderPropertyGUI(particleProperties.ActiveSoftParticle, "Soft-Particle", FeatureType.Addition);
            GUILayout.BeginVertical(boxStyle);

            switch (softParticleMode)
            {
                case ActiveMode.Disable:
                    break;
                case ActiveMode.Enable:
                    if (currentParticleMode == ParticleMode.Geometry)
                        DrawMessageTypeTitleBoxGUI("This function is not work with Geometry Mode", MessageType.Error);

                    editor.ShaderProperty(particleProperties.SoftRange, "Soft Range", 1);
                    break;
            }

            GUILayout.EndVertical();
        }

        private void DrawLightSystemGUI(in MaterialEditor editor)
        {
            if (!UberParticleFeatures.LightSystem)
                return;

            GUILayout.Space(10);
            var lightSystemMode = (LightSystemMode)particleProperties.ActiveLightSystem.floatValue;
            ActiveModeShaderPropertyGUI(particleProperties.ActiveLightSystem, "Light System", FeatureType.Addition);
            GUILayout.BeginVertical(boxStyle);

            switch (lightSystemMode)
            {
                case LightSystemMode.Disable:
                    break;
                case LightSystemMode.VertexLight:
                case LightSystemMode.FragmentLight:
                    DrawBoxTitleGUI("Main Light", true);
                    editor.ShaderProperty(particleProperties.MainLightIntensity, "Main Light Intensity", 1);
                    editor.ShaderProperty(particleProperties.MainLightBounceRange, "Main Light Bounce Range", 1);
                    DrawBoxTitleGUI("Additional Light", true);
                    editor.ShaderProperty(particleProperties.AdditionalLightIntensity, "Additional Light Intensity", 1);
                    editor.ShaderProperty(particleProperties.AdditionalBounceRange, "Additional Light Bounce Range", 1);
                    DrawBoxTitleGUI("Shadow", true);
                    editor.ShaderProperty(particleProperties.ReceiveShadow, "Receive Shadow", 1);
                    break;
            }

            GUILayout.EndVertical();
        }

        private void DrawExternalAlphaGUI(in MaterialEditor editor)
        {
            if (!UberParticleFeatures.ExternalAlpha)
                return;

            GUILayout.Space(10);
            var externalAlphaMode = (ExternalAlphaMode)particleProperties.ActiveExternalAlpha.floatValue;
            ActiveModeShaderPropertyGUI(particleProperties.ActiveExternalAlpha, "External Alpha Template", FeatureType.Addition);
            GUILayout.BeginVertical(boxStyle);
            switch (externalAlphaMode)
            {
                case ExternalAlphaMode.Disable:
                    break;
                case ExternalAlphaMode.Line:
                case ExternalAlphaMode.Gradient:
                    editor.ShaderProperty(particleProperties.LineAlphaMode, "Direction", 1);
                    editor.ShaderProperty(particleProperties.InverseAlphaVal, "Inverse Range", 1);
                    SliderControllerShaderPropertyGUI(particleProperties.ExternalAlphaRange, particleProperties.ExternalAlphaRangeController, "Range",
                        1);
                    SliderControllerShaderPropertyGUI(particleProperties.ExternalAlphaPower, particleProperties.ExternalAlphaPowerController, "Power",
                        1);
                    SliderControllerShaderPropertyGUI(particleProperties.ExternalAlphaIntensity, particleProperties.ExternalAlphaIntensityController, "Intensity",
                        1);
                    break;
                case ExternalAlphaMode.Circle:
                case ExternalAlphaMode.Round:
                    editor.ShaderProperty(particleProperties.InverseAlphaVal, "Inverse Range", 1);
                    SliderControllerShaderPropertyGUI(particleProperties.ExternalAlphaRange, particleProperties.ExternalAlphaRangeController, "Range",
                        1);
                    SliderControllerShaderPropertyGUI(particleProperties.ExternalAlphaPower, particleProperties.ExternalAlphaPowerController, "Power",
                        1);
                    SliderControllerShaderPropertyGUI(particleProperties.ExternalAlphaIntensity, particleProperties.ExternalAlphaIntensityController, "Intensity",
                        1);
                    break;
            }

            GUILayout.EndVertical();
        }

        private void DrawAlphaClipGUI(in MaterialEditor editor)
        {
            if (!UberParticleFeatures.AlphaClip)
                return;

            GUILayout.Space(10);
            var alphaClipMode = (AlphaClipMode)particleProperties.ActiveAlphaClip.floatValue;
            ActiveModeShaderPropertyGUI(particleProperties.ActiveAlphaClip, "Alpha-Clip", FeatureType.Addition);
            GUILayout.BeginVertical(boxStyle);
            switch (alphaClipMode)
            {
                case AlphaClipMode.Disable:
                    break;
                default:
                    SliderControllerShaderPropertyGUI(particleProperties.AlphaClipRange, particleProperties.AlphaClipRangeController,
                        "Alpha Clip Range",
                        1);
                    break;
            }

            GUILayout.EndVertical();
        }

        private void DrawColorAdjustmentGUI(in MaterialEditor editor)
        {
            if (!UberParticleFeatures.ColorAdjustment)
                return;
            GUILayout.Space(10);
            var colorAdjustmentMode = (ActiveMode)particleProperties.ActiveColorAdjustment.floatValue;
            ActiveModeShaderPropertyGUI(particleProperties.ActiveColorAdjustment, "Color Adjustment", FeatureType.Addition);
            GUILayout.BeginVertical(boxStyle);
            switch (colorAdjustmentMode)
            {
                case ActiveMode.Disable:
                    break;
                case ActiveMode.Enable:
                    var tempFunctionVal = (ColorAdjustmentFunction)particleProperties.ColorAdjustmentLayerMask.floatValue;
                    FlagEnumShaderPropertyGUI<ColorAdjustmentFunction>(particleProperties.ColorAdjustmentLayerMask, "Functions", true, 1);
                    if ((tempFunctionVal & ColorAdjustmentFunction.HUEShift) == ColorAdjustmentFunction.HUEShift)
                        SliderControllerShaderPropertyGUI(particleProperties.HUEShiftVal, particleProperties.HUEShiftValController, "HUE-Shift", 1);
                    if ((tempFunctionVal & ColorAdjustmentFunction.Saturation) == ColorAdjustmentFunction.Saturation)
                        SliderControllerShaderPropertyGUI(particleProperties.SaturationVal, particleProperties.SaturationValController, "Saturation",
                            1);
                    if ((tempFunctionVal & ColorAdjustmentFunction.Contrast) == ColorAdjustmentFunction.Contrast)
                        SliderControllerShaderPropertyGUI(particleProperties.ContrastVal, particleProperties.ContrastValController, "Contrast", 1);
                    if ((tempFunctionVal & ColorAdjustmentFunction.AntiShineGlow) == ColorAdjustmentFunction.AntiShineGlow)
                        SliderControllerShaderPropertyGUI(particleProperties.AntiShineGlowVal, particleProperties.AntiShineGlowValController, "Anti ShineGlow Val",
                            1);
                    break;
            }

            GUILayout.EndVertical();
        }

        private void DrawFinalOutputIntensityGUI(in MaterialEditor editor)
        {
            if (!UberParticleConstantProperties.FinalOutput)
                return;

            GUILayout.Space(10);
            DrawTitleGUI("FinalOutput", true);
            GUILayout.BeginVertical(boxStyle);
            SliderControllerShaderPropertyGUI(particleProperties.FinalColorIntensity, particleProperties.FinalColorIntensityController, "Color", 1);
            SliderControllerShaderPropertyGUI(particleProperties.FinalAlphaIntensity, particleProperties.FinalAlphaIntensityController, "Alpha", 1);
            GUILayout.EndVertical();
        }

        private void DrawBlendSettingGUI(in MaterialEditor editor)
        {
            if (!UberParticleConstantProperties.BlendSetting)
                return;

            GUILayout.Space(10);
            DrawTitleGUI("Blend Setting", true);
            GUILayout.BeginVertical(boxStyle);
            {
                EditorGUI.indentLevel++;
                {
                    // Draw Particle Mode:
                    editor.ShaderProperty(particleProperties.ParticleMode, "Particle Mode");

                    // Draw Render Mode:
                    if (currentParticleMode == ParticleMode.Geometry)
                        particleProperties.RenderMode.floatValue = (float)RenderMode.Custom;
                    else
                        particleProperties.RenderMode.floatValue =
                            Convert.ToInt32(EditorGUILayout.EnumPopup("Render Mode", (RenderMode)particleProperties.RenderMode.floatValue));
                }
                EditorGUI.indentLevel--;
                var renderType = (RenderMode)particleProperties.RenderMode.floatValue;

                switch (renderType)
                {
                    case RenderMode.Normal:
                        particleProperties.SourceBlend.floatValue = (float)BlendMode.SrcAlpha;
                        particleProperties.DestBlend.floatValue = (float)BlendMode.OneMinusSrcAlpha;
                        break;
                    case RenderMode.Additive:
                        particleProperties.SourceBlend.floatValue = (float)BlendMode.SrcAlpha;
                        particleProperties.DestBlend.floatValue = (float)BlendMode.One;
                        break;
                    case RenderMode.PreMultiply:
                        particleProperties.SourceBlend.floatValue = (float)BlendMode.One;
                        particleProperties.DestBlend.floatValue = (float)BlendMode.OneMinusSrcAlpha;
                        break;
                    case RenderMode.Custom:
                        GUILayout.BeginVertical("box");
                    {
                        using (new EditorGUI.DisabledScope(currentParticleMode == ParticleMode.Geometry))
                        {
                            editor.ShaderProperty(particleProperties.SourceBlend, "Source", 1);
                            editor.ShaderProperty(particleProperties.DestBlend, "Dest", 1);
                        }

                        if (currentParticleMode == ParticleMode.Geometry)
                        {
                            particleProperties.SourceBlend.floatValue = 1;
                            particleProperties.DestBlend.floatValue = 0;
                            GUILayout.BeginHorizontal();
                            GUILayout.FlexibleSpace();
                            GUILayout.Label(EditorGUIUtility.TrTextContentWithIcon("Geometry Blend need to be One Zero.", MessageType.Info));
                            GUILayout.FlexibleSpace();
                            GUILayout.EndHorizontal();
                        }
                    }
                        GUILayout.EndVertical();
                        break;
                }

                editor.ShaderProperty(particleProperties.Cull, "Cull", 1);
                using (new EditorGUI.DisabledScope(currentParticleMode == ParticleMode.Geometry))
                {
                    editor.ShaderProperty(particleProperties.ZWrite, "ZWrite", 1);
                    if (currentParticleMode == ParticleMode.Geometry)
                        particleProperties.ZWrite.floatValue = 1;
                }

                editor.ShaderProperty(particleProperties.ZTest, "ZTest", 1);
            }
            GUILayout.EndVertical();
        }

        private void DrawStencilSettingGUI(in MaterialEditor editor)
        {
            if (!UberParticleConstantProperties.StencilSetting)
                return;

            GUILayout.Space(10);
            DrawTitleGUI("Stencil Setting", true);
            GUILayout.BeginVertical(boxStyle);
            editor.ShaderProperty(particleProperties.Ref, "Reference Val", 1);
            editor.ShaderProperty(particleProperties.Comp, "Compare Mode", 1);
            editor.ShaderProperty(particleProperties.Pass, "Pass Mode", 1);
            editor.ShaderProperty(particleProperties.Fail, "Fail Mode", 1);
            editor.ShaderProperty(particleProperties.ZFail, "ZFail Mode", 1);
            GUILayout.EndVertical();
        }

        private void DrawRenderQueueGUI(in MaterialEditor editor)
        {
            if (!UberParticleConstantProperties.RenderQueueSetting)
                return;

            editor.RenderQueueField();
        }

        private void DrawGPUInstancingGUI(in MaterialEditor editor)
        {
            if (!UberParticleConstantProperties.GPUInstancingSetting)
                return;

            editor.EnableInstancingField();
        }

        private void DrawAddComponentGUI()
        {
            if (collapseFeatures)
                AddComponentShaderPropertyGUI();
        }

        #endregion

        #region Register Custom Vertex Stream

        private void Initialize_EnableMeshGPUInstance(in ParticleSystemRenderer[] sources)
        {
            // Initialize 'Enable Mesh GPU Instance' first for later compare.
            if (temp_UsingMeshGPUInstance == null)
                temp_UsingMeshGPUInstance = new bool[sources.Length];

            if (temp_UsingMeshGPUInstance_Initialize != true)
            {
                temp_UsingMeshGPUInstance_Initialize = true;
                for (var i = 0; i < sources.Length; i++) temp_UsingMeshGPUInstance.SetValue(sources[i].enableGPUInstancing, i);
            }

            // Initialize 'RenderMode' first for later compare.
            if (temp_RenderModeInstance == null)
                temp_RenderModeInstance = new ParticleSystemRenderMode[sources.Length];

            if (temp_RenderModeInstance_Initialize != true)
            {
                temp_RenderModeInstance_Initialize = true;
                for (var i = 0; i < sources.Length; i++) temp_RenderModeInstance.SetValue(sources[i].renderMode, i);
            }
        }

        private void Check_isEnableMeshGPUInstanceChanged()
        {
            if (!autoFixCustomVertexStreams)
                return;

            var isEnableMeshGPUInstanceChanged = selectedParticleSystemRenderers.Select(go => go.enableGPUInstancing).ToArray();
            var isRenderModeChanged = selectedParticleSystemRenderers.Select(go => go.renderMode).ToArray();

            if (temp_UsingMeshGPUInstance.Length != isEnableMeshGPUInstanceChanged.Length)
            {
                Debug.Log(
                    "Check Enable Mesh GPU Index was outside the bounds of the array.\r\nPlease reselected particle systems to refresh the data.");
                return;
            }

            // Check the 'RenderMode' is been changed or not.
            for (var i = 0; i < temp_RenderModeInstance.Length; i++)
                if (temp_RenderModeInstance[i] != isRenderModeChanged[i])
                {
                    temp_RenderModeInstance[i] = isRenderModeChanged[i];
                    registerVertexStreams = true;
                }

            // Check the 'Enable Mesh GPU Instance' is been changed or not.
            for (var i = 0; i < temp_UsingMeshGPUInstance.Length; i++)
                if (temp_UsingMeshGPUInstance[i] != isEnableMeshGPUInstanceChanged[i])
                {
                    temp_UsingMeshGPUInstance[i] = isEnableMeshGPUInstanceChanged[i];
                    registerVertexStreams = true;
                }
        }

        private void FixCustomDataSettingGUI()
        {
            if (registerVertexStreams && autoFixCustomVertexStreams)
            {
                RegisterCustomVertexStreams();
#if UNITY_2022_3_OR_NEWER
                RegisterTrailCustomVertexStreams();
#endif
            }
        }

        private void RegisterCustomVertexStreams()
        {
            // Make sure the material be control is not a trail material or register will be set wrong. 
            for (var i = 0; i < selectedParticleSystemRenderers.Length; i++)
            {
                if (selectedParticleSystemRenderers[i].sharedMaterial != targetMat)
                    return;
            }

            Debug.Log("Register Custom Vertex Streams!");
            registerVertexStreams = false;

            CheckVertexStreams_CustomData();
            CheckVertexStreams_Normal();
            CheckVertexStreams_Tangent();
            CheckVertexStreams_UV2();
            CheckTextureSheetAndEnableMeshGPUInstancing();

            for (var i = 0; i < selectedParticleSystemRenderers.Length; i++)
            {
                var vertexStreams = new List<ParticleSystemVertexStream>();
                SetVertexStreamData(ref vertexStreams, selectedParticleSystemRenderers[i]);
                selectedParticleSystemRenderers[i].SetActiveVertexStreams(vertexStreams);
                EditorUtility.SetDirty(selectedParticleSystemRenderers[i]); // Tell scene to mark those particles are dirty.
            }
        }
#if UNITY_2022_3_OR_NEWER
        private void RegisterTrailCustomVertexStreams()
        {
            // Make sure the material be control is not a shared material or register will be set wrong. 
            for (var i = 0; i < selectedParticleSystemRenderers.Length; i++)
            {
                if (selectedParticleSystemRenderers[i].trailMaterial != targetMat)
                    return;
            }

            Debug.Log("Register Trail Custom Vertex Streams!");
            registerVertexStreams = false;

            CheckVertexStreams_CustomData();
            CheckVertexStreams_Normal();
            CheckVertexStreams_Tangent();

            for (var i = 0; i < selectedParticleSystemRenderers.Length; i++)
            {
                var vertexStreams = new List<ParticleSystemVertexStream>();
                SetTrailVertexStreamData(ref vertexStreams, selectedParticleSystemRenderers[i]);
                selectedParticleSystemRenderers[i].SetActiveTrailVertexStreams(vertexStreams);
                EditorUtility.SetDirty(selectedParticleSystemRenderers[i]); // Tell scene to mark those particles are dirty.
            }
        }

        private void SetTrailVertexStreamData(ref List<ParticleSystemVertexStream> fixParticleVertexStreams, in ParticleSystemRenderer particleSystemRenderers)
        {
            fixParticleVertexStreams.Clear();

            // Add Base data
            fixParticleVertexStreams.Add(ParticleSystemVertexStream.Position);
            fixParticleVertexStreams.Add(ParticleSystemVertexStream.Color);
            fixParticleVertexStreams.Add(ParticleSystemVertexStream.UV);

            // Add Unused Data (Trail doesn't have UV2 can be used , but we also need to use custom data so keep the queue be neat.)
            if (customDataControllerFlags != CustomDataControllerFlags.None)
                fixParticleVertexStreams.Add(ParticleSystemVertexStream.SizeXY);

            // Add CustomData
            if ((customDataControllerFlags & CustomDataControllerFlags.CustomData2W) == CustomDataControllerFlags.CustomData2W)
            {
                fixParticleVertexStreams.Add(ParticleSystemVertexStream.Custom1XYZW);
                fixParticleVertexStreams.Add(ParticleSystemVertexStream.Custom2XYZW);
            }
            else if ((customDataControllerFlags & CustomDataControllerFlags.CustomData2Z) == CustomDataControllerFlags.CustomData2Z)
            {
                fixParticleVertexStreams.Add(ParticleSystemVertexStream.Custom1XYZW);
                fixParticleVertexStreams.Add(ParticleSystemVertexStream.Custom2XYZ);
            }
            else if ((customDataControllerFlags & CustomDataControllerFlags.CustomData2Y) == CustomDataControllerFlags.CustomData2Y)
            {
                fixParticleVertexStreams.Add(ParticleSystemVertexStream.Custom1XYZW);
                fixParticleVertexStreams.Add(ParticleSystemVertexStream.Custom2XY);
            }
            else if ((customDataControllerFlags & CustomDataControllerFlags.CustomData2X) == CustomDataControllerFlags.CustomData2X)
            {
                fixParticleVertexStreams.Add(ParticleSystemVertexStream.Custom1XYZW);
                fixParticleVertexStreams.Add(ParticleSystemVertexStream.Custom2X);
            }
            else if ((customDataControllerFlags & CustomDataControllerFlags.CustomData1W) == CustomDataControllerFlags.CustomData1W)
            {
                fixParticleVertexStreams.Add(ParticleSystemVertexStream.Custom1XYZW);
            }
            else if ((customDataControllerFlags & CustomDataControllerFlags.CustomData1Z) == CustomDataControllerFlags.CustomData1Z)
            {
                fixParticleVertexStreams.Add(ParticleSystemVertexStream.Custom1XYZ);
            }
            else if ((customDataControllerFlags & CustomDataControllerFlags.CustomData1Y) == CustomDataControllerFlags.CustomData1Y)
            {
                fixParticleVertexStreams.Add(ParticleSystemVertexStream.Custom1XY);
            }
            else if ((customDataControllerFlags & CustomDataControllerFlags.CustomData1X) == CustomDataControllerFlags.CustomData1X)
            {
                fixParticleVertexStreams.Add(ParticleSystemVertexStream.Custom1X);
            }


            // Add Normal
            if (vertexStreams_Normal)
                fixParticleVertexStreams.Add(ParticleSystemVertexStream.Normal);

            // Add Tangent
            if (vertexStreams_Tangent)
                fixParticleVertexStreams.Add(ParticleSystemVertexStream.Tangent);
        }
#endif

        private void SetVertexStreamData(ref List<ParticleSystemVertexStream> fixParticleVertexStreams, in ParticleSystemRenderer particleSystemRenderers)
        {
            fixParticleVertexStreams.Clear();

            // Add Base data
            fixParticleVertexStreams.Add(ParticleSystemVertexStream.Position);
            fixParticleVertexStreams.Add(ParticleSystemVertexStream.Color);
            fixParticleVertexStreams.Add(ParticleSystemVertexStream.UV);

            if (particleSystemRenderers.renderMode == ParticleSystemRenderMode.Mesh && particleSystemRenderers.enableGPUInstancing)
            {
                fixParticleVertexStreams.Add(ParticleSystemVertexStream.UV2);
                fixParticleVertexStreams.Add(ParticleSystemVertexStream.Custom1XYZW);
                fixParticleVertexStreams.Add(ParticleSystemVertexStream.Custom2XYZW);
                fixParticleVertexStreams.Add(ParticleSystemVertexStream.AnimFrame);
            }
            else if (particleSystemRenderers.renderMode != ParticleSystemRenderMode.Mesh || !particleSystemRenderers.enableGPUInstancing)
            {
                // Add UV2
                if (vertexStreams_UV2 || customDataControllerFlags != CustomDataControllerFlags.None)
                    fixParticleVertexStreams.Add(ParticleSystemVertexStream.UV2);

                // Add CustomData
                if ((customDataControllerFlags & CustomDataControllerFlags.CustomData2W) == CustomDataControllerFlags.CustomData2W)
                {
                    fixParticleVertexStreams.Add(ParticleSystemVertexStream.Custom1XYZW);
                    fixParticleVertexStreams.Add(ParticleSystemVertexStream.Custom2XYZW);
                }
                else if ((customDataControllerFlags & CustomDataControllerFlags.CustomData2Z) == CustomDataControllerFlags.CustomData2Z)
                {
                    fixParticleVertexStreams.Add(ParticleSystemVertexStream.Custom1XYZW);
                    fixParticleVertexStreams.Add(ParticleSystemVertexStream.Custom2XYZ);
                }
                else if ((customDataControllerFlags & CustomDataControllerFlags.CustomData2Y) == CustomDataControllerFlags.CustomData2Y)
                {
                    fixParticleVertexStreams.Add(ParticleSystemVertexStream.Custom1XYZW);
                    fixParticleVertexStreams.Add(ParticleSystemVertexStream.Custom2XY);
                }
                else if ((customDataControllerFlags & CustomDataControllerFlags.CustomData2X) == CustomDataControllerFlags.CustomData2X)
                {
                    fixParticleVertexStreams.Add(ParticleSystemVertexStream.Custom1XYZW);
                    fixParticleVertexStreams.Add(ParticleSystemVertexStream.Custom2X);
                }
                else if ((customDataControllerFlags & CustomDataControllerFlags.CustomData1W) == CustomDataControllerFlags.CustomData1W)
                {
                    fixParticleVertexStreams.Add(ParticleSystemVertexStream.Custom1XYZW);
                }
                else if ((customDataControllerFlags & CustomDataControllerFlags.CustomData1Z) == CustomDataControllerFlags.CustomData1Z)
                {
                    fixParticleVertexStreams.Add(ParticleSystemVertexStream.Custom1XYZ);
                }
                else if ((customDataControllerFlags & CustomDataControllerFlags.CustomData1Y) == CustomDataControllerFlags.CustomData1Y)
                {
                    fixParticleVertexStreams.Add(ParticleSystemVertexStream.Custom1XY);
                }
                else if ((customDataControllerFlags & CustomDataControllerFlags.CustomData1X) == CustomDataControllerFlags.CustomData1X)
                {
                    fixParticleVertexStreams.Add(ParticleSystemVertexStream.Custom1X);
                }
            }

            // Add Normal
            if (vertexStreams_Normal)
                fixParticleVertexStreams.Add(ParticleSystemVertexStream.Normal);

            // Add Tangent
            if (vertexStreams_Tangent)
                fixParticleVertexStreams.Add(ParticleSystemVertexStream.Tangent);
        }

        private void CheckVertexStreams_Normal()
        {
            vertexStreams_Normal = (ActiveMode)particleProperties.ActiveNormal.floatValue == ActiveMode.Enable ||
                                   (ActiveMode)particleProperties.ActiveFresnel.floatValue == ActiveMode.Enable ||
                                   (VertexOffsetMode)particleProperties.ActiveVertexOffset.floatValue == VertexOffsetMode.Enable ||
                                   (LightSystemMode)particleProperties.ActiveLightSystem.floatValue == LightSystemMode.VertexLight ||
                                   (LightSystemMode)particleProperties.ActiveLightSystem.floatValue == LightSystemMode.FragmentLight ||
                                   (ParallaxMode)particleProperties.ActiveParallax.floatValue == ParallaxMode.Enable ||
                                   (ParallaxMode)particleProperties.ActiveParallax.floatValue == ParallaxMode.Steep;
        }

        private void CheckVertexStreams_Tangent()
        {
            vertexStreams_Tangent = (ActiveMode)particleProperties.ActiveNormal.floatValue == ActiveMode.Enable ||
                                    (ParallaxMode)particleProperties.ActiveParallax.floatValue == ParallaxMode.Enable ||
                                    (ParallaxMode)particleProperties.ActiveParallax.floatValue == ParallaxMode.Steep ||
                                    (VertexOffsetMode)particleProperties.ActiveVertexOffset.floatValue ==
                                    VertexOffsetMode.Enable;
        }

        private void CheckVertexStreams_UV2()
        {
            vertexStreams_UV2 = (UVChannel)particleProperties.MainTexUVChannel.floatValue == UVChannel.UV2 ||
                                ((UVChannel)particleProperties.DistortTexUVChannel.floatValue == UVChannel.UV2 &&
                                 (ActiveMode)particleProperties.ActiveDistort.floatValue == ActiveMode.Enable) ||
                                ((UVChannel)particleProperties.DissolveTexUVChannel.floatValue == UVChannel.UV2 &&
                                 (ActiveMode)particleProperties.ActiveDissolve.floatValue == ActiveMode.Enable) ||
                                ((UVChannel)particleProperties.NormalTexUVChannel.floatValue == UVChannel.UV2 &&
                                 (ActiveMode)particleProperties.ActiveNormal.floatValue == ActiveMode.Enable) ||
                                ((UVChannel)particleProperties.ParallaxTexUVChannel.floatValue == UVChannel.UV2 &&
                                 (ActiveMode)particleProperties.ActiveParallax.floatValue == ActiveMode.Enable) ||
                                ((UVChannel)particleProperties.VertexOffsetTexUVChannel.floatValue == UVChannel.UV2 &&
                                 (ActiveMode)particleProperties.ActiveVertexOffset.floatValue == ActiveMode.Enable);
        }

        private void CheckTextureSheetAndEnableMeshGPUInstancing()
        {
            vertexStreams_AnimFrame = selectedParticleSystems.Any(x => x.textureSheetAnimation.enabled) &&
                                      selectedParticleSystemRenderers.Any(x => x.enableGPUInstancing);
        }

        private void CheckVertexStreams_CustomData()
        {
            var floatController = new List<float>();

            #region GrabPass

            if (currentParticleMode == ParticleMode.DistortionScreen)
            {
                var grabPassMode = (GrabPassTexMode)particleProperties.GrabPassTexMode.floatValue;
                switch (grabPassMode)
                {
                    case GrabPassTexMode.Normal:
                    case GrabPassTexMode.BumpTex:
                        floatController.Add(particleProperties.GrabPassIntensityController.floatValue);
                        break;
                }
            }

            #endregion

            #region MainTex

            var mainTexMode = (MainTexMode)particleProperties.MainTexMode.floatValue;

            switch (mainTexMode)
            {
                case MainTexMode.FlipBook:
                    floatController.Add(particleProperties.FlipBookSetting.vectorValue.w);
                    break;
                case MainTexMode.Normal:
                case MainTexMode.MultiChannel:
                case MainTexMode.PolarCoord:
                case MainTexMode.ScreenUV:
                    floatController.Add(particleProperties.MainTexUVTileController.vectorValue.x);
                    floatController.Add(particleProperties.MainTexUVTileController.vectorValue.y);
                    floatController.Add(particleProperties.MainTexUVTileController.vectorValue.z);
                    floatController.Add(particleProperties.MainTexUVTileController.vectorValue.w);
                    break;
            }

            if (mainTexMode == MainTexMode.MultiChannel)
            {
                floatController.Add(particleProperties.MainTexGIntensityController.floatValue);
                floatController.Add(particleProperties.MainTexBIntensityController.floatValue);
            }

            #endregion

            #region Distort

            var distortMode = (DistortMode)particleProperties.ActiveDistort.floatValue;
            switch (distortMode)
            {
                case DistortMode.Disable:
                    break;
                case DistortMode.Enable:
                case DistortMode.PolarCoord:
                case DistortMode.FlowMap:
                case DistortMode.ScreenUV:
                    floatController.Add(particleProperties.DistortTexUVTileController.vectorValue.x);
                    floatController.Add(particleProperties.DistortTexUVTileController.vectorValue.y);
                    floatController.Add(particleProperties.DistortTexUVTileController.vectorValue.z);
                    floatController.Add(particleProperties.DistortTexUVTileController.vectorValue.w);
                    floatController.Add(particleProperties.DistortIntensityController.floatValue);
                    break;
            }

            #endregion

            #region Dissolve

            var dissolveMode = (DissolveMode)particleProperties.ActiveDissolve.floatValue;
            switch (dissolveMode)
            {
                case DissolveMode.Disable:
                    break;
                case DissolveMode.Enable:
                case DissolveMode.PolarCoord:
                case DissolveMode.ScreenUV:
                    floatController.Add(particleProperties.DissolveTexUVTileController.vectorValue.x);
                    floatController.Add(particleProperties.DissolveTexUVTileController.vectorValue.y);
                    floatController.Add(particleProperties.DissolveTexUVTileController.vectorValue.z);
                    floatController.Add(particleProperties.DissolveTexUVTileController.vectorValue.w);
                    floatController.Add(particleProperties.DissolveIntensityController.floatValue);
                    break;
            }

            if ((ActiveMode)particleProperties.ActiveRimDissolve.floatValue == ActiveMode.Enable)
                floatController.Add(particleProperties.RimDissolveRangeController.floatValue);

            #endregion

            #region Emission

            var emissionMode = (EmissionMode)particleProperties.ActiveEmission.floatValue;
            switch (emissionMode)
            {
                case EmissionMode.Disable:
                    break;
                case EmissionMode.Enable:
                case EmissionMode.PolarCoord:
                case EmissionMode.ScreenUV:
                    floatController.Add(particleProperties.EmissionTexUVTileController.vectorValue.x);
                    floatController.Add(particleProperties.EmissionTexUVTileController.vectorValue.y);
                    floatController.Add(particleProperties.EmissionTexUVTileController.vectorValue.z);
                    floatController.Add(particleProperties.EmissionTexUVTileController.vectorValue.w);
                    floatController.Add(particleProperties.EmissionIntensityController.floatValue);
                    break;
            }

            #endregion

            #region NormalMap

            var normalMapMode = (ActiveMode)particleProperties.ActiveNormal.floatValue;
            switch (normalMapMode)
            {
                case ActiveMode.Disable:
                    break;
                case ActiveMode.Enable:
                    floatController.Add(particleProperties.NormalTexUVTileController.vectorValue.x);
                    floatController.Add(particleProperties.NormalTexUVTileController.vectorValue.y);
                    floatController.Add(particleProperties.NormalTexUVTileController.vectorValue.z);
                    floatController.Add(particleProperties.NormalTexUVTileController.vectorValue.w);
                    floatController.Add(particleProperties.NormalIntensityController.floatValue);
                    break;
            }

            #endregion

            #region Parallax

            var parallaxMode = (ParallaxMode)particleProperties.ActiveParallax.floatValue;
            switch (parallaxMode)
            {
                case ParallaxMode.Disable:
                    break;
                case ParallaxMode.Enable:
                case ParallaxMode.Steep:
                    floatController.Add(particleProperties.ParallaxTexUVTileController.vectorValue.x);
                    floatController.Add(particleProperties.ParallaxTexUVTileController.vectorValue.y);
                    floatController.Add(particleProperties.ParallaxTexUVTileController.vectorValue.z);
                    floatController.Add(particleProperties.ParallaxTexUVTileController.vectorValue.w);
                    floatController.Add(particleProperties.ParallaxIntensityController.floatValue);
                    break;
            }

            #endregion

            #region Fresnel

            var fresnelMode = (ActiveMode)particleProperties.ActiveFresnel.floatValue;
            switch (fresnelMode)
            {
                case ActiveMode.Disable:
                    break;
                case ActiveMode.Enable:
                    floatController.Add(particleProperties.FresnelRangeController.floatValue);
                    break;
            }

            #endregion

            #region Vertex-Offset

            var vertexOffsetMode = (VertexOffsetMode)particleProperties.ActiveVertexOffset.floatValue;
            switch (vertexOffsetMode)
            {
                case VertexOffsetMode.Enable:
                    floatController.Add(particleProperties.VertexOffsetTexUVTileController.vectorValue.x);
                    floatController.Add(particleProperties.VertexOffsetTexUVTileController.vectorValue.y);
                    floatController.Add(particleProperties.VertexOffsetTexUVTileController.vectorValue.z);
                    floatController.Add(particleProperties.VertexOffsetTexUVTileController.vectorValue.w);
                    floatController.Add(particleProperties.VertexOffsetIntensityController.floatValue);
                    floatController.Add(particleProperties.VertexOffsetMaskUVAutoOffset.floatValue);
                    floatController.Add(particleProperties.VertexOffsetMaskIntensityController.floatValue);
                    floatController.Add(particleProperties.VertexOffsetMaskUVTileController.vectorValue.x);
                    floatController.Add(particleProperties.VertexOffsetMaskUVTileController.vectorValue.y);
                    floatController.Add(particleProperties.VertexOffsetMaskUVTileController.vectorValue.z);
                    floatController.Add(particleProperties.VertexOffsetMaskUVTileController.vectorValue.w);
                    break;
                case VertexOffsetMode.AnimationTex:
                    floatController.Add(particleProperties.VertexOffsetTexUVTileController.vectorValue.x);
                    floatController.Add(particleProperties.VertexOffsetTexUVTileController.vectorValue.y);
                    floatController.Add(particleProperties.VertexOffsetTexUVTileController.vectorValue.z);
                    floatController.Add(particleProperties.VertexOffsetTexUVTileController.vectorValue.w);
                    floatController.Add(particleProperties.AnimationPlayableSliderController.floatValue);
                    break;
                case VertexOffsetMode.CameraZAxisOffset:
                    floatController.Add(particleProperties.VertexOffsetCameraDistanceController.floatValue);
                    break;
            }

            #endregion

            #region External Alpha Template

            var externalAlphaTemplateMode = (ExternalAlphaMode)particleProperties.ActiveExternalAlpha.floatValue;
            switch (externalAlphaTemplateMode)
            {
                case ExternalAlphaMode.Disable:
                    break;
                case ExternalAlphaMode.Line:
                case ExternalAlphaMode.Gradient:
                case ExternalAlphaMode.Circle:
                case ExternalAlphaMode.Round:
                    floatController.Add(particleProperties.ExternalAlphaRangeController.floatValue);
                    floatController.Add(particleProperties.ExternalAlphaPowerController.floatValue);
                    floatController.Add(particleProperties.ExternalAlphaIntensityController.floatValue);
                    break;
            }

            #endregion

            #region Alpha-Clip

            var alphaClipMode = (AlphaClipMode)particleProperties.ActiveAlphaClip.floatValue;
            switch (alphaClipMode)
            {
                case AlphaClipMode.Disable:
                    break;
                case AlphaClipMode.HardClip:
                case AlphaClipMode.SmoothClip:
                    floatController.Add(particleProperties.AlphaClipRangeController.floatValue);
                    break;
            }

            #endregion

            #region Color Adjustment

            var colorAdjustmentMode = (ActiveMode)particleProperties.ActiveColorAdjustment.floatValue;
            switch (colorAdjustmentMode)
            {
                case ActiveMode.Disable:
                    break;
                case ActiveMode.Enable:
                    floatController.Add(particleProperties.HUEShiftValController.floatValue);
                    floatController.Add(particleProperties.SaturationValController.floatValue);
                    floatController.Add(particleProperties.ContrastValController.floatValue);
                    floatController.Add(particleProperties.AntiShineGlowValController.floatValue);
                    break;
            }

            #endregion

            #region FinalColor & FinalAlpha Multiplier

            floatController.Add(particleProperties.FinalColorIntensityController.floatValue);
            floatController.Add(particleProperties.FinalAlphaIntensityController.floatValue);

            #endregion

            customDataControllerFlags = CustomDataControllerFlags.None;

            for (var i = 0; i < floatController.Count; i++)
            {
                customDataControllerFlags |= (CustomDataControllerFlags)(Convert.ToInt32(CheckCustomDataInFloat(floatController[i], 1)) << 0);
                customDataControllerFlags |= (CustomDataControllerFlags)(Convert.ToInt32(CheckCustomDataInFloat(floatController[i], 2)) << 1);
                customDataControllerFlags |= (CustomDataControllerFlags)(Convert.ToInt32(CheckCustomDataInFloat(floatController[i], 3)) << 2);
                customDataControllerFlags |= (CustomDataControllerFlags)(Convert.ToInt32(CheckCustomDataInFloat(floatController[i], 4)) << 3);
                customDataControllerFlags |= (CustomDataControllerFlags)(Convert.ToInt32(CheckCustomDataInFloat(floatController[i], 5)) << 4);
                customDataControllerFlags |= (CustomDataControllerFlags)(Convert.ToInt32(CheckCustomDataInFloat(floatController[i], 6)) << 5);
                customDataControllerFlags |= (CustomDataControllerFlags)(Convert.ToInt32(CheckCustomDataInFloat(floatController[i], 7)) << 6);
                customDataControllerFlags |= (CustomDataControllerFlags)(Convert.ToInt32(CheckCustomDataInFloat(floatController[i], 8)) << 7);
            }
        }

        private bool CheckCustomDataInFloat(float src, int customCoordInt)
        {
            return src == customCoordInt;
        }

        #endregion
    }
}