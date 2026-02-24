using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Text;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class RaycastDebugger : MonoBehaviour
{
    [Header("Debug Settings")]
    [Tooltip("Enable continuous raycast checking (automatically checks when mouse moves)")]
    [SerializeField] private bool enableContinuousCheck = true;

    [Tooltip("Debug key for manual checking (only effective when Continuous Check is false)")]
    [SerializeField] private KeyCode debugKey = KeyCode.LeftShift;

    [Tooltip("Show detailed logs in Console")]
    [SerializeField] private bool showDetailedLog = true;

    [Tooltip("Layers to include in raycast")]
    [SerializeField] private LayerMask raycastLayers = ~0;

    [Header("Visual Settings")]
    [Tooltip("Draw debug lines in Scene view")]
    [SerializeField] private bool drawDebugLines = true;

    [Tooltip("Highlight color for debug visuals")]
    [SerializeField] private Color highlightColor = new Color(1, 0, 0, 0.3f);

    [Tooltip("Background color for GUI display")]
    [SerializeField] private Color guiBackgroundColor = new Color(0, 0, 0, 0.8f);

    private List<RaycastResult> currentResults = new List<RaycastResult>();
    private PointerEventData pointerEventData;
    private EventSystem eventSystem;
    private StringBuilder sb = new StringBuilder();

    private GameObject topmostObject;
    private int topmostSortingOrder = -9999;

    void Start()
    {
        eventSystem = EventSystem.current;
        if (eventSystem == null)
        {
            Debug.LogError("[RaycastDebugger] EventSystem not found!");
            enabled = false;
            return;
        }

        pointerEventData = new PointerEventData(eventSystem);
    }

    void Update()
    {
        if (!enableContinuousCheck && !Input.GetKey(debugKey))
            return;

        PerformRaycast();
    }

    void PerformRaycast()
    {
        pointerEventData.position = Input.mousePosition;

        currentResults.Clear();
        eventSystem.RaycastAll(pointerEventData, currentResults);

        if (currentResults.Count == 0)
        {
            topmostObject = null;
            return;
        }

        AnalyzeResults();

        if (showDetailedLog)
            LogResults();
    }

    void AnalyzeResults()
    {
        topmostObject = currentResults[0].gameObject;
        topmostSortingOrder = GetSortingOrder(topmostObject);

        for (int i = 0; i < currentResults.Count; i++)
        {
            var result = currentResults[i];
            int order = GetSortingOrder(result.gameObject);

            if (i == 0 || order > topmostSortingOrder)
            {
                topmostObject = result.gameObject;
                topmostSortingOrder = order;
            }
        }
    }

    int GetSortingOrder(GameObject obj)
    {
        Canvas canvas = obj.GetComponentInParent<Canvas>();
        if (canvas != null)
        {
            return canvas.sortingOrder;
        }
        return 0;
    }

    void LogResults()
    {
        sb.Clear();
        sb.AppendLine($"=== Raycast Results [{currentResults.Count} objects] ===");
        sb.AppendLine($"Mouse Position: {Input.mousePosition}");
        sb.AppendLine("");

        for (int i = 0; i < currentResults.Count; i++)
        {
            var result = currentResults[i];
            GameObject obj = result.gameObject;
            Canvas canvas = obj.GetComponentInParent<Canvas>();

            string marker = (i == 0) ? "[Top]" : $"    [{i + 1}]";

            sb.AppendLine($"{marker} {obj.name}");
            sb.AppendLine($"    Layer: {LayerMask.LayerToName(obj.layer)}");

            if (canvas != null)
            {
                sb.AppendLine($"    Canvas: {canvas.name}");
                sb.AppendLine($"    Sorting Order: {canvas.sortingOrder}");
                sb.AppendLine($"    Sorting Layer: {canvas.sortingLayerName}");
                sb.AppendLine($"    Override Sorting: {canvas.overrideSorting}");
            }

            var graphic = obj.GetComponent<UnityEngine.UI.Graphic>();
            if (graphic != null)
            {
                sb.AppendLine($"    Raycast Target: {graphic.raycastTarget}");
            }

            var button = obj.GetComponent<UnityEngine.UI.Button>();
            if (button != null) sb.AppendLine($"    Button: {button.interactable}");

            var scrollRect = obj.GetComponent<UnityEngine.UI.ScrollRect>();
            if (scrollRect != null) sb.AppendLine($"    ScrollRect: OK");

            sb.AppendLine("");
        }

        if (topmostObject != null)
        {
            Debug.Log($"<color=cyan>[RaycastDebugger]</color> Topmost Object: <b>{topmostObject.name}</b> (Sorting Order: {topmostSortingOrder})");
        }
    }

    void OnGUI()
    {
        if (currentResults.Count == 0) return;

        float boxWidth = 400;
        float boxHeight = 25 + currentResults.Count * 60;
        Rect boxRect = new Rect(Screen.width - boxWidth - 10, 10, boxWidth, boxHeight);

        GUI.color = guiBackgroundColor;
        GUI.Box(boxRect, "");
        GUI.color = Color.white;

        GUI.Label(new Rect(boxRect.x + 5, boxRect.y + 5, boxWidth, 20),
            $"Raycast [{currentResults.Count}]",
            new GUIStyle(GUI.skin.label) { fontSize = 14, fontStyle = FontStyle.Bold, normal = new GUIStyleState() { textColor = Color.yellow } });

        float yOffset = 30;
        for (int i = 0; i < currentResults.Count && i < 10; i++)
        {
            var result = currentResults[i];
            GameObject obj = result.gameObject;
            Canvas canvas = obj.GetComponentInParent<Canvas>();

            Color textColor = (i == 0) ? Color.green : Color.gray;
            string prefix = (i == 0) ? ">> " : "  ";

            GUIStyle style = new GUIStyle(GUI.skin.label);
            style.normal.textColor = textColor;
            style.fontSize = 12;

            GUI.Label(new Rect(boxRect.x + 5, boxRect.y + yOffset, boxWidth - 10, 20),
                $"{prefix}{obj.name}", style);

            string detail = "";
            if (canvas != null) detail += $"Order:{canvas.sortingOrder} ";
            detail += $"Layer:{LayerMask.LayerToName(obj.layer)}";

            GUIStyle detailStyle = new GUIStyle(GUI.skin.label);
            detailStyle.normal.textColor = new Color(0.7f, 0.7f, 0.7f);
            detailStyle.fontSize = 10;

            GUI.Label(new Rect(boxRect.x + 15, boxRect.y + yOffset + 15, boxWidth - 20, 15),
                detail, detailStyle);

            yOffset += 40;
        }

        if (currentResults.Count > 10)
        {
            GUI.Label(new Rect(boxRect.x + 5, boxRect.y + yOffset, boxWidth, 20),
                $"... and {currentResults.Count - 10} more objects",
                new GUIStyle(GUI.skin.label) { normal = new GUIStyleState() { textColor = Color.gray } });
        }
    }

    void OnDrawGizmos()
    {
        if (!drawDebugLines || currentResults.Count == 0) return;
        if (Camera.main == null) return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(ray.origin, ray.origin + ray.direction * 100);

        foreach (var result in currentResults)
        {
            if (result.gameObject == null) continue;

            Gizmos.color = (result.gameObject == topmostObject) ? Color.red : Color.blue;

            RectTransform rt = result.gameObject.GetComponent<RectTransform>();
            if (rt != null)
            {
                Vector3[] corners = new Vector3[4];
                rt.GetWorldCorners(corners);

                Gizmos.DrawLine(corners[0], corners[1]);
                Gizmos.DrawLine(corners[1], corners[2]);
                Gizmos.DrawLine(corners[2], corners[3]);
                Gizmos.DrawLine(corners[3], corners[0]);
            }
        }
    }

    public GameObject GetTopmostUI()
    {
        return topmostObject;
    }

    public List<RaycastResult> GetCurrentResults()
    {
        return new List<RaycastResult>(currentResults);
    }

    public bool IsBlockingRaycast(GameObject obj)
    {
        return currentResults.Exists(r => r.gameObject == obj);
    }
}

#if UNITY_EDITOR

[CustomEditor(typeof(RaycastDebugger))]
public class RaycastDebuggerEditor : UnityEditor.Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Debug Actions", EditorStyles.boldLabel);

        if (GUILayout.Button("Perform Raycast"))
        {
            var debugger = target as RaycastDebugger;
            var method = typeof(RaycastDebugger).GetMethod("PerformRaycast",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            method?.Invoke(debugger, null);
        }

        EditorGUILayout.HelpBox(
            "Instructions:\n" +
            "1. Automatic raycast checks on mouse move\n" +
            "2. Hold Debug Key for manual checks (when Continuous is disabled)\n" +
            "3. Check Console for detailed information\n" +
            "4. Real-time list displayed in top-right corner\n" +
            "5. Scene view: Yellow=ray, Red=top object, Blue=others",
            MessageType.Info);
    }
}

#endif