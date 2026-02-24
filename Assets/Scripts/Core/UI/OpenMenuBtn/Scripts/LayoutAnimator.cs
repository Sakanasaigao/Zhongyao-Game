using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class LayoutAnimator : MonoBehaviour
{
    [SerializeField] private float targetLeft;
    [SerializeField] private float targetSpacing;
    [SerializeField] private float duration = 0.5f;
    [SerializeField] private Ease easeType = Ease.OutQuad;

    private HorizontalLayoutGroup layoutGroup;
    private CanvasGroup canvasGroup;
    private bool isOpened = false;

    private int rawLeftSpace = 25;

    void Start()
    {
        layoutGroup = GetComponent<HorizontalLayoutGroup>();
        canvasGroup = GetComponent<CanvasGroup>();

        float totalWidth = 0f;
        int childCount = 0;

        foreach (RectTransform child in transform)
        {
            totalWidth += child.rect.width;
            childCount++;
        }

        float averageWidth = childCount > 0 ? totalWidth / childCount : 0f;

        layoutGroup.padding.left = rawLeftSpace;
        layoutGroup.spacing = -averageWidth;
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    public void AnimateOpenOrClose()
    {
        if (isOpened)
        {
            AnimateOut();
        }
        else
        {
            AnimateIn();
        }
    }

    public void AnimateIn()
    {
        isOpened = true;
        DOTween.To(() => layoutGroup.padding.left, x => layoutGroup.padding.left = (int)x, targetLeft, duration)
            .SetEase(easeType);

        DOTween.To(() => layoutGroup.spacing, x => layoutGroup.spacing = x, targetSpacing, duration)
            .SetEase(easeType)
            .OnComplete(() =>
            {
                canvasGroup.interactable = true;
                canvasGroup.blocksRaycasts = true;
            });

        canvasGroup.DOFade(1f, duration).SetEase(easeType);
    }

    public void AnimateOut()
    {
        isOpened = false;
        DOTween.To(() => layoutGroup.padding.left, x => layoutGroup.padding.left = (int)x, rawLeftSpace, duration)
            .SetEase(easeType);

        float totalWidth = 0f;
        int childCount = 0;

        foreach (RectTransform child in transform)
        {
            totalWidth += child.rect.width;
            childCount++;
        }

        float averageWidth = childCount > 0 ? totalWidth / childCount : 0f;

        DOTween.To(() => layoutGroup.spacing, x => layoutGroup.spacing = x, -averageWidth, duration)
            .SetEase(easeType)
            .OnComplete(() =>
             {
                 canvasGroup.interactable = false;
                 canvasGroup.blocksRaycasts = false;
             });

        canvasGroup.DOFade(0f, duration).SetEase(easeType);
    }
}

