using UnityEngine;
using UnityEngine.EventSystems;

public class Mark : MonoBehaviour, IPointerClickHandler
{
    public int theSceneToJumpTo;
    public Vector3 cameraPositon;
    private const string transitionStyle = "Cloud";
    
    [Header("Highlight Settings")]
    [SerializeField] private bool enableHighlight = true;
    [SerializeField] private float highlightSpeed = 1f;

    private CameraManager CameraManager => CameraManager.instance;
    // 移除对SimpleHighlightEffect的依赖

    SceneLoaderManager LoaderManager => SceneLoaderManager.Instance;

    private void Awake()
    {
        // 移除对SimpleHighlightEffect的依赖
        Debug.Log("Mark: Animation functionality disabled");
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        LoaderManager.TransitionToScene(transitionStyle, theSceneToJumpTo, 1f, OnSceneTransitionDone);

    }

    private void OnSceneTransitionDone()
    {
        CameraManager.SetCameraPosition(cameraPositon);
    }

    public void UnlockThisMark()
    {
        // 禁用动画功能
        Debug.Log("Mark: Animation functionality disabled");
    }
    
    public void StartHighlight()
    {
        // 禁用动画功能
        Debug.Log("Mark: Animation functionality disabled");
    }

    public void StopHighlight()
    {
        // 禁用动画功能
        Debug.Log("Mark: Animation functionality disabled");
    }
    
    public bool IsHighlighted()
    {
        // 禁用动画功能
        return false;
    }
}
