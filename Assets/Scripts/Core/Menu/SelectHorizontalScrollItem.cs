using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SelectHorizontalScrollItem : MonoBehaviour {
    [Header("UI 组件绑定")]
    public TMP_Text chapterText;    // 对应 Hierarchy 里的 "次序" (如：第一章)
    public TMP_Text levelNameText;  // 对应 Hierarchy 里的 "关卡名" (如：桃源问津)
    public Image coverImage;        // 对应 Hierarchy 里的 "插画"
    public GameObject darkMask;     // 对应 Hierarchy 里的 "Image" (那个灰色的遮罩)

    private RectTransform _rectTransform;

    private void Awake() {
        _rectTransform = GetComponent<RectTransform>();
    }

    public void SetInfo(LevelData data) {
        if (data == null) return;

        // 1. 设置文字
        if (chapterText) chapterText.text = data.chapterName; // 显示 "第一章"
        if (levelNameText) levelNameText.text = data.levelName; // 显示 "桃源问津"

        // 2. 设置插画
        if (coverImage) coverImage.sprite = data.coverImage;

        // 3. 处理明暗状态
        // 逻辑：如果锁住了(isLocked=true)，就把遮罩打开(SetActive true)，看起来就暗了
        // 如果解锁了，就把遮罩关掉，看起来就是亮的
        if (darkMask) {
            darkMask.SetActive(data.isLocked);
        }
    }

    // 保持之前的动画逻辑不变
    public void UpdateVisual(float xPos, float centerX, float scaleCurveValue) {
        _rectTransform.anchoredPosition = new Vector2(xPos, 0);
        float finalScale = Mathf.Lerp(0.6f, 1.2f, scaleCurveValue); 
        _rectTransform.localScale = Vector3.one * finalScale;
    }
}