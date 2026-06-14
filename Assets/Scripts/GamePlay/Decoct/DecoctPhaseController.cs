using System;
using System.Collections;
using UnityEngine;
using Zenject;

public class DecoctPhaseController : MonoBehaviour
{
    public event Action<QuizDataSO> OnQuizScheduled;

    [Inject] private QuizBankSO quizBank;

    private float minQuizInterval = 10f;
    private float maxQuizInterval = 20f;

    private Coroutine schedulingCoroutine;
    private bool isPaused;

    public void StartScheduling()
    {
        isPaused = false;
        quizBank.ResetUsedQuizzes();
        schedulingCoroutine = StartCoroutine(QuizSchedulingLoop());
    }

    public void PauseScheduling()
    {
        isPaused = true;
    }

    public void ResumeScheduling()
    {
        isPaused = false;
    }

    public void StopScheduling()
    {
        if (schedulingCoroutine != null)
        {
            StopCoroutine(schedulingCoroutine);
            schedulingCoroutine = null;
        }
    }

    private IEnumerator QuizSchedulingLoop()
    {
        while (true)
        {
            float interval = UnityEngine.Random.Range(minQuizInterval, maxQuizInterval);
            yield return new WaitForSeconds(interval);

            if (isPaused)
            {
                yield return new WaitWhile(() => isPaused);
            }

            var quiz = quizBank.GetRandomQuiz();
            if (quiz != null)
            {
                OnQuizScheduled?.Invoke(quiz);
            }
        }
    }
}