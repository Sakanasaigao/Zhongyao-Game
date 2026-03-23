using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Core.TIMELINE
{
    public class TestNode : Node<NodeTester>
    {
        public override void OnEnter()
        {
            Context.NodeIn();
        }

        public override void OnExit()
        {
            Context.NodeOut();
        }

        public override void OnUpdate(float elapsedTime)
        {
            Context.NodeUpdate(elapsedTime);
        }

    }
}
