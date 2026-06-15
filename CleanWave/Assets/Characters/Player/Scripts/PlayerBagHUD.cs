using UnityEngine;

/// <summary>
/// 가방 상태를 화면 좌상단에 텍스트로 표시하는 간단한 HUD.
/// 별도 UI 캔버스 없이 OnGUI로 표시한다 (인벤토리 창 아님).
/// </summary>
[RequireComponent(typeof(PlayerBag))]
public class PlayerBagHUD : MonoBehaviour
{
    PlayerBag bag;
    GUIStyle style;

    void Awake()
    {
        bag = GetComponent<PlayerBag>();
    }

    void OnGUI()
    {
        if (style == null)
        {
            style = new GUIStyle(GUI.skin.label)
            {
                fontSize = 24,
                fontStyle = FontStyle.Bold,
            };
            style.normal.textColor = Color.white;
        }

        string text = $"Trash: {bag.CurrentCount} / {bag.MaxCapacity}";
        if (bag.IsFull)
            text += "  (FULL)";

        // 그림자 + 본문
        GUI.Label(new Rect(17, 13, 320, 40), text, style);
        GUI.Label(new Rect(16, 12, 320, 40), text, style);
    }
}
