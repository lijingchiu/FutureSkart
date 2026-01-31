using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace UberParticleShader.Editor
{
    public static class GradientMapGUI
    {
        public static Texture2D GenerateTextureFromGradient(Gradient gradient, int width = 256, int height = 16)
        {
            var texture = new Texture2D(width, height);
            texture.wrapMode = TextureWrapMode.Clamp;

            for (var i = 0; i < width; i++)
            {
                var t = i / (float)(width - 1);
                for (var j = 0; j < height; j++) texture.SetPixel(i, j, gradient.Evaluate(t));
            }

            texture.Apply();
            return texture;
        }

        public static void SaveTextureAsAsset(Texture2D texture, string path, params Gradient[] gradients)
        {
            // Convert the texture to PNG format
            var pngData = texture.EncodeToPNG();

            if (pngData != null)
            {
                File.WriteAllBytes(path, pngData);
                AssetDatabase.Refresh();

                //Gradient data embedded in userData
                AssetDatabase.ImportAsset(path);

                var assetGradientTex = AssetImporter.GetAtPath(path) as TextureImporter;
                assetGradientTex.mipmapEnabled = false;
                assetGradientTex.userData = GradientToUserData(gradients);
                assetGradientTex.SaveAndReimport();
            }
        }

        public static List<Gradient> GetGradientsFromUserData(string userData)
        {
            var list = new List<Gradient>();

            var split = userData.Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var gradientString in split) list.Add(Deserialize(gradientString));

            return list;
        }

        public static Gradient Deserialize(string serializedGradient)
        {
            // format:
            // gradient:colorKey0,colorTime0,colorKey1,colorTime1,...;alphaKey0,alphaTime0,alphaKey1,alphaTime1...;gradientMode

            var split = serializedGradient.Substring("gradient:".Length).Split(';');
            var colorKeys = new List<GradientColorKey>();
            var alphaKeys = new List<GradientAlphaKey>();

            // parse color keys:
            var colorKeysStr = split[0].Split(',');
            for (var i = 0; i < colorKeysStr.Length; i += 2)
                colorKeys.Add(new GradientColorKey(HexToColor(colorKeysStr[i]), float.Parse(colorKeysStr[i + 1], CultureInfo.InvariantCulture)));

            // parse alpha keys:
            var alphaKeysStr = split[1].Split(',');
            for (var i = 0; i < alphaKeysStr.Length; i += 2)
                alphaKeys.Add(new GradientAlphaKey(float.Parse(alphaKeysStr[i], CultureInfo.InvariantCulture),
                    float.Parse(alphaKeysStr[i + 1], CultureInfo.InvariantCulture)));

            // parse gradient mode:
            var mode = (GradientMode)Enum.Parse(typeof(GradientMode), split[2]);

            var gradient = new Gradient
            {
                colorKeys = colorKeys.ToArray(),
                alphaKeys = alphaKeys.ToArray(),
                mode = mode
            };

            return gradient;
        }

        public static string ConvertAbsolutePathToAssetPath(string absolutePath)
        {
            if (string.IsNullOrEmpty(absolutePath))
                return string.Empty;

            if (absolutePath.StartsWith(Application.dataPath))
            {
                return "Assets" + absolutePath.Substring(Application.dataPath.Length);
            }

            Debug.LogError("The path is outside the Assets folder.");
            return string.Empty;
        }

        public static string ColorToHex(Color32 color)
        {
            var hex = color.r.ToString("X2") + color.g.ToString("X2") + color.b.ToString("X2");
            return hex;
        }

        public static Color HexToColor(string hex)
        {
            var r = byte.Parse(hex.Substring(0, 2), NumberStyles.HexNumber);
            var g = byte.Parse(hex.Substring(2, 2), NumberStyles.HexNumber);
            var b = byte.Parse(hex.Substring(4, 2), NumberStyles.HexNumber);
            return new Color32(r, g, b, 255);
        }

        public static string GradientToUserData(params Gradient[] gradients)
        {
            if (gradients == null || gradients.Length == 0) return null;

            var output = "";
            foreach (var gradient in gradients)
            {
                output += "gradient:";

                for (var i = 0; i < gradient.colorKeys.Length; i++)
                    output += string.Format(CultureInfo.InvariantCulture, "{0},{1},", ColorToHex(gradient.colorKeys[i].color),
                        gradient.colorKeys[i].time);

                output = output.TrimEnd(',');
                output += ";";
                for (var i = 0; i < gradient.alphaKeys.Length; i++)
                    output += string.Format(CultureInfo.InvariantCulture, "{0},{1},", gradient.alphaKeys[i].alpha, gradient.alphaKeys[i].time);

                output = output.TrimEnd(',');
                output += ";" + gradient.mode;
                output += "\n";
            }

            output = output.TrimEnd('\n');

            return output;
        }

        public static void GradientMapDelayedSave(ref Gradient gradientMap, ref string savePath, ref bool canSaveGradientTex)
        {
            Texture2D gradientTex = GenerateTextureFromGradient(gradientMap, 64, 4);
            string textureName = Path.GetFileName(savePath);
            string directory = Path.Combine(Application.dataPath.Remove(Application.dataPath.Length - 7), string.IsNullOrEmpty(savePath) ? "" : savePath.Remove(savePath.Length - textureName.Length));
            string path = EditorUtility.SaveFilePanel("Save GradientMap as PNG", directory, textureName, "png");
            if (string.IsNullOrEmpty(path))
                return;
            path = ConvertAbsolutePathToAssetPath(path);
            canSaveGradientTex = true;
            savePath = path;
            SaveTextureAsAsset(gradientTex, path, gradientMap);
        }

        public static Texture2D SaveGradientMapAndReplaceRampTempTex(ref string savePath)
        {
            return AssetDatabase.LoadAssetAtPath<Texture2D>(savePath);
        }
    }
}