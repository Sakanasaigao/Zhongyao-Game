using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class DecoctSceneManager : MonoBehaviour
{
    public enum DecoctPhase { Idle, HerbSelection, Brewing, QuizPopup, Completed }

    [Inject] private DecoctHerbSelector herbSelector;
    [Inject] private DecoctBrewTimer brewTimer;
    [Inject] private DecoctQuizPopup quizPopup;
    [Inject] private DecoctPhaseController phaseController;
    [Inject(Id = "StartDecoctingBtn")] private UnityEngine.UI.Button startBtn;

    [SerializeField] private GameObject errorFeedbackPanel;
    [SerializeField] private int sceneAIndex = 1;

    private PrescriptionDataSO currentPrescription;
    private DecoctPhase currentPhase;

    void Start()
    {
        currentPrescription = SceneAMenu.CurrentPrescription;

        if (currentPrescription == null)
        {
            Debug.LogError("No prescription set! Cannot start Decoct mini-game.");
            return;
        }

        currentPhase = DecoctPhase.HerbSelection;

        startBtn.onClick.AddListener(OnStartDecoctingClick);

        herbSelector.OnSelectionConfirmed += TransitionToBrewing;
        brewTimer.OnTimerExpired += TransitionToCompleted;
        phaseController.OnQuizScheduled += ShowQuiz;
        quizPopup.OnQuizComplete += ResumeBrewing;

        herbSelector.Setup(currentPrescription);
        quizPopup.SetPrescription(currentPrescription);
    }

    void OnDestroy()
    {
        startBtn.onClick.RemoveListener(OnStartDecoctingClick);
        herbSelector.OnSelectionConfirmed -= TransitionToBrewing;
        brewTimer.OnTimerExpired -= TransitionToCompleted;
        phaseController.OnQuizScheduled -= ShowQuiz;
        quizPopup.OnQuizComplete -= ResumeBrewing;
    }

    private void OnStartDecoctingClick()
    {
        if (currentPhase == DecoctPhase.HerbSelection)
        {
            bool valid = herbSelector.ValidateSelection();
            if (!valid && errorFeedbackPanel != null)
            {
                errorFeedbackPanel.SetActive(true);
                StartCoroutine(HideErrorFeedbackAfterDelay(2f));
            }
        }
    }

    private System.Collections.IEnumerator HideErrorFeedbackAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (errorFeedbackPanel != null)
            errorFeedbackPanel.SetActive(false);
    }

    private void TransitionToBrewing()
    {
        currentPhase = DecoctPhase.Brewing;
        startBtn.interactable = false;
        herbSelector.MoveSelectedCardsToPot();

        brewTimer.StartTimer(currentPrescription.brewingTime);
        // TODO: 答题系统暂不启用，后续完善后再开启
        // phaseController.StartScheduling();
    }

    private void ShowQuiz(QuizDataSO quiz)
    {
        currentPhase = DecoctPhase.QuizPopup;
        phaseController.PauseScheduling();
        quizPopup.ShowQuiz(quiz);
    }

    private void ResumeBrewing()
    {
        currentPhase = DecoctPhase.Brewing;
        phaseController.ResumeScheduling();
    }

    private void TransitionToCompleted()
    {
        currentPhase = DecoctPhase.Completed;
        phaseController.StopScheduling();

        // 设置回调对话脚本，返回SceneA后自动触发
        SceneAMenu.PendingDialogueScript = "13";
        SceneLoaderManager.Instance.TransitionToScene("Cloud", sceneAIndex);
    }
}