using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;
using ITEMS;

public class DecoctHerbSelector : MonoBehaviour
{
    public event Action OnSelectionConfirmed;

    [Inject(Id = "CardHolder")] private HorizontalCardHolder cardHolder;

    private PrescriptionDataSO prescription;
    private List<string> selectedHerbNames = new();

    public PrescriptionDataSO CurrentPrescription => prescription;

    public void Setup(PrescriptionDataSO prescriptionData)
    {
        prescription = prescriptionData;
        selectedHerbNames.Clear();
        BuildAndDisplayHerbCards();
    }

    private void BuildAndDisplayHerbCards()
    {
        var herbList = new List<string>(prescription.requiredHerbs);

        var distractors = ItemWarehouse.Instance.GetRandomItemsExcluding(
            prescription.distractorCount, prescription.requiredHerbs);
        herbList.AddRange(distractors);

        herbList = herbList.OrderBy(_ => UnityEngine.Random.value).ToList();

        cardHolder.LoadSpecificItems(herbList);

        foreach (var card in cardHolder.cards)
        {
            card.SelectEvent.AddListener(OnCardSelected);
        }
    }

    private void OnCardSelected(Card card, bool isSelected)
    {
        if (isSelected)
        {
            if (!selectedHerbNames.Contains(card.ItemName))
                selectedHerbNames.Add(card.ItemName);
        }
        else
        {
            selectedHerbNames.Remove(card.ItemName);
        }
    }

    public bool ValidateSelection()
    {
        var requiredSet = new HashSet<string>(prescription.requiredHerbs);
        var selectedSet = new HashSet<string>(selectedHerbNames);

        if (requiredSet.SetEquals(selectedSet))
        {
            OnSelectionConfirmed?.Invoke();
            return true;
        }
        else
        {
            ResetSelection();
            return false;
        }
    }

    public void ResetSelection()
    {
        selectedHerbNames.Clear();
        foreach (var card in cardHolder.cards)
        {
            card.Deselect();
        }
    }

    public void MoveSelectedCardsToPot()
    {
        cardHolder.MoveSelectedCardToPot();
    }
}