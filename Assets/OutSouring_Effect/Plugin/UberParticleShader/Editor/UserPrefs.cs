using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UberParticleShader.Editor
{
    public class UserPrefs : EditorWindow
    {
        private bool cache_AutoFixCustomVertexStreams;
        private bool cache_CollapseFeatures;
        private bool cache_FinalOutput;
        private bool cache_BlendSetting;
        private bool cache_StencilSetting;
        private bool cache_RenderQueueSetting;
        private bool cache_GPUInstancingSetting;
        private bool folder_ConstantProperties = true;
        private readonly int indentLevel = 10;

        private class ShaderIDs
        {
            internal static readonly int ActiveNormal = Shader.PropertyToID("_ActiveNormal");
            internal static readonly int ActiveFresnel = Shader.PropertyToID("_ActiveFresnel");
            internal static readonly int ActiveVertexOffset = Shader.PropertyToID("_ActiveVertexOffset");
            internal static readonly int ActiveLightSystem = Shader.PropertyToID("_ActiveLightSystem");
            internal static readonly int ActiveParallax = Shader.PropertyToID("_ActiveParallax");
        }


        [MenuItem("Tools/UberParticle Editor/UserPrefs")]
        public static void ShowWindow()
        {
            GetWindow(typeof(UserPrefs));
        }

        private void OnEnable()
        {
            //CheckUserPrefs();
            cache_AutoFixCustomVertexStreams = UberParticleEditorSetting.Instance.AutoFixVertexStreams;
            cache_CollapseFeatures = UberParticleEditorSetting.Instance.CollapseFeatures;
            cache_FinalOutput = UberParticleEditorSetting.Instance.FinalOutput;
            cache_BlendSetting = UberParticleEditorSetting.Instance.BlendSetting;
            cache_StencilSetting = UberParticleEditorSetting.Instance.StencilSetting;
            cache_RenderQueueSetting = UberParticleEditorSetting.Instance.RenderQueueSetting;
            cache_GPUInstancingSetting = UberParticleEditorSetting.Instance.GPUInstancingSetting;
        }

        private void OnGUI()
        {
            RegisterCustomVertexStreamPropertyGUI("Register Custom Vertex Streams Mode");
            CollapseFeatures("Features Display");
            ShowHideConstantProperty("Constant Property Display");
            //FixAllParticleSystemVertexStreams();
        }
        

        private void CollapseFeatures(in string title)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(title);

            var collapseType = cache_CollapseFeatures ? 1 : 0;
            using (var changeCheckScope = new EditorGUI.ChangeCheckScope())
            {
                collapseType = GUILayout.Toolbar(collapseType, new[] { "Extend", "Collapse" }, GUILayout.Width(200));
                if (changeCheckScope.changed)
                {
                    switch (collapseType)
                    {
                        // Extend 
                        case 0:
                            cache_CollapseFeatures = false;
                            break;

                        // Collapse
                        case 1:
                            cache_CollapseFeatures = true;
                            break;
                    }

                    Undo.RecordObject(UberParticleEditorSetting.Instance, "Undo Register CollapseFeatures");
                    UberParticleEditorSetting.Instance.CollapseFeatures = cache_CollapseFeatures;
                    EditorUtility.SetDirty(UberParticleEditorSetting.Instance);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                    UberParticleEditor.refreshSetting = true;
                }
            }

            GUILayout.EndHorizontal();
        }

        private void RegisterCustomVertexStreamPropertyGUI(in string title)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(title);

            var chooseType = cache_AutoFixCustomVertexStreams ? 1 : 0;
            using (var changeCheckScope = new EditorGUI.ChangeCheckScope())
            {
                chooseType = GUILayout.Toolbar(chooseType, new[] { "Manually", "Auto" }, GUILayout.Width(200));
                if (changeCheckScope.changed)
                {
                    switch (chooseType)
                    {
                        // Manually
                        case 0:
                            cache_AutoFixCustomVertexStreams = false;
                            break;

                        // Auto
                        case 1:
                            cache_AutoFixCustomVertexStreams = true;
                            break;
                    }

                    Undo.RecordObject(UberParticleEditorSetting.Instance, "Undo Register AutoFixVertexStreams");
                    UberParticleEditorSetting.Instance.AutoFixVertexStreams = cache_AutoFixCustomVertexStreams;
                    EditorUtility.SetDirty(UberParticleEditorSetting.Instance);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                    UberParticleEditor.refreshSetting = true;
                }
            }

            GUILayout.EndHorizontal();
        }

        private void ShowHideConstantProperty(in string title)
        {
            folder_ConstantProperties = EditorGUILayout.Foldout(folder_ConstantProperties, title, true);

            if (folder_ConstantProperties)
            {
                GUILayout.BeginVertical("helpBox");

                RegisterFinalOutputPropertyGUI();
                RegisterBlendSettingPropertyGUI();
                RegisterStencilSettingPropertyGUI();
                RegisterRenderQueuePropertyGUI();
                RegisterGPUInstancingPropertyGUI();

                GUILayout.EndVertical();
            }
        }

        private void RegisterFinalOutputPropertyGUI()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(indentLevel);
            GUILayout.Label("Final Output");

            var chooseType = cache_FinalOutput ? 1 : 0;
            using (var changeCheckScope = new EditorGUI.ChangeCheckScope())
            {
                chooseType = GUILayout.Toolbar(chooseType, new[] { "Hide", "Show" }, GUILayout.Width(200));
                if (changeCheckScope.changed)
                {
                    switch (chooseType)
                    {
                        // Hide
                        case 0:
                            cache_FinalOutput = false;
                            break;

                        // Show
                        case 1:
                            cache_FinalOutput = true;
                            break;
                    }

                    Undo.RecordObject(UberParticleEditorSetting.Instance, "Undo Register Final Output");
                    UberParticleEditorSetting.Instance.FinalOutput = cache_FinalOutput;
                    EditorUtility.SetDirty(UberParticleEditorSetting.Instance);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                    UberParticleEditor.refreshSetting = true;
                }
            }

            GUILayout.EndHorizontal();
        }

        private void RegisterBlendSettingPropertyGUI()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(indentLevel);
            GUILayout.Label("Blend Setting");

            var chooseType = cache_BlendSetting ? 1 : 0;
            using (var changeCheckScope = new EditorGUI.ChangeCheckScope())
            {
                chooseType = GUILayout.Toolbar(chooseType, new[] { "Hide", "Show" }, GUILayout.Width(200));
                if (changeCheckScope.changed)
                {
                    switch (chooseType)
                    {
                        // Hide
                        case 0:
                            cache_BlendSetting = false;
                            break;

                        // Show
                        case 1:
                            cache_BlendSetting = true;
                            break;
                    }

                    Undo.RecordObject(UberParticleEditorSetting.Instance, "Undo Register Blend Setting");
                    UberParticleEditorSetting.Instance.BlendSetting = cache_BlendSetting;
                    EditorUtility.SetDirty(UberParticleEditorSetting.Instance);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                    UberParticleEditor.refreshSetting = true;
                }
            }

            GUILayout.EndHorizontal();
        }

        private void RegisterStencilSettingPropertyGUI()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(indentLevel);
            GUILayout.Label("Stencil Setting");

            var chooseType = cache_StencilSetting ? 1 : 0;
            using (var changeCheckScope = new EditorGUI.ChangeCheckScope())
            {
                chooseType = GUILayout.Toolbar(chooseType, new[] { "Hide", "Show" }, GUILayout.Width(200));
                if (changeCheckScope.changed)
                {
                    switch (chooseType)
                    {
                        // Hide
                        case 0:
                            cache_StencilSetting = false;
                            break;

                        // Show
                        case 1:
                            cache_StencilSetting = true;
                            break;
                    }

                    Undo.RecordObject(UberParticleEditorSetting.Instance, "Undo Register Stencil Setting");
                    UberParticleEditorSetting.Instance.StencilSetting = cache_StencilSetting;
                    EditorUtility.SetDirty(UberParticleEditorSetting.Instance);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                    UberParticleEditor.refreshSetting = true;
                }
            }

            GUILayout.EndHorizontal();
        }

        private void RegisterRenderQueuePropertyGUI()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(indentLevel);
            GUILayout.Label("Render Queue Setting");

            var chooseType = cache_RenderQueueSetting ? 1 : 0;
            using (var changeCheckScope = new EditorGUI.ChangeCheckScope())
            {
                chooseType = GUILayout.Toolbar(chooseType, new[] { "Hide", "Show" }, GUILayout.Width(200));
                if (changeCheckScope.changed)
                {
                    switch (chooseType)
                    {
                        // Hide
                        case 0:
                            cache_RenderQueueSetting = false;
                            break;

                        // Show
                        case 1:
                            cache_RenderQueueSetting = true;
                            break;
                    }

                    Undo.RecordObject(UberParticleEditorSetting.Instance, "Undo Register Render Queue Setting");
                    UberParticleEditorSetting.Instance.RenderQueueSetting = cache_RenderQueueSetting;
                    EditorUtility.SetDirty(UberParticleEditorSetting.Instance);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                    UberParticleEditor.refreshSetting = true;
                }
            }

            GUILayout.EndHorizontal();
        }

        private void RegisterGPUInstancingPropertyGUI()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(indentLevel);
            GUILayout.Label("GPU Instancing Setting");

            var chooseType = cache_GPUInstancingSetting ? 1 : 0;
            using (var changeCheckScope = new EditorGUI.ChangeCheckScope())
            {
                chooseType = GUILayout.Toolbar(chooseType, new[] { "Hide", "Show" }, GUILayout.Width(200));
                if (changeCheckScope.changed)
                {
                    switch (chooseType)
                    {
                        // Hide
                        case 0:
                            cache_GPUInstancingSetting = false;
                            break;

                        // Show
                        case 1:
                            cache_GPUInstancingSetting = true;
                            break;
                    }

                    Undo.RecordObject(UberParticleEditorSetting.Instance, "Undo Register GPU Instancing Setting");
                    UberParticleEditorSetting.Instance.GPUInstancingSetting = cache_GPUInstancingSetting;
                    EditorUtility.SetDirty(UberParticleEditorSetting.Instance);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                    UberParticleEditor.refreshSetting = true;
                }
            }

            GUILayout.EndHorizontal();
        }

        private void FixAllParticleSystemVertexStreams()
        {
            if (GUILayout.Button(EditorGUIUtility.TrTextContentWithIcon("Fix all particle system's vertex streams (enabled mesh GPU instancing only)!",
                    MessageType.Warning)))
            {
                var guids = AssetDatabase.FindAssets("t:Prefab");
                foreach (var guid in guids)
                {
                    var path = AssetDatabase.GUIDToAssetPath(guid);
                    var particlePrefab = AssetDatabase.LoadAssetAtPath<ParticleSystem>(path);
                    if (particlePrefab != null)
                    {
                        var eachParticles = particlePrefab.GetComponentsInChildren<ParticleSystemRenderer>();
                        foreach (var particleSystemRenderer in eachParticles)
                            if (particleSystemRenderer.sharedMaterial != null && particleSystemRenderer.sharedMaterial.shader == UberParticleEditorSetting.Instance.UberParticleShader)
                                if (particleSystemRenderer.renderMode == ParticleSystemRenderMode.Mesh)
                                    if (particleSystemRenderer != null && particleSystemRenderer.enableGPUInstancing)
                                    {
                                        var vertexStreams = CheckMaterialRegisterVertexStreams(particleSystemRenderer.sharedMaterial);
                                        particleSystemRenderer.SetActiveVertexStreams(vertexStreams);
                                        Debug.Log($"Fix <color=#FF789F>{particleSystemRenderer.name}</color>'s Custom Vertex Streams.");
                                    }
                    }
                }
            }
        }

        private List<ParticleSystemVertexStream> CheckMaterialRegisterVertexStreams(Material currentMat)
        {
            var vertexStreams_Normal = false;
            var vertexStreams_Tangent = false;

            vertexStreams_Normal = currentMat.GetFloat(ShaderIDs.ActiveNormal) == 1 ||
                                   currentMat.GetFloat(ShaderIDs.ActiveFresnel) == 1 ||
                                   currentMat.GetFloat(ShaderIDs.ActiveVertexOffset) == 1 ||
                                   currentMat.GetFloat(ShaderIDs.ActiveLightSystem) == 1 ||
                                   currentMat.GetFloat(ShaderIDs.ActiveLightSystem) == 2 ||
                                   currentMat.GetFloat(ShaderIDs.ActiveParallax) == 1 ||
                                   currentMat.GetFloat(ShaderIDs.ActiveParallax) == 2;

            vertexStreams_Tangent = currentMat.GetFloat(ShaderIDs.ActiveNormal) == 1 ||
                                    currentMat.GetFloat(ShaderIDs.ActiveParallax) == 1 ||
                                    currentMat.GetFloat(ShaderIDs.ActiveParallax) == 2;

            var vertexStreams = new List<ParticleSystemVertexStream>
            {
                // Base
                ParticleSystemVertexStream.Position,
                ParticleSystemVertexStream.Color,
                ParticleSystemVertexStream.UV,
                // Enable GPU Mesh
                ParticleSystemVertexStream.UV2,
                ParticleSystemVertexStream.Custom1XYZW,
                ParticleSystemVertexStream.Custom2XYZW,
                ParticleSystemVertexStream.AnimFrame
            };
            // Additional
            if (vertexStreams_Normal)
                vertexStreams.Add(ParticleSystemVertexStream.Normal);

            if (vertexStreams_Tangent)
                vertexStreams.Add(ParticleSystemVertexStream.Tangent);

            return vertexStreams;
        }
    }
}