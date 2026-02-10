using System.Collections.Generic;
using UnityEngine;
using UI;

public class MarksManager : MonoBehaviour
{
    public static MarksManager Instance { get; private set; }

    private PlayerManager PlayerManager => PlayerManager.instance;

    [SerializeField]
    private List<GameObject> marks = new();
    [SerializeField]
    private List<int> ints = new();
    [SerializeField]
    private Color highlightColor = Color.cyan;
    [SerializeField]
    private float highlightSpeed = 0.8f;

    private int previousScriptIndex = -1;

    private void Awake()
    {
        Instance = this;
        previousScriptIndex = -1;
    }

    private void Start()
    {
        CheckMarksState();
    }

    public void UpdateMarksState()
    {
        CheckMarksState();
    }

    private void CheckMarksState()
    {
        int playerScriptIndex = PlayerManager.GetScriptIndex();
        Dictionary<GameObject, int> markIndexPair = new();

        for (int i = 0; i < marks.Count; i++)
        {
            markIndexPair.Add(marks[i], ints[i]);
        }

        foreach (var pair in markIndexPair)
        {
            bool shouldActivate = pair.Value <= playerScriptIndex || playerScriptIndex == 0;
            
            if (shouldActivate)
            {
                if (!pair.Key.activeSelf)
                {
                    pair.Key.SetActive(true);
                    if (previousScriptIndex < pair.Value)
                    {
                        ApplyHighlightEffect(pair.Key);
                        if (pair.Key.TryGetComponent(out Mark markComponent))
                        {
                            markComponent.UnlockThisMark();
                        }
                    }
                }
            }
            else
            {
                pair.Key.SetActive(false);
            }
        }

        previousScriptIndex = playerScriptIndex;
    }

    private void ApplyHighlightEffect(GameObject mark)
    {
        if (mark.TryGetComponent(out UI.SimpleHighlightEffect highlight))
        {
            highlight.SetAnimationSpeed(highlightSpeed);
            highlight.StartAnimation();
        }
        else
        {
            UI.SimpleHighlightEffect newHighlight = mark.AddComponent<UI.SimpleHighlightEffect>();
            newHighlight.SetAnimationSpeed(highlightSpeed);
            newHighlight.StartAnimation();
        }
    }

    public void RemoveHighlightEffect(GameObject mark)
    {
        if (mark.TryGetComponent(out UI.SimpleHighlightEffect highlight))
        {
            highlight.StopAnimation();
        }
    }

    public void RemoveAllHighlights()
    {
        foreach (GameObject mark in marks)
        {
            if (mark.activeSelf)
            {
                RemoveHighlightEffect(mark);
            }
        }
    }
}