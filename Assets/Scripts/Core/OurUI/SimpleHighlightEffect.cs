using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class SimpleHighlightEffect : MonoBehaviour
    {
        [Header("Highlight Settings")]
        [SerializeField] private Color highlightColor = Color.yellow;
        [SerializeField] private float animationSpeed = 2.0f;
        [SerializeField] private float scaleIntensity = 1.3f;
        [SerializeField] private bool autoStart = true;

        private SpriteRenderer spriteRenderer;
        private Image uiImage;
        private Color originalColor;
        private Vector3 originalScale;
        private float time;
        private bool isHighlighting = false;

        private void Awake()
        {
            originalScale = transform.localScale;
            
            // 尝试获取SpriteRenderer
            spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                originalColor = spriteRenderer.color;
                return;
            }

            // 尝试获取UI Image
            uiImage = GetComponent<Image>();
            if (uiImage != null)
            {
                originalColor = uiImage.color;
            }
        }

        private void Start()
        {
            if (autoStart)
            {
                StartHighlight();
            }
        }

        private void Update()
        {
            if (isHighlighting)
            {
                UpdateAnimation();
            }
        }

        public void StartHighlight()
        {
            isHighlighting = true;
            time = 0f;
        }

        public void StopHighlight()
        {
            isHighlighting = false;
            
            // 恢复原始状态
            if (spriteRenderer != null)
            {
                spriteRenderer.color = originalColor;
            }
            else if (uiImage != null)
            {
                uiImage.color = originalColor;
            }
            
            transform.localScale = originalScale;
        }

        private void UpdateAnimation()
        {
            time += Time.deltaTime * animationSpeed;
            
            // 计算动画值（0-1之间的正弦波）
            float t = Mathf.Sin(time * Mathf.PI * 2) * 0.5f + 0.5f;
            
            // 应用颜色动画
            Color currentColor = Color.Lerp(originalColor, highlightColor, t);
            if (spriteRenderer != null)
            {
                spriteRenderer.color = currentColor;
            }
            else if (uiImage != null)
            {
                uiImage.color = currentColor;
            }
            
            // 应用缩放动画
            float scale = Mathf.Lerp(1f, scaleIntensity, t);
            transform.localScale = originalScale * scale;
        }

        public void SetHighlightColor(Color color)
        {
            highlightColor = color;
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
