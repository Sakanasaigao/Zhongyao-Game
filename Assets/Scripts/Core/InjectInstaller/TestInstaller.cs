using Core.TIMELINE;
using UnityEngine;
using Zenject;

public class TestInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<TimeLineManager>().FromComponentInHierarchy().AsSingle().NonLazy();
    }
}