#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CleanWave;
using UnityEngine.SceneManagement;
using System.IO;

/// <summary>
/// CleanWave → Fix BinSelection Popup
/// 기존 씬의 분리수거 팝업을 새 디자인(5개 수거함 버튼)으로 교체합니다.
/// 맵 전체 재구축 없이 팝업만 수정합니다.
/// </summary>
public static class FixBinPopup
{
    [MenuItem("CleanWave/Fix BinSelection Popup")]
    public static void Run()
    {
        var canvas = GameObject.Find("HUD_Canvas");
        if (canvas == null)
        {
            EditorUtility.DisplayDialog("Error",
                "HUD_Canvas를 찾을 수 없습니다. 먼저 맵을 구축하세요.", "OK");
            return;
        }

        // 기존 팝업 관련 오브젝트 삭제
        DestroyIfFound(canvas, "BinSelectOverlay");
        DestroyIfFound(canvas, "BinSelectionUIController");

        // BinSelectionUI 컴포넌트도 정리
        foreach (var old in Object.FindObjectsByType<BinSelectionUI>(
            FindObjectsInactive.Include, FindObjectsSortMode.None))
            Object.DestroyImmediate(old.gameObject);

        // 새 팝업 생성
        var overlay = BuildOverlay(canvas);

        // BinSelectionUIController (항상 활성)
        var ctrlGo = new GameObject("BinSelectionUIController");
        ctrlGo.transform.SetParent(canvas.transform, false);
        ctrlGo.AddComponent<RectTransform>().sizeDelta = Vector2.zero;

        var bsui = ctrlGo.AddComponent<BinSelectionUI>();
        var closeBtn  = overlay.transform.Find("BinSelectPanel/CloseButton")?.GetComponent<Button>();
        var trashTxt  = overlay.transform.Find("BinSelectPanel/TrashNameText")?.GetComponent<TextMeshProUGUI>();
        var trashIcon = overlay.transform.Find("BinSelectPanel/TrashIcon")?.GetComponent<Image>();
        var binCont   = overlay.transform.Find("BinSelectPanel/BinContainer");

        var so = new SerializedObject(bsui);
        so.FindProperty("panel").objectReferenceValue            = overlay;
        so.FindProperty("trashNameText").objectReferenceValue    = trashTxt;
        so.FindProperty("trashIconImage").objectReferenceValue   = trashIcon;
        so.FindProperty("binButtonContainer").objectReferenceValue = binCont;
        so.FindProperty("closeButton").objectReferenceValue      = closeBtn;
        so.ApplyModifiedProperties();

        // PlayerInteractor에 새 BinSelectionUI 연결
        var interactor = Object.FindFirstObjectByType<PlayerInteractor>();
        if (interactor)
        {
            var soI = new SerializedObject(interactor);
            soI.FindProperty("binSelectionUI").objectReferenceValue = bsui;
            soI.ApplyModifiedProperties();
        }

        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        EditorSceneManager.SaveScene(SceneManager.GetActiveScene());

        EditorUtility.DisplayDialog("Fix BinSelection Popup",
            "분리수거 팝업 교체 완료!\nPlay로 확인하세요.", "OK");
    }

    static void DestroyIfFound(GameObject parent, string name)
    {
        var t = parent.transform.Find(name);
        if (t != null) Object.DestroyImmediate(t.gameObject);
    }

