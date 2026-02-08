using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UI;

[DisallowMultipleComponent]
public class SceneBHighlightManager : MonoBehaviour
{
    // 硬编码五关的红圈圈位置（根据用户要求分配）
    private Vector3[][] levelCirclePositions = new Vector3[][] {
        // 第一关：mark4的位置（中间偏右山峰）
        new Vector3[] {
            new Vector3(3.77f, 1.88f, 0f)  // mark4的位置
        },
        // 第二关：mark3的位置（右上角山顶）
        new Vector3[] {
            new Vector3(3.59f, 4.48f, 0f)  // mark3的位置
        },
        // 第三关：mark5的位置（右下角）
        new Vector3[] {
            new Vector3(5.87f, -2.65f, 0f)  // mark5的位置
        },
        // 第四关：mark1和mark2的位置（中间位置和左下角）
        new Vector3[] {
            new Vector3(0.32f, -0.51f, 0f),  // mark1的位置
            new Vector3(-1.34f, -3.81f, 0f)   // mark2的位置
        },
        // 第五关：空数组，全部动画都关掉
        new Vector3[] {}
    };

    private PlayerManager playerManager;
    private GameObject circlesParent;
    private float animationSpeed = 1.0f;
    private bool initialized = false;

    // 单例模式，确保场景中只有一个实例
    private static SceneBHighlightManager instance;
    public static SceneBHighlightManager Instance
    {
        get
        {
            if (instance == null)
            {
                // 如果场景中没有实例，创建一个
                GameObject managerObj = new GameObject("SceneBHighlightManager");
                instance = managerObj.AddComponent<SceneBHighlightManager>();
                Debug.Log("SceneBHighlightManager: Created new instance");
            }
            return instance;
        }
    }

    private void Awake()
    {
        // 确保单例
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        
        // 确保对象在场景切换时不被销毁
        DontDestroyOnLoad(gameObject);
        
        Debug.Log("SceneBHighlightManager: Awake called");
        
        playerManager = PlayerManager.instance;
        if (playerManager == null)
        {
            Debug.LogError("SceneBHighlightManager: PlayerManager instance not found!");
        }
        else
        {
            Debug.Log("SceneBHighlightManager: PlayerManager instance found");
        }
        
        // 创建一个父对象来管理所有红圈圈
        circlesParent = new GameObject("SceneBCircles");
        circlesParent.transform.parent = transform;
        circlesParent.transform.localPosition = Vector3.zero;
        Debug.Log("SceneBHighlightManager: Circles parent created");
        
        initialized = true;
    }

    private void Start()
    {
        Debug.Log("SceneBHighlightManager: Start called");
        UpdateCircles();
    }

    private void OnEnable()
    {
        Debug.Log("SceneBHighlightManager: OnEnable called");
        UpdateCircles();
        
        // 监听场景加载完成事件
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        Debug.Log("SceneBHighlightManager: OnDisable called");
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("SceneBHighlightManager: Scene loaded: " + scene.name);
        // 当加载SceneB时，更新红圈圈
        if (scene.name == "SceneB")
        {
            Debug.Log("SceneBHighlightManager: SceneB loaded, updating circles");
            UpdateCircles();
        }
    }

    public void UpdateCircles()
    {
        if (!initialized)
        {
            Debug.LogWarning("SceneBHighlightManager: Not initialized yet");
            return;
        }
        
        if (playerManager == null)
        {
            Debug.LogError("SceneBHighlightManager: PlayerManager is null");
            return;
        }
        
        int currentLevel = GetCurrentLevel();
        Debug.Log("SceneBHighlightManager: Current level: " + currentLevel);
        
        // 清除现有的红圈圈
        if (circlesParent != null)
        {
            foreach (Transform child in circlesParent.transform)
            {
                Destroy(child.gameObject);
            }
            Debug.Log("SceneBHighlightManager: Cleared existing circles");
        }
        else
        {
            Debug.LogError("SceneBHighlightManager: Circles parent is null");
            return;
        }
        
        // 为每一关创建红圈圈
        for (int level = 0; level < currentLevel; level++)
        {
            Vector3[] positions = levelCirclePositions[level];
            Debug.Log("SceneBHighlightManager: Level " + (level + 1) + " has " + positions.Length + " circles");
            
            // 只为当前关卡的红圈圈添加动画
            // 第五关全部动画都关掉
            bool addHighlight = level == currentLevel - 1 && currentLevel < 5;
            Debug.Log("SceneBHighlightManager: Level " + (level + 1) + " - addHighlight: " + addHighlight);
            
            foreach (Vector3 position in positions)
            {
                Debug.Log("SceneBHighlightManager: Creating circle at position " + position + " with addHighlight: " + addHighlight);
                CreateCircle(position, addHighlight);
            }
        }
    }

