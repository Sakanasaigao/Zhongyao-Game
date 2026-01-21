using UnityEngine;

[ExecuteInEditMode]
public class ParallaxLayer : MonoBehaviour
{
    public float parallaxFactor;

    public void Move(float delta)
    {
        // 检查所有值是否有效
        if (float.IsNaN(parallaxFactor) || float.IsInfinity(parallaxFactor) || 
            float.IsNaN(delta) || float.IsInfinity(delta))
        {
            Debug.LogWarning($"无效值检测到，移动被忽略: {gameObject.name}");
            return;
        }
            
        // 计算新位置
        Vector3 newPos = transform.localPosition;
        newPos.x -= delta * parallaxFactor;
        
        // 确保新位置有效
        FixPosition(ref newPos);
        transform.localPosition = newPos;
    }
    
    private void Start()
    {
        // 确保parallaxFactor有效
        if (float.IsNaN(parallaxFactor) || float.IsInfinity(parallaxFactor))
        {
            parallaxFactor = 0;
        }
        
        // 确保初始位置有效
        Vector3 pos = transform.localPosition;
        FixPosition(ref pos);
        transform.localPosition = pos;
    }
    
    /// <summary>
    /// 修复位置，确保所有分量都是有效的
    /// </summary>
    /// <param name="position">要修复的位置</param>
    private void FixPosition(ref Vector3 position)
    {
        if (float.IsNaN(position.x) || float.IsInfinity(position.x))
            position.x = 0;
        if (float.IsNaN(position.y) || float.IsInfinity(position.y))
            position.y = 0;
        if (float.IsNaN(position.z) || float.IsInfinity(position.z))
            position.z = transform.localPosition.z; // 保持原z轴位置或使用默认值
    }
}
