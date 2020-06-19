using UnityEngine;
using UnityEngine.Events;

namespace GameScene.Behaviours.AnimationStateMachine.Exiting
{
    public class MajorExitingAnimationStateMachineBehaviour : BaseAnimationStateMachineBehaviour
    {
        public MajorExitingAnimationStateMachineBehaviour()
        {
            StateMachineExiting = new UnityEvent();
        }

        public UnityEvent StateMachineExiting { get; private set; }

        protected override void ProcessExiting()
        {
            StateMachineExiting.Invoke();
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            TryExit(stateInfo);
        }
    }
}