using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    private static UIManager instance;
    public static UIManager Instance => instance;

    [SerializeField] private Transform panelParent;
    [SerializeField] private int baseSortingOrder = 10;
    [SerializeField] private int orderStep = 10;

    private List<UIPanel> panelStack = new List<UIPanel>();
    private Dictionary<string, UIPanel> panelCache = new Dictionary<string, UIPanel>();

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void RegisterPanel(UIPanel panel)
    {
        if (!panelStack.Contains(panel))
        {
            panelStack.Add(panel);
            UpdatePanelSortingOrders();
        }
    }

    public void UnregisterPanel(UIPanel panel)
    {
        panelStack.Remove(panel);
        UpdatePanelSortingOrders();
    }

    private void UpdatePanelSortingOrders()
    {
        for (int i = 0; i < panelStack.Count; i++)
        {
            panelStack[i].SetSortingOrder(baseSortingOrder + i * orderStep);
        }
    }

    public T OpenPanel<T>(object param = null) where T : UIPanel
    {
        Debug.Log("UIManager OpenPanel");
        string panelName = typeof(T).Name;
        Debug.Log(panelName);

        if (!panelCache.TryGetValue(panelName, out UIPanel panel))
        {
            T prefab = Resources.Load<T>($"Prefabs/UI/Panels/{panelName}");
            if (prefab != null)
            {
                panel = Instantiate(prefab, panelParent);
                panelCache[panelName] = panel;
            }
        }

        if (panel != null)
        {
            panel.Open(param);
            return panel as T;
        }

        return null;
    }

    public void ClosePanel<T>() where T : UIPanel
    {
        string panelName = typeof(T).Name;
        if (panelCache.TryGetValue(panelName, out UIPanel panel))
        {
            panel.Close();
        }
    }

    public void CloseAllPanels()
    {
        for (int i = panelStack.Count - 1; i >= 0; i--)
        {
            panelStack[i].Close();
        }
    }
}
