using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace UberParticleShader.Editor
{
    public class SearchMenu : EditorWindow
    {
        private string _searchString = "";
        private readonly List<MenuOption> _allOptions = new();
        private List<MenuOption> _currentOptions = new();
        private bool closeWindow;
        private GUIStyle hideLabelButtonStlye { get; set; }
        private GUIStyle showLabelButtonStlye { get; set; }
        private Vector2 scrollPosition = Vector2.zero;

        public static void ShowWindow(Rect windowRect)
        {
            var window = GetWindow<SearchMenu>("Uber Particle Features");
            window.position = windowRect;
        }

        private void OnLostFocus()
        {
            if (!closeWindow)
            {
                closeWindow = true;
                Close();
            }
        }

        private void OnEnable()
        {
            _allOptions.Add(new MenuOption("Distort", UberParticleEditorGUIStyle.UberParticleFeatures.Distort,
                () => ShowHideFeatures(ref UberParticleEditorGUIStyle.UberParticleFeatures.Distort)));
            _allOptions.Add(new MenuOption("Dissolve", UberParticleEditorGUIStyle.UberParticleFeatures.Dissolve,
                () => ShowHideFeatures(ref UberParticleEditorGUIStyle.UberParticleFeatures.Dissolve)));
            _allOptions.Add(new MenuOption("Emission", UberParticleEditorGUIStyle.UberParticleFeatures.Emission,
                () => ShowHideFeatures(ref UberParticleEditorGUIStyle.UberParticleFeatures.Emission)));
            _allOptions.Add(new MenuOption("ColorRamp", UberParticleEditorGUIStyle.UberParticleFeatures.ColorRamp,
                () => ShowHideFeatures(ref UberParticleEditorGUIStyle.UberParticleFeatures.ColorRamp)));
            _allOptions.Add(new MenuOption("NormalMap", UberParticleEditorGUIStyle.UberParticleFeatures.NormalMap,
                () => ShowHideFeatures(ref UberParticleEditorGUIStyle.UberParticleFeatures.NormalMap)));
            _allOptions.Add(new MenuOption("Parallax", UberParticleEditorGUIStyle.UberParticleFeatures.Parallax,
                () => ShowHideFeatures(ref UberParticleEditorGUIStyle.UberParticleFeatures.Parallax)));
            _allOptions.Add(new MenuOption("Fresnel", UberParticleEditorGUIStyle.UberParticleFeatures.Fresnel,
                () => ShowHideFeatures(ref UberParticleEditorGUIStyle.UberParticleFeatures.Fresnel)));
            _allOptions.Add(new MenuOption("Vertex-Offset", UberParticleEditorGUIStyle.UberParticleFeatures.VertexOffset,
                () => ShowHideFeatures(ref UberParticleEditorGUIStyle.UberParticleFeatures.VertexOffset)));

            _allOptions.Add(new MenuOption("Double-Side Color", UberParticleEditorGUIStyle.UberParticleFeatures.DoubleSideColor,
                () => ShowHideFeatures(ref UberParticleEditorGUIStyle.UberParticleFeatures.DoubleSideColor)));
            _allOptions.Add(new MenuOption("Soft-Particle", UberParticleEditorGUIStyle.UberParticleFeatures.SoftParticle,
                () => ShowHideFeatures(ref UberParticleEditorGUIStyle.UberParticleFeatures.SoftParticle)));
            _allOptions.Add(new MenuOption("Light-Illumination", UberParticleEditorGUIStyle.UberParticleFeatures.LightSystem,
                () => ShowHideFeatures(ref UberParticleEditorGUIStyle.UberParticleFeatures.LightSystem)));
            _allOptions.Add(new MenuOption("External Alpha Template", UberParticleEditorGUIStyle.UberParticleFeatures.ExternalAlpha,
                () => ShowHideFeatures(ref UberParticleEditorGUIStyle.UberParticleFeatures.ExternalAlpha)));
            _allOptions.Add(new MenuOption("Alpha-Clip", UberParticleEditorGUIStyle.UberParticleFeatures.AlphaClip,
                () => ShowHideFeatures(ref UberParticleEditorGUIStyle.UberParticleFeatures.AlphaClip)));
            _allOptions.Add(new MenuOption("Color Adjustment", UberParticleEditorGUIStyle.UberParticleFeatures.ColorAdjustment,
                () => ShowHideFeatures(ref UberParticleEditorGUIStyle.UberParticleFeatures.ColorAdjustment)));

            FilterOptions();

            // GUIStyle
            if (hideLabelButtonStlye == null)
                hideLabelButtonStlye = new GUIStyle(EditorStyles.toolbarButton);
            hideLabelButtonStlye.alignment = TextAnchor.MiddleCenter;
            hideLabelButtonStlye.fontSize = 12;

            if (showLabelButtonStlye == null)
                showLabelButtonStlye = new GUIStyle(EditorStyles.toolbarButton);
            showLabelButtonStlye.alignment = TextAnchor.MiddleCenter;
            showLabelButtonStlye.fontSize = 12;
            showLabelButtonStlye.normal.textColor = Color.red;
            showLabelButtonStlye.hover.textColor = Color.red;
        }

        private void ShowHideFeatures(ref bool showHideFeature)
        {
            showHideFeature = !showHideFeature;
            Close();
        }

        private void OnGUI()
        {
#if UNITY_2022_3_OR_NEWER
            _searchString = EditorGUILayout.TextField("", _searchString, "ToolbarSearchTextField");
#else
            _searchString = EditorGUILayout.TextField("", _searchString, "ToolbarSeachTextField");
#endif

            if (GUI.changed) FilterOptions();

            // Begin the ScrollView
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Width(250), GUILayout.Height(275));
            foreach (var option in _currentOptions)
                if (GUILayout.Button(option.Name, option.showHide ? showLabelButtonStlye : hideLabelButtonStlye))
                    option.Callback?.Invoke();

            GUILayout.EndScrollView();
        }

        private void FilterOptions()
        {
            _currentOptions = _allOptions
                .Where(option => option.Name.ToLower().Contains(_searchString.ToLower()))
                .ToList();
        }
    }
}