using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

public class TextureCombineTool : EditorWindow
{
    enum TextureChannel
    {
        Red = 0,
        Green = 1,
        Blue = 2,
        Alpha = 3
    };
    enum ChannelColor
    {
        Black = 0,
        White = 1
    };
    enum FileFormat
    {
        TGA = 0,
        PNG = 1,
        JPG = 2,
    };
    enum ChannelAmount
    {
        RGBA,
        RGB
    }

    string outputFilePath = "Assets/ArtRes",
        outputFileName = "NewTexture";
    int outputWidth = 1024,
        outputHeight = 1024;
    Texture2D rTextuer, gTextuer, bTextuer, aTextuer;
    bool rInvertColor, gInvertColor, bInvertColor, aInvertColor, lockWH = true, outputSRGB = false/*, rSmapleSRGB, gSmapleSRGB, bSmapleSRGB, aSmapleSRGB*/;
    ChannelColor
        rChannelColor = ChannelColor.Black,
        gChannelColor = ChannelColor.Black,
        bChannelColor = ChannelColor.Black,
        aChannelColor = ChannelColor.Black;
    TextureChannel rTextureChannel, gTextureChannel, bTextureChannel, aTextureChannel;
    ChannelAmount channelAmount;
    Vector2 scrollPosition;
    FileFormat outputFileFormat = FileFormat.TGA;
    float wh = 1;

    [MenuItem("Tools/Combine Texture Tool")]
    static void Init()
    {
        var toolWindow = GetWindow<TextureCombineTool>();
        toolWindow.titleContent = new GUIContent("Combine Texture Tool");
        toolWindow.Show();
    }

    MonoScript monoScript;
    Shader shader;

    private void OnEnable()
    {
        monoScript = MonoScript.FromScriptableObject(this);
        shader = Shader.Find("Hidden/MaskCombine");
    }



    private void OnGUI()
    {
        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.ObjectField("Script:", monoScript, typeof(MonoScript), false);
        EditorGUILayout.ObjectField("Shader:", shader, typeof(Shader), false);
        EditorGUI.EndDisabledGroup();

        // Translated HelpBoxes
        EditorGUILayout.HelpBox("HDRP Lit Mask Texture\nR: Metallic\nG: Ambient Occlusion\nB: Detail Mask\nA: Smoothness", MessageType.Info);
        EditorGUILayout.HelpBox("URP Lit MS Texture\nR: Metallic\nG: Ambient Occlusion\nB: Black\nA: Smoothness", MessageType.Info);
        EditorGUILayout.Separator();

        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
        PackerPopup("R:", ref rTextuer, ref rChannelColor, ref rTextureChannel/*, ref rSmapleSRGB*/, ref rInvertColor);
        PackerPopup("G:", ref gTextuer, ref gChannelColor, ref gTextureChannel/*, ref gSmapleSRGB*/, ref gInvertColor);
        PackerPopup("B:", ref bTextuer, ref bChannelColor, ref bTextureChannel/*, ref bSmapleSRGB*/, ref bInvertColor);
        if (outputFileFormat != FileFormat.JPG && channelAmount != ChannelAmount.RGB)
        {
            PackerPopup("A:", ref aTextuer, ref aChannelColor, ref aTextureChannel/*, ref aSmapleSRGB*/, ref aInvertColor);
        }
        EditorGUILayout.EndScrollView();


        // Output Settings GUI
        OutputSettingGUI();

        EditorGUILayout.Separator();

        bool oversized = outputWidth >= 16384 || outputHeight >= 16384 || outputWidth < 1 || outputHeight < 1;
        if (oversized)
        {
            Color tempColor = GUI.color;
            GUI.color = Color.red;
            EditorGUILayout.LabelField("Resolution is too large or too small.", EditorStyles.boldLabel);
            GUI.color = tempColor;
        }


        if (!oversized && outputFileName.Trim() != string.Empty && outputFilePath.Trim().ToLower().StartsWith("assets"))
        {
            Color tempColor = GUI.color;
            GUI.color = Color.green;
            if (GUILayout.Button("Combine Mask Texture", GUILayout.Height(40)))
            {
                //SynthesisTextureFile();
                CombineForShader();
            }
            GUI.color = tempColor;
        }

    }

