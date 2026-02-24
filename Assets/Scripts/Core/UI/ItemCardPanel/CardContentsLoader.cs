using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardContentsLoader : MonoBehaviour
{
    [SerializeField] private Image itemImage;
    [SerializeField] private TextMeshProUGUI itemNameText;

    public string ItemName { get; private set; }

    public void LoadItemCardContents(string itemName)
    {
        string imagePath = $"Items/Medicine/{itemName}/Images/{itemName}";
        Sprite sprite = Resources.Load<Sprite>(imagePath);
        if (sprite != null && itemImage != null)
        {
            itemImage.sprite = sprite;
            itemImage.preserveAspect = true;
        }
        else
        {
            Debug.LogWarning($"无法加载物品图片: {imagePath}");
        }

        itemNameText.text = itemName;
    }
}