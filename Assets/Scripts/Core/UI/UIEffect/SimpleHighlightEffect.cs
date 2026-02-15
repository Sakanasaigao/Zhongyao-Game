using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class SimpleHighlightEffect : MonoBehaviour
    {
        [Header("Scale Animation Settings")]
    [SerializeField] private float animationSpeed = 2.0f;
    [SerializeField] private float scaleIntensity = 1.3f;
    [SerializeField] private bool autoStart = false;

    // 全局动画开关
    public static bool animationsEnabled = true;

    private Vector3 originalScale;
    private float time;
    private bool isAnimating = false;

        private void Awake()
        {
            originalScale = transform.localScale;
        }

        private void Start()
        {
            if (autoStart && animationsEnabled)
            {
                StartAnimation();
            }
        }

        private void Update()
        {
            if (isAnimating && animationsEnabled)
            {
                UpdateAnimation();
            }
        }

        public void StartAnimation()
        {
            if (animationsEnabled)
            {
                isAnimating = true;
                time = 0f;
            }
        }

        public void StopAnimation()
        {
            isAnimating = false;
            
            // 恢复原始状态
            transform.localScale = originalScale;
        }

        private void UpdateAnimation()
        {
            time += Time.deltaTime * animationSpeed;
            
            // 计算动画值（0-1之间的正弦波）
            float t = Mathf.Sin(time * Mathf.PI * 2) * 0.5f + 0.5f;
            
            // 应用缩放动画
            float scale = Mathf.Lerp(1f, scaleIntensity, t);
            transform.localScale = originalScale * scale;
        }

        public void SetAnimationSpeed(float speed)
        {
            animationSpeed = speed;
        }

        public void SetScaleIntensity(float intensity)
        {
            scaleIntensity = intensity;
        }
    }
}