    #region GUI
    FileFormat oldOutputFileFormat;
    private void OutputSettingGUI()
    {

        float labelWidthBase = EditorGUIUtility.labelWidth;
        EditorGUIUtility.labelWidth = 60;
        EditorGUILayout.LabelField("Output Settings:", EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal();
        EditorGUI.BeginChangeCheck();

        oldOutputFileFormat = outputFileFormat;
        outputFileFormat = (FileFormat)EditorGUILayout.EnumPopup("Format:", outputFileFormat);
        if (EditorGUI.EndChangeCheck())
        {
            if (oldOutputFileFormat == FileFormat.JPG)
            {
                channelAmount = ChannelAmount.RGBA;
            }
        }
        EditorGUI.BeginDisabledGroup(outputFileFormat == FileFormat.JPG);
        channelAmount = (ChannelAmount)EditorGUILayout.EnumPopup("Channels:", channelAmount);
        EditorGUI.EndDisabledGroup();
        if (outputFileFormat == FileFormat.JPG)
        {
            channelAmount = ChannelAmount.RGB;
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUI.BeginChangeCheck();
        EditorGUIUtility.labelWidth = 110; // Adjusted for longer English text
        lockWH = EditorGUILayout.Toggle("Lock Aspect Ratio:", lockWH);
        if (EditorGUI.EndChangeCheck())
        {
            if (lockWH)
            {
                wh = outputWidth / outputHeight;
            }
        }
        EditorGUIUtility.labelWidth = 90;
        outputSRGB = EditorGUILayout.Toggle("Output sRGB:", outputSRGB);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();

        EditorGUI.BeginChangeCheck();
        EditorGUIUtility.labelWidth = 85;
        outputWidth = EditorGUILayout.IntField("Resolution X:", outputWidth);

        if (EditorGUI.EndChangeCheck() && lockWH)
        {
            outputHeight = (int)(outputWidth / wh);
        }

        EditorGUI.BeginChangeCheck();
        EditorGUIUtility.labelWidth = 20;
        outputHeight = EditorGUILayout.IntField("Y:", outputHeight);
        if (GUILayout.Button(EditorGUIUtility.IconContent("d_scrollup"), GUILayout.Width(30)))
        {
            if (outputHeight > 4 && outputWidth > 4)
            {
                outputHeight *= 2;
                outputWidth *= 2;
            }
        }
        if (GUILayout.Button(EditorGUIUtility.IconContent("d_scrolldown"), GUILayout.Width(30)))
        {
            if (outputHeight > 4 && outputWidth > 4)
            {
                outputHeight /= 2;
                outputWidth /= 2;
            }
        }
        if (EditorGUI.EndChangeCheck() && lockWH)
        {
            outputWidth = (int)(outputHeight * wh);
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUIUtility.labelWidth = 90;
        outputFilePath = EditorGUILayout.TextField("Output Folder:", outputFilePath);
        if (GUILayout.Button("Pick Current Path", GUILayout.Width(120)))
        {
            if (Selection.assetGUIDs.Length == 0)
            {
                return;
            }
            string guid = Selection.assetGUIDs[0];
            string path = AssetDatabase.GUIDToAssetPath(guid);
            if (Path.HasExtension(path))
            {
                path = Path.GetDirectoryName(path);
            }

            if (Directory.Exists(path))
            {
                outputFilePath = path;
            }
        }
        EditorGUILayout.EndHorizontal();
        EditorGUIUtility.labelWidth = 90;
        outputFileName = EditorGUILayout.TextField("Output Name:", outputFileName);
        EditorGUIUtility.labelWidth = labelWidthBase;
    }

    /// <summary>
    /// OnGUI Draw Texture Panel
    /// </summary>
    /// <param name="title">Title</param>
    /// <param name="texture">Texture</param>
    /// <param name="color">Color</param>
    /// <param name="samplingChannel">Sampling Channel</param>
    /// <param name="inversionChannel">Inversion Channel</param>
    void PackerPopup(string title, ref Texture2D texture, ref ChannelColor color, ref TextureChannel samplingChannel/*, ref bool sampleSRGB*/, ref bool inversionChannel)
    {
        EditorGUILayout.LabelField(title, EditorStyles.boldLabel);

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.BeginVertical();
        //EditorGUI.BeginChangeCheck();
        texture = (Texture2D)EditorGUILayout.ObjectField(texture, typeof(Texture2D), false, GUILayout.Width(70), GUILayout.Height(70));
        //if (EditorGUI.EndChangeCheck())
        //{
        //    if (texture)
        //    {
        //        TextureImporter textureImporter = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(texture)) as TextureImporter;
        //        sampleSRGB = textureImporter.sRGBTexture;
        //    }
        //}
        EditorGUILayout.EndVertical();
        if (texture == null)
        {
            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField("Color:", EditorStyles.boldLabel, GUILayout.Width(70));
            color = (ChannelColor)EditorGUILayout.EnumPopup(color, GUILayout.Width(70));
            EditorGUILayout.EndVertical();
        }
        else
        {
            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField("Sample Ch:", EditorStyles.boldLabel, GUILayout.Width(75)); // Shortened to fit
            samplingChannel = (TextureChannel)EditorGUILayout.EnumPopup(samplingChannel, GUILayout.Width(70));
            EditorGUILayout.EndVertical();

            //EditorGUILayout.BeginVertical();
            //EditorGUILayout.LabelField("Sample SRGB:", EditorStyles.boldLabel, GUILayout.Width(70));
            //sampleSRGB = EditorGUILayout.Toggle(sampleSRGB, GUILayout.Width(70));
            //EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField("Invert:", EditorStyles.boldLabel, GUILayout.Width(70));
            inversionChannel = EditorGUILayout.Toggle(inversionChannel, GUILayout.Width(70));
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical();
            if (GUILayout.Button("Pick Tex Path", GUILayout.Width(120)))
            {
                outputFilePath = Path.GetDirectoryName(AssetDatabase.GetAssetPath(texture));
            }
            if (GUILayout.Button("Pick Tex Name", GUILayout.Width(120)))
            {
                outputFileName = texture.name;
            }
            if (GUILayout.Button("Pick Tex Res", GUILayout.Width(120)))
            {
                outputWidth = texture.width;
                outputHeight = texture.height;
            }
            EditorGUILayout.EndVertical();
        }

        EditorGUILayout.EndHorizontal();
    }
    #endregion

    #region TextureSourceInfo Instantiation
    public struct TextureSourceInfo
    {
        public Texture2D texture2D;
        public string texturePath;
        public TextureImporter textureImporter;
        public bool textureSRGB;
        public TextureImporterCompression textureImporterCompression;
    }

    public static TextureSourceInfo CreateTextureInfo(Texture2D texture2D/*, bool sampleSRGB*/)
    {
        TextureSourceInfo textureInfo = new TextureSourceInfo();

        if (!texture2D)
        {
            return textureInfo;
        }
        textureInfo.texture2D = texture2D;
        textureInfo.texturePath = AssetDatabase.GetAssetPath(texture2D);
        textureInfo.textureImporter = AssetImporter.GetAtPath(textureInfo.texturePath) as TextureImporter;
        textureInfo.textureSRGB = textureInfo.textureImporter.sRGBTexture;
        textureInfo.textureImporterCompression = textureInfo.textureImporter.textureCompression;

        TurnToTextureLinear(textureInfo/*, sampleSRGB*/);

        return textureInfo;
    }
    #endregion

    #region CombineForShader
    private void CombineForShader()
    {
        Material combinerMaterial = new Material(shader);
        combinerMaterial.hideFlags = HideFlags.DontUnloadUnusedAsset;

        TextureSourceInfo rTexInfo = CreateTextureInfo(rTextuer/*, rSmapleSRGB*/);
        TextureSourceInfo gTexInfo = CreateTextureInfo(gTextuer/*, gSmapleSRGB*/);
        TextureSourceInfo bTexInfo = CreateTextureInfo(bTextuer/*, bSmapleSRGB*/);
        TextureSourceInfo aTexInfo = CreateTextureInfo(aTextuer/*, aSmapleSRGB*/);

        combinerMaterial.SetTexture("_RTexture", TurnTexture(rTexInfo, rChannelColor));
        combinerMaterial.SetTexture("_GTexture", TurnTexture(gTexInfo, gChannelColor));
        combinerMaterial.SetTexture("_BTexture", TurnTexture(bTexInfo, bChannelColor));
        combinerMaterial.SetTexture("_ATexture", TurnTexture(aTexInfo, aChannelColor));

        combinerMaterial.SetInt("_RChannel", (int)rTextureChannel);
        combinerMaterial.SetInt("_GChannel", (int)gTextureChannel);
        combinerMaterial.SetInt("_BChannel", (int)bTextureChannel);
        combinerMaterial.SetInt("_AChannel", (int)aTextureChannel);

        combinerMaterial.SetInt("_RInverse", rTextuer == null ? 0 : (rInvertColor ? 1 : 0));
        combinerMaterial.SetInt("_GInverse", gTextuer == null ? 0 : (gInvertColor ? 1 : 0));
        combinerMaterial.SetInt("_BInverse", bTextuer == null ? 0 : (bInvertColor ? 1 : 0));
        combinerMaterial.SetInt("_AInverse", aTextuer == null ? 0 : (aInvertColor ? 1 : 0));



        // Create a RenderTexture
        RenderTexture combinedRT = new RenderTexture(outputWidth, outputHeight, 0, GraphicsFormat.R32G32B32A32_SFloat);

        combinedRT.Create();

        // Set RenderTexture as active
        RenderTexture previousActive = RenderTexture.active;
        RenderTexture.active = combinedRT;

        // Use Graphics.Blit to copy rendering result to RenderTexture
        Graphics.Blit(Texture2D.blackTexture, combinedRT, combinerMaterial);

        // Create a Texture2D and read pixel data from RenderTexture
        Texture2D combined;
        if (channelAmount == ChannelAmount.RGBA)
        {
            combined = new Texture2D(outputWidth, outputHeight, GraphicsFormat.R32G32B32A32_SFloat, TextureCreationFlags.None);
        }
        else
        {
            combined = new Texture2D(outputWidth, outputHeight, GraphicsFormat.R32G32B32_SFloat, TextureCreationFlags.None);
        }
        combined.hideFlags = HideFlags.DontUnloadUnusedAsset;
        combined.ReadPixels(new Rect(0, 0, outputWidth, outputHeight), 0, 0, false);
        combined.Apply();

        // Restore previous RenderTexture setting
        RenderTexture.active = previousActive;

        // Save Texture2D to file
        SaveOutputTexture(combined, outputFilePath, outputFileName, outputFileFormat);

        RevertTextureImporter(rTexInfo);
        RevertTextureImporter(gTexInfo);
        RevertTextureImporter(bTexInfo);
        RevertTextureImporter(aTexInfo);

    }

    static void TurnToTextureLinear(TextureSourceInfo textureInfo/*, bool sRGB*/)
    {
        bool isDirt = false;

        if (textureInfo.textureImporter.sRGBTexture)
        {
            textureInfo.textureImporter.sRGBTexture = false;
            isDirt = true;
        }

        if (textureInfo.textureImporter.textureCompression != TextureImporterCompression.Uncompressed)
        {
            textureInfo.textureImporter.textureCompression = TextureImporterCompression.Uncompressed;
            isDirt = true;
        }
        if (isDirt)
        {
            textureInfo.textureImporter.SaveAndReimport();
        }
    }


    /// <summary>
    /// Output Texture (Return texture if present, otherwise return solid color)
    /// </summary>
    /// <param name="texture">Original texture config</param>
    /// <param name="channelColor">Color selected if no texture present</param>
    /// <returns></returns>
    Texture2D TurnTexture(TextureSourceInfo texture, ChannelColor channelColor)
    {
        Texture2D tex;
        if (texture.texture2D == null)
        {
            switch (channelColor)
            {
                case ChannelColor.Black:
                    tex = Texture2D.blackTexture;
                    break;
                case ChannelColor.White:
                    tex = Texture2D.whiteTexture;
                    break;
                default:
                    tex = Texture2D.blackTexture;
                    break;
            }
        }
        else
        {
            tex = texture.texture2D;
        }
        return tex;
    }

    /// <summary>
    /// Revert Texture Configuration
    /// </summary>
    /// <param name="textureInfo"></param>
    void RevertTextureImporter(TextureSourceInfo textureInfo)
    {
        if (!textureInfo.texture2D)
        {
            return;
        }
        bool isDirt = false;
        if (textureInfo.textureImporter.sRGBTexture != textureInfo.textureSRGB)
        {
            textureInfo.textureImporter.sRGBTexture = textureInfo.textureSRGB;
            isDirt = true;
        }
        if (textureInfo.textureImporter.textureCompression != textureInfo.textureImporterCompression)
        {
            textureInfo.textureImporter.textureCompression = textureInfo.textureImporterCompression;
            isDirt = true;
        }
        if (isDirt)
        {
            textureInfo.textureImporter.SaveAndReimport();
        }
    }
    #endregion

    #region File Save
    private void SaveOutputTexture(Texture2D tex, string path, string name, FileFormat format)
    {
        path += "/";

        string fileSuffix;
        switch (format)
        {
            case FileFormat.PNG:
                fileSuffix = ".png";
                break;
            case FileFormat.TGA:
                fileSuffix = ".tga";
                break;
            case FileFormat.JPG:
                fileSuffix = ".jpg";
                break;
            default:
                fileSuffix = ".tga";
                break;
        }

        path += name + fileSuffix;

        bool replace = true;
        if (File.Exists(path))
        {
            replace = EditorUtility.DisplayDialog("File Already Exists", $"Do you want to replace {name}?", "Replace", "Cancel");
        }

        if (!replace)
        {
            return;
        }
        switch (format)
        {
            case FileFormat.PNG:
                File.WriteAllBytes(path, tex.EncodeToPNG());
                break;
            case FileFormat.TGA:
                File.WriteAllBytes(path, tex.EncodeToTGA());
                break;
            case FileFormat.JPG:
                File.WriteAllBytes(path, tex.EncodeToJPG());
                break;
            default:
                File.WriteAllBytes(path, tex.EncodeToTGA());
                break;
        }

        AssetDatabase.Refresh();

        TextureImporter textureImporter = (TextureImporter)AssetImporter.GetAtPath(path);
        textureImporter.sRGBTexture = outputSRGB;
        textureImporter.SaveAndReimport();

        //AssetDatabase.SaveAssets();
        AssetDatabase.ImportAsset(path);
        AssetDatabase.Refresh();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = AssetDatabase.LoadAssetAtPath(path, typeof(Object));
        EditorGUIUtility.PingObject(Selection.activeObject);
    }

    #endregion

    #region Legacy Methods
    /// <summary>
    /// Generate Texture File
    /// </summary>
    private void SynthesisTextureFile()
    {
        Texture2D _outputTexture = new Texture2D(outputWidth, outputHeight);

        bool ra0 = false, ra1 = false, ra2 = false, ra3 = false;

        if (rTextuer)
        {
            ra0 = SetTextureReadable(rTextuer);
        }
        if (gTextuer)
        {
            ra1 = SetTextureReadable(gTextuer);
        }
        if (bTextuer)
        {
            ra2 = SetTextureReadable(bTextuer);
        }
        if (aTextuer)
        {
            ra3 = SetTextureReadable(aTextuer);
        }

        for (int i = 0; i < outputWidth; i++)
        {
            Vector2 pPos = new Vector2();
            pPos.y = (float)i / (float)outputHeight;
            for (int j = 0; j < outputHeight; j++)
            {
                pPos.x = (float)j / (float)outputWidth;

                float rf = ReadPixelValue(rTextuer, pPos, rChannelColor, rInvertColor, rTextureChannel);
                float gf = ReadPixelValue(gTextuer, pPos, gChannelColor, gInvertColor, gTextureChannel);
                float bf = ReadPixelValue(bTextuer, pPos, bChannelColor, bInvertColor, bTextureChannel);
                float af = ReadPixelValue(aTextuer, pPos, aChannelColor, aInvertColor, aTextureChannel);

                Color pixelColor = new Color(rf, gf, bf, af);

                _outputTexture.SetPixel(j, i, pixelColor);
                EditorUtility.DisplayProgressBar("Generating Progress", "Generating new texture...", (float)(outputHeight * i + j + 1) / (float)(outputWidth * outputHeight));
            }
        }
        _outputTexture.Apply();
        SaveOutputTexture(_outputTexture, outputFilePath, FilterFilename(outputFileName), outputFileFormat);

        if (rTextuer)
        {
            SetTextureReadable(rTextuer, ra0);
            TextureImporter textureImporter = (TextureImporter)TextureImporter.GetAtPath(AssetDatabase.GetAssetPath(rTextuer));
            textureImporter.sRGBTexture = false;
        }

        if (gTextuer)
        {
            SetTextureReadable(gTextuer, ra1);
            TextureImporter textureImporter = (TextureImporter)TextureImporter.GetAtPath(AssetDatabase.GetAssetPath(gTextuer));
            textureImporter.sRGBTexture = true;
        }

        if (bTextuer)
        {
            SetTextureReadable(bTextuer, ra2);
            TextureImporter textureImporter = (TextureImporter)TextureImporter.GetAtPath(AssetDatabase.GetAssetPath(bTextuer));
            textureImporter.sRGBTexture = true;
        }

        if (aTextuer)
        {
            SetTextureReadable(aTextuer, ra3);
            TextureImporter textureImporter = (TextureImporter)TextureImporter.GetAtPath(AssetDatabase.GetAssetPath(aTextuer));
            textureImporter.sRGBTexture = false;
        }

        EditorUtility.ClearProgressBar();
    }
    private string FilterFilename(string name)
    {
        List<char> notAllowedFilenameChars = new List<char>(Path.GetInvalidFileNameChars());
        List<char> filename = new List<char>();

        foreach (char c in name)
        {
            if (!notAllowedFilenameChars.Contains(c))
                filename.Add(c);
        }

        return new string(filename.ToArray());
    }

    private bool SetTextureReadable(Texture2D texture, bool setReadable = true)
    {
        string texturePath = AssetDatabase.GetAssetPath(texture);
        TextureImporter textureImporter = (TextureImporter)TextureImporter.GetAtPath(texturePath);
        bool isReadable = textureImporter.isReadable;
        textureImporter.isReadable = setReadable;
        AssetDatabase.ImportAsset(texturePath);
        AssetDatabase.Refresh();

        return isReadable;
    }

    float ReadPixelValue(Texture2D texture, Vector2 pPos, ChannelColor channelColor, bool inversionChannel, TextureChannel samplingChannel)
    {
        if (texture != null)
        {
            int width, height;
            width = Mathf.RoundToInt((float)texture.width * pPos.x);
            height = Mathf.RoundToInt((float)texture.height * pPos.y);

            Color tempColor = texture.GetPixel(width, height);
            float channel;
            switch (samplingChannel)
            {
                case TextureChannel.Red:
                    channel = tempColor.r;
                    break;
                case TextureChannel.Green:
                    channel = tempColor.g;
                    break;
                case TextureChannel.Blue:
                    channel = tempColor.b;
                    break;
                case TextureChannel.Alpha:
                    channel = tempColor.a;
                    break;
                default:
                    channel = tempColor.r;
                    break;
            }

            if (inversionChannel)
            {
                return 1 - channel;
            }
            else
            {
                return channel;
            }
        }
        else
        {
            switch (channelColor)
            {
                case ChannelColor.Black:
                    return 0;
                case ChannelColor.White:
                    return 1;
                default:
                    return 0;
            }
        }
    }
    #endregion
}
