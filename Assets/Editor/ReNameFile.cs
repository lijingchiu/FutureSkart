using System;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

public class ReNameFile: EditorWindow
{
    [MenuItem("Tools/ReName File")]
    public static void ShowWindow()
    {
        GetWindow<ReNameFile>("ReNameFile");
    }

    private string ContainsName;
    private string ReplaceName;
    private SelectionMode _selectionMode = SelectionMode.DeepAssets;

// GUI
private Vector2 _scrollView;
    private readonly Color _defaultTextColor = new(0.75f, 0.75f, 0.75f, 1);

    private void OnGUI()
    {
        _scrollView = GUILayout.BeginScrollView(_scrollView);
        {
            DrawHowToUse();
            DrawChangeNameGUI();
        }
        GUILayout.EndScrollView();
    }

    private void DrawHowToUse()
    {
        GUILayout.BeginVertical("Box");
        {
            GUILayout.Label("How To Use", new GUIStyle
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 16,
                normal =
                {
                    textColor = _defaultTextColor
                }
            });

            GUILayout.Label("1. 點選要改名的檔案");
            GUILayout.Label("2. 選擇'物件模式'");
            GUILayout.BeginVertical("helpBox");
            {
                GUILayout.Label("物件模式", EditorStyles.toolbarSearchField);
                GUILayout.Label("Unfiltered : 返回單選中的物件 ");
                GUILayout.Label("TopLevel : 返回選中物件的母物件 ");
                GUILayout.Label("Deep : 返回選中物件及其所有子物件 ");
                GUILayout.Label("ExcludePrefab : 返回選中物件 (排除Prefab) ");
                GUILayout.Label("Editable : 返回選中物件 (排除任何不可編輯的物件) ");
                GUILayout.Label("Assets : 返回選中物件 (只限Assets資料) ");
                GUILayout.Label("DeepAssets : 返回選中物件及其子物件 (只限Assets資料) ");
            }
            GUILayout.EndVertical();
        }
        GUILayout.EndVertical();
    }

    private void DrawChangeNameGUI()
    {
        GUILayout.BeginVertical("Box");
        {
            GUILayout.Label("Re Name", new GUIStyle
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 16,
                normal =
                {
                    textColor = _defaultTextColor
                }
            });

            _selectionMode = (SelectionMode)EditorGUILayout.EnumPopup("選擇物件模式", _selectionMode);
            ContainsName = EditorGUILayout.TextField("要取代的文字", ContainsName);
            ReplaceName = EditorGUILayout.TextField("要替換的文字", ReplaceName);
            if (GUILayout.Button("Replace Name"))
            {
                var objects = Selection.GetFiltered(typeof(Object), _selectionMode);
                var materials = Selection.GetFiltered(typeof(Material), _selectionMode);
                var textures = Selection.GetFiltered(typeof(Texture), _selectionMode);

// Record undo step for these objects's old name.
                //【Note】 This way is nor suitable for Assets data.
Undo.RecordObjects(objects, "Objects");
                Undo.RecordObjects(materials, "Materials");
                Undo.RecordObjects(textures, "Textures");

// GameObjects
foreach (var obj in objects)
                {
                    GameObject castGameObject = obj as GameObject;
                    if (castGameObject != null)
                    {
// If this object have more objects include it,then foreach to rename.
var childOfObj = castGameObject.GetComponentsInChildren<Transform>();
                        foreach (var eachObj in childOfObj)
                        {
                            var eachOldName = eachObj.name;
                            var eachNewName = eachOldName.Replace(ContainsName, ReplaceName);
                            eachObj.name = eachNewName;
                            EditorUtility.SetDirty(eachObj);
                        }

// Check the file is asset or not, if yes then use AssetDataBase to change name.
if (_selectionMode == SelectionMode.Assets|| _selectionMode == SelectionMode.DeepAssets)
                        {
                            var oldName = obj.name;
                            var newName = oldName.Replace(ContainsName, ReplaceName);
                            EditorUtility.SetDirty(AssetDatabase.LoadAssetAtPath<Object>(newName));
                            AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(obj), newName);
                            PrefabUtility.SavePrefabAsset(castGameObject);
                        }
                    }
                }

// Materials
foreach (var mat in materials)
                {
                    var oldName = mat.name;
                    var newName = oldName.Replace(ContainsName, ReplaceName);
                    AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(mat), newName);
                    EditorUtility.SetDirty(mat);
                }

// Textures
foreach (var tex in textures)
                {
                    var oldName = tex.name;
                    var newName = oldName.Replace(ContainsName, ReplaceName);
                    AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(tex), newName);
                    EditorUtility.SetDirty(tex);
                }

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }
        GUILayout.EndVertical();
    }
}