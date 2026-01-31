using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UI;

public class MarksManager : MonoBehaviour
{
    public static MarksManager instance {  get; private set; }

    private PlayerManager playerManager => PlayerManager.instance;

    [SerializeField]
    private List<GameObject> marks = new List<GameObject>();
    [SerializeField]
    private List<int> ints = new List<int>();
    [SerializeField]
    private Color highlightColor = new Color(0f, 1f, 1f); // 亮蓝色，更醒目
    [SerializeField]
    private float highlightSpeed = 0.8f; // 更快的动画速度

    private int previousScriptIndex = -1;

    private void Awake()
    {
        instance = this;
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
        int playerScriptIndex = playerManager.GetScriptIndex();
        Dictionary<GameObject, int> markIndexPair = new Dictionary<GameObject, int>();

        for (int i = 0; i < marks.Count; i++)
        {
            markIndexPair.Add(marks[i], ints[i]);
        }

        foreach (var pair in markIndexPair)
        {
            // 对于测试目的，当playerScriptIndex为0时，也激活所有标记
            // 这样可以确保在初始状态下就能看到高光效果
            bool shouldActivate = pair.Value <= playerScriptIndex || playerScriptIndex == 0;
            
            if (shouldActivate)
            {
                if (!pair.Key.activeSelf)
                {
                    pair.Key.SetActive(true);
                    // 检查是否是新解锁的标记
                    // 对于第一次运行（previousScriptIndex为-1），所有激活的标记都视为新解锁
                    if (previousScriptIndex < pair.Value)
                    {
                        ApplyHighlightEffect(pair.Key);
                        // 调用Mark组件的UnlockThisMark方法
                        Mark markComponent = pair.Key.GetComponent<Mark>();
                        if (markComponent != null)
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

        // 更新previousScriptIndex为当前值
        // 这样下次调用时，只有新解锁的标记（索引大于当前值）会被识别
        previousScriptIndex = playerScriptIndex;
    }

    private void ApplyHighlightEffect(GameObject mark)
    {
        // 使用SimpleHighlightEffect替代OutlineHighlight2D，确保在构建后也能正常工作
        UI.SimpleHighlightEffect highlight = mark.GetComponent<UI.SimpleHighlightEffect>();
        if (highlight == null)
        {
            highlight = mark.AddComponent<UI.SimpleHighlightEffect>();
        }

        highlight.SetHighlightColor(highlightColor);
        highlight.SetAnimationSpeed(highlightSpeed);
        highlight.StartHighlight();
    }

    public void RemoveHighlightEffect(GameObject mark)
    {
        UI.SimpleHighlightEffect highlight = mark.GetComponent<UI.SimpleHighlightEffect>();
        if (highlight != null)
        {
            highlight.StopHighlight();
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
