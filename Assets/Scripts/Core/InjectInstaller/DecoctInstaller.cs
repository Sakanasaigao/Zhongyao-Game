using UnityEngine;
using Zenject;
using Core.TIMELINE;

public class DecoctInstaller : MonoInstaller
{
    [SerializeField] private Transform potGroup;
    public override void InstallBindings()
    {
        Container.Bind<TimeLineManager>()
        .FromNewComponentOnNewGameObject()
        .AsSingle()
        .NonLazy();

        Container.BindInstance(potGroup).WithId("PotGroup");
    }
}