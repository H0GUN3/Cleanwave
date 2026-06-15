using UnityEngine;
using UnityEngine.UI;

public class ScoreUI : MonoBehaviour
{
    [SerializeField] Sprite[] digitSprites;
    [SerializeField] Image[] digitImages;

    int score;

    void Start() => UpdateDisplay();

    public void AddCoins(int amount)
    {
        score += amount;
        UpdateDisplay();
    }

    void UpdateDisplay()
    {
        string s = score.ToString();
        for (int i = 0; i < digitImages.Length; i++)
        {
            int ci = s.Length - digitImages.Length + i;
            if (ci < 0)
                digitImages[i].gameObject.SetActive(false);
            else
            {
                digitImages[i].gameObject.SetActive(true);
                digitImages[i].sprite = digitSprites[s[ci] - '0'];
            }
        }
    }
}
