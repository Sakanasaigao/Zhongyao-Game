using UnityEngine;

[CreateAssetMenu(fileName = "QuizData", menuName = "Decoct/Quiz")]
public class QuizDataSO : ScriptableObject
{
    public string questionText;
    public string[] options = new string[3];
    public int correctAnswerIndex;
}