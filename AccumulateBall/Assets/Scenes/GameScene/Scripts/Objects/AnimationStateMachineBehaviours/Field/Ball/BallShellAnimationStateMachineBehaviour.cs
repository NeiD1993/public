using GameScene.Behaviours.AnimationStateMachine.Ball.Enums;
using GameScene.Behaviours.AnimationStateMachine.Launching;
using UnityEngine;
using UnityEngine.Events;

namespace GameScene.Behaviours.AnimationStateMachine.Ball
{
    public class BallShellAnimationStateMachineBehaviour : LaunchingAnimationStateMachineBehaviour
    {
        public BallShellAnimationStateMachineBehaviour()
        {
            MovingStateEntered = new UnityEvent();
            RotationStateEntered = new UnityEvent();
        }

        public UnityEvent MovingStateEntered { get; private set; }

        public UnityEvent RotationStateEntered { get; private set; }

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateEnter(animator, stateInfo, layerIndex);

            if (animationStateService.IsStateOfTag(stateInfo, BallShellAnimationStateTag.Moving))
                MovingStateEntered.Invoke();
            else if (animationStateService.IsStateOfTag(stateInfo, BallShellAnimationStateTag.Rotation))
                RotationStateEntered.Invoke();
        }
    }
}

namespace GameScene.Behaviours.AnimationStateMachine.Ball.Enums
{
    public enum BallShellAnimationStateTag
    {
        Moving,

        Rotation,

        RotationEnd
    }
}