using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace UI
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class OutlineHighlight2D : MonoBehaviour
    {
        [Header("Outline Settings")]
        [SerializeField] private Color outlineColor = Color.cyan; // 亮蓝色，更醒目
        [SerializeField] private float animationSpeed = 0.8f; // 更快的动画速度
        [SerializeField] private bool isAnimated = true;
        [SerializeField] private bool autoStart = false;
        [SerializeField] private float scaleIntensity = 1.2f; // 缩放强度
        [SerializeField] private float pulseIntensity = 1.5f; // 脉冲强度

        private SpriteRenderer spriteRenderer;
        private Image uiImage;
        private Material originalMaterial;
        private Material outlineMaterial;
        private Color originalColor;
        private Vector3 originalScale;
        private Tween colorTween;
        private Tween scaleTween;

        private void Awake()
        {
            originalScale = transform.localScale;
            
            // 尝试获取SpriteRenderer
            spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                originalMaterial = spriteRenderer.material;
                originalColor = spriteRenderer.color;
                CreateOutlineMaterial();
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

        private void CreateOutlineMaterial()
        {
            outlineMaterial = new(Shader.Find("Sprites/Default"));
            outlineMaterial.color = outlineColor;
        }

        public void StartHighlight()
        {
            // 应用颜色变化
            if (spriteRenderer != null)
            {
                if (outlineMaterial != null)
                {
                    spriteRenderer.material = outlineMaterial;
                }
                else
                {
                    spriteRenderer.color = outlineColor;
                }
            }
            else if (uiImage != null)
            {
                uiImage.color = outlineColor;
            }
            
            // 应用缩放动画
            if (transform != null)
            {
                scaleTween = transform.DOScale(originalScale * scaleIntensity, animationSpeed * 0.5f)
                    .SetLoops(-1, LoopType.Yoyo)
                    .SetEase(Ease.InOutBack);
            }
            
            if (isAnimated)
            {
                StartAnimation();
            }
        }

        public void StopHighlight()
        {
            // 停止所有动画
            if (colorTween != null && colorTween.IsPlaying())
            {
                colorTween.Kill();
            }
            
            if (scaleTween != null && scaleTween.IsPlaying())
            {
                scaleTween.Kill();
            }

            // 恢复原始状态
            if (spriteRenderer != null)
            {
                if (originalMaterial != null)
                {
                    spriteRenderer.material = originalMaterial;
                }
                else
                {
                    spriteRenderer.color = originalColor;
                }
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

        private void StartAnimation()
        {
            Color startColor = outlineColor;
            Color endColor = new Color(startColor.r * pulseIntensity, startColor.g * pulseIntensity, startColor.b * pulseIntensity, 0.8f);

            if (spriteRenderer != null)
            {
                if (outlineMaterial != null)
                {
                    colorTween = outlineMaterial.DOColor(endColor, animationSpeed)
                        .SetLoops(-1, LoopType.Yoyo)
                        .SetEase(Ease.InOutExpo);
                }
                else
                {
                    colorTween = spriteRenderer.DOColor(endColor, animationSpeed)
                        .SetLoops(-1, LoopType.Yoyo)
                        .SetEase(Ease.InOutExpo);
                }
            }
            else if (uiImage != null)
            {
                colorTween = uiImage.DOColor(endColor, animationSpeed)
                    .SetLoops(-1, LoopType.Yoyo)
                    .SetEase(Ease.InOutExpo);
            }
        }

        public void SetOutlineColor(Color color)
        {
            outlineColor = color;
            if (outlineMaterial != null)
            {
                outlineMaterial.color = color;
            }
            else if (spriteRenderer != null)
            {
                spriteRenderer.color = color;
            }
            else if (uiImage != null)
            {
                uiImage.color = color;
            }
        }

        public void SetAnimationSpeed(float speed)
        {
            animationSpeed = speed;
            if (colorTween != null && colorTween.IsPlaying())
            {
                colorTween.Kill();
                StartAnimation();
            }
            
            if (scaleTween != null && scaleTween.IsPlaying())
            {
                scaleTween.Kill();
                if (transform != null)
                {
                    scaleTween = transform.DOScale(originalScale * scaleIntensity, animationSpeed * 0.5f)
                        .SetLoops(-1, LoopType.Yoyo)
                        .SetEase(Ease.InOutBack);
                }
            }
        }

        private void OnDestroy()
        {
            colorTween?.Kill();
            scaleTween?.Kill();

            if (outlineMaterial != null)
            {
                Destroy(outlineMaterial);
            }
        }
    }
}
