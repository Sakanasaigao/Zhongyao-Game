using UnityEngine;
using UnityEngine.UI;
using TMPro;
// using DG.Tweening; // 不需要这个动画库了
using UnityEngine.EventSystems; 

public class SelectHorizontalScrollItem : MonoBehaviour, IPointerClickHandler {

    [Header("【请在Unity里把物体拖进去】")]
    public TMP_Text chapterText;    
    public TMP_Text levelNameText;  
    public Image coverImage;        
    public GameObject darkMask;     

    // 内部数据
    [HideInInspector] public LevelData data;
    [HideInInspector] public int indexInList;
    private SelectHorizontalScroll _controller;
    private RectTransform _rectTransform;

    private void Awake() {
        _rectTransform = GetComponent<RectTransform>();
    }

    public void SetInfo(LevelData data, int index, SelectHorizontalScroll controller) {
        this.data = data;
        this.indexInList = index;
        this._controller = controller;

        if (data != null) {
            if (chapterText) chapterText.text = data.chapterName;
            if (levelNameText) levelNameText.text = data.levelName;
            if (coverImage) coverImage.sprite = data.coverImage;
            if (darkMask) darkMask.SetActive(data.isLocked);
        }
    }

    public void UpdateVisual(float xPos, float centerX, float scaleCurveValue) {
        _rectTransform.anchoredPosition = new Vector2(xPos, 0);
        float finalScale = Mathf.Lerp(0.6f, 1.2f, scaleCurveValue); 
        _rectTransform.localScale = Vector3.one * finalScale;
    }

    // --- 监听点击 ---
    public void OnPointerClick(PointerEventData eventData) {
        if (_controller != null) {
            _controller.OnItemClicked(this);
        }
    }

    // 【拒绝动画的方法已删除】
}