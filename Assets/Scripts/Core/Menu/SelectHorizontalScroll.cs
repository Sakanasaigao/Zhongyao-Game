using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SelectHorizontalScroll : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler
{
    [Header("配置")]
    // 直接在这里初始化 5 个示例数据，这样 Inspector 里默认就会有
    public LevelData[] levelDatas = new LevelData[] {
        new LevelData { 
            levelId=1, 
            chapterName="第一章", // 【新增】
            levelName="桃源问津", 
            isLocked=false 
        },
        new LevelData { 
            levelId=2, 
            chapterName="第二章", // 【新增】
            levelName="姑苏夜游", 
            isLocked=false 
        },
        new LevelData { 
            levelId=3, 
            chapterName="第三章", // 【新增】
            levelName="蜀道通天", 
            isLocked=true  // 这个是锁住的，会变暗
        },
        new LevelData { 
            levelId=4, 
            chapterName="第四章", 
            levelName="敦煌飞天", 
            isLocked=true 
        },
        new LevelData { 
            levelId=5, 
            chapterName="终  章", 
            levelName="紫禁之巅", 
            isLocked=true 
        }
    };   // 在 Inspector 里配置你的关卡数据
    public SelectHorizontalScrollItem itemPrefab; // 你的 Card Prefab
    public Transform itemParent;            // ItemParent 节点
    
    [Header("参数调整")]
    public float itemSpace = 300f;          // 卡片间距
    public float scaleRange = 200f;         // 缩放生效的距离范围 (影响缩放曲线)
    public float snapSpeed = 10f;           // 自动吸附的速度

    private List<SelectHorizontalScrollItem> _spawnedItems = new List<SelectHorizontalScrollItem>();
    private float _currentScrollX = 0f;     // 当前滚动的虚拟 X 值
    private float _targetScrollX = 0f;      // 目标吸附 X 值
    private bool _isDragging = false;

    private float _totalWidth;              // 总宽 (用于取模循环)

    void Start()
    {
        // 1. 初始化生成
        // 只要数据够多，直接生成对应数量。如果数据太少(比如只有2个)，
        // 无限循环需要生成 副本 (Ghost items)，这里暂时假设数据 > 4 个
        SpawnItems();
        
        // 计算总宽度
        _totalWidth = levelDatas.Length * itemSpace;
    }

    void SpawnItems()
    {
        // 清理旧物体
        foreach (Transform child in itemParent) Destroy(child.gameObject);
        _spawnedItems.Clear();

        for (int i = 0; i < levelDatas.Length; i++)
        {
            var item = Instantiate(itemPrefab, itemParent);
            item.SetInfo(levelDatas[i]);
            _spawnedItems.Add(item);
        }
    }

    void Update()
    {
        // 1. 如果没在拖拽，平滑吸附到目标位置
        if (!_isDragging)
        {
            _currentScrollX = Mathf.Lerp(_currentScrollX, _targetScrollX, Time.deltaTime * snapSpeed);
        }

        // 2. 遍历所有卡片，计算它们的位置 (无限循环的核心)
        for (int i = 0; i < _spawnedItems.Count; i++)
        {
            // 基础位置：根据索引和间距
            float basePos = i * itemSpace;
            
            // 加上滚动偏移
            float finalPos = basePos + _currentScrollX;

            // --- 无限循环数学核心 (Modulo) ---
            // 这一步把坐标限制在 [-_totalWidth/2, _totalWidth/2] 范围内
            // 这样当卡片移出左边，就会瞬移到右边，反之亦然
            while (finalPos > _totalWidth * 0.5f) finalPos -= _totalWidth;
            while (finalPos < -_totalWidth * 0.5f) finalPos += _totalWidth;

            // --- 计算缩放曲线 ---
            // 距离中心的绝对距离
            float distToCenter = Mathf.Abs(finalPos);
            // 计算一个 0~1 的系数：0表示很远，1表示在正中心
            float scalePercent = 1 - Mathf.Clamp01(distToCenter / scaleRange);

            // 调用子物体刷新视觉
            _spawnedItems[i].UpdateVisual(finalPos, distToCenter, scalePercent);
            
            // --- 动态层级排序 ---
            // 距离中心越近，Z 轴越靠前 (Unity UI 中 Z轴越小越近，或者不需要Z轴只靠SiblingIndex)
            // 这里用一个小技巧：根据 scalePercent 修改 Canvas sortingOrder 或者 SiblingIndex
            // 简单的做法：每帧判断谁离中心最近，SetAsLastSibling()
            if (distToCenter < itemSpace / 2)
            {
                _spawnedItems[i].transform.SetAsLastSibling();
            }
        }
    }

    // --- 交互接口实现 ---

    public void OnBeginDrag(PointerEventData eventData)
    {
        _isDragging = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        // 累加鼠标/手指的移动量
        _currentScrollX += eventData.delta.x;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _isDragging = false;
        
        // --- 自动吸附逻辑 ---
        // 1. 计算当前实际上是第几个卡片在中间
        // 因为 _currentScrollX 是负值表示向右滚，所以取反
        float indexFloat = -_currentScrollX / itemSpace;
        
        // 2. 四舍五入到最近的整数索引
        int nearestIndex = Mathf.RoundToInt(indexFloat);

        // 3. 计算这个索引对应的精确位置
        _targetScrollX = -nearestIndex * itemSpace;
    }
}