using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// F1/F2/F3 키로 퀴즈 화면 표시. 같은 키 다시 누르면 닫힌다.
/// QuizPanel 3개를 Inspector에서 연결하거나, 자동으로 자식에서 찾는다.
/// </summary>
public class QuizManager : MonoBehaviour
{
    static readonly QuizEntry[] QuizData =
    {
        new QuizEntry(
            "플라스틱 페트병을 올바르게 분리수거하는 방법은?",
            new[] {
                "라벨을 제거하고 압착해서 버린다",
                "뚜껑을 닫아서 그대로 버린다",
                "물로 씻지 않고 바로 버린다",
                "색깔별로 분리해서 버린다"
            },
            0),

        new QuizEntry(
            "다음 중 음식물 쓰레기에 해당하지 않는 것은?",
            new[] {
                "과일 껍질",
                "달걀 껍데기",
                "채소 뿌리",
                "음식물 찌꺼기"
            },
            1),

        new QuizEntry(
            "해양 쓰레기가 바다 생태계에 미치는 영향으로 옳은 것은?",
            new[] {
                "해양 생물이 먹이로 오해해 섭취한다",
                "바다가 더 깨끗해진다",
                "물고기가 더 잘 번식한다",
                "해수 온도를 낮춰준다"
            },
            0)
    };

    [SerializeField] QuizPanel[] panels;   // Inspector에서 직접 연결

    void Start()
    {
        // 연결 안 됐으면 자식에서 자동 탐색
        if (panels == null || panels.Length == 0)
            panels = GetComponentsInChildren<QuizPanel>(true);

        for (int i = 0; i < panels.Length && i < QuizData.Length; i++)
        {
            panels[i].Setup(QuizData[i].Question, QuizData[i].Options, QuizData[i].CorrectIndex);
            panels[i].gameObject.SetActive(false);
        }
    }

    void Update()
    {
        var kb = Keyboard.current;
        if (kb == null) return;

        if (kb.f1Key.wasPressedThisFrame) Toggle(0);
        else if (kb.f2Key.wasPressedThisFrame) Toggle(1);
        else if (kb.f3Key.wasPressedThisFrame) Toggle(2);
    }

    void Toggle(int index)
    {
        if (panels == null || index >= panels.Length) return;
        bool wasActive = panels[index].gameObject.activeSelf;
        foreach (var p in panels)
            p.gameObject.SetActive(false);
        if (!wasActive)
            panels[index].gameObject.SetActive(true);
    }
}

[System.Serializable]
class QuizEntry
{
    public string Question;
    public string[] Options;
    public int CorrectIndex;

    public QuizEntry(string q, string[] opts, int correct)
    {
        Question = q;
        Options = opts;
        CorrectIndex = correct;
    }
}
