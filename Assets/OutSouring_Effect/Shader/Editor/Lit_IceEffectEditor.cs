using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace OutSourcing.Editor
{
    /// <summary>
    /// Custom ShaderGUI for OutSouring/Lit_IceEffect shader.
    /// Organized to match the Unity URP Lit Shader inspector style.
    /// </summary>
    public class LitIceEffectShaderGUI : ShaderGUI
    {
        // =====================================================================
        // Foldout State (persisted via SessionState)
        // =====================================================================
        private static class Foldouts
        {
            private const string Prefix = "LitIceEffect_";

            public static bool SurfaceOptions
            {
                get => SessionState.GetBool(Prefix + "SurfaceOptions", true);
                set => SessionState.SetBool(Prefix + "SurfaceOptions", value);
            }

            public static bool SurfaceInputs
            {
                get => SessionState.GetBool(Prefix + "SurfaceInputs", true);
                set => SessionState.SetBool(Prefix + "SurfaceInputs", value);
            }

            public static bool Emission
            {
                get => SessionState.GetBool(Prefix + "Emission", false);
                set => SessionState.SetBool(Prefix + "Emission", value);
            }

            public static bool IceEffect
            {
                get => SessionState.GetBool(Prefix + "IceEffect", false);
                set => SessionState.SetBool(Prefix + "IceEffect", value);
            }

            public static bool AdvancedOptions
            {
                get => SessionState.GetBool(Prefix + "AdvancedOptions", false);
                set => SessionState.SetBool(Prefix + "AdvancedOptions", value);
            }
        }

        // =====================================================================
        // Styles
        // =====================================================================
        private static class Styles
        {
            public static readonly GUIContent SurfaceOptions     = new GUIContent("Surface Options");
            public static readonly GUIContent SurfaceInputs      = new GUIContent("Surface Inputs");
            public static readonly GUIContent Emission            = new GUIContent("Emission");
            public static readonly GUIContent IceEffect           = new GUIContent("Ice Effect");
            public static readonly GUIContent AdvancedOptions     = new GUIContent("Advanced Options");

            // Surface Options
            public static readonly GUIContent RenderFace          = new GUIContent("Render Face");
            public static readonly GUIContent AlphaClipping       = new GUIContent("Alpha Clipping");
            public static readonly GUIContent Threshold           = new GUIContent("Threshold", "Alpha clip threshold value.");
            public static readonly GUIContent UseShadowThreshold  = new GUIContent("Use Shadow Threshold");
            public static readonly GUIContent ShadowThreshold     = new GUIContent("Shadow Threshold");

            // Surface Inputs
            public static readonly GUIContent BaseColor           = new GUIContent("Base Color");
            public static readonly GUIContent Brightness          = new GUIContent("Brightness");
            public static readonly GUIContent BaseMap             = new GUIContent("Base Map");
            public static readonly GUIContent UVSet               = new GUIContent("UV Set");
            public static readonly GUIContent MainUVs             = new GUIContent("Main UVs (Tiling / Offset)");
            public static readonly GUIContent MetallicMap         = new GUIContent("Metallic Map");
            public static readonly GUIContent Metallic            = new GUIContent("Metallic");
            public static readonly GUIContent SmoothnessChannel   = new GUIContent("Smoothness Source", "MetallicAlpha or AlbedoAlpha");
            public static readonly GUIContent Smoothness          = new GUIContent("Smoothness");
            public static readonly GUIContent OcclusionMap        = new GUIContent("Occlusion Map");
            public static readonly GUIContent OcclusionStrength   = new GUIContent("Occlusion Strength");
            public static readonly GUIContent NormalMode          = new GUIContent("Double-Sided Normal Mode");
            public static readonly GUIContent NormalMap           = new GUIContent("Normal Map");
            public static readonly GUIContent NormalScale         = new GUIContent("Normal Scale");

            // Emission
            public static readonly GUIContent EnableEmission      = new GUIContent("Enable Emission");
            public static readonly GUIContent EmissiveColor       = new GUIContent("Emissive Color");
            public static readonly GUIContent EmissiveMap         = new GUIContent("Emissive Map");
            public static readonly GUIContent EmissiveUVSet       = new GUIContent("UV Set");
            public static readonly GUIContent EmissiveUVs         = new GUIContent("Emissive UVs (Tiling / Offset)");
            public static readonly GUIContent EmissiveIntensity   = new GUIContent("Emissive Intensity");

            // Ice Effect
            public static readonly GUIContent Freeze              = new GUIContent("Freeze");
            public static readonly GUIContent FreezeMap           = new GUIContent("Freeze Map");
            public static readonly GUIContent FreezeMapColor      = new GUIContent("Freeze Map Color");
            public static readonly GUIContent FreezeMapRange      = new GUIContent("Freeze Map Range");
            public static readonly GUIContent FreezeNormalStr     = new GUIContent("Freeze Normal Strength");
            public static readonly GUIContent FreezeSmoothness    = new GUIContent("Freeze Smoothness");
            public static readonly GUIContent FreezeEmissionStr   = new GUIContent("Freeze Emission Strength");
            public static readonly GUIContent FreezeEmissionRange = new GUIContent("Freeze Emission Range");
            public static readonly GUIContent FreezeEmissionColor = new GUIContent("Freeze Emission Color");
            public static readonly GUIContent VertexOffsetNoise   = new GUIContent("Vertex Offset Noise Scale");

            // Advanced
            public static readonly GUIContent SpecularHighlights  = new GUIContent("Specular Highlights");
            public static readonly GUIContent EnvReflections      = new GUIContent("Environment Reflections");
            public static readonly GUIContent ReceiveShadows      = new GUIContent("Receive Shadows");
            public static readonly GUIContent QueueOffset         = new GUIContent("Sorting Priority", "Determines render queue offset.");
        }

        // =====================================================================
        // Material Property Cache
        // =====================================================================
        // Surface Options
        private MaterialProperty _cull;
        private MaterialProperty _alphaClip;
        private MaterialProperty _cutoff;
        private MaterialProperty _useShadowThreshold;
        private MaterialProperty _alphaCutoffShadow;

        // Surface Inputs
        private MaterialProperty _baseColor;
        private MaterialProperty _brightness;
        private MaterialProperty _baseMap;
        private MaterialProperty _uvBase;
        private MaterialProperty _baseMapST;
        private MaterialProperty _metallicGlossMap;
        private MaterialProperty _metallic;
        private MaterialProperty _smoothnessChannel;
        private MaterialProperty _glossMapScale;
        private MaterialProperty _occlusionMap;
        private MaterialProperty _occlusionStrength;
        private MaterialProperty _doubleSidedNormalMode;
        private MaterialProperty _bumpMap;
        private MaterialProperty _bumpScale;

        // Emission
        private MaterialProperty _enableEmission;
        private MaterialProperty _emissionColor;
        private MaterialProperty _emissionMap;
        private MaterialProperty _uvEmissive;
        private MaterialProperty _emissionMapST;
        private MaterialProperty _emissiveIntensity;

        // Ice Effect
        private MaterialProperty _freeze;
        private MaterialProperty _freezeMap;
        private MaterialProperty _freezeMapColor;
        private MaterialProperty _freezeMapRange;
        private MaterialProperty _freezeNormalStrength;
        private MaterialProperty _freezeSmoothness;
        private MaterialProperty _freezeEmissionStrength;
        private MaterialProperty _freezeEmissionRange;
        private MaterialProperty _freezeEmissionColor;
        private MaterialProperty _vertexOffsetNoiseScale;

        // Advanced
        private MaterialProperty _specularHighlights;
        private MaterialProperty _environmentReflections;
        private MaterialProperty _receiveShadows;
        private MaterialProperty _queueOffset;

        // =====================================================================
        // Find Properties
        // =====================================================================
        private void FindProperties(MaterialProperty[] properties)
        {
            // Surface Options
            _cull                   = FindProperty("_Cull", properties, false);
            _alphaClip              = FindProperty("_AlphaClip", properties, false);
            _cutoff                 = FindProperty("_Cutoff", properties, false);
            _useShadowThreshold     = FindProperty("_UseShadowThreshold", properties, false);
            _alphaCutoffShadow      = FindProperty("_AlphaCutoffShadow", properties, false);

            // Surface Inputs
            _baseColor              = FindProperty("_BaseColor", properties, false);
            _brightness             = FindProperty("_Brightness", properties, false);
            _baseMap                = FindProperty("_BaseMap", properties, false);
            _uvBase                 = FindProperty("_UVBase", properties, false);
            _baseMapST              = FindProperty("_BaseMap_ST", properties, false);
            _metallicGlossMap       = FindProperty("_MetallicGlossMap", properties, false);
            _metallic               = FindProperty("_Metallic", properties, false);
            _smoothnessChannel      = FindProperty("_SmoothnesstexturechannelM", properties, false);
            _glossMapScale          = FindProperty("_GlossMapScale", properties, false);
            _occlusionMap           = FindProperty("_OcclusionMap", properties, false);
            _occlusionStrength      = FindProperty("_OcclusionStrength", properties, false);
            _doubleSidedNormalMode  = FindProperty("_DoubleSidedNormalMode", properties, false);
            _bumpMap                = FindProperty("_BumpMap", properties, false);
            _bumpScale              = FindProperty("_BumpScale", properties, false);

            // Emission
            _enableEmission         = FindProperty("_EnableEmission", properties, false);
            _emissionColor          = FindProperty("_EmissionColor", properties, false);
            _emissionMap            = FindProperty("_EmissionMap", properties, false);
            _uvEmissive             = FindProperty("_UVEmissive", properties, false);
            _emissionMapST          = FindProperty("_EmissionMap_ST", properties, false);
            _emissiveIntensity      = FindProperty("_EmissiveIntensity", properties, false);

            // Ice Effect
            _freeze                 = FindProperty("_Freeze", properties, false);
            _freezeMap              = FindProperty("_FreezeMap", properties, false);
            _freezeMapColor         = FindProperty("_FreezeMapColor", properties, false);
            _freezeMapRange         = FindProperty("_FreezeMapRange", properties, false);
            _freezeNormalStrength   = FindProperty("_FreezeNormalStrength", properties, false);
            _freezeSmoothness       = FindProperty("_FreezeSmoothness", properties, false);
            _freezeEmissionStrength = FindProperty("_FreezeEmissionStrength", properties, false);
            _freezeEmissionRange    = FindProperty("_FreezeEmissionRange", properties, false);
            _freezeEmissionColor    = FindProperty("_FreezeEmiisionColor", properties, false);
            _vertexOffsetNoiseScale = FindProperty("_VertexOffsetNoiseScale", properties, false);

            // Advanced
            _specularHighlights     = FindProperty("_SpecularHighlights", properties, false);
            _environmentReflections = FindProperty("_EnvironmentReflections", properties, false);
            _receiveShadows         = FindProperty("_ReceiveShadows", properties, false);
            _queueOffset            = FindProperty("_QueueOffset", properties, false);
        }

        // =====================================================================
        // Main GUI
        // =====================================================================
        public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
        {
            FindProperties(properties);

            EditorGUIUtility.labelWidth = 0f;

            DrawSurfaceOptions(materialEditor);
            DrawSurfaceInputs(materialEditor);
            DrawEmission(materialEditor);
            DrawIceEffect(materialEditor);
            DrawAdvancedOptions(materialEditor);

            // Apply keyword changes
            foreach (var obj in materialEditor.targets)
            {
                if (obj is Material mat)
                    SetupMaterialKeywords(mat);
            }
        }

        // =====================================================================
        // Section Drawing
        // =====================================================================

        private void DrawSurfaceOptions(MaterialEditor materialEditor)
        {
            Foldouts.SurfaceOptions = DrawFoldoutHeader(Styles.SurfaceOptions, Foldouts.SurfaceOptions);
            if (!Foldouts.SurfaceOptions) return;

            EditorGUI.indentLevel++;

            DrawProperty(materialEditor, _cull, Styles.RenderFace);
            EditorGUILayout.Space(2);

            DrawToggleProperty(materialEditor, _alphaClip, Styles.AlphaClipping);
            if (_alphaClip != null && _alphaClip.floatValue >= 0.5f)
            {
                EditorGUI.indentLevel++;
                DrawProperty(materialEditor, _cutoff, Styles.Threshold);
                DrawToggleProperty(materialEditor, _useShadowThreshold, Styles.UseShadowThreshold);
                if (_useShadowThreshold != null && _useShadowThreshold.floatValue >= 0.5f)
                {
                    EditorGUI.indentLevel++;
                    DrawProperty(materialEditor, _alphaCutoffShadow, Styles.ShadowThreshold);
                    EditorGUI.indentLevel--;
                }
                EditorGUI.indentLevel--;
            }

            EditorGUI.indentLevel--;
            EditorGUILayout.Space(4);
        }

        private void DrawSurfaceInputs(MaterialEditor materialEditor)
        {
            Foldouts.SurfaceInputs = DrawFoldoutHeader(Styles.SurfaceInputs, Foldouts.SurfaceInputs);
            if (!Foldouts.SurfaceInputs) return;

            EditorGUI.indentLevel++;

            // Base Map & Color
            DrawTexturePropertyLine(materialEditor, _baseMap, null, Styles.BaseMap);
            DrawProperty(materialEditor, _baseColor, Styles.BaseColor);
            DrawProperty(materialEditor, _brightness, Styles.Brightness);
            DrawEnumProperty(materialEditor, _uvBase, Styles.UVSet, typeof(UVSet));
            DrawTilingOffset(materialEditor, _baseMapST, Styles.MainUVs);

            DrawSeparatorLine();

            // Metallic & Smoothness
            DrawTexturePropertyLine(materialEditor, _metallicGlossMap, _metallic, Styles.MetallicMap);
            DrawEnumProperty(materialEditor, _smoothnessChannel, Styles.SmoothnessChannel, typeof(SmoothnessSource));
            DrawProperty(materialEditor, _glossMapScale, Styles.Smoothness);

            DrawSeparatorLine();

            // Normal Map
            DrawEnumProperty(materialEditor, _doubleSidedNormalMode, Styles.NormalMode, typeof(DoubleSidedNormalMode));
            DrawTexturePropertyLine(materialEditor, _bumpMap, _bumpScale, Styles.NormalMap);

            DrawSeparatorLine();

            // Occlusion
            DrawTexturePropertyLine(materialEditor, _occlusionMap, _occlusionStrength, Styles.OcclusionMap);

            EditorGUI.indentLevel--;
            EditorGUILayout.Space(4);
        }

        private void DrawEmission(MaterialEditor materialEditor)
        {
            Foldouts.Emission = DrawFoldoutHeader(Styles.Emission, Foldouts.Emission);
            if (!Foldouts.Emission) return;

            EditorGUI.indentLevel++;

            DrawToggleProperty(materialEditor, _enableEmission, Styles.EnableEmission);

            if (_enableEmission != null && _enableEmission.floatValue >= 0.5f)
            {
                DrawTexturePropertyLine(materialEditor, _emissionMap, _emissionColor, Styles.EmissiveMap);
                DrawProperty(materialEditor, _emissiveIntensity, Styles.EmissiveIntensity);
                DrawEnumProperty(materialEditor, _uvEmissive, Styles.EmissiveUVSet, typeof(UVSet));
                DrawTilingOffset(materialEditor, _emissionMapST, Styles.EmissiveUVs);
            }

            EditorGUI.indentLevel--;
            EditorGUILayout.Space(4);
        }

        private void DrawIceEffect(MaterialEditor materialEditor)
        {
            Foldouts.IceEffect = DrawFoldoutHeader(Styles.IceEffect, Foldouts.IceEffect);
            if (!Foldouts.IceEffect) return;

            EditorGUI.indentLevel++;

            DrawProperty(materialEditor, _freeze, Styles.Freeze);

            if (_freeze != null && _freeze.floatValue > 0f)
            {
                EditorGUILayout.Space(4);

                // Freeze Appearance
                EditorGUILayout.LabelField("Appearance", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;
                DrawTexturePropertyLine(materialEditor, _freezeMap, null, Styles.FreezeMap);
                DrawProperty(materialEditor, _freezeMapColor, Styles.FreezeMapColor);
                DrawProperty(materialEditor, _freezeMapRange, Styles.FreezeMapRange);
                DrawProperty(materialEditor, _freezeNormalStrength, Styles.FreezeNormalStr);
                DrawProperty(materialEditor, _freezeSmoothness, Styles.FreezeSmoothness);
                EditorGUI.indentLevel--;

                EditorGUILayout.Space(4);

                // Freeze Emission
                EditorGUILayout.LabelField("Freeze Emission", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;
                DrawProperty(materialEditor, _freezeEmissionStrength, Styles.FreezeEmissionStr);
                DrawProperty(materialEditor, _freezeEmissionRange, Styles.FreezeEmissionRange);
                DrawProperty(materialEditor, _freezeEmissionColor, Styles.FreezeEmissionColor);
                EditorGUI.indentLevel--;

                EditorGUILayout.Space(4);

                // Vertex Offset
                EditorGUILayout.LabelField("Vertex Displacement", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;
                DrawProperty(materialEditor, _vertexOffsetNoiseScale, Styles.VertexOffsetNoise);
                EditorGUI.indentLevel--;
            }

            EditorGUI.indentLevel--;
            EditorGUILayout.Space(4);
        }

        private void DrawAdvancedOptions(MaterialEditor materialEditor)
        {
            Foldouts.AdvancedOptions = DrawFoldoutHeader(Styles.AdvancedOptions, Foldouts.AdvancedOptions);
            if (!Foldouts.AdvancedOptions) return;

            EditorGUI.indentLevel++;

            DrawToggleProperty(materialEditor, _specularHighlights, Styles.SpecularHighlights);
            DrawToggleProperty(materialEditor, _environmentReflections, Styles.EnvReflections);
            DrawToggleProperty(materialEditor, _receiveShadows, Styles.ReceiveShadows);

            EditorGUILayout.Space(4);

            if (_queueOffset != null)
            {
                materialEditor.ShaderProperty(_queueOffset, Styles.QueueOffset);
            }

            EditorGUILayout.Space(4);
            materialEditor.EnableInstancingField();
            materialEditor.RenderQueueField();

            EditorGUI.indentLevel--;
            EditorGUILayout.Space(4);
        }

        // =====================================================================
        // UI Helpers
        // =====================================================================

        /// <summary>
        /// Draws a URP-style foldout header with a background box.
        /// </summary>
        private bool DrawFoldoutHeader(GUIContent title, bool expanded)
        {
            var backgroundRect = GUILayoutUtility.GetRect(1f, 22f, GUILayout.ExpandWidth(true));
            backgroundRect.xMin -= 3f;
            backgroundRect.xMax += 3f;

            var headerColor = EditorGUIUtility.isProSkin
                ? new Color(0.18f, 0.18f, 0.18f, 1f)
                : new Color(0.75f, 0.75f, 0.75f, 1f);

            EditorGUI.DrawRect(backgroundRect, headerColor);

            // Top / Bottom border lines
            var borderColor = EditorGUIUtility.isProSkin
                ? new Color(0.12f, 0.12f, 0.12f, 1f)
                : new Color(0.6f, 0.6f, 0.6f, 1f);

            EditorGUI.DrawRect(new Rect(backgroundRect.x, backgroundRect.y, backgroundRect.width, 1f), borderColor);
            EditorGUI.DrawRect(new Rect(backgroundRect.x, backgroundRect.yMax - 1f, backgroundRect.width, 1f), borderColor);

            var foldoutRect = new Rect(backgroundRect.x + 4f, backgroundRect.y + 2f, backgroundRect.width - 4f, 18f);

            var e = Event.current;
            if (e.type == EventType.MouseDown && backgroundRect.Contains(e.mousePosition))
            {
                expanded = !expanded;
                e.Use();
            }

            EditorGUI.Foldout(foldoutRect, expanded, title, true, EditorStyles.boldLabel);

            EditorGUILayout.Space(2);

            return expanded;
        }

        private static void DrawProperty(MaterialEditor editor, MaterialProperty prop, GUIContent label)
        {
            if (prop == null) return;
            editor.ShaderProperty(prop, label);
        }

        private static void DrawToggleProperty(MaterialEditor editor, MaterialProperty prop, GUIContent label)
        {
            if (prop == null) return;
            editor.ShaderProperty(prop, label);
        }

        private static void DrawEnumProperty(MaterialEditor editor, MaterialProperty prop, GUIContent label, Type enumType)
        {
            if (prop == null) return;

            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = prop.hasMixedValue;

            var value = (int)prop.floatValue;
            value = EditorGUILayout.Popup(label, value, Enum.GetNames(enumType));

            EditorGUI.showMixedValue = false;
            if (EditorGUI.EndChangeCheck())
                prop.floatValue = value;
        }

        /// <summary>
        /// Unified texture property line drawer.
        /// Temporarily resets EditorGUI.indentLevel to 0 and uses manual GUILayout.Space
        /// to preserve visual indentation. This prevents TexturePropertySingleLine from
        /// miscalculating the available width for inline controls (color picker, slider, etc.).
        /// </summary>
        private static void DrawTexturePropertyLine(MaterialEditor editor, MaterialProperty tex, MaterialProperty extra, GUIContent label)
        {
            if (tex == null) return;

            // Save and reset indent to prevent TexturePropertySingleLine layout issues
            int savedIndent = EditorGUI.indentLevel;
            float indentPixels = savedIndent * 15f;
            EditorGUI.indentLevel = 0;

            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(indentPixels);
            EditorGUILayout.BeginVertical();

            if (extra != null)
                editor.TexturePropertySingleLine(label, tex, extra);
            else
                editor.TexturePropertySingleLine(label, tex);

            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();

            EditorGUI.indentLevel = savedIndent;
        }

        /// <summary>
        /// Draw Tiling/Offset from a Vector4 property (xy = tiling, zw = offset).
        /// </summary>
        private static void DrawTilingOffset(MaterialEditor editor, MaterialProperty stProp, GUIContent label)
        {
            if (stProp == null) return;

            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = stProp.hasMixedValue;

            var st = stProp.vectorValue;
            var tiling = new Vector2(st.x, st.y);
            var offset = new Vector2(st.z, st.w);

            EditorGUI.indentLevel++;
            tiling = EditorGUILayout.Vector2Field("Tiling", tiling);
            offset = EditorGUILayout.Vector2Field("Offset", offset);
            EditorGUI.indentLevel--;

            EditorGUI.showMixedValue = false;
            if (EditorGUI.EndChangeCheck())
                stProp.vectorValue = new Vector4(tiling.x, tiling.y, offset.x, offset.y);
        }

        private static void DrawSeparatorLine()
        {
            EditorGUILayout.Space(4);
            var rect = EditorGUILayout.GetControlRect(false, 1f);
            rect.xMin += EditorGUI.indentLevel * 15f;
            var lineColor = EditorGUIUtility.isProSkin
                ? new Color(0.3f, 0.3f, 0.3f, 1f)
                : new Color(0.7f, 0.7f, 0.7f, 1f);
            EditorGUI.DrawRect(rect, lineColor);
            EditorGUILayout.Space(4);
        }

        // =====================================================================
        // Keyword Setup
        // =====================================================================
        private static void SetupMaterialKeywords(Material material)
        {
            // Alpha Clipping
            if (material.HasProperty("_AlphaClip"))
            {
                bool alphaClip = material.GetFloat("_AlphaClip") >= 0.5f;
                CoreUtils.SetKeyword(material, "_ALPHATEST_ON", alphaClip);
            }

            // Normal Map
            if (material.HasProperty("_BumpMap"))
            {
                bool hasNormal = material.GetTexture("_BumpMap") != null;
                CoreUtils.SetKeyword(material, "_NORMALMAP", hasNormal);
            }

            // Metallic Map
            if (material.HasProperty("_MetallicGlossMap"))
            {
                bool hasMetallic = material.GetTexture("_MetallicGlossMap") != null;
                CoreUtils.SetKeyword(material, "_METALLICSPECGLOSSMAP", hasMetallic);
            }

            // Occlusion Map
            if (material.HasProperty("_OcclusionMap"))
            {
                bool hasOcclusion = material.GetTexture("_OcclusionMap") != null;
                CoreUtils.SetKeyword(material, "_OCCLUSIONMAP", hasOcclusion);
            }

            // Emission
            if (material.HasProperty("_EnableEmission"))
            {
                bool emission = material.GetFloat("_EnableEmission") >= 0.5f;
                CoreUtils.SetKeyword(material, "_EMISSION", emission);
                material.globalIlluminationFlags = emission
                    ? MaterialGlobalIlluminationFlags.BakedEmissive
                    : MaterialGlobalIlluminationFlags.EmissiveIsBlack;
            }

            // Specular Highlights
            if (material.HasProperty("_SpecularHighlights"))
            {
                bool specOff = material.GetFloat("_SpecularHighlights") < 0.5f;
                CoreUtils.SetKeyword(material, "_SPECULARHIGHLIGHTS_OFF", specOff);
            }

            // Environment Reflections
            if (material.HasProperty("_EnvironmentReflections"))
            {
                bool envOff = material.GetFloat("_EnvironmentReflections") < 0.5f;
                CoreUtils.SetKeyword(material, "_ENVIRONMENTREFLECTIONS_OFF", envOff);
            }

            // Receive Shadows
            if (material.HasProperty("_ReceiveShadows"))
            {
                bool rcvOff = material.GetFloat("_ReceiveShadows") < 0.5f;
                CoreUtils.SetKeyword(material, "_RECEIVE_SHADOWS_OFF", rcvOff);
            }
        }

        // =====================================================================
        // Enums for popup display
        // =====================================================================
        private enum UVSet { UV0, UV1, UV2, UV3 }
        private enum SmoothnessSource { MetallicAlpha, AlbedoAlpha }
        private enum DoubleSidedNormalMode { Flip, Mirror, None }
    }
}