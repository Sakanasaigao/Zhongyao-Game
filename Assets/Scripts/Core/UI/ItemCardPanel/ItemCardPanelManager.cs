using UnityEngine;

public class ItemCardPanelManager : MonoBehaviour
{
    private static ItemCardPanelManager instance;
    public static ItemCardPanelManager Instance => instance;

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

    public void OpenItemCardPanel()
    {
        UIManager.Instance.OpenPanel<ItemCardPanel>();
    }

    public static void ShowItemCards()
    {
        if (Instance != null)
            Instance.OpenItemCardPanel();
        else
            Debug.LogError("ItemCardPanelManager 实例不存在");
    }
}