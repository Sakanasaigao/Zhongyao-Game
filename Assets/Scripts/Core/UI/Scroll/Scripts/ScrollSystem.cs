using DG.Tweening;
using DIALOGUE;
using System.Collections.Generic;
using UnityEngine;

public class ScrollSystem : UIPanel
{
    [Header("Reference")]
    [SerializeField] private Transform container;
    [SerializeField] private GameObject containItemPrefab;

    private List<ContainItem> activeItems = new List<ContainItem>();

    protected override void Awake()
    {
        base.Awake();
        if (container == null)
            container = transform;
        if (contentTransform == null)
            contentTransform = container;
    }

    public override void Open(object param = null)
    {
        if (param is List<DIALOGUE_LINE> lines)
        {
            InitializeContainItems(lines);
        }
        else
        {
            InitializeContainItems();
        }
        base.Open(param);
    }

    public void InitializeContainItems()
    {
        List<DIALOGUE_LINE> lines = PassedDialogueLineManager.instance.dialogueLines;
        InitializeContainItems(lines);
    }

    public void InitializeContainItems(List<DIALOGUE_LINE> lines)
    {
        ClearContainer();

        for (int i = 0; i < lines.Count; i++)
        {
            AddItemToContainer(lines[i], i);
        }

        UpdateContentHeight();
    }

    private void ClearContainer()
    {
        foreach (var item in activeItems)
        {
            if (item != null)
                Destroy(item.gameObject);
        }
        activeItems.Clear();

        for (int i = container.childCount - 1; i >= 0; i--)
        {
            Transform child = container.GetChild(i);
            Destroy(child.gameObject);
        }
    }

    private void AddItemToContainer(DIALOGUE_LINE dialogueLine, int index = 0)
    {
        GameObject itemPrefab = Instantiate(containItemPrefab, container);
        ContainItem item = itemPrefab.GetComponent<ContainItem>();

        item.SetValue(dialogueLine);
        activeItems.Add(item);

        if (useScaleAnimation && contentTransform != null)
        {
            itemPrefab.transform.localScale = Vector3.zero;
            itemPrefab.transform.DOScale(1, scaleDuration * 0.5f)
                .SetEase(openEase)
                .SetDelay(index * 0.05f)
                .SetUpdate(true);
        }
    }

    public override void Close(bool destroy = false)
    {
        Sequence closeSequence = DOTween.Sequence().SetUpdate(true);

        for (int i = activeItems.Count - 1; i >= 0; i--)
        {
            if (activeItems[i] != null)
            {
                int index = i;
                closeSequence.Join(activeItems[i].transform.DOScale(0, scaleDuration * 0.3f)
                    .SetEase(closeEase)
                    .SetDelay((activeItems.Count - 1 - index) * 0.02f));
            }
        }

        // closeSequence.OnComplete(() => base.Close(destroy));
        base.Close(destroy);
        closeSequence.Play();
    }

    public void RefreshItems()
    {
        InitializeContainItems();
    }

    public void AddNewItem(DIALOGUE_LINE dialogueLine)
    {
        AddItemToContainer(dialogueLine, activeItems.Count);
        UpdateContentHeight();
    }

    private void UpdateContentHeight()
    {
        if (container == null)
            return;

        RectTransform containerRect = container.GetComponent<RectTransform>();
        if (containerRect == null)
            return;

        float totalHeight = 0;
        float spacing = 30; // Match the spacing in the VerticalLayoutGroup

        foreach (var item in activeItems)
        {
            if (item != null)
            {
                RectTransform itemRect = item.GetComponent<RectTransform>();
                if (itemRect != null)
                {
                    totalHeight += itemRect.rect.height + spacing;
                }
            }
        }

        // Remove the last spacing
        if (totalHeight > spacing)
            totalHeight -= spacing;

        containerRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, totalHeight);
    }

    protected override void OnBeforeClose()
    {
        base.OnBeforeClose();
    }

    protected override void OnAfterClose()
    {
        base.OnAfterClose();
        ClearContainer();
    }
}
