using UnityEngine;

[ExecuteInEditMode]
public class ParallaxCamera : MonoBehaviour
{
    public delegate void ParallaxCameraDelegate(float deltaMovement);
    public ParallaxCameraDelegate onCameraTranslate;

    private float oldPosition;

    void Start()
    {
        // 确保初始位置有效
        EnsureValidPosition();
        oldPosition = transform.position.x;
    }

    void Update()
    {
        // 确保当前位置有效
        EnsureValidPosition();
        
        if (transform.position.x != oldPosition)
        {
            if (onCameraTranslate != null)
            {
                // 检查delta是否有效
                float delta = oldPosition - transform.position.x;
                if (!float.IsNaN(delta) && !float.IsInfinity(delta))
                {
                    onCameraTranslate(delta);
                }
            }

            oldPosition = transform.position.x;
        }
    }
    
    /// <summary>
    /// 确保相机位置始终有效
    /// </summary>
    private void EnsureValidPosition()
    {
        Vector3 pos = transform.position;
        if (float.IsNaN(pos.x) || float.IsInfinity(pos.x))
            pos.x = 0;
        if (float.IsNaN(pos.y) || float.IsInfinity(pos.y))
            pos.y = 0;
        if (float.IsNaN(pos.z) || float.IsInfinity(pos.z))
            pos.z = -10;
        
        transform.position = pos;
    }
}
