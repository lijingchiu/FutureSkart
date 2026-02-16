using UnityEngine;
using UnityEditor;
using System.IO;

public class RenameEffectMaterial : Editor
{
    // Priority 12 讓它排在 Duplicate Material 下方
    [MenuItem("GameObject/Effects/Rename Material", false, 12)]
    static void RenameMaterial()
    {
        // 1. 獲取當前選中的物件
        GameObject selectedObj = Selection.activeGameObject;

        if (selectedObj == null)
        {
            Debug.LogWarning("請先在 Hierarchy 中選中一個物件！");
            return;
        }

        // 2. 嘗試獲取物件上的 Renderer
        Renderer rend = selectedObj.GetComponent<Renderer>();

        if (rend == null || rend.sharedMaterial == null)
        {
            Debug.LogError($"物件 '{selectedObj.name}' 上找不到 Renderer 組件，或 Renderer 沒有指派材質球，無法進行重新命名。");
            return;
        }

        // 3. 獲取當前材質球
        Material currentMat = rend.sharedMaterial;
        string currentPath = AssetDatabase.GetAssetPath(currentMat);

        // 檢查材質球是否已存檔（不是運行時產生的實例）
        if (string.IsNullOrEmpty(currentPath))
        {
            Debug.LogError("當前材質球是未存檔的實例，請先將材質球存檔後再進行重新命名。");
            return;
        }

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

        // 5. 設定目標路徑（使用原材質球所在資料夾，或預設路徑）
        string targetDir = Path.GetDirectoryName(currentPath);
        string newPath = $"{targetDir}/{finalName}.mat";
        newPath = AssetDatabase.GenerateUniqueAssetPath(newPath);

        // 6. 執行重新命名（移動資源）
        string result = AssetDatabase.MoveAsset(currentPath, newPath);
        
        if (!string.IsNullOrEmpty(result))
        {
            Debug.LogError($"重新命名失敗：{result}");
            return;
        }

        AssetDatabase.SaveAssets();

        // 7. 重新命名材質球物件本身
        currentMat.name = finalName;

        // 8. 確保 Renderer 仍然引用該材質（MoveAsset 會自動更新引用，但這裡確保名稱同步）
        EditorUtility.SetDirty(rend);

        Debug.Log($"<color=#00FF00>成功重新命名材質：</color> 從 '{Path.GetFileNameWithoutExtension(currentPath)}' 更名為 '{finalName}'");
    }
}