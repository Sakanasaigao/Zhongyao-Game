using System.IO;
using UnityEngine;

public class VideoResourceManager : MonoBehaviour
{
    [SerializeField] private string videoFolderName = "Video";

    public string GetVideoPath(string videoName)
    {
        string path = Path.Combine(Application.streamingAssetsPath, videoFolderName, videoName);
        path = path.Replace('\\', '/');
        
        string urlPath = "file://" + path;
        return urlPath;
    }
}
