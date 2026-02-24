using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class SceneSwitcherWindow : EditorWindow
{
    private Vector2 scrollPosition;
    private string[] scenePaths;
    private string[] sceneNames;

    [MenuItem("Tools/Scene Switcher", false, 100)]
    public static void ShowWindow()
    {
        GetWindow<SceneSwitcherWindow>("Scenes");
    }

    private void OnEnable()
    {
        RefreshSceneList();
    }

    private void OnGUI()
    {
        if (GUILayout.Button("Refresh", GUILayout.Height(25)))
        {
            RefreshSceneList();
        }

        EditorGUILayout.Space(5);

        if (sceneNames == null || sceneNames.Length == 0)
        {
            EditorGUILayout.LabelField("No scenes found in Assets/Scenes");
            return;
        }

        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        for (int i = 0; i < sceneNames.Length; i++)
        {
            EditorGUILayout.BeginHorizontal();

            GUIStyle style = new GUIStyle(GUI.skin.button);
            if (IsCurrentScene(scenePaths[i]))
            {
                style.normal.textColor = Color.green;
                style.fontStyle = FontStyle.Bold;
            }

            if (GUILayout.Button(sceneNames[i], style, GUILayout.Height(30)))
            {
                SwitchScene(scenePaths[i]);
            }

            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.EndScrollView();
    }

    private void RefreshSceneList()
    {
        string[] guids = AssetDatabase.FindAssets("t:SceneAsset", new[] { "Assets/Scenes" });
        scenePaths = new string[guids.Length];
        sceneNames = new string[guids.Length];

        for (int i = 0; i < guids.Length; i++)
        {
            scenePaths[i] = AssetDatabase.GUIDToAssetPath(guids[i]);
            sceneNames[i] = System.IO.Path.GetFileNameWithoutExtension(scenePaths[i]);
        }

        System.Array.Sort(sceneNames, scenePaths);
    }

    private bool IsCurrentScene(string path)
    {
        return EditorSceneManager.GetActiveScene().path == path;
    }

    private void SwitchScene(string path)
    {
        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            EditorSceneManager.OpenScene(path);
        }
    }
}