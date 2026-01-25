using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class LayoutFixer : MonoBehaviour
    {
        void Awake()
        {
            // 强制所有Canvas重新计算布局，解决EndLayoutGroup错误
            Canvas.ForceUpdateCanvases();
        }
    }
}