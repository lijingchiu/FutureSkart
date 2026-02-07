using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace UberParticleShader.Editor
{
    public class UberParticleEditorGUIStyle : ShaderGUI
    {
        internal MaterialEditor MatEditor { get; set; }
        internal ParticleProperties particleProperties { get; set; }
        internal Material targetMat { get; set; }
        internal bool initialize { get; set; }


        #region GradientMapGUI Data

        internal Gradient gradientMap;
        internal Texture2D gradientTex;
        internal Texture2D beforePreivewGradientTex;
        internal string saveGradientTexPath;
        internal bool previewRampColor { get; set; }
        internal bool canSaveGradientTex;

        #endregion

        #region Enable Custom Vertex Streams Data

        internal bool registerVertexStreams { get; set; }
        internal CustomDataControllerFlags customDataControllerFlags { get; set; }
        internal bool vertexStreams_Normal { get; set; }
        internal bool vertexStreams_Tangent { get; set; }
        internal bool vertexStreams_UV2 { get; set; }
        internal bool vertexStreams_AnimFrame { get; set; }

        #endregion

        #region GUI Data

        internal Color textDefaultColor = new(0.7f, 0.7f, 0.7f, 1);
        internal Texture2D LogoTex { get; set; }
        internal Texture2D basementTex { get; set; }
        internal Texture2D additionTex { get; set; }
        internal Texture2D experimentalTex { get; set; }
        internal GUIStyle activeModeStyle { get; set; }
        internal GUIStyle lineStyle { get; set; }
        internal GUIStyle boxStyle { get; set; }
        internal GUIStyle toggleStyle { get; set; }
        internal GUIStyle folderStyle { get; set; }
        internal GUIStyle upperCenterTitleStyle { get; set; }
        internal GUIStyle leftCenterTitleStyle { get; set; }
        internal GUIStyle bigTitleStyle { get; set; }
        internal GUIStyle boldLabelStyle { get; set; }
        internal GUIStyle normalLabelStyle { get; set; }
        internal GUIStyle popupStyle { get; set; }
        internal GUIStyle dropDownStyle { get; set; }
        internal GUIStyle textureStyle { get; set; }
        internal GUIStyle additionalTitleStyle { get; set; }

        internal float tileOffsetTextLabelSpace { get; set; }
        internal float flexibleLabelSpace { get; set; }
        internal float singlePropertyHeight { get; set; }
        internal float texturesinglePropertyHeight { get; set; }

        // Controller
        internal bool MainTexTileOffsetController = EditorPrefs.GetBool("MainTexTileOffsetController", false);
        internal bool DistortTexTileOffsetController = EditorPrefs.GetBool("DistortTexTileOffsetController", false);
        internal bool DissolveTexTileOffsetController = EditorPrefs.GetBool("DissolveTexTileOffsetController", false);
        internal bool NormalTexTileOffsetController = EditorPrefs.GetBool("NormalTexTileOffsetController", false);
        internal bool ParallaxTexTileOffsetController = EditorPrefs.GetBool("ParallaxTexTileOffsetController", false);
        internal bool VertexOffsetTexTileOffsetController = EditorPrefs.GetBool("VertexOffsetTexTileOffsetController", false);
        internal bool VertexOffsetMaskTileOffsetController = EditorPrefs.GetBool("VertexOffsetMaskTileOffsetController", false);
        internal bool EmissionTexTileOffsetController = EditorPrefs.GetBool("EmissionTexTileOffsetController", false);

        // Folder
        internal bool Parallax_HeightMapNoiseFolder = EditorPrefs.GetBool("Parallax_HeightMapNoiseFolder", false);

        #endregion

        public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
        {
            MatEditor = materialEditor;
            particleProperties = new ParticleProperties(properties);
            targetMat = MatEditor.target as Material;

            DrawingGUIStyle();
        }

        private void DrawingGUIStyle()
        {
            ActiveModeGUIStyle();
            LineGUIStyle();
            BoxGUIStyle();
            ToggleGUIStyle();
            FolderGUIStyle();
            UpperCenterTitleGUIStyle();
            LeftCenterTitleGUIStyle();
            PreCalculateGUIData();
            BigTitleGUIStyle();
            BoldLabelGUIStyle();
            NormalLabelGUIStyle();
            PopupGUIStyle();
            DropDownButtonGUIStyle();
            TextureGUIStyle();
            TexturePreparing();
            AdditionalTitleGUIStyle();
        }

        #region GUIStyle

        private void PreCalculateGUIData()
        {
            flexibleLabelSpace = EditorGUIUtility.currentViewWidth * 0.25f;
            tileOffsetTextLabelSpace = EditorGUIUtility.labelWidth * 0.45f;
            singlePropertyHeight = EditorGUIUtility.singleLineHeight;
            texturesinglePropertyHeight = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
        }

        private void ActiveModeGUIStyle()
        {
            if (activeModeStyle == null)
                activeModeStyle = new GUIStyle(EditorStyles.label);
            activeModeStyle.fontStyle = FontStyle.Bold;
            activeModeStyle.alignment = TextAnchor.UpperLeft;
            activeModeStyle.fontSize = 16;
        }

        private void LineGUIStyle()
        {
            if (lineStyle == null)
                lineStyle = new GUIStyle();
            lineStyle.normal.background = Texture2D.grayTexture;
            lineStyle.fixedHeight = 1;
        }

        private void BoxGUIStyle()
        {
            if (boxStyle == null)
                boxStyle = new GUIStyle(EditorStyles.helpBox);
        }

        private void ToggleGUIStyle()
        {
            if (toggleStyle == null)
                toggleStyle = new GUIStyle(EditorStyles.toggle);
            toggleStyle.normal.background = Resources.Load<Texture2D>("UberParticle/Icon/Toggle_Off");
            toggleStyle.onNormal.background = Resources.Load<Texture2D>("UberParticle/Icon/Toggle_On");
            toggleStyle.fixedWidth = toggleStyle.normal.background.width;
            toggleStyle.fixedHeight = toggleStyle.normal.background.height;
            toggleStyle.alignment = TextAnchor.LowerLeft;
        }

        private void FolderGUIStyle()
        {
            if (folderStyle == null)
                folderStyle = new GUIStyle(EditorStyles.foldoutHeader);
            folderStyle.alignment = TextAnchor.UpperCenter;
            folderStyle.contentOffset = new Vector2(EditorGUIUtility.currentViewWidth / 4, 0);
        }

        private void UpperCenterTitleGUIStyle()
        {
            if (upperCenterTitleStyle == null)
                upperCenterTitleStyle = new GUIStyle(EditorStyles.boldLabel);
            upperCenterTitleStyle.alignment = TextAnchor.UpperCenter;
        }

        private void LeftCenterTitleGUIStyle()
        {
            if (leftCenterTitleStyle == null)
                leftCenterTitleStyle = new GUIStyle(EditorStyles.boldLabel);
            leftCenterTitleStyle.alignment = TextAnchor.MiddleLeft;
        }

        private void BigTitleGUIStyle()
        {
            if (bigTitleStyle == null)
                bigTitleStyle = new GUIStyle(EditorStyles.boldLabel);
            bigTitleStyle.alignment = TextAnchor.UpperCenter;
            bigTitleStyle.fontSize = 16;
        }

        private void BoldLabelGUIStyle()
        {
            if (boldLabelStyle == null)
                boldLabelStyle = new GUIStyle(EditorStyles.boldLabel);
            boldLabelStyle.alignment = TextAnchor.MiddleLeft;
        }

        private void NormalLabelGUIStyle()
        {
            if (normalLabelStyle == null)
                normalLabelStyle = new GUIStyle(EditorStyles.label);
            normalLabelStyle.alignment = TextAnchor.MiddleLeft;
        }

        private void PopupGUIStyle()
        {
            if (popupStyle == null)
                popupStyle = new GUIStyle(EditorStyles.popup);
            popupStyle.alignment = TextAnchor.MiddleLeft;
        }

        private void DropDownButtonGUIStyle()
        {
            if (dropDownStyle == null)
                dropDownStyle = new GUIStyle(GUI.skin.button);
        }

        private void TextureGUIStyle()
        {
            if (textureStyle == null)
                textureStyle = new GUIStyle(EditorStyles.label);
            textureStyle.alignment = TextAnchor.LowerLeft;
        }

        private void AdditionalTitleGUIStyle()
        {
            if (additionalTitleStyle == null)
                additionalTitleStyle = new GUIStyle(EditorStyles.label);
            additionalTitleStyle.alignment = TextAnchor.MiddleCenter;
            additionalTitleStyle.fontStyle = FontStyle.Bold;
        }

        private void TexturePreparing()
        {
            if (LogoTex == null)
                LogoTex = Resources.Load<Texture2D>("UberParticle/Icon/UberParticleShader Logo");
            if (basementTex == null)
                basementTex = Resources.Load<Texture2D>("UberParticle/Icon/Base");
            if (additionTex == null)
                additionTex = Resources.Load<Texture2D>("UberParticle/Icon/Addition");
            if (experimentalTex == null)
                experimentalTex = Resources.Load<Texture2D>("UberParticle/Icon/Experimental");
        }

        private Texture2D FeatureTex(in FeatureType featureType)
        {
            switch (featureType)
            {
                case FeatureType.Base:
                    return basementTex;
                case FeatureType.Addition:
                    return additionTex;
                case FeatureType.Experimental:
                    return null;
            }

            return null;
        }

        #endregion

        #region Custom Shader Property GUI

        internal void AdditionalTitleGUI(in string titleName)
        {
            GUILayout.BeginHorizontal("box");
            GUILayout.FlexibleSpace();
            GUILayout.Label(titleName, additionalTitleStyle);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        internal void MultiChannelsShaderPropertyGUI(MaterialProperty property, in string titleName, int labelIndent)
        {
            EditorGUI.indentLevel += labelIndent;
            var propertyRect = EditorGUI.IndentedRect(EditorGUILayout.GetControlRect(false, singlePropertyHeight));
            EditorGUI.indentLevel -= labelIndent;

            GUI.Label(propertyRect, titleName, normalLabelStyle);
            var toggleButton = new GUIStyle("Button");

            propertyRect.xMin += EditorGUIUtility.labelWidth - 15f;
            propertyRect.width *= 0.25f;

            // cant straightly modify vector value from Material Property. So create one temp here.
            var tempPropertyVector4 = property.vectorValue;

            tempPropertyVector4.x = Convert.ToInt32(GUI.Toggle(propertyRect, Convert.ToBoolean(property.vectorValue.x), "R", toggleButton));
            propertyRect.x += propertyRect.width;
            tempPropertyVector4.y = Convert.ToInt32(GUI.Toggle(propertyRect, Convert.ToBoolean(property.vectorValue.y), "G", toggleButton));
            propertyRect.x += propertyRect.width;
            tempPropertyVector4.z = Convert.ToInt32(GUI.Toggle(propertyRect, Convert.ToBoolean(property.vectorValue.z), "B", toggleButton));
            propertyRect.x += propertyRect.width;
            tempPropertyVector4.w = Convert.ToInt32(GUI.Toggle(propertyRect, Convert.ToBoolean(property.vectorValue.w), "A", toggleButton));

            property.vectorValue = tempPropertyVector4;
        }

        internal void MultiChannelsShaderPropertyGUI(MaterialProperty property, in string titleName, int labelIndent, string[] buttonsName)
        {
            EditorGUI.indentLevel += labelIndent;
            var propertyRect = EditorGUI.IndentedRect(EditorGUILayout.GetControlRect(false, singlePropertyHeight));
            EditorGUI.indentLevel -= labelIndent;

            GUI.Label(propertyRect, titleName, normalLabelStyle);
            var toggleButton = new GUIStyle("Button");

            propertyRect.xMin += EditorGUIUtility.labelWidth - 15f;
            propertyRect.width *= 0.25f;

            // cant straightly modify vector value from Material Property. So create one temp here.
            var tempPropertyVector4 = property.vectorValue;

            tempPropertyVector4.x = Convert.ToInt32(GUI.Toggle(propertyRect, Convert.ToBoolean(property.vectorValue.x), buttonsName[0], toggleButton));
            propertyRect.x += propertyRect.width;
            tempPropertyVector4.y = Convert.ToInt32(GUI.Toggle(propertyRect, Convert.ToBoolean(property.vectorValue.y), buttonsName[1], toggleButton));
            propertyRect.x += propertyRect.width;
            tempPropertyVector4.z = Convert.ToInt32(GUI.Toggle(propertyRect, Convert.ToBoolean(property.vectorValue.z), buttonsName[2], toggleButton));
            propertyRect.x += propertyRect.width;
            tempPropertyVector4.w = Convert.ToInt32(GUI.Toggle(propertyRect, Convert.ToBoolean(property.vectorValue.w), buttonsName[3], toggleButton));

            property.vectorValue = tempPropertyVector4;
        }

        internal void ToggleShaderPropertyGUI(MaterialProperty property, in string titleName, int labelIndent)
        {
            EditorGUI.indentLevel += labelIndent;
            var toggleRect = EditorGUI.IndentedRect(EditorGUILayout.GetControlRect(false, singlePropertyHeight));
            EditorGUI.indentLevel -= labelIndent;

            GUI.Label(toggleRect, titleName, normalLabelStyle);
            toggleRect.xMin += toggleRect.width * 0.5f;
            property.floatValue = Convert.ToInt32(EditorGUI.Toggle(toggleRect, Convert.ToBoolean(property.floatValue), toggleStyle));
        }

        internal void ActiveModeShaderPropertyGUI(MaterialProperty property, in string titleName, in FeatureType featureType)
        {
            var rect = EditorGUI.IndentedRect(EditorGUILayout.GetControlRect(false, singlePropertyHeight));

            GUI.Label(rect, titleName, activeModeStyle);
            rect.xMin += EditorGUIUtility.labelWidth * 1.25f;
            var featureTex = FeatureTex(featureType);
            GUI.Label(rect, featureTex, textureStyle);
            rect.xMin += featureTex.width;
            using (var checkScope = new EditorGUI.ChangeCheckScope())
            {
                MatEditor.ShaderProperty(rect, property, "");
                if (checkScope.changed)
                    registerVertexStreams = true;
            }

            EditorGUILayout.LabelField("", lineStyle);
            GUILayout.Space(-15);
        }

        internal void DrawMessageTypeTitleBoxGUI(in string titleName, MessageType messageType)
        {
            GUILayout.Label(EditorGUIUtility.TrTextContentWithIcon(titleName, messageType), new GUIStyle("box") { stretchWidth = true, alignment = TextAnchor.MiddleCenter, normal = { textColor = textDefaultColor } });
        }

        internal void DrawMessageTypeTitleHelpBoxGUI(in string titleName, MessageType messageType)
        {
            GUILayout.Label(EditorGUIUtility.TrTextContentWithIcon(titleName, messageType), new GUIStyle("helpbox") { wordWrap = true, alignment = TextAnchor.MiddleCenter });
        }

        internal void DrawBoxTitleGUI(in string titleName, bool textInCenter)
        {
            GUILayout.BeginVertical("box");
            GUILayout.Box(titleName, textInCenter ? upperCenterTitleStyle : leftCenterTitleStyle);
            GUILayout.EndVertical();
        }

        internal void DrawTitleGUI(in string titleName, bool textInCenter)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(titleName, textInCenter ? bigTitleStyle : activeModeStyle);
            GUILayout.EndHorizontal();
            EditorGUILayout.LabelField("", lineStyle);
            GUILayout.Space(-15);
        }

        internal void DrawTitleGUI(in string titleName, bool textInCenter, Color textColor)
        {
            GUILayout.BeginHorizontal();

            var titleGUIStyle = textInCenter ? bigTitleStyle : activeModeStyle;
            var colorGUIStyle = new GUIStyle(titleGUIStyle)
            {
                normal = { textColor = textColor },
                hover = { textColor = textColor }
            };

            GUILayout.Label(titleName, colorGUIStyle);
            GUILayout.EndHorizontal();
            EditorGUILayout.LabelField("", lineStyle);
            GUILayout.Space(-15);
        }

        internal void TileOffsetControllerShaderPropertyGUI(MaterialProperty tileOffsetProperty, MaterialProperty autoOffsetProperty, in string folderName, ref bool folderEditorPrefs, in string folderEditorPrefsName, in bool enableIndentLevel)
        {
            EditorGUI.indentLevel += enableIndentLevel ? 1 : 0;
            GUILayout.BeginVertical("box");
            EditorGUI.indentLevel += enableIndentLevel ? 0 : 1;
            folderEditorPrefs = EditorGUILayout.Foldout(folderEditorPrefs, folderName, true, folderStyle);
            EditorGUI.indentLevel -= enableIndentLevel ? 0 : 1;
            if (folderEditorPrefs)
            {
                EditorPrefs.SetBool(folderEditorPrefsName, true);
                var tileOffsetValue = tileOffsetProperty.vectorValue;
                // One rect for one space on GUI.
                var tilingRect = EditorGUI.IndentedRect(EditorGUILayout.GetControlRect(false, EditorGUIUtility.singleLineHeight));
                var offsetRect = EditorGUI.IndentedRect(EditorGUILayout.GetControlRect(false, EditorGUIUtility.singleLineHeight));
                var autoOffsetRect = EditorGUI.IndentedRect(EditorGUILayout.GetControlRect(false, EditorGUIUtility.singleLineHeight));

                EditorGUI.BeginChangeCheck();

                // Tile
                var xRect = tilingRect;
                xRect.xMin += 2;
                GUI.Label(xRect, "Tiling");
                xRect.xMin += tileOffsetTextLabelSpace;
                GUI.Label(xRect, "X");
                xRect.xMin += enableIndentLevel ? 0 : 12;
                xRect.width *= 0.5f;
                tileOffsetValue.x = Convert.ToInt32(EditorGUI.Popup(xRect, (int)tileOffsetValue.x, CustomDataController));

                var yRect = xRect;
                yRect.x += yRect.width + 2;
                yRect.xMax -= 2;
                GUI.Label(yRect, "Y");
                yRect.xMin += enableIndentLevel ? 0 : 12;
                tileOffsetValue.y = Convert.ToInt32(EditorGUI.Popup(yRect, (int)tileOffsetValue.y, CustomDataController));

                // Offset
                var zRect = offsetRect;
                zRect.xMin += 2;
                GUI.Label(zRect, "Offset");
                zRect.xMin += tileOffsetTextLabelSpace;
                GUI.Label(zRect, "X");
                zRect.xMin += enableIndentLevel ? 0 : 12;
                zRect.width *= 0.5f;
                tileOffsetValue.z = Convert.ToInt32(EditorGUI.Popup(zRect, (int)tileOffsetValue.z, CustomDataController));

                var wRect = zRect;
                wRect.x += wRect.width + 2;
                wRect.xMax -= 2;
                GUI.Label(wRect, "Y");
                wRect.xMin += enableIndentLevel ? 0 : 12;
                tileOffsetValue.w = Convert.ToInt32(EditorGUI.Popup(wRect, (int)tileOffsetValue.w, CustomDataController));

                if (EditorGUI.EndChangeCheck())
                {
                    tileOffsetProperty.vectorValue = tileOffsetValue;
                    registerVertexStreams = true;
                }

                // UV Auto Offset
                autoOffsetRect.xMin += 2;
                GUI.Label(autoOffsetRect, "UV Auto Offset");
                autoOffsetRect.xMin += autoOffsetRect.width * 0.5f;
                autoOffsetProperty.floatValue = Convert.ToInt32(EditorGUI.Toggle(autoOffsetRect, Convert.ToBoolean(autoOffsetProperty.floatValue), toggleStyle));
            }
            else
            {
                EditorPrefs.SetBool(folderEditorPrefsName, false);
            }

            GUILayout.EndVertical();
            EditorGUI.indentLevel -= enableIndentLevel ? 1 : 0;
        }

        internal void FlipBookShaderPropertyGUI(MaterialProperty property, in string titleName, int labelIndent)
        {
            EditorGUI.indentLevel += labelIndent;

            GUILayout.BeginVertical("box");
            GUILayout.Label(titleName, upperCenterTitleStyle);
            var tilesRect = EditorGUI.IndentedRect(EditorGUILayout.GetControlRect(false, EditorGUIUtility.singleLineHeight));
            var sliderRect = EditorGUI.IndentedRect(EditorGUILayout.GetControlRect(false, EditorGUIUtility.singleLineHeight));

            EditorGUI.indentLevel -= labelIndent;

            EditorGUI.BeginChangeCheck();

            var flipBookTiles = property.vectorValue;

            GUI.Label(tilesRect, "Tiles");
            tilesRect.xMin += flexibleLabelSpace;
            GUI.Label(tilesRect, "X");
            tilesRect.xMin += 15f;
            tilesRect.width *= 0.5f;
            tilesRect.xMax -= 5f;
            flipBookTiles.x = EditorGUI.IntField(tilesRect, (int)flipBookTiles.x);
            tilesRect.x += tilesRect.width + 2f;
            GUI.Label(tilesRect, "Y");
            tilesRect.xMin += 15f;
            tilesRect.xMax += 5f + 3f;
            flipBookTiles.y = EditorGUI.IntField(tilesRect, (int)flipBookTiles.y);

            GUI.Label(sliderRect, "FrameCount");
            sliderRect.xMin += flexibleLabelSpace;

            var popupRect = sliderRect;
            sliderRect.width *= 0.65f;
            using (new EditorGUI.DisabledScope(flipBookTiles.w != 0))
            {
                flipBookTiles.z = EditorGUI.IntSlider(sliderRect, (int)flipBookTiles.z, 0, (int)(flipBookTiles.x * flipBookTiles.y - 1));
            }

            sliderRect.x += sliderRect.width + 2f;
            sliderRect.xMax -= popupRect.width * 0.3f;
            sliderRect.xMax -= 2f;

            using (var checkScope = new EditorGUI.ChangeCheckScope())
            {
                flipBookTiles.w = EditorGUI.Popup(sliderRect, (int)flipBookTiles.w, CustomDataController);
                if (checkScope.changed)
                    registerVertexStreams = true;
            }

            if (EditorGUI.EndChangeCheck()) property.vectorValue = flipBookTiles;

            GUILayout.EndVertical();
        }

        internal void SliderControllerShaderPropertyGUI(MaterialProperty sliderProperty, MaterialProperty controllerProperty, in string titleName, int labelIndent)
        {
            EditorGUI.indentLevel += labelIndent;

            var sliderRangeLimit = sliderProperty.rangeLimits;
            var sliderRect = EditorGUI.IndentedRect(EditorGUILayout.GetControlRect(false, EditorGUIUtility.singleLineHeight));

            EditorGUI.indentLevel -= labelIndent;

            // Draw Slider
            GUI.Label(sliderRect, titleName, normalLabelStyle);

            sliderRect.xMin += EditorGUIUtility.labelWidth + 2f;
            sliderRect.xMin -= 15f * labelIndent;


            var popupRect = sliderRect;
            sliderRect.width *= 0.65f;
            using (new EditorGUI.DisabledScope(controllerProperty.floatValue != 0))
            {
                sliderProperty.floatValue = EditorGUI.Slider(sliderRect, sliderProperty.floatValue, sliderRangeLimit.x, sliderRangeLimit.y);
            }

            sliderRect.x += sliderRect.width + 2f;
            sliderRect.xMax -= popupRect.width * 0.3f;
            sliderRect.xMax -= 2f;
            EditorGUI.BeginChangeCheck();
            controllerProperty.floatValue = EditorGUI.Popup(sliderRect, (int)controllerProperty.floatValue, CustomDataController);
            if (EditorGUI.EndChangeCheck()) registerVertexStreams = true;
        }

        internal void TextureShaderPropertyGUI(MaterialProperty textureProperty, MaterialProperty wrapModeProperty, MaterialProperty uvChannelProperty, bool showTileOffset, bool disableTileOffsetChange, bool showUVChannel)
        {
            var texturePropertyRectHeight = showUVChannel ? 4f : 3f;
            texturePropertyRectHeight *= EditorGUIUtility.singleLineHeight;

            var texturePropertyRectVerticalSpacing = showUVChannel ? 4f : 3f;
            texturePropertyRectVerticalSpacing *= EditorGUIUtility.standardVerticalSpacing;

            // Rect
            var texturePropertyRect =
                EditorGUILayout.GetControlRect(false, texturePropertyRectHeight + texturePropertyRectVerticalSpacing);
            texturePropertyRect.x += 2f;
            texturePropertyRect.y += EditorGUIUtility.standardVerticalSpacing;
            var tileRect = texturePropertyRect;

            texturePropertyRect.width = 56f;
            texturePropertyRect.height = 56f;

            var textureRect = texturePropertyRect;
            textureRect.x += 10f;
            textureRect.y += showUVChannel ? EditorGUIUtility.singleLineHeight * 0.5f : 0f;

            tileRect.xMin += (texturePropertyRect.width + flexibleLabelSpace) * 0.5f;
            tileRect.height = EditorGUIUtility.singleLineHeight;

            var temp_texturesinglePropertyHeight = texturesinglePropertyHeight;

            var offsetRect = tileRect;
            offsetRect.y += temp_texturesinglePropertyHeight;
            temp_texturesinglePropertyHeight += texturesinglePropertyHeight;

            var uvChannelRect = tileRect;
            if (showUVChannel)
            {
                uvChannelRect.y += temp_texturesinglePropertyHeight;
                temp_texturesinglePropertyHeight += texturesinglePropertyHeight;
            }

            var wrapModeRect = tileRect;
            wrapModeRect.y += temp_texturesinglePropertyHeight;
            temp_texturesinglePropertyHeight += texturesinglePropertyHeight;

            // Texture type
            Type textureType = null;
            switch (textureProperty.textureDimension)
            {
                case TextureDimension.Unknown:
                case TextureDimension.None:
                case TextureDimension.Any:
                    textureType = typeof(Texture);
                    break;
                case TextureDimension.Tex2D:
                case TextureDimension.Cube:
                    textureType = typeof(Texture2D);
                    break;
                case TextureDimension.Tex3D:
                    textureType = typeof(Texture3D);
                    break;
                case TextureDimension.Tex2DArray:
                case TextureDimension.CubeArray:
                    textureType = typeof(Texture2DArray);
                    break;
            }

            // Texture tile
            var tileOffset = textureProperty.textureScaleAndOffset;
            var tile = new Vector2(tileOffset.x, tileOffset.y);
            var offset = new Vector2(tileOffset.z, tileOffset.w);

            EditorGUI.BeginChangeCheck();

            // Draw texture shader property
            using (var changeCheckScope = new EditorGUI.ChangeCheckScope())
            {
                var tempTexture = (Texture)EditorGUI.ObjectField(textureRect, new GUIContent(""), textureProperty.textureValue, textureType, false);
                if (changeCheckScope.changed)
                {
                    MatEditor.RegisterPropertyChangeUndo(textureProperty.name);
                    textureProperty.textureValue = tempTexture;
                }
            }

            // Draw Tile property
            if (showTileOffset)
                using (new EditorGUI.DisabledScope(disableTileOffsetChange))
                {
                    GUI.Label(tileRect, "Tiling");
                    tileRect.xMin += flexibleLabelSpace;
                    tileRect.xMax -= 2f;
                    tile = EditorGUI.Vector2Field(tileRect, string.Empty, tile);
                }

            // Draw Offset property
            if (showTileOffset)
                using (new EditorGUI.DisabledScope(disableTileOffsetChange))
                {
                    GUI.Label(offsetRect, "Offset");
                    offsetRect.xMin += flexibleLabelSpace;
                    offsetRect.xMax -= 2f;
                    offset = EditorGUI.Vector2Field(offsetRect, string.Empty, offset);
                }

            // Draw UV Channel property
            if (showUVChannel)
            {
                GUI.Label(uvChannelRect, "UV Channel");
                uvChannelRect.xMin += flexibleLabelSpace;
                uvChannelRect.xMax -= 2f;
                using (var changeCheckScope = new EditorGUI.ChangeCheckScope())
                {
                    MatEditor.ShaderProperty(uvChannelRect, uvChannelProperty, "");
                    if (changeCheckScope.changed)
                        registerVertexStreams = true;
                }
            }

            // Draw WrapMode property
            GUI.Label(wrapModeRect, "WrapMode");
            wrapModeRect.xMin += flexibleLabelSpace;
            wrapModeRect.xMax -= 2f;
            MatEditor.ShaderProperty(wrapModeRect, wrapModeProperty, "");

            if (EditorGUI.EndChangeCheck())
                // Return the value to tile offset
                textureProperty.textureScaleAndOffset = new Vector4(tile.x, tile.y, offset.x, offset.y);
        }

        internal void TextureShaderPropertyGUI(MaterialProperty textureProperty, MaterialProperty wrapModeProperty, bool showTileOffset, bool disableTileOffsetChange)
        {
            // Rect
            var texturePropertyRect =
                EditorGUILayout.GetControlRect(false, EditorGUIUtility.singleLineHeight * 3f + EditorGUIUtility.standardVerticalSpacing * 3f);
            texturePropertyRect.x += 2f;
            texturePropertyRect.y += EditorGUIUtility.standardVerticalSpacing;
            var tileRect = texturePropertyRect;

            texturePropertyRect.width = 56f;
            texturePropertyRect.height = 56f;

            var textureRect = texturePropertyRect;
            textureRect.x += 10f;

            tileRect.xMin += (texturePropertyRect.width + flexibleLabelSpace) * 0.5f;
            tileRect.height = EditorGUIUtility.singleLineHeight;

            var offsetRect = tileRect;
            offsetRect.y += texturesinglePropertyHeight;

            var wrapModeRect = offsetRect;
            wrapModeRect.y += texturesinglePropertyHeight;

            // Texture type
            Type textureType = null;
            switch (textureProperty.textureDimension)
            {
                case TextureDimension.Unknown:
                case TextureDimension.None:
                case TextureDimension.Any:
                    textureType = typeof(Texture);
                    break;
                case TextureDimension.Tex2D:
                case TextureDimension.Cube:
                    textureType = typeof(Texture2D);
                    break;
                case TextureDimension.Tex3D:
                    textureType = typeof(Texture3D);
                    break;
                case TextureDimension.Tex2DArray:
                case TextureDimension.CubeArray:
                    textureType = typeof(Texture2DArray);
                    break;
            }

            // Texture tile
            var tileOffset = textureProperty.textureScaleAndOffset;
            var tile = new Vector2(tileOffset.x, tileOffset.y);
            var offset = new Vector2(tileOffset.z, tileOffset.w);

            EditorGUI.BeginChangeCheck();

            // Draw texture shader property
            using (var changeCheckScope = new EditorGUI.ChangeCheckScope())
            {
                var tempTexture = (Texture)EditorGUI.ObjectField(textureRect, new GUIContent(""), textureProperty.textureValue, textureType, false);
                if (changeCheckScope.changed)
                {
                    MatEditor.RegisterPropertyChangeUndo(textureProperty.name);
                    textureProperty.textureValue = tempTexture;
                }
            }

            // Draw Tile property
            if (showTileOffset)
                using (new EditorGUI.DisabledScope(disableTileOffsetChange))
                {
                    GUI.Label(tileRect, "Tiling");
                    tileRect.xMin += flexibleLabelSpace;
                    tileRect.xMax -= 2f;
                    tile = EditorGUI.Vector2Field(tileRect, string.Empty, tile);
                }

            // Draw Offset property
            if (showTileOffset)
                using (new EditorGUI.DisabledScope(disableTileOffsetChange))
                {
                    GUI.Label(offsetRect, "Offset");
                    offsetRect.xMin += flexibleLabelSpace;
                    offsetRect.xMax -= 2f;
                    offset = EditorGUI.Vector2Field(offsetRect, string.Empty, offset);
                }

            // Draw WrapMode property
            GUI.Label(wrapModeRect, "WrapMode");
            wrapModeRect.xMin += flexibleLabelSpace;
            wrapModeRect.xMax -= 2f;
            MatEditor.ShaderProperty(wrapModeRect, wrapModeProperty, "");

            if (EditorGUI.EndChangeCheck())
                // Return the value to tile offset
                textureProperty.textureScaleAndOffset = new Vector4(tile.x, tile.y, offset.x, offset.y);
        }

        internal void TextureShaderPropertyGUI(MaterialProperty textureProperty, bool showTileOffset, bool disableTileOffsetChange)
        {
            // Rect
            var texturePropertyRect =
                EditorGUILayout.GetControlRect(false, EditorGUIUtility.singleLineHeight * 3f + EditorGUIUtility.standardVerticalSpacing * 3f);
            texturePropertyRect.x += 2f;
            texturePropertyRect.y += EditorGUIUtility.standardVerticalSpacing;
            var tileRect = texturePropertyRect;

            texturePropertyRect.width = 56f;
            texturePropertyRect.height = 56f;

            var textureRect = texturePropertyRect;
            textureRect.x += 10f;

            tileRect.y += texturesinglePropertyHeight * 0.5f;
            tileRect.xMin += (texturePropertyRect.width + flexibleLabelSpace) * 0.5f;
            tileRect.height = EditorGUIUtility.singleLineHeight;

            var offsetRect = tileRect;
            offsetRect.y += texturesinglePropertyHeight;


            // Texture type
            Type textureType = null;
            switch (textureProperty.textureDimension)
            {
                case TextureDimension.Unknown:
                case TextureDimension.None:
                case TextureDimension.Any:
                    textureType = typeof(Texture);
                    break;
                case TextureDimension.Tex2D:
                case TextureDimension.Cube:
                    textureType = typeof(Texture2D);
                    break;
                case TextureDimension.Tex3D:
                    textureType = typeof(Texture3D);
                    break;
                case TextureDimension.Tex2DArray:
                case TextureDimension.CubeArray:
                    textureType = typeof(Texture2DArray);
                    break;
            }

            // Texture tile
            var tileOffset = textureProperty.textureScaleAndOffset;
            var tile = new Vector2(tileOffset.x, tileOffset.y);
            var offset = new Vector2(tileOffset.z, tileOffset.w);

            EditorGUI.BeginChangeCheck();

            // Draw texture shader property
            using (var changeCheckScope = new EditorGUI.ChangeCheckScope())
            {
                var tempTexture = (Texture)EditorGUI.ObjectField(textureRect, new GUIContent(""), textureProperty.textureValue, textureType, false);
                if (changeCheckScope.changed)
                {
                    MatEditor.RegisterPropertyChangeUndo(textureProperty.name);
                    textureProperty.textureValue = tempTexture;
                }
            }

            // Draw Tile property
            if (showTileOffset)
                using (new EditorGUI.DisabledScope(disableTileOffsetChange))
                {
                    GUI.Label(tileRect, "Tiling");
                    tileRect.xMin += flexibleLabelSpace;
                    tileRect.xMax -= 2f;
                    tile = EditorGUI.Vector2Field(tileRect, string.Empty, tile);
                }

            // Draw Offset property
            if (showTileOffset)
                using (new EditorGUI.DisabledScope(disableTileOffsetChange))
                {
                    GUI.Label(offsetRect, "Offset");
                    offsetRect.xMin += flexibleLabelSpace;
                    offsetRect.xMax -= 2f;
                    offset = EditorGUI.Vector2Field(offsetRect, string.Empty, offset);
                }

            if (EditorGUI.EndChangeCheck())
                // Return the value to tile offset
                textureProperty.textureScaleAndOffset = new Vector4(tile.x, tile.y, offset.x, offset.y);
        }

        internal void FlagEnumShaderPropertyGUI<T>(MaterialProperty enumProperty, in string titleName, bool boldText, int labelIndent)
        {
            EditorGUI.indentLevel += labelIndent;
            var enumRect = EditorGUI.IndentedRect(EditorGUILayout.GetControlRect(false, singlePropertyHeight));
            EditorGUI.indentLevel -= labelIndent;

            GUI.Label(enumRect, titleName, boldText ? boldLabelStyle : normalLabelStyle);

            enumRect.xMin += EditorGUIUtility.labelWidth + 2f;
            enumRect.xMin -= 15f * labelIndent;
            enumProperty.floatValue =
                Convert.ToInt32(EditorGUI.EnumFlagsField(enumRect, (Enum)Enum.ToObject(typeof(T), (int)enumProperty.floatValue)));
        }

        internal void GradientMapShaderPropertyGUI(in MaterialProperty textureProperty, in string titleName, bool boldText, int labelIndent)
        {
            EditorGUI.indentLevel += labelIndent;
            var gradientRect = EditorGUI.IndentedRect(EditorGUILayout.GetControlRect(false, singlePropertyHeight));
            var toggleRect = EditorGUI.IndentedRect(EditorGUILayout.GetControlRect(false, singlePropertyHeight));
            var checkRect = EditorGUI.IndentedRect(EditorGUILayout.GetControlRect(false, singlePropertyHeight));
            EditorGUI.indentLevel -= labelIndent;

            GUI.Label(gradientRect, titleName, boldText ? boldLabelStyle : normalLabelStyle);
            gradientRect.xMin += EditorGUIUtility.labelWidth + 2f;
            gradientRect.xMin -= 15f * labelIndent;

            gradientMap ??= new Gradient();
            gradientMap = EditorGUI.GradientField(gradientRect, gradientMap);

            checkRect.width *= 0.5f;
            if (GUI.Button(checkRect, "Save/Create Gradient to Texture")) EditorApplication.delayCall += () => GradientMapGUI.GradientMapDelayedSave(ref gradientMap, ref saveGradientTexPath, ref canSaveGradientTex);

            checkRect.x += checkRect.width + 2f;
            if (GUI.Button(checkRect, "Sample Texture to Gradient"))
            {
                var gradientUserData = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(textureProperty.textureValue)) as TextureImporter;
                if (gradientUserData == null)
                {
                    Debug.LogError("This texture might be cached in memory, make sure the texture is created or saved by this GUI inspector.");
                    return;
                }

                if (string.IsNullOrEmpty(gradientUserData.userData))
                {
                    Debug.LogError(
                        "This texture doesn't store the gradient color's keys . make sure the texture is created or saved by this GUI inspector.");
                }
                else
                {
                    var gradientMap_List = GradientMapGUI.GetGradientsFromUserData(gradientUserData.userData);
                    if (gradientMap_List.Count == 1) gradientMap = gradientMap_List[0];
                }
            }

            GUI.Label(toggleRect, "Preview Color Ramp", normalLabelStyle);
            toggleRect.xMin += toggleRect.width * 0.5f;
            using (var changeCheckScope = new EditorGUI.ChangeCheckScope())
            {
                previewRampColor = EditorGUI.Toggle(toggleRect, previewRampColor, toggleStyle);
                if (changeCheckScope.changed)
                {
                    if (previewRampColor)
                    {
                        if (textureProperty.textureValue != null)
                            saveGradientTexPath = AssetDatabase.GetAssetPath(textureProperty.textureValue);
                        beforePreivewGradientTex = (Texture2D)textureProperty.textureValue;
                    }
                    else
                    {
                        textureProperty.textureValue = beforePreivewGradientTex;
                    }
                }
            }

            if (previewRampColor)
            {
                gradientTex = GradientMapGUI.GenerateTextureFromGradient(gradientMap, 64, 4);
                textureProperty.textureValue = gradientTex;
            }

            if (canSaveGradientTex)
            {
                previewRampColor = false;
                canSaveGradientTex = false;
                EditorApplication.delayCall += () =>
                    particleProperties.RampTex.textureValue = GradientMapGUI.SaveGradientMapAndReplaceRampTempTex(ref saveGradientTexPath);
            }
        }

        internal void AddComponentShaderPropertyGUI()
        {
            var addComponentRect = EditorGUI.IndentedRect(EditorGUILayout.GetControlRect(false, singlePropertyHeight * 1.4f));

            addComponentRect.x += addComponentRect.width * 0.5f;
            addComponentRect.width = 225;
            addComponentRect.x -= addComponentRect.width * 0.5f;

            if (EditorGUI.DropdownButton(addComponentRect, new GUIContent("Add Particle Features"), FocusType.Keyboard, dropDownStyle))
            {
                var dropDownRect = Rect.zero;
                var dropDownRectPosition = Vector2.one;
                dropDownRectPosition = GUIUtility.GUIToScreenPoint(addComponentRect.position);
                dropDownRect.x = dropDownRectPosition.x;
                dropDownRect.y = dropDownRectPosition.y;
                dropDownRect.width = 250;
                dropDownRect.height = 300;
                SearchMenu.ShowWindow(dropDownRect);
            }
        }

        #endregion

        #region Material Properties Type

        internal static class UberParticleFeatures
        {
            public static bool Distort;
            public static bool Dissolve;
            public static bool Emission;
            public static bool ColorRamp;
            public static bool NormalMap;
            public static bool Parallax;
            public static bool Fresnel;
            public static bool VertexOffset;
            public static bool DoubleSideColor;
            public static bool SoftParticle;
            public static bool LightSystem;
            public static bool ExternalAlpha;
            public static bool AlphaClip;
            public static bool ColorAdjustment;
        }

        internal static class UberParticleConstantProperties
        {
            public static bool FinalOutput;
            public static bool BlendSetting;
            public static bool StencilSetting;
            public static bool RenderQueueSetting;
            public static bool GPUInstancingSetting;
        }

        internal enum GrabPassTexMode
        {
            Normal,
            BumpTex
        }

        internal enum MainTexMode
        {
            Normal,
            MultiChannel,
            FlipBook,
            PolarCoord,
            ScreenUV
        }

        internal enum DistortMode
        {
            Disable,
            Enable,
            PolarCoord,
            FlowMap,
            ScreenUV
        }

        internal enum DissolveMode
        {
            Disable,
            Enable,
            PolarCoord,
            ScreenUV
        }

        internal enum EmissionMode
        {
            Disable,
            Enable,
            PolarCoord,
            ScreenUV
        }


        internal enum ActiveMode
        {
            Disable,
            Enable
        }

        internal enum ParallaxMode
        {
            Disable,
            Enable,
            Steep
        }

        [Flags]
        internal enum FresnelFunction
        {
            FresnelColor = 1 << 0,
            FresnelAlpha = 1 << 1
        }

        internal enum VertexOffsetMode
        {
            Disable,
            Enable,
            AnimationTex,
            FullScreen,
            LocalBillboard,
            CameraZAxisOffset
        }

        internal enum DoubleSideColorMode
        {
            Disable,
            Replace,
            Add,
            Multiply
        }

        internal enum LightSystemMode
        {
            Disable,
            VertexLight,
            FragmentLight
        }

        internal enum ExternalAlphaMode
        {
            Disable,
            Line,
            Gradient,
            Circle,
            Round
        }

        internal enum AlphaClipMode
        {
            Disable,
            HardClip,
            SmoothClip
        }

        [Flags]
        internal enum ColorAdjustmentFunction
        {
            HUEShift = 1 << 0,
            Saturation = 1 << 1,
            Contrast = 1 << 2,
            AntiShineGlow = 1 << 3
        }

        internal readonly string[] CustomDataController =
        {
            "None",
            "CustomData1.x",
            "CustomData1.y",
            "CustomData1.z",
            "CustomData1.w",
            "CustomData2.x",
            "CustomData2.y",
            "CustomData2.z",
            "CustomData2.w"
        };

        [Flags]
        internal enum CustomDataControllerFlags
        {
            None = 0,
            CustomData1X = 1 << 0,
            CustomData1Y = 1 << 1,
            CustomData1Z = 1 << 2,
            CustomData1W = 1 << 3,
            CustomData2X = 1 << 4,
            CustomData2Y = 1 << 5,
            CustomData2Z = 1 << 6,
            CustomData2W = 1 << 7
        }

        internal enum RenderMode
        {
            Normal,
            Additive,
            PreMultiply,
            Custom
        }

        [Flags]
        internal enum DistortImpactTargets
        {
            MainTex = 1 << 0,
            Dissolve = 1 << 1,
            NormalMap = 1 << 2,
            Emission = 1 << 3
        }

        [Flags]
        internal enum ParallaxImpactTargets
        {
            MainTex = 1 << 0,
            Dissolve = 1 << 1,
            NormalMap = 1 << 2,
            Distort = 1 << 3,
            Emission = 1 << 4
        }

        internal enum ParticleMode
        {
            Normal,
            Geometry,
            Decal,
            DistortionScreen
        }

        internal enum UVChannel
        {
            UV1,
            UV2
        }

        internal enum FeatureType
        {
            Base,
            Addition,
            Experimental
        }

        #endregion

        #region Material Properties Data

        internal struct ParticleProperties
        {
            // Workflow Mode
            public readonly MaterialProperty ParticleMode;

            // MainTex
            public readonly MaterialProperty MainTexMode;
            public readonly MaterialProperty MainTex;
            public readonly MaterialProperty MainTexColor;
            public readonly MaterialProperty MainTexUVChannel;
            public readonly MaterialProperty MainTexWrapMode;
            public readonly MaterialProperty MainTexUVAutoOffset;
            public readonly MaterialProperty MainTexUVTileController;
            public readonly MaterialProperty ClampMainTexUV;

            // MainTex (MultiChannel)
            public readonly MaterialProperty MainTexG_Intensity;
            public readonly MaterialProperty MainTexGIntensityController;
            public readonly MaterialProperty MainTexB_Intensity;

            public readonly MaterialProperty MainTexBIntensityController;

            // MainTex (FlipBook)
            public readonly MaterialProperty FlipBookSetting;

            // Distort
            public readonly MaterialProperty ActiveDistort;
            public readonly MaterialProperty DistortTex;
            public readonly MaterialProperty DistortTexUVChannel;
            public readonly MaterialProperty DistortTexWrapMode;
            public readonly MaterialProperty DistortChannel;
            public readonly MaterialProperty DistortTexUVAutoOffset;
            public readonly MaterialProperty DistortTexUVTileController;
            public readonly MaterialProperty DistortIntensity;
            public readonly MaterialProperty DistortIntensityController;
            public readonly MaterialProperty DistortImpactTarget;
            // Distort (FlowMap)
            public readonly MaterialProperty EnableHighPrecisionFlowMap;
            public readonly MaterialProperty ReverseFlowMap_G_Color;

            // Dissolve
            public readonly MaterialProperty ActiveDissolve;
            public readonly MaterialProperty DissolveTex;
            public readonly MaterialProperty DissolveTexUVChannel;
            public readonly MaterialProperty DissolveTexWrapMode;
            public readonly MaterialProperty DissolveChannel;
            public readonly MaterialProperty DissolveTexUVAutoOffset;
            public readonly MaterialProperty DissolveTexUVTileController;
            public readonly MaterialProperty DissolveCalculateMode;
            public readonly MaterialProperty DissolveIntensity;
            public readonly MaterialProperty DissolveIntensityController;

            public readonly MaterialProperty ActiveHardClipDissolve;

            // Dissolve (Rim)
            public readonly MaterialProperty ActiveRimDissolve;
            public readonly MaterialProperty RimDissolveRange;
            public readonly MaterialProperty RimDissolveRangeController;
            public readonly MaterialProperty RimDissolveColor;
            public readonly MaterialProperty RimDissolveColorMode;

            // Emission
            public readonly MaterialProperty ActiveEmission;
            public readonly MaterialProperty EmissionTex;
            public readonly MaterialProperty EmissionTexUVChannel;
            public readonly MaterialProperty EmissionTexWrapMode;
            public readonly MaterialProperty EmissionTexUVAutoOffset;
            public readonly MaterialProperty EmissionTexUVTileController;
            public readonly MaterialProperty EmissionIntensity;
            public readonly MaterialProperty EmissionIntensityController;
            public readonly MaterialProperty EmissionColorMode;
            public readonly MaterialProperty EmissionColor;


            // Ramp 
            public readonly MaterialProperty ActiveRamp;
            public readonly MaterialProperty RampTex;
            public readonly MaterialProperty EnhanceRampColor;

            // Normal
            public readonly MaterialProperty ActiveNormal;
            public readonly MaterialProperty NormalTex;
            public readonly MaterialProperty NormalTexUVChannel;
            public readonly MaterialProperty NormalTexWrapMode;
            public readonly MaterialProperty NormalTexUVAutoOffset;
            public readonly MaterialProperty NormalTexUVTileController;
            public readonly MaterialProperty NormalIntensity;
            public readonly MaterialProperty NormalIntensityController;
            public readonly MaterialProperty NormalBounceLightRange;

            // Parallax 
            public readonly MaterialProperty ActiveParallax;
            public readonly MaterialProperty ParallaxTex;
            public readonly MaterialProperty ParallaxTexUVChannel;
            public readonly MaterialProperty ParallaxTexWrapMode;
            public readonly MaterialProperty ParallaxTexChannel;
            public readonly MaterialProperty ParallaxTexUVAutoOffset;
            public readonly MaterialProperty ParallaxTexUVTileController;
            public readonly MaterialProperty ParallaxIntensity;
            public readonly MaterialProperty ParallaxIntensityController;
            public readonly MaterialProperty HeightSampleSteps;
            public readonly MaterialProperty ParallaxNoiseStepTex;
            public readonly MaterialProperty ParallaxNoiseStepTexWrapMode;
            public readonly MaterialProperty ParallaxNoiseTexChannel;
            public readonly MaterialProperty ParallaxNoiseIntensity;
            public readonly MaterialProperty ParallaxImpactTarget;

            // Fresnel
            public readonly MaterialProperty ActiveFresnel;
            public readonly MaterialProperty FresnelFunction;
            public readonly MaterialProperty FresnelRange;
            public readonly MaterialProperty FresnelRangeController;
            public readonly MaterialProperty FresnelPower;
            public readonly MaterialProperty FresnelPowerController;
            public readonly MaterialProperty FresnelColor;
            public readonly MaterialProperty FlipFresnelColorRange;
            public readonly MaterialProperty FlipFresnelAlphaRange;
            public readonly MaterialProperty FlipInnerFaceNormal;

            // Vertex-Offset
            public readonly MaterialProperty ActiveVertexOffset;
            public readonly MaterialProperty VertexOffsetTexUVChannel;
            public readonly MaterialProperty VertexOffsetTex;
            public readonly MaterialProperty VertexOffsetTexWrapMode;
            public readonly MaterialProperty VertexOffsetTexUVAutoOffset;
            public readonly MaterialProperty VertexOffsetTexUVTileController;
            public readonly MaterialProperty VertexOffsetTexSampleChannels;
            public readonly MaterialProperty VertexOffsetDirection;
            public readonly MaterialProperty VertexOffsetIntensity;

            public readonly MaterialProperty VertexOffsetIntensityController;

            // Vertex-Offset (Mask)
            public readonly MaterialProperty ActiveVertexOffsetMask;
            public readonly MaterialProperty VertexOffsetMask;
            public readonly MaterialProperty VertexOffsetMaskWrapMode;
            public readonly MaterialProperty VertexOffsetMaskUVAutoOffset;
            public readonly MaterialProperty VertexOffsetMaskUVTileController;
            public readonly MaterialProperty VertexOffsetMaskSampleChannels;
            public readonly MaterialProperty VertexOffsetMaskIntensity;

            public readonly MaterialProperty VertexOffsetMaskIntensityController;

            // Vertex-Offset (Animation Map)
            public readonly MaterialProperty AnimationPlayableSlider;
            public readonly MaterialProperty AnimationPlayableSliderController;
            public readonly MaterialProperty AnimationTexUVChannel;
            public readonly MaterialProperty AnimationNormalOffsetTex;
            public readonly MaterialProperty AnimationVertexVectorMode;
            public readonly MaterialProperty ReClampAnimationVertexVector;
            public readonly MaterialProperty VertexOffsetCameraDistance;
            public readonly MaterialProperty VertexOffsetCameraDistanceController;

            // Double-Side Color
            public readonly MaterialProperty ActiveSideFaceColor;
            public readonly MaterialProperty SideColor;
            public readonly MaterialProperty ChangeSide;

            // Soft-Particle
            public readonly MaterialProperty ActiveSoftParticle;
            public readonly MaterialProperty SoftRange;

            // Light-Illumination
            public readonly MaterialProperty ActiveLightSystem;
            public readonly MaterialProperty MainLightIntensity;
            public readonly MaterialProperty MainLightBounceRange;
            public readonly MaterialProperty AdditionalLightIntensity;
            public readonly MaterialProperty AdditionalBounceRange;

            // External Alpha Template
            public readonly MaterialProperty ActiveExternalAlpha;
            public readonly MaterialProperty LineAlphaMode;
            public readonly MaterialProperty InverseAlphaVal;
            public readonly MaterialProperty ExternalAlphaRange;
            public readonly MaterialProperty ExternalAlphaRangeController;
            public readonly MaterialProperty ExternalAlphaPower;
            public readonly MaterialProperty ExternalAlphaPowerController;
            public readonly MaterialProperty ExternalAlphaIntensity;
            public readonly MaterialProperty ExternalAlphaIntensityController;

            // Alpha-Clip
            public readonly MaterialProperty ActiveAlphaClip;
            public readonly MaterialProperty AlphaClipRange;
            public readonly MaterialProperty AlphaClipRangeController;


            // Color Adjustment
            public readonly MaterialProperty ActiveColorAdjustment;
            public readonly MaterialProperty ColorAdjustmentLayerMask;
            public readonly MaterialProperty HUEShiftVal;
            public readonly MaterialProperty HUEShiftValController;
            public readonly MaterialProperty SaturationVal;
            public readonly MaterialProperty SaturationValController;
            public readonly MaterialProperty ContrastVal;
            public readonly MaterialProperty ContrastValController;
            public readonly MaterialProperty AntiShineGlowVal;
            public readonly MaterialProperty AntiShineGlowValController;

            // FinalColor & FinalAlpha Multiplier
            public readonly MaterialProperty FinalColorIntensity;
            public readonly MaterialProperty FinalColorIntensityController;
            public readonly MaterialProperty FinalAlphaIntensity;
            public readonly MaterialProperty FinalAlphaIntensityController;

            // Receive Shadow
            public readonly MaterialProperty ReceiveShadow;

            // Blend Setting
            public readonly MaterialProperty RenderMode;
            public readonly MaterialProperty SourceBlend;
            public readonly MaterialProperty DestBlend;
            public readonly MaterialProperty Cull;
            public readonly MaterialProperty ZWrite;
            public readonly MaterialProperty ZTest;

            // Stencil Setting
            public readonly MaterialProperty Ref;
            public readonly MaterialProperty Comp;
            public readonly MaterialProperty Pass;
            public readonly MaterialProperty Fail;
            public readonly MaterialProperty ZFail;

            // GrabPass 
            public readonly MaterialProperty GrabPassTexMode;
            public readonly MaterialProperty DistortCenterUV;
            public readonly MaterialProperty GrabPassIntensity;
            public readonly MaterialProperty GrabPassIntensityController;

            //Decal
            public readonly MaterialProperty DecalRenderMode;
            public readonly MaterialProperty ActiveDecalWorldNormalMask;
            public readonly MaterialProperty DecalWorldNormalMaskAxis;


            public ParticleProperties(MaterialProperty[] properties)
            {
                // Workflow Mode
                ParticleMode = FindProperty("_ParticleMode", properties);

                // MainTex 
                MainTexMode = FindProperty("_MainTexMode", properties);
                MainTex = FindProperty("_MainTex", properties);
                MainTexColor = FindProperty("_MainTexColor", properties);
                MainTexUVChannel = FindProperty("_MainTexUVChannel", properties);
                MainTexWrapMode = FindProperty("_MainTexWrapMode", properties);
                MainTexUVAutoOffset = FindProperty("_MainTexUVAutoOffset", properties);
                MainTexUVTileController = FindProperty("_MainTexUVTileController", properties);
                ClampMainTexUV = FindProperty("_ClampMainTexUV", properties);
                // MainTex (MultiChannel)
                MainTexG_Intensity = FindProperty("_MainTexG_Intensity", properties);
                MainTexGIntensityController = FindProperty("_MainTexGIntensityController", properties);
                MainTexB_Intensity = FindProperty("_MainTexB_Intensity", properties);
                MainTexBIntensityController = FindProperty("_MainTexBIntensityController", properties);
                // MainTex (FlipBook)
                FlipBookSetting = FindProperty("_FlipBookSetting", properties);

                // Distort
                ActiveDistort = FindProperty("_ActiveDistort", properties);
                DistortTex = FindProperty("_DistortTex", properties);
                DistortTexUVChannel = FindProperty("_DistortTexUVChannel", properties);
                DistortTexWrapMode = FindProperty("_DistortTexWrapMode", properties);
                DistortChannel = FindProperty("_DistortChannel", properties);
                DistortTexUVAutoOffset = FindProperty("_DistortTexUVAutoOffset", properties);
                DistortTexUVTileController = FindProperty("_DistortTexUVTileController", properties);
                DistortIntensity = FindProperty("_DistortIntensity", properties);
                DistortIntensityController = FindProperty("_DistortIntensityController", properties);
                DistortImpactTarget = FindProperty("_DistortImpactTarget", properties);
                // Distort (FlowMap)
                EnableHighPrecisionFlowMap = FindProperty("_EnableHighPrecisionFlowMap", properties);
                ReverseFlowMap_G_Color = FindProperty("_ReverseFlowMap_G_Color", properties);

                // Dissolve
                ActiveDissolve = FindProperty("_ActiveDissolve", properties);
                DissolveTex = FindProperty("_DissolveTex", properties);
                DissolveTexUVChannel = FindProperty("_DissolveTexUVChannel", properties);
                DissolveTexWrapMode = FindProperty("_DissolveTexWrapMode", properties);
                DissolveChannel = FindProperty("_DissolveChannel", properties);
                DissolveTexUVAutoOffset = FindProperty("_DissolveTexUVAutoOffset", properties);
                DissolveTexUVTileController = FindProperty("_DissolveTexUVTileController", properties);
                DissolveCalculateMode = FindProperty("_DissolveCalculateMode", properties);
                DissolveIntensity = FindProperty("_DissolveIntensity", properties);
                DissolveIntensityController = FindProperty("_DissolveIntensityController", properties);
                ActiveHardClipDissolve = FindProperty("_ActiveHardClipDissolve", properties);
                // Dissolve (Rim)
                ActiveRimDissolve = FindProperty("_ActiveRimDissolve", properties);
                RimDissolveRange = FindProperty("_RimDissolveRange", properties);
                RimDissolveRangeController = FindProperty("_RimDissolveRangeController", properties);
                RimDissolveColor = FindProperty("_RimDissolveColor", properties);
                RimDissolveColorMode = FindProperty("_RimDissolveColorMode", properties);

                // Emission
                ActiveEmission = FindProperty("_ActiveEmission", properties);
                EmissionTex = FindProperty("_EmissionTex", properties);
                EmissionTexUVChannel = FindProperty("_EmissionTexUVChannel", properties);
                EmissionTexWrapMode = FindProperty("_EmissionTexWrapMode", properties);
                EmissionTexUVAutoOffset = FindProperty("_EmissionTexUVAutoOffset", properties);
                EmissionTexUVTileController = FindProperty("_EmissionTexUVTileController", properties);
                EmissionIntensity = FindProperty("_EmissionIntensity", properties);
                EmissionIntensityController = FindProperty("_EmissionIntensityController", properties);
                EmissionColorMode = FindProperty("_EmissionColorMode", properties);
                EmissionColor = FindProperty("_EmissionColor", properties);


                // Ramp 
                ActiveRamp = FindProperty("_ActiveRamp", properties);
                RampTex = FindProperty("_RampTex", properties);
                EnhanceRampColor = FindProperty("_EnhanceRampColor", properties);

                // Normal
                ActiveNormal = FindProperty("_ActiveNormal", properties);
                NormalTex = FindProperty("_NormalTex", properties);
                NormalTexUVChannel = FindProperty("_NormalTexUVChannel", properties);
                NormalTexWrapMode = FindProperty("_NormalTexWrapMode", properties);
                NormalTexUVAutoOffset = FindProperty("_NormalTexUVAutoOffset", properties);
                NormalTexUVTileController = FindProperty("_NormalTexUVTileController", properties);
                NormalIntensity = FindProperty("_NormalIntensity", properties);
                NormalIntensityController = FindProperty("_NormalIntensityController", properties);
                NormalBounceLightRange = FindProperty("_NormalBounceLightRange", properties);

                // Parallax 
                ActiveParallax = FindProperty("_ActiveParallax", properties);
                ParallaxTex = FindProperty("_ParallaxTex", properties);
                ParallaxTexUVChannel = FindProperty("_ParallaxTexUVChannel", properties);
                ParallaxTexWrapMode = FindProperty("_ParallaxTexWrapMode", properties);
                ParallaxTexChannel = FindProperty("_ParallaxTexChannel", properties);
                ParallaxTexUVAutoOffset = FindProperty("_ParallaxTexUVAutoOffset", properties);
                ParallaxTexUVTileController = FindProperty("_ParallaxTexUVTileController", properties);
                ParallaxIntensity = FindProperty("_ParallaxIntensity", properties);
                ParallaxIntensityController = FindProperty("_ParallaxIntensityController", properties);
                HeightSampleSteps = FindProperty("_HeightSampleSteps", properties);
                ParallaxNoiseStepTex = FindProperty("_ParallaxNoiseStepTex", properties);
                ParallaxNoiseStepTexWrapMode = FindProperty("_ParallaxNoiseStepTexWrapMode", properties);
                ParallaxNoiseTexChannel = FindProperty("_ParallaxNoiseTexChannel", properties);
                ParallaxNoiseIntensity = FindProperty("_ParallaxNoiseIntensity", properties);
                ParallaxImpactTarget = FindProperty("_ParallaxImpactTarget", properties);

                // Fresnel
                ActiveFresnel = FindProperty("_ActiveFresnel", properties);
                FresnelFunction = FindProperty("_FresnelFunction", properties);
                FresnelRange = FindProperty("_FresnelRange", properties);
                FresnelRangeController = FindProperty("_FresnelRangeController", properties);
                FresnelPower = FindProperty("_FresnelPower", properties);
                FresnelPowerController = FindProperty("_FresnelPowerController", properties);
                FresnelColor = FindProperty("_FresnelColor", properties);
                FlipFresnelColorRange = FindProperty("_FlipFresnelColorRange", properties);
                FlipFresnelAlphaRange = FindProperty("_FlipFresnelAlphaRange", properties);
                FlipInnerFaceNormal = FindProperty("_FlipInnerFaceNormal", properties);

                // Vertex-Offset
                ActiveVertexOffset = FindProperty("_ActiveVertexOffset", properties);
                VertexOffsetTexUVChannel = FindProperty("_VertexOffsetTexUVChannel", properties);
                VertexOffsetTex = FindProperty("_VertexOffsetTex", properties);
                VertexOffsetTexWrapMode = FindProperty("_VertexOffsetTexWrapMode", properties);
                VertexOffsetTexUVAutoOffset = FindProperty("_VertexOffsetTexUVAutoOffset", properties);
                VertexOffsetTexUVTileController = FindProperty("_VertexOffsetTexUVTileController", properties);
                VertexOffsetTexSampleChannels = FindProperty("_VertexOffsetTexSampleChannels", properties);
                VertexOffsetDirection = FindProperty("_VertexOffsetDirection", properties);
                VertexOffsetIntensity = FindProperty("_VertexOffsetIntensity", properties);
                VertexOffsetIntensityController = FindProperty("_VertexOffsetIntensityController", properties);
                // Vertex-Offset (Mask)
                ActiveVertexOffsetMask = FindProperty("_ActiveVertexOffsetMask", properties);
                VertexOffsetMask = FindProperty("_VertexOffsetMask", properties);
                VertexOffsetMaskWrapMode = FindProperty("_VertexOffsetMaskWrapMode", properties);
                VertexOffsetMaskUVAutoOffset = FindProperty("_VertexOffsetMaskUVAutoOffset", properties);
                VertexOffsetMaskUVTileController = FindProperty("_VertexOffsetMaskUVTileController", properties);
                VertexOffsetMaskSampleChannels = FindProperty("_VertexOffsetMaskSampleChannels", properties);
                VertexOffsetMaskIntensity = FindProperty("_VertexOffsetMaskIntensity", properties);
                VertexOffsetMaskIntensityController = FindProperty("_VertexOffsetMaskIntensityController", properties);
                // Vertex-Offset (Animation Map)
                AnimationPlayableSlider = FindProperty("_AnimationPlayableSlider", properties);
                AnimationPlayableSliderController = FindProperty("_AnimationPlayableSliderController", properties);
                AnimationTexUVChannel = FindProperty("_AnimationTexUVChannel", properties);
                AnimationNormalOffsetTex = FindProperty("_AnimationNormalOffsetTex", properties);
                AnimationVertexVectorMode = FindProperty("_AnimationVertexVectorMode", properties);
                ReClampAnimationVertexVector = FindProperty("_ReClampAnimationVertexVector", properties);
                // Vertex-Offset (Camera Z-Axis Offset)
                VertexOffsetCameraDistance = FindProperty("_VertexOffsetCameraDistance", properties);
                VertexOffsetCameraDistanceController = FindProperty("_VertexOffsetCameraDistanceController", properties);

                // Double-Side Color
                ActiveSideFaceColor = FindProperty("_ActiveSideFaceColor", properties);
                SideColor = FindProperty("_SideColor", properties);
                ChangeSide = FindProperty("_ChangeSide", properties);

                // Soft-Particle
                ActiveSoftParticle = FindProperty("_ActiveSoftParticle", properties);
                SoftRange = FindProperty("_SoftRange", properties);

                // Light-Illumination
                ActiveLightSystem = FindProperty("_ActiveLightSystem", properties);
                MainLightIntensity = FindProperty("_MainLightIntensity", properties);
                MainLightBounceRange = FindProperty("_MainLightBounceRange", properties);
                AdditionalLightIntensity = FindProperty("_AdditionalLightIntensity", properties);
                AdditionalBounceRange = FindProperty("_AdditionalBounceRange", properties);

                // External Alpha Template
                ActiveExternalAlpha = FindProperty("_ActiveExternalAlpha", properties);
                LineAlphaMode = FindProperty("_LineAlphaMode", properties);
                InverseAlphaVal = FindProperty("_InverseAlphaVal", properties);
                ExternalAlphaRange = FindProperty("_ExternalAlphaRange", properties);
                ExternalAlphaRangeController = FindProperty("_ExternalAlphaRangeController", properties);
                ExternalAlphaPower = FindProperty("_ExternalAlphaPower", properties);
                ExternalAlphaPowerController = FindProperty("_ExternalAlphaPowerController", properties);
                ExternalAlphaIntensity = FindProperty("_ExternalAlphaIntensity", properties);
                ExternalAlphaIntensityController = FindProperty("_ExternalAlphaIntensityController", properties);

                // Alpha-Clip
                ActiveAlphaClip = FindProperty("_ActiveAlphaClip", properties);
                AlphaClipRange = FindProperty("_AlphaClipRange", properties);
                AlphaClipRangeController = FindProperty("_AlphaClipRangeController", properties);

                // Color Adjustment
                ActiveColorAdjustment = FindProperty("_ActiveColorAdjustment", properties);
                ColorAdjustmentLayerMask = FindProperty("_ColorAdjustmentLayerMask", properties);
                HUEShiftVal = FindProperty("_HUEShiftVal", properties);
                HUEShiftValController = FindProperty("_HUEShiftValController", properties);
                SaturationVal = FindProperty("_SaturationVal", properties);
                SaturationValController = FindProperty("_SaturationValController", properties);
                ContrastVal = FindProperty("_ContrastVal", properties);
                ContrastValController = FindProperty("_ContrastValController", properties);
                AntiShineGlowVal = FindProperty("_AntiShineGlowVal", properties);
                AntiShineGlowValController = FindProperty("_AntiShineGlowValController", properties);

                // FinalColor & FinalAlpha Multiplier
                FinalColorIntensity = FindProperty("_FinalColorIntensity", properties);
                FinalColorIntensityController = FindProperty("_FinalColorIntensityController", properties);
                FinalAlphaIntensity = FindProperty("_FinalAlphaIntensity", properties);
                FinalAlphaIntensityController = FindProperty("_FinalAlphaIntensityController", properties);

                // Receive Shadow
                ReceiveShadow = FindProperty("_ReceiveShadow", properties);

                // Blend Setting
                RenderMode = FindProperty("_RenderMode", properties);
                SourceBlend = FindProperty("_SourceBlend", properties);
                DestBlend = FindProperty("_DestBlend", properties);
                Cull = FindProperty("_Cull", properties);
                ZWrite = FindProperty("_ZWrite", properties);
                ZTest = FindProperty("_ZTest", properties);

                // Stencil Setting
                Ref = FindProperty("_Ref", properties);
                Comp = FindProperty("_Comp", properties);
                Pass = FindProperty("_Pass", properties);
                Fail = FindProperty("_Fail", properties);
                ZFail = FindProperty("_ZFail", properties);

                // GrabPass
                GrabPassTexMode = FindProperty("_GrabPassTexMode", properties);
                DistortCenterUV = FindProperty("_DistortCenterUV", properties);
                GrabPassIntensity = FindProperty("_GrabPassIntensity", properties);
                GrabPassIntensityController = FindProperty("_GrabPassIntensityController", properties);

                // Decal
                DecalRenderMode = FindProperty("_DecalRenderMode", properties);
                ActiveDecalWorldNormalMask = FindProperty("_ActiveDecalWorldNormalMask", properties);
                DecalWorldNormalMaskAxis = FindProperty("_DecalWorldNormalMaskAxis", properties);
            }
        }

        #endregion
    }
}