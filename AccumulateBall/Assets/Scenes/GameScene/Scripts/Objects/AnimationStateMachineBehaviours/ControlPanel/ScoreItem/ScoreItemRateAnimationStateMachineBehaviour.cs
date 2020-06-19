using GameScene.Behaviours.AnimationStateMachine.Exiting;
using GameScene.Behaviours.AnimationStateMachine.ScoreItem.Enums;
using UnityEngine;
using UnityEngine.Events;

namespace GameScene.Behaviours.AnimationStateMachine.ScoreItem
{
    public class ScoreItemRateAnimationStateMachineBehaviour : MinorExitingAnimationStateMachineBehaviour
    {
        private bool areTuningWithConfiguringAnimationsActivated;

        public ScoreItemRateAnimationStateMachineBehaviour()
        {
            areTuningWithConfiguringAnimationsActivated = false;
            BlinkingAnimationStateEnteredAfterTuningWithConfiguring = new UnityEvent();
            OutputConfiguringStateEntered = new UnityEvent();
        }

        public UnityEvent BlinkingAnimationStateEnteredAfterTuningWithConfiguring { get; private set; }

        public UnityEvent OutputConfiguringStateEntered { get; private set; }

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateEnter(animator, stateInfo, layerIndex);

            if (animationStateService.IsStateOfTag(stateInfo, ScoreItemRateAnimationStateTag.Blinking) && areTuningWithConfiguringAnimationsActivated)
            {
                areTuningWithConfiguringAnimationsActivated = false;
                BlinkingAnimationStateEnteredAfterTuningWithConfiguring.Invoke();
            }
            else if ((animationStateService.IsStateOfTag(stateInfo, ScoreItemRateAnimationStateTag.InputConfiguring) && !areTuningWithConfiguringAnimationsActivated) || 
                animationStateService.IsStateOfTag(stateInfo, ScoreItemRateAnimationStateTag.Tuning))
                areTuningWithConfiguringAnimationsActivated = true;
            else if (animationStateService.IsStateOfTag(stateInfo, ScoreItemRateAnimationStateTag.OutputConfiguring))
                OutputConfiguringStateEntered.Invoke();
        }
    }
}

namespace GameScene.Behaviours.AnimationStateMachine.ScoreItem.Enums
{
    public enum ScoreItemRateAnimationStateTag
    {
        Blinking,

        InputConfiguring,

        OutputConfiguring,

        Tuning
    }
}