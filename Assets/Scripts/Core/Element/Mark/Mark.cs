using UnityEngine;
using UnityEngine.EventSystems;
using UI;

[RequireComponent(typeof(UI.SimpleHighlightEffect))]
public class Mark : MonoBehaviour, IPointerClickHandler
{
    public int theSceneToJumpTo;
    public Vector3 cameraPositon;
    private const string transitionStyle = "Cloud";
    
    [Header("Highlight Settings")]
    [SerializeField] private bool enableHighlight = true;
    [SerializeField] private float highlightSpeed = 1f;

    private CameraManager CameraManager => CameraManager.instance;
    private UI.SimpleHighlightEffect highlightComponent;

    SceneLoaderManager LoaderManager => SceneLoaderManager.Instance;

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
        LoaderManager.TransitionToScene(transitionStyle, theSceneToJumpTo, 1f, OnSceneTransitionDone);

    }

    private void OnSceneTransitionDone()
    {
        CameraManager.SetCameraPosition(cameraPositon);
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
