using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

// 确保与UIManager在同一命名空间
public abstract class UIPanel : MonoBehaviour
{
    [SerializeField] protected CanvasGroup canvasGroup;
    [SerializeField] protected Transform contentTransform;
    [SerializeField] protected float fadeDuration = 0.3f;
    [SerializeField] protected float scaleDuration = 0.3f;
    [SerializeField] protected Ease openEase = Ease.OutBack;
    [SerializeField] protected Ease closeEase = Ease.InBack;
    [SerializeField] protected bool useScaleAnimation = true;
    [SerializeField] protected bool useFadeAnimation = true;
    [SerializeField] protected bool pauseGameWhenOpen = false;
    [SerializeField] protected bool blockRaycastsWhenClosed = true;

    protected bool isOpen = false;
    protected bool isAnimating = false;
    protected int originalSortOrder;
    protected Sequence currentTween;

    public bool IsOpen => isOpen;
    public bool IsAnimating => isAnimating;
    public string PanelName => GetType().Name;

    protected virtual void Awake()
    {
        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>() ?? gameObject.AddComponent<CanvasGroup>();

        if (contentTransform == null)
            contentTransform = transform;

        // originalSortOrder = GetComponent<Canvas>()?.sortingOrder ?? 0;

        if (!isOpen)
        {
            canvasGroup.alpha = 0;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
            contentTransform.localScale = Vector3.one * 0.8f;
        }
    }

    public virtual void Open(object param = null)
    {
        if (isOpen || isAnimating) return;

        isAnimating = true;
        OnBeforeOpen(param);

        gameObject.SetActive(true);
        UIManager.Instance?.RegisterPanel(this);

        if (pauseGameWhenOpen)
            Time.timeScale = 0;

        PlayOpenAnimation(() =>
        {
            isOpen = true;
            isAnimating = false;
            OnAfterOpen();
        });
    }

    public virtual void Close(bool destroy = false)
    {
        if (!isOpen || isAnimating) return;

        isAnimating = true;
        OnBeforeClose();

        PlayCloseAnimation(() =>
        {
            isOpen = false;
            isAnimating = false;

            if (pauseGameWhenOpen)
                Time.timeScale = 1;

            OnAfterClose();
            UIManager.Instance?.UnregisterPanel(this);

            if (destroy)
                Destroy(gameObject);
            else
                Debug.Log("UI Closed without destroying"); ;
                //gameObject.SetActive(false);
        });
    }

    protected virtual void PlayOpenAnimation(TweenCallback onComplete)
    {
        currentTween?.Kill();

        Sequence sequence = DOTween.Sequence().SetUpdate(true);

        if (useFadeAnimation)
        {
            canvasGroup.alpha = 0;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
            sequence.Append(canvasGroup.DOFade(1, fadeDuration).SetEase(Ease.Linear));
        }
        else
        {
            canvasGroup.alpha = 1;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }

        if (useScaleAnimation && contentTransform != null)
        {
            contentTransform.localScale = Vector3.one * 0.8f;
            sequence.Join(contentTransform.DOScale(1, scaleDuration).SetEase(openEase));
        }

        sequence.OnComplete(onComplete);
        currentTween = sequence;
    }

    protected virtual void PlayCloseAnimation(TweenCallback onComplete)
    {
        currentTween?.Kill();

        Sequence sequence = DOTween.Sequence().SetUpdate(true);

        if (useScaleAnimation)
        {
            sequence.Append(contentTransform.DOScale(0.8f, scaleDuration).SetEase(closeEase));
        }

        if (useFadeAnimation)
        {
            canvasGroup.interactable = false;
            sequence.Join(canvasGroup.DOFade(0, fadeDuration).SetEase(Ease.Linear));
        }

        sequence.OnComplete(() =>
        {
            canvasGroup.blocksRaycasts = blockRaycastsWhenClosed;
            onComplete?.Invoke();
        });

        currentTween = sequence;
    }

    public virtual void ForceClose()
    {
        currentTween?.Kill();
        isOpen = false;
        isAnimating = false;
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = blockRaycastsWhenClosed;
        contentTransform.localScale = Vector3.one * 0.8f;

        if (pauseGameWhenOpen)
            Time.timeScale = 1;

        gameObject.SetActive(false);
        UIManager.Instance?.UnregisterPanel(this);
    }

    public virtual void SetSortingOrder(int order)
    {
        Canvas canvas = GetComponent<Canvas>();
        if (canvas != null)
            canvas.sortingOrder = order;
    }

    public virtual void ResetSortingOrder()
    {
        SetSortingOrder(originalSortOrder);
    }

    protected virtual void OnBeforeOpen(object param) { }
    protected virtual void OnAfterOpen() { }
    protected virtual void OnBeforeClose() { }
    protected virtual void OnAfterClose() { }

    protected virtual void OnDestroy()
    {
        currentTween?.Kill();
        if (isOpen && pauseGameWhenOpen)
            Time.timeScale = 1;
    }
}
