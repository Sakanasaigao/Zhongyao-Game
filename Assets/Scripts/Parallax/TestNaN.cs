using UnityEngine;

/// <summary>
/// 测试脚本，用于检查NaN位置问题是否已解决
/// </summary>
public class TestNaN : MonoBehaviour
{
    public Transform[] testObjects;
    
    void Update()
    {
        CheckAllObjects();
    }
    
    void CheckAllObjects()
    {
        bool hasNaN = false;
        
        // 检查指定对象
        if (testObjects != null)
        {
            foreach (Transform obj in testObjects)
            {
                if (obj != null && IsPositionNaN(obj.position))
                {
                    Debug.LogError($"发现NaN位置: {obj.name}");
                    hasNaN = true;
                }
            }
        }
        
        // 检查所有带有视差脚本的对象
        MenuParallax[] menuParallax = FindObjectsOfType<MenuParallax>();
        foreach (MenuParallax mp in menuParallax)
        {
            if (IsPositionNaN(mp.transform.position))
            {
                Debug.LogError($"MenuParallax发现NaN位置: {mp.gameObject.name}");
                hasNaN = true;
            }
        }
        
        ParallaxLayer[] parallaxLayers = FindObjectsOfType<ParallaxLayer>();
        foreach (ParallaxLayer pl in parallaxLayers)
        {
            if (IsPositionNaN(pl.transform.position))
            {
                Debug.LogError($"ParallaxLayer发现NaN位置: {pl.gameObject.name}");
                hasNaN = true;
            }
        }
        
        if (!hasNaN)
        {
            Debug.Log("未发现NaN位置，修复成功！");
        }
    }
    
    bool IsPositionNaN(Vector3 pos)
    {
        return float.IsNaN(pos.x) || float.IsNaN(pos.y) || float.IsNaN(pos.z) ||
               float.IsInfinity(pos.x) || float.IsInfinity(pos.y) || float.IsInfinity(pos.z);
    }
}