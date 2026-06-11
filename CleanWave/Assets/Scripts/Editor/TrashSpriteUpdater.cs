#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using CleanWave;

/// <summary>
/// 메뉴: CleanWave → 쓰레기 스프라이트 갱신
/// 씬의 TrashItem SpriteRenderer에 Assets/Sprites/Trash/Trash/ 스프라이트를 할당합니다.
/// </summary>
public class TrashSpriteUpdater : EditorWindow
{
    const string TRASH_BASE = "Assets/Sprites/Trash/Trash";

    // TrashType enum 순서 일치 (Paper=0, Can=1, Plastic=2, Vinyl=3, Food=4, Net=5, Oil=6)
    static readonly string[] SPRITE_NAMES =
    {
        "Trash_Paper",
        "Trash_Can",
        "Trash_Plastic",
        "Trash_Vinyl",
        "Trash_Food",
        "Trash_Net",
        "Trash_Oil",
    };

    [MenuItem("CleanWave/쓰레기 스프라이트 갱신")]
    public static void Run()
    {
        // 임포트 설정: textureType·spriteMode 만 보정 (PPU는 건드리지 않음)
        EnsureSpriteImport();

        var items = Object.FindObjectsByType<TrashItem>(
            FindObjectsInactive.Include, FindObjectsSortMode.None);

        if (items.Length == 0)
        {
            EditorUtility.DisplayDialog("알림",
                "씬에 TrashItem이 없습니다.\n먼저 맵을 구축하세요.", "확인");
            return;
        }

        int updated = 0, missing = 0;

        foreach (var item in items)
        {
            int typeIdx = (int)item.TrashType;
            if (typeIdx < 0 || typeIdx >= SPRITE_NAMES.Length) continue;

            string path = $"{TRASH_BASE}/{SPRITE_NAMES[typeIdx]}.png";
            var spr = AssetDatabase.LoadAssetAtPath<Sprite>(path);

            if (spr == null)
            {
                Debug.LogWarning($"[TrashSprite] 스프라이트 없음: {path}");
                missing++;
                continue;
            }

            var sr = item.GetComponent<SpriteRenderer>();
            if (sr == null)
            {
                sr = item.gameObject.AddComponent<SpriteRenderer>();
                sr.sortingOrder = 3;
            }

            Undo.RecordObject(sr, "Update Trash Sprite");
            sr.sprite = spr;
            sr.sortingOrder = 3;
            EditorUtility.SetDirty(sr);

            // 콜라이더 반지름도 스프라이트 크기에 맞게 조정 (1024px / PPU)
            var col = item.GetComponent<CircleCollider2D>();
            if (col != null)
            {
                Undo.RecordObject(col, "Update Trash Collider");
                col.radius = spr.bounds.extents.x * 0.6f;  // 스프라이트 반지름의 60%
                EditorUtility.SetDirty(col);
            }

            updated++;
        }

        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        EditorSceneManager.SaveScene(SceneManager.GetActiveScene());

        string msg = $"완료! {updated}개 갱신";
        if (missing > 0) msg += $", {missing}개 파일 없음";
        EditorUtility.DisplayDialog("쓰레기 스프라이트 갱신", msg, "확인");
    }

    static void EnsureSpriteImport()
    {
        bool changed = false;
        foreach (var name in SPRITE_NAMES)
        {
            string path = $"{TRASH_BASE}/{name}.png";
            var imp = AssetImporter.GetAtPath(path) as TextureImporter;
            if (imp == null) continue;

            bool dirty = false;
            if (imp.textureType != TextureImporterType.Sprite)
            { imp.textureType = TextureImporterType.Sprite; dirty = true; }
            if (imp.spriteImportMode != SpriteImportMode.Single)
            { imp.spriteImportMode = SpriteImportMode.Single; dirty = true; }
            if (imp.alphaIsTransparency != true)
            { imp.alphaIsTransparency = true; dirty = true; }
            // PPU는 사용자 설정(900) 그대로 유지

            if (dirty) { imp.SaveAndReimport(); changed = true; }
        }
        if (changed) AssetDatabase.Refresh();
    }
}
#endif
