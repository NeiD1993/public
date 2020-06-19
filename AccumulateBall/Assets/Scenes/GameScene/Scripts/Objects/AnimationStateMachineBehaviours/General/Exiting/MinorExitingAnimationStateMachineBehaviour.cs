using UnityEngine;
using UnityEngine.Events;

namespace GameScene.Behaviours.AnimationStateMachine.Exiting
{
    public class MinorExitingAnimationStateMachineBehaviour : BaseAnimationStateMachineBehaviour
    {
        public MinorExitingAnimationStateMachineBehaviour()
        {
            StateMachinePreExiting = new UnityEvent();
        }

        public UnityEvent StateMachinePreExiting { get; private set; }

        protected override void ProcessExiting()
        {
            StateMachinePreExiting.Invoke();
        }

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            TryExit(stateInfo);
        }
    }
}