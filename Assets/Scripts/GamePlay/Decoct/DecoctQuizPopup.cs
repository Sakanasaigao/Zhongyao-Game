using System;
using UnityEngine;
using UnityEngine.UIElements;
using Zenject;

public class DecoctQuizPopup : MonoBehaviour
{
    public event Action OnQuizComplete;

    [Inject(Id = "AnswerDocument")] private UIDocument answerDocument;
    [Inject(Id = "CardCanvasRaycaster")] private UnityEngine.UI.GraphicRaycaster cardCanvasRaycaster;
    [Inject] private DecoctBrewTimer brewTimer;

    private PrescriptionDataSO prescription;
    private QuizDataSO currentQuiz;

    private Label questionLabel;
    private Button optionA;
    private Button optionB;
    private Button optionC;
    private VisualElement rootElement;
    private bool uiInitialized;

    private void InitializeUI()
    {
        if (uiInitialized)
            return;

        rootElement = answerDocument.rootVisualElement.Q("root");
        questionLabel = rootElement.Q<Label>("txt");
        optionA = rootElement.Q<Button>("A");
        optionB = rootElement.Q<Button>("B");
        optionC = rootElement.Q<Button>("C");

        uiInitialized = true;
    }

    public void SetPrescription(PrescriptionDataSO prescriptionData)
    {
        prescription = prescriptionData;
    }

    public void ShowQuiz(QuizDataSO quiz)
    {
        InitializeUI();

        currentQuiz = quiz;

        questionLabel.text = quiz.questionText;
        optionA.text = quiz.options[0];
        optionB.text = quiz.options[1];
        optionC.text = quiz.options[2];

        optionA.RegisterCallback<ClickEvent>(OnOptionAClick);
        optionB.RegisterCallback<ClickEvent>(OnOptionBClick);
        optionC.RegisterCallback<ClickEvent>(OnOptionCClick);

        rootElement.style.display = DisplayStyle.Flex;

        if (cardCanvasRaycaster != null)
            cardCanvasRaycaster.enabled = false;

        brewTimer.Pause();
    }

    public void HideQuiz()
    {
        optionA.UnregisterCallback<ClickEvent>(OnOptionAClick);
        optionB.UnregisterCallback<ClickEvent>(OnOptionBClick);
        optionC.UnregisterCallback<ClickEvent>(OnOptionCClick);

        rootElement.style.display = DisplayStyle.None;

        if (cardCanvasRaycaster != null)
            cardCanvasRaycaster.enabled = true;

        brewTimer.Resume();

        currentQuiz = null;
        OnQuizComplete?.Invoke();
    }

    private void OnOptionAClick(ClickEvent evt) => HandleAnswer(0);
    private void OnOptionBClick(ClickEvent evt) => HandleAnswer(1);
    private void OnOptionCClick(ClickEvent evt) => HandleAnswer(2);

    private void HandleAnswer(int selectedIndex)
    {
        if (selectedIndex == currentQuiz.correctAnswerIndex)
        {
            brewTimer.ReduceTime(prescription.timeReductionPerCorrect);
        }

        HideQuiz();
    }
}