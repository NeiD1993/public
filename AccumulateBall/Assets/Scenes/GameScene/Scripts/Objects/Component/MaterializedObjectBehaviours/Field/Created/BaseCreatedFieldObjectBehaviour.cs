using System;
using System.Collections;
using GameScene.Behaviours.AnimationStateMachine.Launching;
using GameScene.Behaviours.FieldObject.Info;
using GameScene.Behaviours.FieldObject.Interfaces;
using GameScene.Behaviours.MaterializedObject.Descriptions;
using GameScene.Behaviours.MaterializedObject.Events;
using GameScene.Behaviours.MaterializedObject.Interfaces;
using UnityEngine;
using UnityEngine.Events;

namespace GameScene.Behaviours.FieldObject
{
    public abstract class BaseCreatedFieldObjectBehaviour<T1, T2, T3, T4, T5> : BaseFieldObjectBehaviour<MaterializedObjectBehaviourEvent, T1, SinglePartAnimationStateMachinesBehavioursDescription<T2>, T2, T4, T5>, 
        IBlockableCreatedFieldObjectBehaviour, ISetupableMaterializedObjectBehaviour<T3> where T1 : BaseCreatedFieldObjectAnimatorInfo<T2, T4, T5> where T2 : LaunchingBlockingAnimationStateMachineBehaviour 
        where T3 : struct where T4 : Enum where T5 : Enum
    {
        public BaseCreatedFieldObjectBehaviour()
        {
            BlockingAnimation = new AnimationPassingEvents<UnityEvent>();
        }

        public AnimationPassingEvents<UnityEvent> BlockingAnimation { get; private set; }

        protected override void RemoveMajorAnimationStateMachineBehaviourEventsListeners(T2 majorAnimationStateMachineBehaviour)
        {
            base.RemoveMajorAnimationStateMachineBehaviourEventsListeners(majorAnimationStateMachineBehaviour);
            majorAnimationStateMachineBehaviour.BlockingStateEntered.RemoveListener(OnBlockingAnimationStateEntered);
        }

        protected override void SetupMajorAnimationStateMachineBehaviour(T2 majorAnimationStateMachineBehaviour)
        {
            base.SetupMajorAnimationStateMachineBehaviour(majorAnimationStateMachineBehaviour);
            majorAnimationStateMachineBehaviour.BlockingStateEntered.AddListener(OnBlockingAnimationStateEntered);
        }

        public abstract void Setup(T3 setupParameter);

        private void OnBlockingAnimationStateEntered()
        {
            IEnumerator ProcessBlockingAnimationStateEnteredIteratively()
            {
                BlockingAnimation.Started.Invoke();

                yield return new WaitUntil(() => AnimatorInfo.IsCurrentStateAnimationFragmentPlayed(AnimatorInfo.GetMajorLayer()));

                BlockingAnimation.Ended.Invoke();
            }

            StartCoroutine(ProcessBlockingAnimationStateEnteredIteratively());
        }

        protected override void OnMajorAnimationStateMachineExiting()
        {
            AnimatedlyDisappeared.Invoke(gameObject);
        }
    }

    public abstract class BaseInternallyFeaturedCreatedFieldObjectBehaviour<T1, T2, T3, T4, T5> : BaseCreatedFieldObjectBehaviour<T1, T2, T3, T4, T5> 
        where T1 : BaseCreatedFieldObjectAnimatorInfo<T2, T4, T5> where T2 : LaunchingBlockingAnimationStateMachineBehaviour where T3 : struct where T4 : Enum where T5 : Enum
    {
        protected T3 FeaturesInfo { get; set; }
    }
}

namespace GameScene.Behaviours.FieldObject.Info
{
    public abstract class BaseCreatedFieldObjectAnimatorInfo<T1, T2, T3> : BaseFieldObjectAnimatorInfo<SinglePartAnimationStateMachinesBehavioursDescription<T1>, T1, T2, T3> 
        where T1 : LaunchingBlockingAnimationStateMachineBehaviour where T2 : Enum where T3 : Enum
    {
        public BaseCreatedFieldObjectAnimatorInfo(Animator animator) : base(animator) { }

        protected override SinglePartAnimationStateMachinesBehavioursDescription<T1> CreateAnimationStateMachinesBehavioursDescription(Animator animator)
        {
            return new SinglePartAnimationStateMachinesBehavioursDescription<T1>(animator);
        }
    }
}

namespace GameScene.Behaviours.FieldObject.Interfaces
{
    public interface IBlockableCreatedFieldObjectBehaviour
    {
        AnimationPassingEvents<UnityEvent> BlockingAnimation { get; }
    }

    public interface IExternallyFeaturedCreatedFieldObjectBehaviour<T> where T : struct
    {
        T FeaturesInfo { get; }
    }
}