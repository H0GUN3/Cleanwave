using System;
using System.Collections;
using UnityEngine;

public class GateSpriteAnimator : MonoBehaviour
{
    [SerializeField] SpriteRenderer targetRenderer;
    [SerializeField] Sprite[] openFrames;
    [SerializeField] float frameSeconds = 0.12f;

    Coroutine runningAnimation;
    Sprite closedFrame;

    void Awake()
    {
        if (targetRenderer == null)
            targetRenderer = GetComponentInChildren<SpriteRenderer>();

        if (targetRenderer != null)
            closedFrame = targetRenderer.sprite;
    }

    public void ShowClosed()
    {
        StopRunningAnimation();

        if (targetRenderer != null)
        {
            targetRenderer.gameObject.SetActive(true);
            targetRenderer.sprite = closedFrame;
        }
    }

    public void ShowOpen()
    {
        StopRunningAnimation();
        ShowLastOpenFrame();
    }

    public void PlayOpen(Action onComplete)
    {
        StopRunningAnimation();

        if (targetRenderer == null || openFrames == null || openFrames.Length == 0)
        {
            onComplete?.Invoke();
            return;
        }

        targetRenderer.gameObject.SetActive(true);
        runningAnimation = StartCoroutine(PlayOpenRoutine(onComplete));
    }

    IEnumerator PlayOpenRoutine(Action onComplete)
    {
        foreach (Sprite frame in openFrames)
        {
            if (frame != null)
                targetRenderer.sprite = frame;

            yield return new WaitForSeconds(frameSeconds);
        }

        ShowLastOpenFrame();
        runningAnimation = null;
        onComplete?.Invoke();
    }

    void ShowLastOpenFrame()
    {
        if (targetRenderer == null || openFrames == null || openFrames.Length == 0)
            return;

        Sprite lastFrame = openFrames[openFrames.Length - 1];
        if (lastFrame != null)
        {
            targetRenderer.gameObject.SetActive(true);
            targetRenderer.sprite = lastFrame;
        }
    }

    void StopRunningAnimation()
    {
        if (runningAnimation == null)
            return;

        StopCoroutine(runningAnimation);
        runningAnimation = null;
    }
}
