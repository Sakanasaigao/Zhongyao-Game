using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReviewManager : MonoBehaviour
{
    public static ReviewManager Instance;

    [Header("UI 组件引用")]
    public GameObject reviewPanel;
    public Transform contentParent;
    public GameObject itemPrefab;
    
    [Header("配置按钮")]
    public Button closeButton;

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

        if(reviewPanel != null) reviewPanel.SetActive(false);

        if (closeButton != null && reviewPanel != null)
        {
            closeButton.transform.SetParent(reviewPanel.transform, false);
            closeButton.transform.SetAsLastSibling();
            closeButton.onClick.RemoveAllListeners();
            closeButton.onClick.AddListener(CloseReviewPanel);
        }
    }

    public void AddDialogue(string roleName, string textContent)
    {
        DialogueRecord newRecord = new DialogueRecord { name = roleName, content = textContent };
        historyList.Add(newRecord);
        
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

        List<GameObject> toDestroy = new List<GameObject>();

        foreach (Transform child in contentParent)
        {
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

        foreach (var record in historyList)
        {
            GenerateSingleItem(record);
        }

        StartCoroutine(RefreshLayoutAndScroll());
    }

    private void GenerateSingleItem(DialogueRecord record)
    {
        GameObject newItem = Instantiate(itemPrefab, contentParent);
        newItem.transform.localScale = Vector3.one;

        Text nameText = newItem.transform.Find("NameText")?.GetComponent<Text>();
        Text contentText = newItem.transform.Find("ContentText")?.GetComponent<Text>();

        if (nameText == null || contentText == null)
        {
            Text[] allTexts = newItem.GetComponentsInChildren<Text>();
            if (allTexts.Length >= 2)
            {
                if (nameText == null) nameText = allTexts[0];
                if (contentText == null) contentText = allTexts[1];
            }
            else if (allTexts.Length == 1)
            {
                if (contentText == null) contentText = allTexts[0];
            }
        }

        if (nameText != null) nameText.text = record.name + "：";
        if (contentText != null) contentText.text = record.content;
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
        
        yield return null; 

        ScrollRect sr = contentParent.GetComponentInParent<ScrollRect>();
        if(sr != null) sr.verticalNormalizedPosition = 0f;
    }

   
}