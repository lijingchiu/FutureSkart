using UnityEngine;
using UnityEditor;
using System.IO;

public class CreateEffectMaterial : Editor
{
    // 設定選單路徑，這裡設為 Hierarchy 右鍵選單，方便操作
    // Priority 10 讓它出現在比較靠上的位置
    [MenuItem("GameObject/Effects/Create Uber Material", false, 10)]
    static void CreateMaterialFromSelection()
    {
        // 1. 獲取當前選中的物件
        GameObject selectedObj = Selection.activeGameObject;

        if (selectedObj == null)
        {
            Debug.LogWarning("請先在 Hierarchy 中選中一個物件！");
            return;
        }

        // 2. 處理命名邏輯
        string originalName = selectedObj.name;
        string finalName;

        if (originalName.StartsWith("FX_"))
        {
            // 規則 1: 若以 FX_ 開頭，改為 M_
            finalName = originalName.Replace("FX_", "M_");
        }
        else
        {
            // 額外補充: 若不是 FX_ 開頭，為了保持特效命名一致性，也建議補上 M_
            finalName = "M_" + originalName;
        }

        // 3. 設定並檢查目標路徑
        string targetDir = "Assets/OutSouring_Effect/Material";
        
        // 如果資料夾不存在，則建立它
        if (!Directory.Exists(targetDir))
        {
            Directory.CreateDirectory(targetDir);
            AssetDatabase.Refresh(); // 刷新讓 Unity 知道有新資料夾
        }

        // 4. 尋找 Shader
        string shaderName = "UberParticleShader";
        Shader targetShader = Shader.Find(shaderName);

        if (targetShader == null)
        {
            Debug.LogError($"找不到名為 '{shaderName}' 的 Shader，請確認 Shader 名稱是否正確或已導入專案。");
            // 若找不到，使用標準 Shader 作為備案，避免流程中斷 (可選)
            targetShader = Shader.Find("Standard");
        }

        // 5. 建立材質球
        Material newMat = new Material(targetShader);
        
        // 組合完整路徑
        string fullPath = $"{targetDir}/{finalName}.mat";

        // 確保檔名唯一 (避免覆蓋舊檔案)
        fullPath = AssetDatabase.GenerateUniqueAssetPath(fullPath);

        // 6. 寫入檔案
        AssetDatabase.CreateAsset(newMat, fullPath);
        AssetDatabase.SaveAssets();

        // =========================================================
        // 7. 新增功能：將材質球應用回選中物件
        // =========================================================
        // 獲取物件上的 Renderer (包含 ParticleSystemRenderer, MeshRenderer 等)
        Renderer rend = selectedObj.GetComponent<Renderer>();

        if (rend != null)
        {
            // 加入 Undo 紀錄，讓這個操作可以被 Ctrl+Z 復原
            Undo.RecordObject(rend, "Assign Created Material");

            // 在 Editor 腳本中，修改材質建議使用 sharedMaterial 避免產生 Instance 洩漏
            rend.sharedMaterial = newMat;
            
            Debug.Log($"<color=#00FFFF>已將新材質自動賦予給：{selectedObj.name}</color>");
        }
        else
        {
            Debug.LogWarning($"物件 {selectedObj.name} 身上找不到 Renderer 組件，無法自動賦予材質。");
        }
        // =========================================================

        // 8. 用戶體驗優化：建立後自動選中新材質並高亮顯示
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = newMat;
        EditorGUIUtility.PingObject(newMat);

        Debug.Log($"<color=#00FF00>成功建立材質球：</color> {fullPath}");
    }
}
