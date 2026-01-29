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
    [SerializeField] private VideoResourceManager videoRecManager;

    public Action OnVideoComplete;
    public Action<string> OnVideoError;

    private void Awake()
    {
        if (videoPlayer == null) videoPlayer = GetComponent<VideoPlayer>();
        if (audioSource == null) audioSource = GetComponent<AudioSource>();

        videoPlayer.playOnAwake = false;
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
        if (videoDisplay != null && renderTexture != null)
        {
            videoDisplay.texture = renderTexture;
        }
    }

    public void PlayVideo(string videoName, bool canSkip)
    {
        if (videoRecManager == null)
        {
            Debug.LogError("VideoRecManager is null");
            OnVideoError?.Invoke("VideoRecManager is null");
            return;
        }

        string videoPath = videoRecManager.GetVideoPath(videoName);
        videoPlayer.url = videoPath;
        videoPlayer.Prepare();
    }

    public void volumeFadeOut(float duration)
    {
        DOVirtual.Float(1f, 0f, duration * 0.7f, (volume) =>
        {
            audioSource.volume = Mathf.Clamp01(volume);
        });
    }

    public void SkipVideo()
    {
        OnVideoComplete?.Invoke();
    }

    private void OnVideoPrepared(VideoPlayer source)
    {
        source.Play();
    }

    private void OnVideoFinished(VideoPlayer source)
    {
        OnVideoComplete?.Invoke();
    }

    private void OnVideoErrorReceived(VideoPlayer source, string message)
    {
        Debug.LogError("Video error: " + message);
        OnVideoError?.Invoke(message);
    }
}
