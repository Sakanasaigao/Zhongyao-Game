using UnityEngine;
using UnityEngine.UIElements;
using Zenject;
using Core.TIMELINE;

public class DecoctInstaller : MonoInstaller
{
    [SerializeField] private Transform potGroup;
    [SerializeField] private UnityEngine.UI.Button startDecoctingButton;
    [SerializeField] private QuizBankSO quizBankSO;

    [SerializeField] private HorizontalCardHolder cardHolder;
    [SerializeField] private UIDocument answerDocument;
    [SerializeField] private UnityEngine.UI.GraphicRaycaster cardCanvasRaycaster;
    [SerializeField] private UnityEngine.UI.Slider timerSlider;
    [SerializeField] private TMPro.TMP_Text timerCountdownText;

    public override void InstallBindings()
    {
        Container.Bind<TimeLineManager>()
        .FromNewComponentOnNewGameObject()
        .AsSingle()
        .NonLazy();

        Container.BindInstance(potGroup).WithId("PotGroup");

        Container.Bind<DecoctHerbSelector>().FromNewComponentOnNewGameObject().AsSingle();
        Container.Bind<DecoctBrewTimer>().FromNewComponentOnNewGameObject().AsSingle();
        Container.Bind<DecoctQuizPopup>().FromNewComponentOnNewGameObject().AsSingle();
        Container.Bind<DecoctPhaseController>().FromNewComponentOnNewGameObject().AsSingle();

        Container.BindInstance(startDecoctingButton).WithId("StartDecoctingBtn");
        Container.BindInstance(quizBankSO);
        Container.BindInstance(cardHolder).WithId("CardHolder");
        Container.BindInstance(answerDocument).WithId("AnswerDocument");
        Container.BindInstance(cardCanvasRaycaster).WithId("CardCanvasRaycaster");
        Container.BindInstance(timerSlider).WithId("TimerSlider");
        Container.BindInstance(timerCountdownText).WithId("TimerCountdownText");
    }
}