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

    // 保存历史数据的列表
    private List<DialogueRecord> historyList = new List<DialogueRecord>();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        // 1. 初始化面板状态
        if(reviewPanel != null) reviewPanel.SetActive(false);

        // ============================================================
        // 【核心修复 保留原样】：游戏一启动，立刻把按钮移出危险区！
        // ============================================================
        if (closeButton != null && reviewPanel != null)
        {
            closeButton.transform.SetParent(reviewPanel.transform, false);
            closeButton.transform.SetAsLastSibling();
            closeButton.onClick.RemoveAllListeners();
            closeButton.onClick.AddListener(CloseReviewPanel);
        }
    }

    /// <summary>
    /// 【外部接口】请在你的对话系统（例如打字机效果结束时）调用此方法
    /// </summary>
    public void AddDialogue(string roleName, string textContent)
    {
        // 1. 存入数据
        DialogueRecord newRecord = new DialogueRecord { name = roleName, content = textContent };
        historyList.Add(newRecord);
        
        Debug.Log($"【回顾系统】已捕获数据：{roleName}: {textContent}");

        // 2. 【新增】如果面板当前是打开的，立刻生成这条 UI，实现实时刷新
        if (reviewPanel.activeSelf)
        {
            GenerateSingleItem(newRecord);
            StartCoroutine(RefreshLayoutAndScroll());
        }
    }

    public void OpenReviewPanel()
    {
        if (reviewPanel == null) return;
        reviewPanel.SetActive(true);

        // ============================================================
        // 2. 清空旧条目（保留你的“免死金牌”逻辑）
        // ============================================================
        List<GameObject> toDestroy = new List<GameObject>();

        foreach (Transform child in contentParent)
        {
            // 只要名字包含 Close 或 Button 就不删
            if (child.name.Contains("Close") || child.name.Contains("Button")) 
            {
                continue; 
            }
            toDestroy.Add(child.gameObject);
        }

        foreach (var obj in toDestroy)
        {
            Destroy(obj);
        }

        // 3. 重新生成所有条目
        foreach (var record in historyList)
        {
            GenerateSingleItem(record);
        }

        // 4. 刷新并滚动到底部
        StartCoroutine(RefreshLayoutAndScroll());
    }

    /// <summary>
    /// 【新增内部方法】统一处理单条生成的逻辑，解决“读不出内容”的问题
    /// </summary>
    private void GenerateSingleItem(DialogueRecord record)
    {
        GameObject newItem = Instantiate(itemPrefab, contentParent);
        newItem.transform.localScale = Vector3.one; // 防止UI缩放变形

        // --- 核心修改：更强壮的查找逻辑 ---
        
        // 1. 尝试通过标准名字查找
        Text nameText = newItem.transform.Find("NameText")?.GetComponent<Text>();
        Text contentText = newItem.transform.Find("ContentText")?.GetComponent<Text>();

        // 2. 【容错兜底】如果找不到标准名字，尝试自动抓取子物体里的 Text 组件
        if (nameText == null || contentText == null)
        {
            Text[] allTexts = newItem.GetComponentsInChildren<Text>();
            if (allTexts.Length >= 2)
            {
                // 假设第一个 Text 是名字，第二个是内容（根据层级顺序）
                if (nameText == null) nameText = allTexts[0];
                if (contentText == null) contentText = allTexts[1];
            }
            else if (allTexts.Length == 1)
            {
                // 如果只有一个 Text，那就只显示内容
                if (contentText == null) contentText = allTexts[0];
            }
        }

        // 3. 赋值
        if (nameText != null) nameText.text = record.name + "："; // 加上冒号
        if (contentText != null) contentText.text = record.content;
    }

    public void CloseReviewPanel()
    {
        if (reviewPanel != null) reviewPanel.SetActive(false);
    }

    IEnumerator RefreshLayoutAndScroll()
    {
        // 等待这一帧 UI 生成完毕
        yield return new WaitForEndOfFrame();
        
        // 强制刷新 Content 的高度
        if (contentParent.TryGetComponent<RectTransform>(out var rect))
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(rect);
        }
        
        // 等下一帧再滚动，确保高度计算正确
        yield return null; 

        // 滚动到底部
        ScrollRect sr = contentParent.GetComponentInParent<ScrollRect>();
        if(sr != null) sr.verticalNormalizedPosition = 0f;
    }

   
}