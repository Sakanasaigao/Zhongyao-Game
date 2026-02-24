using DIALOGUE;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class ContainItem : MonoBehaviour
{
    [SerializeField] private GameObject nameText;
    [SerializeField] private GameObject dialogueLineText;

    public void SetValue(DIALOGUE_LINE rawLine)
    {
        string name = rawLine.speakerData.displayname;
        if (name == DialogueSystem.narratorName)
        {
            name = "ет╟в";
        }
        string dialogueContent = "";

        foreach (var segment in rawLine.dialogueData.segments)
        {
            dialogueContent += segment.dialogue;
        }

        nameText.GetComponent<TextMeshProUGUI>().text = name;
        dialogueLineText.GetComponent<TextMeshProUGUI>().text = dialogueContent;

        FitSize(dialogueLineText.GetComponent<Transform>());
    }

    private void FitSize(Transform line)
    {
        TextMeshProUGUI tmpUI = line.GetComponent<TextMeshProUGUI>();
        RectTransform thisRect = this.GetComponent<RectTransform>();

        if (tmpUI != null && thisRect != null)
        {
            float lineHeight = tmpUI.preferredHeight;
            thisRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, lineHeight);
        }
    }
}
