using CleanWave.Core;
using UnityEngine;

public class SortingFeedbackPopup : MonoBehaviour
{
    const float MessageDuration = 2.5f;
    const float MaxMessageWidth = 420f;
    const float HorizontalMargin = 32f;
    const float VerticalPadding = 22f;
    const float TopOffset = 96f;

    static SortingFeedbackPopup instance;

    GUIStyle boxStyle;
    string message;
    float hideAt;

    public static SortingFeedbackPopup EnsureInstance()
    {
        if (instance != null)
            return instance;

        SortingFeedbackPopup found = FindFirstObjectByType<SortingFeedbackPopup>();
        if (found != null)
        {
            instance = found;
            return instance;
        }

        GameObject popupObject = new GameObject("SortingFeedbackPopup");
        return popupObject.AddComponent<SortingFeedbackPopup>();
    }

    public static void ShowMessage(string text)
    {
        EnsureInstance().Show(text);
    }

    public static void ShowCorrectSort(TrashType trashType, BinType binType)
    {
        PlayLogUI.AddSuccessLog($"정답: {TrashSortingUtility.GetKoreanTrashName(trashType)} → {TrashSortingUtility.GetKoreanBinName(binType)}");
    }

    public static void ShowWrongSort(TrashType trashType, BinType expectedBin, BinType actualBin)
    {
        PlayLogUI.AddLog($"오분류: {TrashSortingUtility.GetKoreanTrashName(trashType)} → {TrashSortingUtility.GetKoreanBinName(expectedBin)}");
        PlayLogUI.AddLog("환경 점수 -1");
    }

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    void Show(string text)
    {
        message = text;
        hideAt = Time.time + MessageDuration;
    }

    void OnGUI()
    {
        if (string.IsNullOrEmpty(message) || Time.time > hideAt)
            return;

        if (boxStyle == null)
        {
            boxStyle = new GUIStyle(GUI.skin.box)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 22,
                wordWrap = true,
            };
            boxStyle.normal.textColor = Color.white;
        }

        float width = Mathf.Min(Screen.width - HorizontalMargin * 2f, MaxMessageWidth);
        GUIContent content = new GUIContent(message);
        float textHeight = boxStyle.CalcHeight(content, width);
        float height = Mathf.Max(82f, textHeight + VerticalPadding * 2f);
        Rect rect = new Rect((Screen.width - width) * 0.5f, TopOffset, width, height);
        GUI.Box(rect, message, boxStyle);
    }
}
