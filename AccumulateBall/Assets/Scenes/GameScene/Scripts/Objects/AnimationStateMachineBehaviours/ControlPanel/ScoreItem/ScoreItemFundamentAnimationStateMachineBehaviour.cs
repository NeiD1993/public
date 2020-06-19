using GameScene.Behaviours.AnimationStateMachine.Exiting;
using GameScene.Behaviours.AnimationStateMachine.ScoreItem.Enums;
using GameScene.Behaviours.AnimationStateMachine.ScoreItem.Events;
using GameScene.Behaviours.AnimationStateMachine.ScoreItem.Events.Enums;
using UnityEngine;
using UnityEngine.Events;

namespace GameScene.Behaviours.AnimationStateMachine.ScoreItem
{
    public class ScoreItemFundamentAnimationStateMachineBehaviour : MajorExitingAnimationStateMachineBehaviour
    {
        private bool isFirstCalibratingAnimationPlayed;

        public ScoreItemFundamentAnimationStateMachineBehaviour()
        {
            isFirstCalibratingAnimationPlayed = false;
            PostInformingStateEntered = new UnityEvent();
            InputCalibratingStateEntered = new ScoreItemFundamentInputCalibratingAnimationStateEnteredEvent();
        }

        public UnityEvent PostInformingStateEntered { get; private set; }

        public ScoreItemFundamentInputCalibratingAnimationStateEnteredEvent InputCalibratingStateEntered { get; private set; }

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (animationStateService.IsStateOfTag(stateInfo, ScoreItemFundamentAnimationStateTag.InputCalibrating))
                InputCalibratingStateEntered.Invoke(!isFirstCalibratingAnimationPlayed ? ScoreItemFundamentInputCalibratingAnimationStateEnteringType.Firstly : 
                    ScoreItemFundamentInputCalibratingAnimationStateEnteringType.Repeatedly);
            else if (animationStateService.IsStateOfTag(stateInfo, ScoreItemFundamentAnimationStateTag.PostInforming))
                PostInformingStateEntered.Invoke();
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateExit(animator, stateInfo, layerIndex);

            if (animationStateService.IsStateOfTag(stateInfo, ScoreItemFundamentAnimationStateTag.OutputCalibrating) && (isFirstCalibratingAnimationPlayed == false))
                isFirstCalibratingAnimationPlayed = true;
        }
    }
}

namespace GameScene.Behaviours.AnimationStateMachine.ScoreItem.Enums
{
    public enum ScoreItemFundamentAnimationStateTag
    {
        Informing,

        InputCalibrating,

        OutputCalibrating,

        PostInforming
    }
}

namespace GameScene.Behaviours.AnimationStateMachine.ScoreItem.Events
{
    public class ScoreItemFundamentInputCalibratingAnimationStateEnteredEvent : UnityEvent<ScoreItemFundamentInputCalibratingAnimationStateEnteringType> { }
}

namespace GameScene.Behaviours.AnimationStateMachine.ScoreItem.Events.Enums
{
    public enum ScoreItemFundamentInputCalibratingAnimationStateEnteringType
    {
        Firstly,

        Repeatedly
    }
}