using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TrashSortUI : MonoBehaviour
{
    public static TrashSortUI Instance { get; private set; }

    [SerializeField] Text messageText;

    Coroutine hideCoroutine;

    void Awake()
    {
        Instance = this;
        if (messageText != null)
            messageText.gameObject.SetActive(false);
    }

    public void ShowMessage(string msg, Color color, float duration = 2f)
    {
        if (messageText == null) return;
        messageText.text = msg;
        messageText.color = color;
        messageText.gameObject.SetActive(true);
        if (hideCoroutine != null) StopCoroutine(hideCoroutine);
        hideCoroutine = StartCoroutine(HideAfter(duration));
    }

    IEnumerator HideAfter(float t)
    {
        yield return new WaitForSeconds(t);
        if (messageText != null)
            messageText.gameObject.SetActive(false);
    }
}