    private void CreateCircle(Vector3 position, bool addHighlight)
    {
        // 创建红圈圈GameObject
        GameObject circle = new GameObject("HighlightCircle");
        circle.transform.parent = circlesParent.transform;
        circle.transform.localPosition = position;
        circle.transform.localScale = new Vector3(0.5f, 0.5f, 1f);
        
        // 添加SpriteRenderer组件
        SpriteRenderer spriteRenderer = circle.AddComponent<SpriteRenderer>();
        
        // 使用简单的红色精灵，避免动态创建纹理可能带来的问题
        spriteRenderer.color = Color.red;
        spriteRenderer.sortingOrder = 10; // 确保在最上层显示
        
        // 创建一个简单的圆形精灵
        Texture2D circleTexture = CreateCircleTexture(64, 64, Color.red);
        Sprite circleSprite = Sprite.Create(circleTexture, new Rect(0, 0, 64, 64), new Vector2(0.5f, 0.5f));
        spriteRenderer.sprite = circleSprite;
        
        Debug.Log("SceneBHighlightManager: Created circle at position: " + position + ", addHighlight: " + addHighlight);
        
        // 如果是当前关新增的红圈圈，添加缩放动画
        if (addHighlight)
        {
            SimpleHighlightEffect highlight = circle.AddComponent<SimpleHighlightEffect>();
            highlight.SetAnimationSpeed(animationSpeed);
            highlight.StartAnimation();
            Debug.Log("SceneBHighlightManager: Added scale animation to circle");
        }
    }

    private Texture2D CreateCircleTexture(int width, int height, Color color)
    {
        Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector2 center = new Vector2(width / 2, height / 2);
                float distance = Vector2.Distance(new Vector2(x, y), center);
                
                if (distance < width / 4)
                {
                    texture.SetPixel(x, y, color);
                }
                else if (distance < width / 3)
                {
                    float alpha = 1.0f - (distance - width / 4) / (width / 12);
                    texture.SetPixel(x, y, new Color(color.r, color.g, color.b, alpha));
                }
                else
                {
                    texture.SetPixel(x, y, Color.clear);
                }
            }
        }
        
        texture.Apply();
        return texture;
    }

    private int GetCurrentLevel()
    {
        if (playerManager == null)
        {
            Debug.LogError("SceneBHighlightManager: PlayerManager is null in GetCurrentLevel");
            return 1; // 默认返回第一关
        }
        
        int scriptIndex = playerManager.GetScriptIndex();
        Debug.Log("SceneBHighlightManager: Player script index: " + scriptIndex);
        
        // 根据scriptIndex判断当前关卡
        // 简化处理，确保初始状态下能显示第一关的红圈圈
        if (scriptIndex < 1) return 1;  // 初始状态（scriptIndex=0）显示第一关
        if (scriptIndex < 2) return 2;
        if (scriptIndex < 3) return 3;
        if (scriptIndex < 4) return 4;
        return 5;
    }

    private void OnDestroy()
    {
        Debug.Log("SceneBHighlightManager: OnDestroy called");
        // 清理资源
        if (circlesParent != null)
        {
            foreach (Transform child in circlesParent.transform)
            {
                Destroy(child.gameObject);
            }
        }
    }
}

// 辅助类，用于在场景加载时自动创建SceneBHighlightManager
public class SceneBHighlightManagerInitializer : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        Debug.Log("SceneBHighlightManagerInitializer: Initializing...");
        // 访问Instance属性，触发创建
        var manager = SceneBHighlightManager.Instance;
        Debug.Log("SceneBHighlightManagerInitializer: Initialized successfully");
    }
}
