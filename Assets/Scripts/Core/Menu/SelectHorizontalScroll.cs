using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using DIALOGUE; // 引入对话系统命名空间

public class SelectHorizontalScroll : MonoBehaviour {

    [Header("【关卡数据配置】")]
    public LevelData[] levelDatas; 

    [Header("【必须拖入的组件】")]
    public SelectHorizontalScrollItem itemPrefab; 
    public Transform itemParent;            

    [Header("【界面软跳转引用 (在此处赋值)】")]
    public GameObject selectPanel; 
    public GameObject mainUIRoot;  

    [Header("【参数调整】")]
    public float itemSpace = 400f;          
    public float scaleRange = 300f;         
    public float snapSpeed = 10f;           

    private List<SelectHorizontalScrollItem> _spawnedItems = new List<SelectHorizontalScrollItem>();
    private float _currentScrollX = 0f;
    private float _targetScrollX = 0f;
    private bool _isDragging = false;
    private float _totalWidth;

    void Start() {
        LoadSaveData();
        SpawnItems();
        _totalWidth = levelDatas.Length * itemSpace;
    }

    void LoadSaveData() {
        for (int i = 0; i < levelDatas.Length; i++) {
            if (i == 0) {
                levelDatas[i].isLocked = false;
                continue;
            }
            if (PlayerPrefs.GetInt("Level_" + i + "_Unlocked", 0) == 1) {
                levelDatas[i].isLocked = false;
            }
        }
    }

    void SpawnItems() {
        foreach (Transform child in itemParent) Destroy(child.gameObject);
        _spawnedItems.Clear();

        for (int i = 0; i < levelDatas.Length; i++) {
            var item = Instantiate(itemPrefab, itemParent);
            item.SetInfo(levelDatas[i], i, this);
            _spawnedItems.Add(item);
        }
    }

    public void OnItemClicked(SelectHorizontalScrollItem item) {
        if (_isDragging) return; 

        float dist = Mathf.Abs((-item.indexInList * itemSpace) - _currentScrollX);
        bool isCenter = dist < (itemSpace / 2);

        if (isCenter) {
            if (item.data.isLocked) {
                Debug.Log("🔒 拒绝：关卡锁定");
            } else {
                string fileName = (item.indexInList + 1).ToString() + "1";
                Debug.Log("🚀 准备进入剧本: " + fileName);

                if (StoryTransition.Instance != null) {
                    StoryTransition.Instance.Play(() => {
                        
                        // 1. 暗中切换 UI 场景
                        if (selectPanel != null) selectPanel.SetActive(false);
                        if (mainUIRoot != null) mainUIRoot.SetActive(true);

                        // 2. 读取剧本文件
                        TextAsset scriptAsset = Resources.Load<TextAsset>("GameScripts/" + fileName);

                        if (scriptAsset != null) {
                            string[] lines = scriptAsset.text.Split(new[] { "\n", "\r" }, System.StringSplitOptions.RemoveEmptyEntries);
                            List<string> conversation = new List<string>(lines);

                            // 3. 严格遵循总监架构：仅通过单例调用，绝不擅自寻找兜底
                            if (DialogueSystem.instance != null && DialogueSystem.instance.conversationManager != null) {
                                DialogueSystem.instance.conversationManager.StartConversation(conversation);
                                Debug.Log("✅ 剧本启动成功！");

                                DialogueSystem.instance.conversationManager.onConversationEnd = () => {
                                    Debug.Log("🏁 剧本播放完毕。");
                                };
                            } else {
                                // 忠实汇报错误，将控制权交还给您来排查
                                Debug.LogError("❌ 警报：DialogueSystem.instance 为空！");
                            }
                        } else {
                            Debug.LogError($"❌ 找不到剧本文件：Resources/GameScripts/{fileName}");
                        }
                    });
                } else {
                    Debug.LogError("⚠️ 场景里没找到 StoryTransition 脚本！");
                }
            }
        } else {
            _targetScrollX = -item.indexInList * itemSpace;
        }
    }

    void Update() {
        if (!_isDragging) {
            _currentScrollX = Mathf.Lerp(_currentScrollX, _targetScrollX, Time.deltaTime * snapSpeed);
        }

        for (int i = 0; i < _spawnedItems.Count; i++) {
            float basePos = i * itemSpace;
            float finalPos = basePos + _currentScrollX;

            while (finalPos > _totalWidth * 0.5f) finalPos -= _totalWidth;
            while (finalPos < -_totalWidth * 0.5f) finalPos += _totalWidth;

            float distToCenter = Mathf.Abs(finalPos);
            float scalePercent = 1 - Mathf.Clamp01(distToCenter / scaleRange);

            _spawnedItems[i].UpdateVisual(finalPos, distToCenter, scalePercent);

            if (distToCenter < itemSpace / 2) {
                _spawnedItems[i].transform.SetAsLastSibling();
            }
        }
    }

    public void OnBeginDrag(UnityEngine.EventSystems.PointerEventData eventData) { _isDragging = true; }
    public void OnDrag(UnityEngine.EventSystems.PointerEventData eventData) { _currentScrollX += eventData.delta.x; }
    public void OnEndDrag(UnityEngine.EventSystems.PointerEventData eventData) {
        _isDragging = false;
        float indexFloat = -_currentScrollX / itemSpace;
        int nearestIndex = Mathf.RoundToInt(indexFloat);
        _targetScrollX = -nearestIndex * itemSpace;
    }
}