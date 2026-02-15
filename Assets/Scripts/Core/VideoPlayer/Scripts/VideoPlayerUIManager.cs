using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VideoPlayerUIManager : MonoBehaviour
{
    [SerializeField] private CanvasGroup rootCanvasGroup;
    [SerializeField] private RawImage videoDisplay;
    [SerializeField] private CanvasGroup videoCanvasGroup;
    [SerializeField] private CanvasGroup backgroundCanvasGroup;
    [SerializeField] private CanvasGroup UIRootCanvasGroup;
    [SerializeField] private Image skipIcon;
    [SerializeField] private TextMeshProUGUI loadingText;
    [SerializeField] private AspectRatioFitter aspectRatioFitter;

    [SerializeField] private VideoPlayerController videoPlayer;

    private bool uiIsShowing;
    private Coroutine fadeOffCoroutine;
    private Material skipIconMt => skipIcon.material;

    public void HandleSkipIcon(float skipTimeThreshold, float currantTime)
    {
        float progress = currantTime / skipTimeThreshold;

        SetSkipIconProgress(progress);
    }

    private void SetSkipIconProgress(float progress)
    {
        skipIconMt.SetFloat("_Progress", progress);
    }

    public void ShowUITemporarily(float alpha, float showTime, float fadeDuration)
    {
        if (!uiIsShowing)
        {
            uiIsShowing = true;
            UIFadeIn(alpha, fadeDuration);
        }

        if (fadeOffCoroutine != null)
            StopCoroutine(fadeOffCoroutine);

        fadeOffCoroutine = StartCoroutine(UIFadeOffCutDown(showTime, fadeDuration));
    }

    public void UIFadeIn(float targetAlpha, float duration)
    {
        DOVirtual.Float(0f, targetAlpha, duration, (alpha) =>
        {
            UIRootCanvasGroup.alpha = alpha;
        });
    }

    public void UIFadeOff(float duration)
    {
        float startAlpha = UIRootCanvasGroup.alpha;
        DOVirtual.Float(startAlpha, 0f, duration, (alpha) =>
        {
            UIRootCanvasGroup.alpha = alpha;
        });
    }

    private IEnumerator UIFadeOffCutDown(float cutDown, float fadeDuration)
    {
        yield return new WaitForSeconds(cutDown);
        UIFadeOff(fadeDuration);
        uiIsShowing = false;
    }

    public void FadeOutAndDestroyRoot(float duration)
    {
        videoPlayer.volumeFadeOut(duration);
        rootCanvasGroup.DOFade(0f, duration)
            .SetEase(Ease.Flash)
            .OnComplete(() => {
                Destroy(rootCanvasGroup.gameObject);
            });
    }
}
