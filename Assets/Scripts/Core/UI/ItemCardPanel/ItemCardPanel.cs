using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ITEMS;

public class ItemCardPanel : UIPanel
{
    [SerializeField] private Transform cardContainer;
    [SerializeField] private GameObject itemCardPrefab;
    [SerializeField] private Button closeButton;
    [SerializeField] private Text titleText;
    [SerializeField] private string panelTitle = "物品卡牌";

    private List<CardContentsLoader> itemCards = new List<CardContentsLoader>();

    protected override void Awake()
    {
        base.Awake();
        if (closeButton != null)
            closeButton.onClick.AddListener(() => Close());
        if (titleText != null)
            titleText.text = panelTitle;
    }

    public override void Open(object param = null)
    {
        ClearCards();
        LoadItems();

        base.Open(param);
    }

    protected override void OnBeforeOpen(object param)
    {
        base.OnBeforeOpen(param);
        ClearCards();
        LoadItems();
    }

    private void LoadItems()
    {
        List<string> items = ItemWarehouse.Instance.GetAllItems();
        foreach (string itemName in items)
        {
            CreateItemCard(itemName);
        }
    }

    private void CreateItemCard(string itemName)
    {
        GameObject cardObj = Instantiate(itemCardPrefab, cardContainer);
        CardContentsLoader itemCard = cardObj.GetComponent<CardContentsLoader>();
        if (itemCard != null)
        {
            itemCards.Add(itemCard);
        }
    }

    private void ClearCards()
    {
        foreach (CardContentsLoader card in itemCards)
        {
            if (card != null)
                Destroy(card.gameObject);
        }
        itemCards.Clear();
    }

    protected override void OnAfterClose()
    {
        base.OnAfterClose();
        ClearCards();
    }

    private void OnItemAdded(string itemName)
    {
        if (isOpen)
        {
            CreateItemCard(itemName);
        }
    }

    private void OnItemRemoved(string itemName)
    {
        if (isOpen)
        {
            CardContentsLoader cardToRemove = itemCards.Find(card => card.ItemName == itemName);
            if (cardToRemove != null)
            {
                itemCards.Remove(cardToRemove);
                Destroy(cardToRemove.gameObject);
            }
        }
    }
}