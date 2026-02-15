using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    [Header("鼠标移动时显示UI")]
    [SerializeField] float showUIAlpha = 0.7f;
    [SerializeField] float showUITime = 3f;
    [SerializeField] float fadeDuration = 1;

    [Header("跳过键按下时")]
    [SerializeField] float skipThreshold = 3f;

    [Header("Reference")]
    [SerializeField] private InputManager input;
    [SerializeField] private VideoPlayerUIManager uiManager;
    [SerializeField] private VideoPlayerController videoPlayer;

    private float skipPressTime;
    private bool isSkipped = false;

    void Start()
    {
        skipPressTime = 0f;
        input.OnMouseMove += () => uiManager.ShowUITemporarily(showUIAlpha, showUITime, fadeDuration);
    }

    void Update()
    {
        if (input.IsHoldingSkipKey)
        {
            skipPressTime += Time.deltaTime;
            uiManager.ShowUITemporarily(showUIAlpha, showUITime, fadeDuration);
        }
        else
        {
            skipPressTime = 0f;
        }

        if (skipPressTime > skipThreshold)
        {
            if (!isSkipped)
            {
                videoPlayer.SkipVideo();
                uiManager.FadeOutAndDestroyRoot(1f);

                isSkipped = true;
            }
        }

        uiManager.HandleSkipIcon(skipThreshold, skipPressTime);
    }
}
