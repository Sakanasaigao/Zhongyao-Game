using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "QuizBank", menuName = "Decoct/QuizBank")]
public class QuizBankSO : ScriptableObject
{
    public QuizDataSO[] quizzes;

    private HashSet<QuizDataSO> _usedQuizzes = new();

    public QuizDataSO GetRandomQuiz()
    {
        var available = quizzes.Where(q => !_usedQuizzes.Contains(q)).ToList();
        if (available.Count == 0)
        {
            _usedQuizzes.Clear();
            available = quizzes.ToList();
        }
        var quiz = available[Random.Range(0, available.Count)];
        _usedQuizzes.Add(quiz);
        return quiz;
    }

    public void ResetUsedQuizzes()
    {
        _usedQuizzes.Clear();
    }
}