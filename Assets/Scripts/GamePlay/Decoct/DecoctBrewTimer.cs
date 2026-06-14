using System;
using UnityEngine;
using TMPro;
using Zenject;

public class DecoctBrewTimer : MonoBehaviour
{
    public event Action OnTimerExpired;

    [Inject(Id = "TimerSlider")] private UnityEngine.UI.Slider progressSlider;
    [Inject(Id = "TimerCountdownText")] private TMP_Text countdownText;

    private float totalTime;
    private float remainingTime;
    private bool isRunning;
    private bool isPaused;

    public float RemainingTime => remainingTime;
    public bool IsRunning => isRunning;

    public void StartTimer(float seconds)
    {
        totalTime = seconds;
        remainingTime = seconds;
        isRunning = true;
        isPaused = false;

        if (progressSlider != null)
        {
            progressSlider.maxValue = totalTime;
            progressSlider.value = totalTime;
        }

        UpdateDisplay();
    }

    public void Pause()
    {
        isPaused = true;
    }

    public void Resume()
    {
        isPaused = false;
    }

    public void ReduceTime(float seconds)
    {
        remainingTime -= seconds;
        if (remainingTime < 0)
            remainingTime = 0;

        UpdateDisplay();

        if (remainingTime <= 0 && isRunning)
        {
            StopTimer();
            OnTimerExpired?.Invoke();
        }
    }

    public void StopTimer()
    {
        isRunning = false;
    }

    void Update()
    {
        if (!isRunning || isPaused)
            return;

        remainingTime -= Time.deltaTime;

        if (remainingTime <= 0)
        {
            remainingTime = 0;
            isRunning = false;
            OnTimerExpired?.Invoke();
        }

        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        if (progressSlider != null)
            progressSlider.value = remainingTime;

        if (countdownText != null)
        {
            int seconds = Mathf.CeilToInt(remainingTime);
            countdownText.text = $"{seconds}s";
        }
    }
}