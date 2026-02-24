using UnityEngine;
using UnityEditor;
using System.Reflection;
using System.Linq;

public class MethodExecutor : MonoBehaviour
{
    public GameObject target;
    public Component targetComponent;
    public string methodName;
    public bool execute;
}

#if UNITY_EDITOR
[CustomEditor(typeof(MethodExecutor))]
public class MethodExecutorEditor : UnityEditor.Editor
{
    SerializedProperty targetProp;
    SerializedProperty componentProp;
    SerializedProperty methodProp;
    SerializedProperty executeProp;

    GameObject lastTarget;
    Component[] components;
    string[] componentNames;
    int selectedComponentIndex;

    string[] methodNames;
    int selectedMethodIndex;

    void OnEnable()
    {
        targetProp = serializedObject.FindProperty("target");
        componentProp = serializedObject.FindProperty("targetComponent");
        methodProp = serializedObject.FindProperty("methodName");
        executeProp = serializedObject.FindProperty("execute");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(targetProp);

        GameObject currentTarget = targetProp.objectReferenceValue as GameObject;

        if (currentTarget != lastTarget)
        {
            RefreshComponents(currentTarget);
            lastTarget = currentTarget;
        }

        if (currentTarget != null && components != null && components.Length > 0)
        {
            selectedComponentIndex = EditorGUILayout.Popup("Component", selectedComponentIndex, componentNames);

            if (selectedComponentIndex >= 0 && selectedComponentIndex < components.Length)
            {
                Component selectedComp = components[selectedComponentIndex];
                componentProp.objectReferenceValue = selectedComp;

                if (GUI.changed || methodNames == null)
                {
                    RefreshMethods(selectedComp);
                }

                if (methodNames != null && methodNames.Length > 0)
                {
                    selectedMethodIndex = EditorGUILayout.Popup("Method", selectedMethodIndex, methodNames);
                    if (selectedMethodIndex >= 0 && selectedMethodIndex < methodNames.Length)
                    {
                        methodProp.stringValue = methodNames[selectedMethodIndex];
                    }
                }
            }
        }

        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(executeProp, new GUIContent("Execute"));
        if (EditorGUI.EndChangeCheck())
        {
            if (executeProp.boolValue)
            {
                ExecuteMethod();
                executeProp.boolValue = false;
            }
        }

        serializedObject.ApplyModifiedProperties();
    }

    void RefreshComponents(GameObject target)
    {
        if (target == null)
        {
            components = null;
            return;
        }

        components = target.GetComponents<MonoBehaviour>();
        componentNames = components.Select(c => c.GetType().Name).ToArray();
        selectedComponentIndex = 0;

        if (componentProp.objectReferenceValue != null)
        {
            for (int i = 0; i < components.Length; i++)
            {
                if (components[i] == componentProp.objectReferenceValue)
                {
                    selectedComponentIndex = i;
                    break;
                }
            }
        }

        methodNames = null;
    }

    void RefreshMethods(Component comp)
    {
        if (comp == null) return;

        MethodInfo[] methods = comp.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Where(m => !m.IsSpecialName && m.GetParameters().Length == 0)
            .ToArray();

        methodNames = methods.Select(m => m.Name).ToArray();

        if (methodProp.stringValue != null)
        {
            for (int i = 0; i < methodNames.Length; i++)
            {
                if (methodNames[i] == methodProp.stringValue)
                {
                    selectedMethodIndex = i;
                    return;
                }
            }
        }

        selectedMethodIndex = 0;
    }

    void ExecuteMethod()
    {
        Component comp = componentProp.objectReferenceValue as Component;
        string method = methodProp.stringValue;

        if (comp != null && !string.IsNullOrEmpty(method))
        {
            MethodInfo methodInfo = comp.GetType().GetMethod(method);
            if (methodInfo != null)
            {
                methodInfo.Invoke(comp, null);
            }
        }
    }
}
#endif