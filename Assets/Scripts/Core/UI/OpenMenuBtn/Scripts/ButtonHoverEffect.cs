using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image img;
    [SerializeField] private TextMeshProUGUI txt;
    [SerializeField] private float hoverScale = 1.2f;
    [SerializeField] private float animDuration = 0.3f;
    [SerializeField] private Ease easeType = Ease.OutQuad;

    private RectTransform rectTransform;
    private Outline outline;
    private Vector3 originalScale;
    private Tweener scaleTween;
    private Tweener alphaTween;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        originalScale = rectTransform.localScale;

        if (img != null)
        {
            outline = img.GetComponent<Outline>();
            if (outline != null)
            {
                Color c = outline.effectColor;
                c.a = 0f;
                outline.effectColor = c;
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (scaleTween != null && scaleTween.IsActive()) scaleTween.Kill();
        scaleTween = rectTransform.DOScale(originalScale * hoverScale, animDuration).SetEase(easeType);

        if (outline != null)
        {
            if (alphaTween != null && alphaTween.IsActive()) alphaTween.Kill();
            alphaTween = DOTween.To(() => outline.effectColor.a, x =>
            {
                Color c = outline.effectColor;
                c.a = x;
                outline.effectColor = c;
            }, 1f, animDuration).SetEase(easeType);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (scaleTween != null && scaleTween.IsActive()) scaleTween.Kill();
        scaleTween = rectTransform.DOScale(originalScale, animDuration).SetEase(easeType);

        if (outline != null)
        {
            if (alphaTween != null && alphaTween.IsActive()) alphaTween.Kill();
            alphaTween = DOTween.To(() => outline.effectColor.a, x =>
            {
                Color c = outline.effectColor;
                c.a = x;
                outline.effectColor = c;
            }, 0f, animDuration).SetEase(easeType);
        }
    }

    void OnDestroy()
    {
        if (scaleTween != null && scaleTween.IsActive()) scaleTween.Kill();
        if (alphaTween != null && alphaTween.IsActive()) alphaTween.Kill();
    }
}

