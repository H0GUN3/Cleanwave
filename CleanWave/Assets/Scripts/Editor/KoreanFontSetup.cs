#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.IO;

public class KoreanFontSetup : EditorWindow
{
    const string TTF_DST = "Assets/Fonts/KoreanFont.ttf";
    const string SDF_DST = "Assets/Fonts/KoreanFont SDF.asset";

    [MenuItem("CleanWave/★ 한글 폰트 적용 ★")]
    public static void Run()
    {
        // 1. 시스템 한글 폰트 찾기
        string src = null;
        string[] candidates = {
            @"C:\Windows\Fonts\malgun.ttf",
            @"C:\Windows\Fonts\malgunbd.ttf",
            @"C:\Windows\Fonts\gulim.ttf",
            @"C:\Windows\Fonts\dotum.ttf",
        };
        foreach (var f in candidates)
            if (File.Exists(f)) { src = f; break; }

        if (src == null)
        {
            EditorUtility.DisplayDialog("오류",
                "한글 폰트 파일을 찾을 수 없습니다.\n" +
                "C:\\Windows\\Fonts\\malgun.ttf 가 필요합니다.", "확인");
            return;
        }

        // 2. Assets/Fonts/ 에 복사
        Directory.CreateDirectory(Application.dataPath + "/Fonts");
        File.Copy(src, Application.dataPath + "/Fonts/KoreanFont.ttf", true);
        AssetDatabase.Refresh();

        // 3. Font 로드
        var font = AssetDatabase.LoadAssetAtPath<Font>(TTF_DST);
        if (font == null)
        {
            EditorUtility.DisplayDialog("오류", "폰트 임포트 실패:\n" + TTF_DST, "확인");
            return;
        }

        // 4. TMP Font Asset 생성 (Dynamic — 한글 런타임 자동 추가)
        var sdfAsset = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(SDF_DST);
        if (sdfAsset == null)
        {
            sdfAsset = TMP_FontAsset.CreateFontAsset(font);
            if (sdfAsset == null)
            {
                EditorUtility.DisplayDialog("오류", "TMP Font Asset 생성 실패", "확인");
                return;
            }
            sdfAsset.name = "KoreanFont SDF";
            AssetDatabase.CreateAsset(sdfAsset, SDF_DST);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        // 5. 씬의 모든 TMP 텍스트에 폰트 적용
        int count = 0;

        var uiTexts = Object.FindObjectsByType<TextMeshProUGUI>(
            FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (var t in uiTexts)
        {
            Undo.RecordObject(t, "Korean Font");
            t.font = sdfAsset;
            EditorUtility.SetDirty(t);
            count++;
        }

        var worldTexts = Object.FindObjectsByType<TextMeshPro>(
            FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (var t in worldTexts)
        {
            Undo.RecordObject(t, "Korean Font");
            t.font = sdfAsset;
            EditorUtility.SetDirty(t);
            count++;
        }

        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        EditorSceneManager.SaveScene(SceneManager.GetActiveScene());

        EditorUtility.DisplayDialog("완료",
            $"한글 폰트 적용 완료!\n적용된 텍스트: {count}개\n\nPlay 버튼으로 확인하세요.", "확인");
    }
}
#endif
