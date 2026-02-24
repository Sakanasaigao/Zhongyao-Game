using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingItem : MonoBehaviour
{
    [SerializeField] private GameObject labelText;
    [SerializeField] private GameObject valueText;
    [SerializeField] private GameObject slider;
    [SerializeField] private GameObject colorPicker;
    [SerializeField] private GameObject keyCodeInput;

    private SettingType settingType;
    private System.Action<object> onValueChanged;
    private object additionalData;

    public void SetValue(string label, string value, SettingType type, System.Action<object> onChange, object data = null)
    {
        labelText.GetComponent<TextMeshProUGUI>().text = label;
        valueText.GetComponent<TextMeshProUGUI>().text = value;

        settingType = type;
        onValueChanged = onChange;
        additionalData = data;

        SetupUIElements();
        // FitSize();
    }

    private void SetupUIElements()
    {
        slider.SetActive(settingType == SettingType.Slider);
        colorPicker.SetActive(settingType == SettingType.Color);
        keyCodeInput.SetActive(settingType == SettingType.KeyCode);

        if (settingType == SettingType.Slider && additionalData is float initialValue)
        {
            Slider sliderComponent = slider.GetComponent<Slider>();
            sliderComponent.value = initialValue;
            sliderComponent.onValueChanged.AddListener(OnSliderValueChanged);
        }

        if (settingType == SettingType.Color && additionalData is Color initialColor)
        {
            // 实现颜色选择器逻辑
        }

        if (settingType == SettingType.KeyCode)
        {
            // 实现按键输入逻辑
        }
    }

    private void OnSliderValueChanged(float value)
    {
        valueText.GetComponent<TextMeshProUGUI>().text = value.ToString("F2");
        onValueChanged?.Invoke(value);
    }

    private void FitSize()
    {
        RectTransform thisRect = GetComponent<RectTransform>();
        if (thisRect == null)
            return;

        float height = 60f;
        if (settingType == SettingType.Slider)
            height = 80f;

        thisRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
    }
}
