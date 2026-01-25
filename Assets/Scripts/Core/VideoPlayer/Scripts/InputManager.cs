using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    [SerializeField] private KeyCode skipKey = KeyCode.Escape;
    [SerializeField] private float holdThreshold = 0.3f;
    [SerializeField] private float mouseStayThreshold = 3f;

    private bool isHoldingSkipKey;
    private float holdStartTime;
    private float mouseStayTime;
    private bool wasPressedLastFrame;
    private Vector3 lastMousePosition;

    public Action OnSkipKeyPressed;
    public Action OnSkipKeyReleased;
    public Action OnSkipKeyLongPressed;
    public Action OnMouseMove;
    public Action OnMouseStay;
    public Action<float> OnSkipKeyHoldProgress;

    private void Update()
    {
        bool isPressed = Input.GetKey(skipKey);

        if (isPressed && !wasPressedLastFrame)
        {
            OnSkipKeyDown();
        }
        else if (!isPressed && wasPressedLastFrame)
        {
            OnSkipKeyUp();
        }

        if (isHoldingSkipKey)
        {
            float holdDuration = Time.time - holdStartTime;
            OnSkipKeyHoldProgress?.Invoke(holdDuration);

            if (holdDuration >= holdThreshold && !hasTriggeredLongPress)
            {
                OnSkipKeyLongPressed?.Invoke();
                hasTriggeredLongPress = true;
            }
        }

        if (Input.mousePosition != lastMousePosition)
        {
            OnMouseMove?.Invoke();
            lastMousePosition = Input.mousePosition;
            mouseStayTime = 0f;
        }

        if (Input.mousePosition == lastMousePosition)
        {
            mouseStayTime += Time.deltaTime;

            if (mouseStayTime > mouseStayThreshold)
            {
                OnMouseStay?.Invoke();
            }
        }

        wasPressedLastFrame = isPressed;
    }

    private void OnSkipKeyDown()
    {
        isHoldingSkipKey = true;
        holdStartTime = Time.time;
        hasTriggeredLongPress = false;
        OnSkipKeyPressed?.Invoke();
    }

    private void OnSkipKeyUp()
    {
        isHoldingSkipKey = false;
        OnSkipKeyReleased?.Invoke();
    }

    private bool hasTriggeredLongPress;

    public bool IsHoldingSkipKey => isHoldingSkipKey;
    public float GetHoldDuration() => isHoldingSkipKey ? Time.time - holdStartTime : 0f;
}
