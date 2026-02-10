using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReviewManager : MonoBehaviour
{
    public static ReviewManager Instance;

    [Header("UI 组件引用")]
    public GameObject reviewPanel;       // 回顾面板（全屏背景）
    public Transform contentParent;      // Scroll View 的 Content
    public GameObject itemPrefab;        // 对话条目预制体
    
    [Header("配置按钮")]
    public Button closeButton;           // 关闭按钮

    [System.Serializable] 
    public class DialogueRecord
    {
        public string name;
        public string content;
    }

    private List<DialogueRecord> historyList = new List<DialogueRecord>();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        // 1. 初始化面板状态
        if(reviewPanel != null) reviewPanel.SetActive(false);

        // ============================================================
        // 【核心修复】：游戏一启动，立刻把按钮移出危险区！
        // ============================================================
        if (closeButton != null && reviewPanel != null)
        {
            // 无论它原本在哪里，强制把它变成 ReviewPanel 的直系子物体
            // 这样它就永远不会出现在 contentParent 里被误删了
            closeButton.transform.SetParent(reviewPanel.transform, false);
            
            // 让它显示在最前面（防止被背景遮挡）
            closeButton.transform.SetAsLastSibling();

            // 重新绑定事件
            closeButton.onClick.RemoveAllListeners();
            closeButton.onClick.AddListener(CloseReviewPanel);
        }
    }

    public void AddDialogue(string roleName, string textContent)
    {
        DialogueRecord newRecord = new DialogueRecord { name = roleName, content = textContent };
        historyList.Add(newRecord);
        Debug.Log($"【回顾系统】记录：{roleName}: {textContent}");
    }

    public void OpenReviewPanel()
    {
        if (reviewPanel == null) return;
        reviewPanel.SetActive(true);

        // ============================================================
        // 2. 清空旧条目（带“免死金牌”的删除逻辑）
        // ============================================================
        List<GameObject> toDestroy = new List<GameObject>();

        foreach (Transform child in contentParent)
        {
            // 【免死金牌】如果名字里包含 "Close" 或者 "Button"，绝对不删！
            if (child.name.Contains("Close") || child.name.Contains("Button")) 
            {
                continue; // 跳过这个物体，去检查下一个
            }

            // 其他的都加入死亡名单
            toDestroy.Add(child.gameObject);
        }

        // 统一处决
        foreach (var obj in toDestroy)
        {
            Destroy(obj);
        }

        // 3. 生成新条目
        foreach (var record in historyList)
        {
            GameObject newItem = Instantiate(itemPrefab, contentParent);
            
            Text nameText = newItem.transform.Find("NameText")?.GetComponent<Text>();
            Text contentText = newItem.transform.Find("ContentText")?.GetComponent<Text>();

            if (nameText != null) nameText.text = record.name;
            if (contentText != null) contentText.text = record.content;
        }

        StartCoroutine(RefreshLayoutAndScroll());
    }

    public void CloseReviewPanel()
    {
        if (reviewPanel != null) reviewPanel.SetActive(false);
    }

    IEnumerator RefreshLayoutAndScroll()
    {
        yield return new WaitForEndOfFrame();
        if (contentParent.TryGetComponent<RectTransform>(out var rect))
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(rect);
        }
        ScrollRect sr = contentParent.GetComponentInParent<ScrollRect>();
        if(sr != null) sr.verticalNormalizedPosition = 0f;
    }

    [Header("测试模式")]
    public bool enableTestKeys = true;
    void Update()
    {
        if (enableTestKeys && Input.GetKeyDown(KeyCode.Space))
        {
            AddDialogue("老者", "此去经年，应是良辰好景虚设。");
            OpenReviewPanel(); 
        }
    }
}