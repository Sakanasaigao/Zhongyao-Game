using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _test : MonoBehaviour
{
    [SerializeField] private string videoName;
    [SerializeField] private VideoPlayerController videoPlayer;
    [SerializeField] private InputManager inputMr;
    // Start is called before the first frame update
    void Start()
    {
        inputMr.OnSkipKeyPressed += () => Debug.Log("skip");
        videoPlayer.PlayVideo(videoName, true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
