using DIALOGUE;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class ScrollSystem : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private Transform container;
    [SerializeField] private GameObject containItemPrefab;

    public void InitializeContainItems()
    {
        List<DIALOGUE_LINE> lines = PassedDialogueLineManager.instance.dialogueLines;

        for (int i = container.childCount - 1; i >= 0; i--)
        {
            Destroy(container.GetChild(i).gameObject);
        }

        for (int i = 0; i < lines.Count; i++)
        {
            AddItemToContainer(lines[i]);
        }
    }

    private void AddItemToContainer(DIALOGUE_LINE dialogueLine)
    {
        GameObject itemPrefab = Instantiate(containItemPrefab, container);
        ContainItem item = itemPrefab.GetComponent<ContainItem>();

        item.SetValue(dialogueLine);
    }
}
