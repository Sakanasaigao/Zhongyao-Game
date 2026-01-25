using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using COMMANDS;
using DIALOGUE;

[RequireComponent(typeof(VideoPlayer))]
public class PlayVideo : MonoBehaviour
{
    [Tooltip("Do on video finished")]
    public UnityEngine.Events.UnityEvent onVideoFinished;

    [SerializeField] private GameObject videoPlayer;

    private const string FirstScriptID = "11";
    private VideoPlayerController videoPlayerController;

    void Start()
    {
        videoPlayerController = videoPlayer.GetComponent<VideoPlayerController>();

        videoPlayerController.OnVideoComplete += OnVideoEnd;
        CommandManager.instance.Execute("StopSong");

        videoPlayerController.PlayVideo("01.mp4", true);
    }

    private void OnVideoEnd()
    {
        onVideoFinished?.Invoke();
    }

    void OnDestroy()
    {
        if (videoPlayerController != null)
        {
            videoPlayerController.OnVideoComplete -= OnVideoEnd;
        }
    }

    public void TurnScene()
    {
        SceneLoaderManager.Instance.TransitionToScene("Cloud", 1, 1.5f, StartEvent);
    }

    private void StartEvent()
    {
        CommandManager.instance.Execute("startdialogue", "-f", FirstScriptID);
    }
}
