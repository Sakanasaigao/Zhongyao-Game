using System.Collections;
using System.Collections.Generic;
using DIALOGUE;
using UnityEngine;

public class PlayMusic : MonoBehaviour
{
    public AudioClip AudioClip;
    public bool loop;
    public float volume = 1;
    private string musicName => AudioClip.name;
    private string musicLoop => loop.ToString();
    private string musicVolume => volume.ToString();

    void Start()
    {
        if (CommandManager.instance != null && AudioClip != null)
        {
            CommandManager.instance.Execute("PlaySong", "-s", musicName, "-l", musicLoop, "-v", musicVolume);
        }
        else
        {
            Debug.LogWarning("CommandManager instance or AudioClip is null, cannot play music.");
        }
    }
}
