using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using COMMANDS;
using DIALOGUE;

[RequireComponent(typeof(VideoPlayer))]
public class PlayVideo : MonoBehaviour
{
    [Tooltip("��Ƶ������ɺ���õķ���")]
    public UnityEngine.Events.UnityEvent onVideoFinished;

    private const string FirstScriptID = "11";
    private const float P_KEY_HOLD_DURATION = 3f; // 按住P键跳过CG的持续时间

    private VideoPlayer videoPlayer;
    private float pKeyHoldTime = 0f; // 按住P键的时间

    void Start()
    {
        videoPlayer = GetComponent<VideoPlayer>();

        CommandManager.instance.Execute("StopSong");

        videoPlayer.loopPointReached += OnVideoEnd;

        videoPlayer.Play();
    }

    private void OnVideoEnd(VideoPlayer source)
    {
        onVideoFinished?.Invoke();
    }

    void OnDestroy()
    {
        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached -= OnVideoEnd;
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

    void Update()
    {
        // 检查视频是否正在播放
        if (!videoPlayer.isPlaying) return;

        // 检查P键是否被按住
        if (Input.GetKey(KeyCode.P))
        {
            // 累加按住时间
            pKeyHoldTime += Time.deltaTime;
            
            // 当按住时间达到3秒时，跳过CG
            if (pKeyHoldTime >= P_KEY_HOLD_DURATION)
            {
                Debug.Log("P键按住3秒，跳过CG");
                TurnScene(); // 调用跳转到下一场景的方法
                pKeyHoldTime = 0f; // 重置时间
            }
        }
        else
        {
            // 松开P键时，重置时间
            pKeyHoldTime = 0f;
        }
    }
}
