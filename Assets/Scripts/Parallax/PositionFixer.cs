using UnityEngine;

/// <summary>
/// 位置修复脚本，确保对象的位置始终是有效的（非NaN和非Infinity）
/// </summary>
public class PositionFixer : MonoBehaviour
{
    private Vector3 safePosition;
    
    void Start()
    {
        // 初始化安全位置
        FixPosition();
        safePosition = transform.position;
    }
    
    void Update()
    {
        FixPosition();
    }
    
    /// <summary>
    /// 检查并修复当前位置
    /// </summary>
    private void FixPosition()
    {
        Vector3 currentPosition = transform.position;
        
        // 检查位置是否有效
        if (IsPositionInvalid(currentPosition))
        {
            // 如果无效，使用安全位置
            Debug.LogWarning($"位置无效，已修复: {gameObject.name}");
            transform.position = safePosition;
        }
        else
        {
            // 更新安全位置
            safePosition = currentPosition;
        }
    }
    
    /// <summary>
    /// 检查位置是否无效（包含NaN或Infinity）
    /// </summary>
    /// <param name="position">要检查的位置</param>
    /// <returns>如果位置无效则返回true，否则返回false</returns>
    private bool IsPositionInvalid(Vector3 position)
    {
        return float.IsNaN(position.x) || float.IsNaN(position.y) || float.IsNaN(position.z) ||
               float.IsInfinity(position.x) || float.IsInfinity(position.y) || float.IsInfinity(position.z);
    }
}