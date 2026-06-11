#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// CleanWave → Remove All Walls
/// 씬에서 투명 벽 오브젝트를 모두 삭제합니다.
/// </summary>
public static class RemoveWallsTool
{
    [MenuItem("CleanWave/Remove All Walls (Current Scene)")]
    public static void Run()
    {
        int removed = 0;

        // "Walls" 루트 오브젝트
        var wallsRoot = GameObject.Find("Walls");
        if (wallsRoot != null)
        {
            Undo.DestroyObjectImmediate(wallsRoot);
            removed++;
        }

        // 이름에 "Border" 또는 "Wall" 포함된 루트 오브젝트
        foreach (var go in SceneManager.GetActiveScene().GetRootGameObjects())
        {
            if (go == null) continue;
            string n = go.name;
            if (n.Contains("Border") || n.Contains("Wall") || n.Contains("River_Water")
                || n.Contains("Ocean") || n.Contains("City_"))
            {
                Undo.DestroyObjectImmediate(go);
                removed++;
            }
        }

        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        EditorSceneManager.SaveScene(SceneManager.GetActiveScene());

        EditorUtility.DisplayDialog("Remove Walls",
            removed > 0
                ? $"Done! {removed} wall object(s) removed."
                : "No wall objects found in scene.",
            "OK");
    }
}
#endif
