using UnityEngine;
using UnityEditor;
using System.IO;

public class DuplicateEffectMaterial : Editor
{
    // Priority 11 讓它排在剛剛那個腳本的下方
    [MenuItem("GameObject/Effects/Duplicate Material", false, 11)]
    static void DuplicateAndRenameMaterial()
    {
        // 1. 獲取當前選中的物件
        GameObject selectedObj = Selection.activeGameObject;

        if (selectedObj == null)
        {
            Debug.LogWarning("請先在 Hierarchy 中選中一個物件！");
            return;
        }

        // 2. 嘗試獲取物件上的 Renderer (MeshRenderer, ParticleSystemRenderer, TrailRenderer 等)
        Renderer rend = selectedObj.GetComponent<Renderer>();

        if (rend == null || rend.sharedMaterial == null)
        {
            Debug.LogError($"物件 '{selectedObj.name}' 上找不到 Renderer 組件，或 Renderer 沒有指派材質球，無法進行複製。");
            return;
        }

        // 3. 執行複製：基於現有材質球創建一個新實例
        Material sourceMat = rend.sharedMaterial;
        Material newMat = new Material(sourceMat);

        // 4. 處理命名邏輯
        string originalName = selectedObj.name;
        string finalName;

        if (originalName.StartsWith("FX_"))
        {
            finalName = originalName.Replace("FX_", "M_");
        }
        else
        {
            finalName = "M_" + originalName;
        }

        // 5. 設定並檢查目標路徑
        string targetDir = "Assets/OutSouring_Effect/Material";
        
        if (!Directory.Exists(targetDir))
        {
            Directory.CreateDirectory(targetDir);
            AssetDatabase.Refresh();
        }

        // 6. (保留您的註解) 強制設定 _ZWrite 為 0
        // if (newMat.HasProperty("_ZWrite"))
        // {
        //     newMat.SetFloat("_ZWrite", 0);
        // }
        // else
        // {
        //     Debug.LogWarning($"源材質 '{sourceMat.name}' 的 Shader 不包含 '_ZWrite' 屬性，無法設置。");
        // }

        // 7. 存檔
        string fullPath = $"{targetDir}/{finalName}.mat";
        fullPath = AssetDatabase.GenerateUniqueAssetPath(fullPath);

        AssetDatabase.CreateAsset(newMat, fullPath);
        AssetDatabase.SaveAssets();

        // --- 新增功能開始 ---
        
        // 8. 將新材質應用回選中的物件
        // 使用 Undo.RecordObject 讓這個操作可以被 Ctrl+Z 復原
        Undo.RecordObject(rend, "Assign Duplicated Material");
        rend.sharedMaterial = newMat;
        
        // --- 新增功能結束 ---

        // 9. 選中並高亮新材質
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = newMat;
        EditorGUIUtility.PingObject(newMat);

        Debug.Log($"<color=#00FF00>成功複製並替換材質：</color> 從 '{sourceMat.name}' 複製為 '{finalName}'");
    }
}
