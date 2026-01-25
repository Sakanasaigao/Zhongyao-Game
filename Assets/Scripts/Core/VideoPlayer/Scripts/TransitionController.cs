using System;
using UnityEngine;

public class TransitionController : MonoBehaviour
{
    [SerializeField] private CanvasGroup transitionCanvasGroup;
    [SerializeField] private AnimationCurve fadeCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
    [SerializeField] private AudioClip transitionSound;
    [SerializeField] private AudioSource audioSource;

    public Action OnTransitionComplete;

    public void CrossFadeIn(float duration)
    {
        StartCoroutine(TransitionCoroutine(0f, 1f, duration));
        PlayTransitionSound();
    }

    public void CrossFadeOut(float duration)
    {
        StartCoroutine(TransitionCoroutine(1f, 0f, duration));
    }

    private System.Collections.IEnumerator TransitionCoroutine(float startAlpha, float endAlpha, float duration)
    {
        float elapsedTime = 0f;
        transitionCanvasGroup.gameObject.SetActive(true);

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / duration;
            float curveValue = fadeCurve.Evaluate(progress);
            transitionCanvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, curveValue);
            yield return null;
        }

        transitionCanvasGroup.alpha = endAlpha;

        if (endAlpha == 0f) transitionCanvasGroup.gameObject.SetActive(false);

        OnTransitionComplete?.Invoke();
    }

    public void PlayTransitionSound()
    {
        if (audioSource != null && transitionSound != null)
        {
            audioSource.PlayOneShot(transitionSound);
        }
    }

    public void SetTransitionSpeed(float speed)
    {
        Time.timeScale = speed;
    }
}
