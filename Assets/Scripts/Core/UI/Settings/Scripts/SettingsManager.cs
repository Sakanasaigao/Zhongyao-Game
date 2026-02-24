using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    public void OpenSettings()
    {
        UIManager.Instance.OpenPanel<SettingsSystem>();
    }
}
