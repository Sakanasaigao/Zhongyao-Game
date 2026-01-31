using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class SimpleHighlightEffect : MonoBehaviour
    {
        [Header("Highlight Settings")]
        [SerializeField] private Color highlightColor = new Color(0f, 1f, 1f); // 亮蓝色，更醒目
        [SerializeField] private float animationSpeed = 1.0f; // 动画速度
        [SerializeField] private float scaleIntensity = 1.2f; // 缩放强度
        [SerializeField] private bool autoStart = true; // 是否自动开始

        private SpriteRenderer spriteRenderer;
        private Image uiImage;
        private Transform transform;
        private Color originalColor;
        private Vector3 originalScale;
        private float time;

        private void Awake()
        {
            transform = GetComponent<Transform>();
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

        private bool isHighlighting = false;

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
            
            if (transform != null)
            {
                transform.localScale = originalScale;
            }
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
            if (transform != null)
            {
                transform.localScale = originalScale * scale;
            }
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
