using System;
using System.Collections;
using System.Collections.Generic;
using ITEMS;
using UnityEditor.Rendering;
using UnityEngine;

public class MedicineDisplayManager : MonoBehaviour
{
    public static MedicineDisplayManager instance {  get; private set; }

    [SerializeField]
    private GameObject MedicineDisplayPrefab;
    [SerializeField]
    private Transform spawnParent;

    private void Awake()
    {
        instance = this;
    }

    public void InitializeMedicineDisplay(string medicineName)
    {
        var medicineInfo = ItemsManager.instance.GetItemInfo(medicineName);

        string name = medicineInfo.config.name;
        string description = medicineInfo.config.characteristic;
        Sprite image = null;

        // 添加日志调试
        Debug.Log($"Loading medicine: {medicineName}");
        Debug.Log($"Image path: {ItemsManager.instance.GetItemImagePath(medicineName)}");
        
        Sprite[] images = Resources.LoadAll<Sprite>(ItemsManager.instance.GetItemImagePath(medicineName));
        if (images != null && images.Length > 0)
        {
            image = images[0];
            Debug.Log($"Found image: {image.name}");
        }
        else
        {
            Debug.LogError($"No images found for {medicineName} at path {ItemsManager.instance.GetItemImagePath(medicineName)}");
        }

        GameObject newMedicineDisplay = Instantiate(MedicineDisplayPrefab, spawnParent);
        MedicineDisplay displayer = newMedicineDisplay.GetComponent<MedicineDisplay>();

        if (displayer != null)
        {
            displayer.Initialize(name, description, image);
            displayer.Show();
        }
        else
        {
            Debug.LogError("MedicineDisplay component missing on prefab");
            Destroy(newMedicineDisplay);
        }
    }
}