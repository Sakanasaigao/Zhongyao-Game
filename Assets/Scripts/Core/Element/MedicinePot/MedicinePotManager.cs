using System.Collections.Generic;
using UnityEngine;

public class MedicinePotManager : MonoBehaviour
{
    public static MedicinePotManager Instance { get; private set; }

    [SerializeField] private GameObject ykgrPrefab;
    [SerializeField] private string resetAnimationName = "MedicinePot_Reset";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public MedicinePot CreateMedicinePot(
        string[] targetMedicineNames,
        string[] allMedicineNames,
        Vector3 position,
        string ScriptID
    )
    {
        if (ykgrPrefab == null)
        {
            Debug.LogError("ҩ��Ԥ����δ���䣡");
            return null;
        }
        Debug.Log(allMedicineNames.Length + "diergekdjfie");

        GameObject medicinePot = Instantiate(ykgrPrefab, position, Quaternion.identity);
        MedicinePot medicinePotScript = medicinePot.GetComponent<MedicinePot>();

        if (medicinePotScript != null)
        {
            int minLength = Mathf.Min(targetMedicineNames.Length, allMedicineNames.Length);
            string[] validTargets = new string[minLength];
            string[] validPrefabs = new string[minLength];

            System.Array.Copy(targetMedicineNames, validTargets, minLength);
            System.Array.Copy(allMedicineNames, validPrefabs, minLength);

            medicinePotScript.Initialize(targetMedicineNames, allMedicineNames, ScriptID);
            return medicinePotScript;
        }
        else
        {
            Debug.LogError("MedicinePot�ű�δ���ӵ�Ԥ���壡");
            Destroy(medicinePot);
            return null;
        }
    }
}