#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using TMPro;
using System.IO;

/// <summary>
/// CleanWave → [빠른 수정] EventSystem + 한글 폰트
/// EventSystem 추가와 한글 폰트 적용을 한 번에 처리합니다.
/// </summary>
public class QuickFix : EditorWindow
{
    const string TTF_DST   = "Assets/Fonts/KoreanFont.ttf";
    const string SDF_DST   = "Assets/Fonts/KoreanFont SDF.asset";
    const string CHAR_RANGE = "0020-00FF,AC00-D7A3,1100-11FF,3130-318F,A960-A97F,D7B0-D7FF";

    static readonly string[] WIN_FONTS =
    {
        @"C:\Windows\Fonts\malgun.ttf",
        @"C:\Windows\Fonts\malgunbd.ttf",
        @"C:\Windows\Fonts\gulim.ttf",
        @"C:\Windows\Fonts\dotum.ttf",
    };

    [MenuItem("CleanWave/[빠른 수정] EventSystem + 한글 폰트")]
    public static void Run()
    {
        var log = new System.Text.StringBuilder();

        // ── 1. EventSystem ──────────────────────────────
        if (Object.FindFirstObjectByType<EventSystem>() == null)
        {
            var es = new GameObject("EventSystem");
            es.AddComponent<EventSystem>();
            es.AddComponent<StandaloneInputModule>();
            log.AppendLine("✔ EventSystem 추가");
        }
        else
        {
            log.AppendLine("✔ EventSystem 이미 존재");
        }

        // ── 2. 한글 폰트 ────────────────────────────────
        var fontAsset = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(SDF_DST);
        if (fontAsset != null)
        {
            log.AppendLine("✔ 폰트 에셋 이미 존재: " + SDF_DST);
        }
        else
        {
            fontAsset = TryBuildFont(log);
        }

        if (fontAsset != null)
        {
            int n = ApplyFont(fontAsset);
            log.AppendLine($"✔ 폰트 {n}개 텍스트에 적용");
        }
        else
        {
            log.AppendLine("✘ 폰트 에셋 생성 실패 → 수동 단계를 따라주세요");
        }

        // ── 3. 씬 저장 ──────────────────────────────────
        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        EditorSceneManager.SaveScene(SceneManager.GetActiveScene());

        // ── 결과 다이얼로그 ─────────────────────────────
        string manualGuide = fontAsset == null
            ? "\n\n──── 한글 폰트 수동 방법 ────\n" +
              "1. 한글 TTF를 Assets/Fonts/ 에 복사\n" +
              "2. Window → TextMeshPro → Font Asset Creator\n" +
              "3. Source Font: KoreanFont\n" +
              "4. Atlas: 4096×4096\n" +
              "5. Character Set: Unicode Range (Hex)\n" +
              "6. Character Sequence: AC00-D7A3\n" +
              "7. Generate Font Atlas → Save\n" +
              "8. 모든 TMP에 새 에셋 드래그"
            : "";

        EditorUtility.DisplayDialog("빠른 수정 완료",
            log.ToString() + manualGuide, "확인");
    }

    static TMP_FontAsset TryBuildFont(System.Text.StringBuilder log)
    {
        // 시스템 폰트 복사
        string src = null;
        foreach (var f in WIN_FONTS)
            if (File.Exists(f)) { src = f; break; }

        if (src == null)
        {
            log.AppendLine("✘ 시스템 한글 폰트 없음");
            return null;
        }

        Directory.CreateDirectory(Application.dataPath + "/Fonts");
        File.Copy(src, Application.dataPath + "/Fonts/KoreanFont.ttf", true);
        AssetDatabase.Refresh();

        var font = AssetDatabase.LoadAssetAtPath<Font>(TTF_DST);
        if (font == null) { log.AppendLine("✘ 폰트 로드 실패"); return null; }

        // Dynamic 폰트 에셋 생성
        var asset = TMP_FontAsset.CreateFontAsset(font);
        if (asset == null) { log.AppendLine("✘ CreateFontAsset 실패"); return null; }

        asset.name = "KoreanFont SDF";
        AssetDatabase.CreateAsset(asset, SDF_DST);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        log.AppendLine("✔ 폰트 에셋 생성: " + SDF_DST);
        return asset;
    }

    static int ApplyFont(TMP_FontAsset fa)
    {
        int n = 0;
        foreach (var t in Object.FindObjectsByType<TextMeshProUGUI>(
            FindObjectsInactive.Include, FindObjectsSortMode.None))
        { Undo.RecordObject(t, "KoreanFont"); t.font = fa; EditorUtility.SetDirty(t); n++; }
        foreach (var t in Object.FindObjectsByType<TextMeshPro>(
            FindObjectsInactive.Include, FindObjectsSortMode.None))
        { Undo.RecordObject(t, "KoreanFont"); t.font = fa; EditorUtility.SetDirty(t); n++; }
        return n;
    }
}
#endif
