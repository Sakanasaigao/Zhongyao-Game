using System;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Video;

public class VideoResourceManager : MonoBehaviour
{
    [SerializeField] private string videoFolderName = "Video";

    public VideoClip LoadVideoClipFromResources(string videoName)
    {
        string path = Path.Combine(videoFolderName, videoName);
        VideoClip clip = Resources.Load<VideoClip>(path);
        return clip;
    }

    public string GetVideoPathFromResources(string videoName)
    {
        string path = Path.Combine(Application.dataPath, "Resources", videoFolderName, videoName);
        return path;
    }

    public string GetStreamingAssetsVideoPath(string videoName)
    {
        string path = Path.Combine(Application.streamingAssetsPath, videoFolderName, videoName);

#if UNITY_ANDROID && !UNITY_EDITOR
        return path;
#else
        return "file://" + path;
#endif
    }

    public bool VideoExists(string videoName)
    {
        string path = Path.Combine(Application.streamingAssetsPath, videoFolderName, videoName);
        return File.Exists(path);
    }

    public string[] GetAvailableVideoFiles()
    {
        string directory = Path.Combine(Application.streamingAssetsPath, videoFolderName);

        if (Directory.Exists(directory))
        {
            return Directory.GetFiles(directory, "*.mp4");
        }

        return new string[0];
    }

    public void PreloadVideo(string videoName, Action onComplete = null)
    {
        StartCoroutine(PreloadVideoCoroutine(videoName, onComplete));
    }

    private System.Collections.IEnumerator PreloadVideoCoroutine(string videoName, Action onComplete)
    {
        string path = GetStreamingAssetsVideoPath(videoName);
        UnityWebRequest www = UnityWebRequest.Get(path);

        yield return www.SendWebRequest();

        onComplete?.Invoke();
    }

    public void SetVideoQuality(VideoPlayer videoPlayer, int width, int height, int bitrate)
    {
        videoPlayer.targetCameraAlpha = 1.0f;
    }
}
