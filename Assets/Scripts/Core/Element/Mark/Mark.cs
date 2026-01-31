using UnityEngine;
using UnityEngine.EventSystems;
using UI;

public class Mark : MonoBehaviour, IPointerClickHandler
{
    public int theSceneToJumpTo;
    public Vector3 cameraPositon;
    private string transitionStyle = "Cloud";
    
    [Header("Highlight Settings")]
    [SerializeField] private bool enableHighlight = true;
    [SerializeField] private Color highlightColor = Color.yellow;
    [SerializeField] private float highlightSpeed = 1f;

    private CameraManager cameraManager => CameraManager.instance;
    private UI.SimpleHighlightEffect highlightComponent;

    SceneLoaderManager loaderManager => SceneLoaderManager.Instance;

    private void Awake()
    {
        highlightComponent = GetComponent<UI.SimpleHighlightEffect>();
        if (highlightComponent == null)
        {
            highlightComponent = gameObject.AddComponent<UI.SimpleHighlightEffect>();
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        loaderManager.TransitionToScene(transitionStyle, theSceneToJumpTo, 1f, OnSceneTransitionDone);

    }

    private void OnSceneTransitionDone()
    {
        cameraManager.SetCameraPosition(cameraPositon);
    }

    public void UnlockThisMark()
    {
        if (enableHighlight)
        {
            StartHighlight();
        }
    }

    public void StartHighlight()
    {
        if (highlightComponent == null)
        {
            highlightComponent = gameObject.AddComponent<UI.SimpleHighlightEffect>();
        }
        
        highlightComponent.SetHighlightColor(Color.yellow);
        highlightComponent.SetAnimationSpeed(2f);
        highlightComponent.StartHighlight();
    }

    public void StopHighlight()
    {
        if (highlightComponent != null)
        {
            highlightComponent.StopHighlight();
        }
    }

    public bool IsHighlighted()
    {
        return highlightComponent != null;
    }
}
