using DG.Tweening;
using DIALOGUE;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SettingsSystem : UIPanel
{
    [Header("Reference")]
    [SerializeField] private Transform container;
    [SerializeField] private GameObject settingItemPrefab;
    [SerializeField] private DialogueSystemConfigurationSO dialogueConfig;
    [SerializeField] private AudioMixer audioMixer;

    private List<SettingItem> activeItems = new List<SettingItem>();

    protected override void Awake()
    {
        base.Awake();
        if (container == null)
            container = transform;
        if (contentTransform == null)
            contentTransform = container;
    }

    public override void Open(object param = null)
    {
        InitializeSettingItems();
        base.Open(param);
    }

    public void InitializeSettingItems()
    {
        ClearContainer();

        AddDialogueSettings();
        AddAudioSettings();

        // UpdateContentHeight();
    }

    private void AddDialogueSettings()
    {
        AddSettingItem("跳过键", dialogueConfig.skipKey.ToString(), SettingType.KeyCode, OnSkipKeyChanged);
        //AddSettingItem("默认文本颜色", "", SettingType.Color, OnTextColorChanged, dialogueConfig.defaultTextColor);
        //AddSettingItem("默认字体", dialogueConfig.defaultFont != null ? dialogueConfig.defaultFont.name : "无", SettingType.Font, OnFontChanged);
    }

    private void AddAudioSettings()
    {
        float musicVolume = GetVolumeFromMixer("MusicVolume");
        float sfxVolume = GetVolumeFromMixer("SFXVolume");
        float voiceVolume = GetVolumeFromMixer("VoiceVolume");

        AddSettingItem("音乐音量", musicVolume.ToString("F2"), SettingType.Slider, OnMusicVolumeChanged, musicVolume);
        AddSettingItem("音效音量", sfxVolume.ToString("F2"), SettingType.Slider, OnSFXVolumeChanged, sfxVolume);
        AddSettingItem("语音音量", voiceVolume.ToString("F2"), SettingType.Slider, OnVoiceVolumeChanged, voiceVolume);
    }

    private void AddSettingItem(string label, string value, SettingType type, System.Action<object> onValueChanged, object additionalData = null)
    {
        GameObject itemPrefab = Instantiate(settingItemPrefab, container);
        SettingItem item = itemPrefab.GetComponent<SettingItem>();

        item.SetValue(label, value, type, onValueChanged, additionalData);
        activeItems.Add(item);

        if (useScaleAnimation && contentTransform != null)
        {
            itemPrefab.transform.localScale = Vector3.zero;
            itemPrefab.transform.DOScale(1, scaleDuration * 0.5f)
                .SetEase(openEase)
                .SetDelay(activeItems.Count * 0.05f)
                .SetUpdate(true);
        }
    }

    private void ClearContainer()
    {
        foreach (var item in activeItems)
        {
            if (item != null)
                Destroy(item.gameObject);
        }
        activeItems.Clear();

        for (int i = container.childCount - 1; i >= 0; i--)
        {
            Transform child = container.GetChild(i);
            Destroy(child.gameObject);
        }
    }

    private void UpdateContentHeight()
    {
        if (container == null)
            return;

        RectTransform containerRect = container.GetComponent<RectTransform>();
        if (containerRect == null)
            return;

        float totalHeight = 0;
        float spacing = 30;

        foreach (var item in activeItems)
        {
            if (item != null)
            {
                RectTransform itemRect = item.GetComponent<RectTransform>();
                if (itemRect != null)
                {
                    totalHeight += itemRect.rect.height + spacing;
                }
            }
        }

        if (totalHeight > spacing)
            totalHeight -= spacing;

        containerRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, totalHeight);
    }

    public override void Close(bool destroy = false)
    {
        Sequence closeSequence = DOTween.Sequence().SetUpdate(true);

        for (int i = activeItems.Count - 1; i >= 0; i--)
        {
            if (activeItems[i] != null)
            {
                int index = i;
                closeSequence.Join(activeItems[i].transform.DOScale(0, scaleDuration * 0.3f)
                    .SetEase(closeEase)
                    .SetDelay((activeItems.Count - 1 - index) * 0.02f));
            }
        }

        base.Close(destroy);
        closeSequence.Play();
    }

    private void OnSkipKeyChanged(object value)
    {
        if (value is KeyCode newKey)
        {
            dialogueConfig.skipKey = newKey;
        }
    }

    private void OnTextColorChanged(object value)
    {
        if (value is Color newColor)
        {
            dialogueConfig.defaultTextColor = newColor;
        }
    }

    private void OnFontChanged(object value)
    {
        // 实现字体选择逻辑
    }

    private void OnMusicVolumeChanged(object value)
    {
        if (value is float newVolume)
        {
            SetVolumeToMixer("MusicVolume", newVolume);
        }
    }

    private void OnSFXVolumeChanged(object value)
    {
        if (value is float newVolume)
        {
            SetVolumeToMixer("SFXVolume", newVolume);
        }
    }

    private void OnVoiceVolumeChanged(object value)
    {
        if (value is float newVolume)
        {
            SetVolumeToMixer("VoiceVolume", newVolume);
        }
    }

    private float GetVolumeFromMixer(string parameter)
    {
        if (audioMixer == null)
            return 1f;

        float volume;
        audioMixer.GetFloat(parameter, out volume);
        return Mathf.Pow(10, volume / 20);
    }

    private void SetVolumeToMixer(string parameter, float volume)
    {
        if (audioMixer == null)
            return;

        float dbValue = Mathf.Log10(Mathf.Max(volume, 0.0001f)) * 20;
        audioMixer.SetFloat(parameter, dbValue);
    }

    protected override void OnAfterClose()
    {
        base.OnAfterClose();
        ClearContainer();
    }
}

public enum SettingType
{
    KeyCode,
    Color,
    Font,
    Slider
}
