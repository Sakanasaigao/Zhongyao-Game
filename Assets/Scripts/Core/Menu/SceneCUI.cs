using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ARCHIVE;

public class SceneCUI : MonoBehaviour
{
    public void Return(int sceneIndex)
    {
        // 保存当前游戏进度
        ArchivingManager.Instance.Save();
        
        SceneLoaderManager.Instance.TransitionToScene("Cloud", sceneIndex);
    }
}
