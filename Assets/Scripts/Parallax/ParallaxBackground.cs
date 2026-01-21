using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    public ParallaxCamera parallaxCamera;
    public Vector2 parallaxEffectMultiplier;
    public bool infiniteHorizontal;
    public bool infiniteVertical;

    private Transform cameraTransform;
    private Vector3 lastCameraPosition;
    private float spriteWidth, spriteHeight;
    private float textureUnitSizeX, textureUnitSizeY;
    private ParallaxLayer[] layers;

    void Start()
    {
        // 初始化layers数组
        layers = GetComponentsInChildren<ParallaxLayer>();
        
        // 设置相机引用
        if (parallaxCamera == null && Camera.main != null)
        {
            parallaxCamera = Camera.main.GetComponent<ParallaxCamera>();
        }
        
        if (parallaxCamera != null)
        {
            parallaxCamera.onCameraTranslate += Move;
        }
        
        // 确保parallaxEffectMultiplier有效
        FixVector2(ref parallaxEffectMultiplier);
    }

    public void Move(float delta)
    {
        // 检查delta是否有效
        if (float.IsNaN(delta) || float.IsInfinity(delta))
        {
            Debug.LogWarning($"无效delta值检测到，移动被忽略");
            return;
        }
        
        // 确保layers数组有效
        if (layers != null)
        {
            foreach (ParallaxLayer layer in layers)
            {
                if (layer != null)
                {
                    layer.Move(delta);
                }
            }
        }
    }
    
    /// <summary>
    /// 修复Vector2，确保所有分量都是有效的
    /// </summary>
    /// <param name="vector">要修复的Vector2</param>
    private void FixVector2(ref Vector2 vector)
    {
        if (float.IsNaN(vector.x) || float.IsInfinity(vector.x))
            vector.x = 1;
        if (float.IsNaN(vector.y) || float.IsInfinity(vector.y))
            vector.y = 1;
    }
}