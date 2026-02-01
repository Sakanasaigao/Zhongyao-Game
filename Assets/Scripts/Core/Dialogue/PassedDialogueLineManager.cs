using DIALOGUE;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassedDialogueLineManager : MonoBehaviour
{
    public static PassedDialogueLineManager instance { get; private set; }

    public List<DIALOGUE_LINE> dialogueLines = new List<DIALOGUE_LINE>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
            DestroyImmediate(gameObject);
    }

    public void AddLineToPassedLines(DIALOGUE_LINE line)
    {
        dialogueLines.Add(line);
    }
}
