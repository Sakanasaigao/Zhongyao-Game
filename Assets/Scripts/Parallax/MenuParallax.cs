using UnityEngine;

public class MenuParallax : MonoBehaviour
{
    public float offsetMultiplier = 0.1f;
    public float smoothTime = 0.3f;

    private Vector3 startPosition;
    private Vector3 velocity = Vector3.zero;

    void Start()
    {
        // 确保初始位置有效，如果无效则重置为(0, 0, z)
        Vector3 pos = transform.position;
        SetSafePosition(ref pos);
        transform.position = pos;
        startPosition = pos;
    }

    void Update()
    {
        if (Camera.main != null)
        {
            // 检查offsetMultiplier是否有效，避免NaN值
            if (float.IsNaN(offsetMultiplier) || float.IsInfinity(offsetMultiplier))
                return;
            
            Vector3 mousePosition = Camera.main.ScreenToViewportPoint(Input.mousePosition);
            Vector3 offset = new Vector3(mousePosition.x - 0.5f, mousePosition.y - 0.5f, 0) * offsetMultiplier;
            Vector3 targetPosition = startPosition + offset;
            
            // 使用SmoothDamp计算新位置
            Vector3 newPosition = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
            
            // 确保设置的位置是安全的
            SetSafePosition(ref newPosition);
            transform.position = newPosition;
        }
    }
    
    // 确保位置是安全的（非NaN和非Infinity）
    private void SetSafePosition(ref Vector3 position)
    {
        if (float.IsNaN(position.x) || float.IsInfinity(position.x))
            position.x = startPosition.x;
        if (float.IsNaN(position.y) || float.IsInfinity(position.y))
            position.y = startPosition.y;
        if (float.IsNaN(position.z) || float.IsInfinity(position.z))
            position.z = startPosition.z;
    }
}