#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor.SceneManagement;

/// <summary>
/// 메뉴: CleanWave → 배경 이미지 적용
/// Assets/Sprites/Background/bg_map_01.png 를 씬에 적용합니다.
/// </summary>
public class BackgroundSetup : EditorWindow
{
    private const string SPRITE_PATH = "Assets/Sprites/Background/bg_map_01.png";

    [MenuItem("CleanWave/배경 이미지 적용")]
    public static void ApplyBackground()
    {
        // 1. 스프라이트 임포트 설정
        var importer = AssetImporter.GetAtPath(SPRITE_PATH) as TextureImporter;
        if (importer == null)
        {
            EditorUtility.DisplayDialog("오류", $"파일을 찾을 수 없습니다:\n{SPRITE_PATH}", "확인");
            return;
        }

        importer.textureType          = TextureImporterType.Sprite;
        importer.spriteImportMode     = SpriteImportMode.Single;
        importer.filterMode           = FilterMode.Point;
        importer.textureCompression   = TextureImporterCompression.Uncompressed;
        importer.spritePixelsPerUnit  = 22;       // 1448px / 22 ≈ 65.8 유닛 → 맵 폭 64유닛 커버
        importer.mipmapEnabled        = false;
        importer.spritePivot          = new Vector2(0.5f, 0.5f);
        importer.maxTextureSize       = 2048;
        importer.SaveAndReimport();

        AssetDatabase.Refresh();

        // 2. 기존 배경 오브젝트 제거
        var existing = GameObject.Find("Background_Map");
        if (existing != null) Object.DestroyImmediate(existing);

        // 3. 배경 스프라이트 오브젝트 생성
        var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(SPRITE_PATH);
        if (sprite == null)
        {
            Debug.LogError("[Background] 스프라이트 로드 실패: " + SPRITE_PATH);
            return;
        }

        var bgGo = new GameObject("Background_Map");
        var sr = bgGo.AddComponent<SpriteRenderer>();
        sr.sprite       = sprite;
        sr.sortingOrder = -10;   // 모든 오브젝트 뒤

        // 4. 위치: 맵 중앙 (x=32, y=7) — Ground 타일맵 기준
        //    맵: x=0~63(64유닛), y=0~14(15유닛), 중앙 = (32, 7)
        bgGo.transform.position = new Vector3(32f, 7f, 1f);  // z=1 → 카메라보다 뒤

        // 5. Ground/Decoration 타일맵을 투명하게 → 배경 이미지가 보이도록
        ClearGroundTilemaps();

        // 6. 씬 저장
        EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
        EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene());

        Debug.Log("[Background] 배경 이미지 적용 완료! 위치=(32,7), PPU=22, SortingOrder=-10");
        EditorUtility.DisplayDialog("완료", "배경 이미지가 씬에 적용되었습니다!\n\nPlay 버튼으로 확인하세요.", "확인");
    }

    /// <summary>
    /// Ground / Decoration 타일맵의 색깔 더미 타일을 제거해서 배경이 보이게 함
    /// (Wall 타일맵은 충돌 담당이라 유지)
    /// </summary>
    static void ClearGroundTilemaps()
    {
        var allTilemaps = Object.FindObjectsByType<Tilemap>(FindObjectsSortMode.None);
        foreach (var tm in allTilemaps)
        {
            string name = tm.gameObject.name;
            if (name == "Ground" || name == "Decoration")
            {
                tm.ClearAllTiles();
                Debug.Log($"[Background] {name} 타일맵 비움 → 배경 이미지 표시");
            }
        }
    }
}
#endif
