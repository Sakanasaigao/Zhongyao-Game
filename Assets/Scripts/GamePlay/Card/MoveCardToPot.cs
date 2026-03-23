using Core.TIMELINE;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Core.TIMELINE
{
    public class MoveCardToPot : Core.TIMELINE.Node<Card>
    {
        [Inject(Id = "PotGroup")] private Transform potGroup;
        [Inject] private TimeLineManager timeLineManager;

        public override void OnEnter()
        {
            if (potGroup == null)
            {
                Debug.LogError("potGroup is null! Check Zenject binding.");
                //return;
            }
            Debug.Log($"context: {Context}");
            Transform cardSlot = Context.transform.parent;
            cardSlot.SetParent(potGroup);
        }

        public override void OnExit()
        {
            timeLineManager.UnloadTimeLine(TimeLine);
        }

        public override void OnUpdate(float elapsedTime)
        {
        }
    }
}
