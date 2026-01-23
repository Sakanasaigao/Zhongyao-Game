using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ProgressRingController : MonoBehaviour
{
    [SerializeField] private Image ringImage;
    [SerializeField] private float animationSpeed = 5f;

    private float targetFillAmount;
    private float currentFillAmount;

    private void Awake()
    {
        if (ringImage == null) ringImage = GetComponent<Image>();
        ringImage.type = Image.Type.Filled;
        ringImage.fillMethod = Image.FillMethod.Radial360;
        ringImage.fillClockwise = true;
        ringImage.fillOrigin = (int)Image.Origin360.Top;
    }

    private void Update()
    {
        if (Mathf.Abs(currentFillAmount - targetFillAmount) > 0.01f)
        {
            currentFillAmount = Mathf.MoveTowards(currentFillAmount, targetFillAmount, Time.deltaTime * animationSpeed);
            ringImage.fillAmount = currentFillAmount;
        }
    }

    public void SetProgress(float progress)
    {
        targetFillAmount = Mathf.Clamp01(progress);
    }

    public void SetProgressImmediate(float progress)
    {
        targetFillAmount = Mathf.Clamp01(progress);
        currentFillAmount = targetFillAmount;
        ringImage.fillAmount = currentFillAmount;
    }

    public void ResetProgress()
    {
        SetProgressImmediate(0f);
    }
}
