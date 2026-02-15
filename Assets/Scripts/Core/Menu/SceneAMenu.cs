using System.Collections;
using System.Collections.Generic;
using ARCHIVE;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

public class SceneAMenu : MonoBehaviour
{
    public static SceneAMenu Instance { get; private set; }

    [SerializeField] private GameObject menuPanel;
    [SerializeField] private GameObject mountain;
    [SerializeField] private string transationStyle;

    private void Awake()
    {
        Instance = this;
    }

    public void OpenMenuPanel()
    {
        menuPanel.SetActive(true);
    }
    public void CloseMenuPanel()
    {
        menuPanel.SetActive(false);
    }

    public void TurnToMainMenu()
    {
        if (SceneLoaderManager.Instance == null)
        {
            Debug.LogWarning("SceneLoaderManager instance not found, skipping scene transition");
            return;
        }

        if (string.IsNullOrEmpty(transationStyle))
        {
            Debug.LogWarning("Transition style not set, skipping scene transition");
            return;
        }

        SceneLoaderManager.Instance.TransitionToScene(transationStyle, 0);

        if (ArchivingManager.Instance != null)
        {
            ArchivingManager.Instance.Save();
        }
        else
        {
            Debug.LogWarning("ArchivingManager instance not found, skipping save");
        }
    }

    public void TurnToMountain()
    {
        if (SceneLoaderManager.Instance == null)
        {
            Debug.LogWarning("SceneLoaderManager instance not found, skipping scene transition");
            return;
        }

        if (string.IsNullOrEmpty(transationStyle))
        {
            Debug.LogWarning("Transition style not set, skipping scene transition");
            return;
        }

        SceneLoaderManager.Instance.TransitionToScene(transationStyle, 2);

        if (ArchivingManager.Instance != null)
        {
            ArchivingManager.Instance.Save();
        }
        else
        {
            Debug.LogWarning("ArchivingManager instance not found, skipping save");
        }
    }

    public void CloseMountain()
    {
        mountain.SetActive(false);
    }

    public void OpenMountain()
    {
        mountain.SetActive(true);
    }
}
