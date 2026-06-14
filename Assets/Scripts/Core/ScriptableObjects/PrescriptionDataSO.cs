using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PrescriptionData", menuName = "Decoct/Prescription")]
public class PrescriptionDataSO : ScriptableObject
{
    public string prescriptionName;
    public List<string> requiredHerbs;
    public int distractorCount = 3;
    public float brewingTime = 30f;
    public float timeReductionPerCorrect = 15f;
}