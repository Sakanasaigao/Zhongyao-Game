using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoPlayerController : MonoBehaviour
{
    [SerializeField] private RawImage videoDisplay;
    [SerializeField] private RenderTexture renderTexture;
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private CanvasGroup videoCanvasGroup;
    [SerializeField] private CanvasGroup backgroundCanvasGroup;
    [SerializeField] private VideoResourceManager videoRecManager;
    [SerializeField] private float fadeDuration = 1.0f;

    public Action OnVideoStart;
    public Action OnVideoComplete;
    public Action<string> OnVideoError;

    private bool isPrepared;
    private bool isPlaying;
    private bool canSkip;
    private string currentVideoPath;

    private void Awake()
    {
        if (videoPlayer == null) videoPlayer = GetComponent<VideoPlayer>();
        if (audioSource == null) audioSource = GetComponent<AudioSource>();

        videoPlayer.playOnAwake = false;
        videoPlayer.waitForFirstFrame = true;
        videoPlayer.renderMode = VideoRenderMode.RenderTexture;
        videoPlayer.targetTexture = renderTexture;
        videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;
        videoPlayer.SetTargetAudioSource(0, audioSource);

        videoPlayer.prepareCompleted += OnVideoPrepared;
        videoPlayer.loopPointReached += OnVideoFinished;
        videoPlayer.errorReceived += OnVideoErrorReceived;
    }

    public void InitializeVideoSystem()
    {
        videoDisplay.texture = renderTexture;
        isPrepared = false;
        isPlaying = false;
        canSkip = false;
        backgroundCanvasGroup.alpha = 1f;
        videoCanvasGroup.alpha = 0f;
    }

    public void PlayVideo(string videoName, bool _canSkip)
    {
        currentVideoPath = videoRecManager.GetVideoPathFromResources(videoName);

        videoPlayer.url = currentVideoPath;
        videoPlayer.Prepare();

        isPrepared = false;
        isPlaying = false;
        canSkip = _canSkip;
    }

    public void PauseVideo()
    {
        if (videoPlayer.isPlaying)
        {
            videoPlayer.Pause();
            isPlaying = false;
        }
    }

    public void ResumeVideo()
    {
        if (videoPlayer.isPrepared && !videoPlayer.isPlaying)
        {
            videoPlayer.Play();
            isPlaying = true;
        }
    }

    public void StopVideo()
    {
        videoPlayer.Stop();
        isPrepared = false;
        isPlaying = false;
    }

    public void SkipAndClose()
    {
        StopVideo();
        FadeOut(0.3f);
    }

    public void SkipVideo()
    {
        // StopVideo();
        OnVideoComplete?.Invoke();
    }

    public void volumeFadeOut(float duration)
    {
        DOVirtual.Float(1f, 0f, duration * 0.7f, (volume) =>
        {
            SetVolume(volume);
        });
    }

    public void SetVolume(float volume)
    {
        audioSource.volume = Mathf.Clamp01(volume);
    }

    private void OnVideoPrepared(VideoPlayer source)
    {
        isPrepared = true;
        isPlaying = true;
        source.Play();

        OnVideoStart?.Invoke();
        FadeIn(fadeDuration);
    }

    private void OnVideoFinished(VideoPlayer source)
    {
        isPlaying = false;
        isPrepared = false;
        OnVideoComplete?.Invoke();
    }

    private void OnVideoErrorReceived(VideoPlayer source, string message)
    {
        OnVideoError?.Invoke(message);
    }

    public void FadeIn(float duration)
    {
        StartCoroutine(FadeCanvasGroup(videoCanvasGroup, 0f, 1f, duration));
        StartCoroutine(FadeCanvasGroup(backgroundCanvasGroup, 1f, 0f, duration));
    }

    public void FadeOut(float duration)
    {
        StartCoroutine(FadeCanvasGroup(videoCanvasGroup, 1f, 0f, duration));
        StartCoroutine(FadeCanvasGroup(backgroundCanvasGroup, 0f, 1f, duration));
    }

    private System.Collections.IEnumerator FadeCanvasGroup(CanvasGroup canvasGroup, float startAlpha, float endAlpha, float duration)
    {
        float elapsedTime = 0f;
        canvasGroup.gameObject.SetActive(true);

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / duration);
            yield return null;
        }

        canvasGroup.alpha = endAlpha;
        if (endAlpha == 0f) canvasGroup.gameObject.SetActive(false);
    }

    public bool IsPlaying => isPlaying;
    public bool IsPrepared => isPrepared;
}