    static GameObject BuildOverlay(GameObject canvasParent)
    {
        // 오버레이
        var overlay = new GameObject("BinSelectOverlay");
        overlay.transform.SetParent(canvasParent.transform, false);
        var ovRect = overlay.AddComponent<RectTransform>();
        ovRect.anchorMin = Vector2.zero; ovRect.anchorMax = Vector2.one;
        ovRect.offsetMin = ovRect.offsetMax = Vector2.zero;
        var ovImg = overlay.AddComponent<Image>();
        ovImg.color = new Color(0, 0, 0, 0.55f);

        // 독립 Canvas + GraphicRaycaster → 버튼 클릭 확실히 처리
        var overlayCv = overlay.AddComponent<Canvas>();
        overlayCv.overrideSorting = true;
        overlayCv.sortingOrder = 50;
        overlay.AddComponent<GraphicRaycaster>();

        // 팝업 박스
        var popup = new GameObject("BinSelectPanel");
        popup.transform.SetParent(overlay.transform, false);
        var pRect = popup.AddComponent<RectTransform>();
        pRect.anchorMin = pRect.anchorMax = new Vector2(0.5f, 0.5f);
        pRect.pivot = new Vector2(0.5f, 0.5f);
        pRect.anchoredPosition = Vector2.zero;
        pRect.sizeDelta = new Vector2(760, 420);
        var pImg = popup.AddComponent<Image>();
        pImg.color = new Color(0.1f, 0.1f, 0.15f, 0.97f);

        // 제목
        AddTMP(popup, "TitleText", "Recycling Station",
            new Vector2(0, 170), new Vector2(720, 52), 36, new Color(1f, 0.9f, 0.3f));

        // 쓰레기 아이콘
        var iconGo = new GameObject("TrashIcon");
        iconGo.transform.SetParent(popup.transform, false);
        var iRect = iconGo.AddComponent<RectTransform>();
        iRect.anchorMin = iRect.anchorMax = new Vector2(0.5f, 0.5f);
        iRect.pivot = new Vector2(0.5f, 0.5f);
        iRect.anchoredPosition = new Vector2(-300, 100);
        iRect.sizeDelta = new Vector2(50, 50);
        var iImg = iconGo.AddComponent<Image>();
        iImg.preserveAspect = true;

        // 쓰레기 이름
        var trashTxt = AddTMP(popup, "TrashNameText", "",
            new Vector2(20, 100), new Vector2(580, 44), 28, Color.white);
        trashTxt.alignment = TextAlignmentOptions.Left;

        // 안내 문구
        AddTMP(popup, "PromptText", "Select the correct bin:",
            new Vector2(0, 45), new Vector2(720, 34), 22, new Color(0.8f, 0.8f, 0.8f));

        // 수거함 버튼 컨테이너
        var binCont = new GameObject("BinContainer");
        binCont.transform.SetParent(popup.transform, false);
        var bcRect = binCont.AddComponent<RectTransform>();
        bcRect.anchorMin = bcRect.anchorMax = new Vector2(0.5f, 0.5f);
        bcRect.pivot = new Vector2(0.5f, 0.5f);
        bcRect.anchoredPosition = new Vector2(0, -70);
        bcRect.sizeDelta = new Vector2(720, 165);
        var hLayout = binCont.AddComponent<HorizontalLayoutGroup>();
        hLayout.spacing = 10;
        hLayout.childAlignment = TextAnchor.MiddleCenter;
        hLayout.childForceExpandWidth = false;
        hLayout.childForceExpandHeight = false;

        // 닫기 버튼
        AddButton(popup, "CloseButton", "Close",
            new Vector2(0, -175), new Vector2(160, 50), new Color(0.55f, 0.15f, 0.15f));

        overlay.SetActive(false);
        return overlay;
    }

    static TextMeshProUGUI AddTMP(GameObject parent, string name, string text,
        Vector2 pos, Vector2 size, int fontSize, Color color)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent.transform, false);
        var r = go.AddComponent<RectTransform>();
        r.anchorMin = r.anchorMax = new Vector2(0.5f, 0.5f);
        r.pivot = new Vector2(0.5f, 0.5f);
        r.anchoredPosition = pos; r.sizeDelta = size;
        var t = go.AddComponent<TextMeshProUGUI>();
        t.text = text; t.fontSize = fontSize; t.color = color;
        t.alignment = TextAlignmentOptions.Center;
        t.enableWordWrapping = false;
        return t;
    }

    static void AddButton(GameObject parent, string name, string label,
        Vector2 pos, Vector2 size, Color color)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent.transform, false);
        var r = go.AddComponent<RectTransform>();
        r.anchorMin = r.anchorMax = new Vector2(0.5f, 0.5f);
        r.pivot = new Vector2(0.5f, 0.5f);
        r.anchoredPosition = pos; r.sizeDelta = size;
        var img = go.AddComponent<Image>(); img.color = color;
        var btn = go.AddComponent<Button>(); btn.targetGraphic = img;

        var labelGo = new GameObject("Label");
        labelGo.transform.SetParent(go.transform, false);
        var lr = labelGo.AddComponent<RectTransform>();
        lr.anchorMin = Vector2.zero; lr.anchorMax = Vector2.one;
        lr.offsetMin = lr.offsetMax = Vector2.zero;
        var t = labelGo.AddComponent<TextMeshProUGUI>();
        t.text = label; t.fontSize = 24; t.color = Color.white;
        t.alignment = TextAlignmentOptions.Center;
        t.fontStyle = FontStyles.Bold;
        t.enableWordWrapping = false;
    }
}
#endif
