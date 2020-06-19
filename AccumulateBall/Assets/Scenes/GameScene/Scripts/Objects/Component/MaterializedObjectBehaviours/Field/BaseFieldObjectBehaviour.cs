using System;
using GameScene.Behaviours.AnimationStateMachine.Enums;
using GameScene.Behaviours.AnimationStateMachine.Launching;
using GameScene.Behaviours.FieldObject.Info;
using GameScene.Behaviours.FieldObject.Interfaces;
using GameScene.Behaviours.MaterializedObject;
using GameScene.Behaviours.MaterializedObject.Descriptions;
using GameScene.Behaviours.MaterializedObject.Events;
using GameScene.Behaviours.MaterializedObject.Info;
using GameScene.Services.Subscription.Events;
using UnityEngine;
using UnityEngine.Events;

namespace GameScene.Behaviours.FieldObject
{
    public abstract class BaseFieldObjectBehaviour<T1, T2, T3, T4, T5, T6> : BaseMaterializedObjectBehaviour<T1, T2, T3, T4, T5, T6>, ILaunchableFieldObjectBehaviour, 
        IRetainableFieldObjectBehaviour where T1 : UnityEventBase, new() where T2 : BaseFieldObjectAnimatorInfo<T3, T4, T5, T6> 
        where T3 : SinglePartAnimationStateMachinesBehavioursDescription<T4> where T4 : LaunchingAnimationStateMachineBehaviour where T5 : Enum where T6 : Enum
    {
        public BaseFieldObjectBehaviour()
        {
            Resumed = new UnityEvent();
            AnimatedlyLaunched = new MaterializedObjectBehaviourEvent();
        }

        private UnityEvent Resumed { get; set; }

        public MaterializedObjectBehaviourEvent AnimatedlyLaunched { get; private set; }

        protected override void RemoveMajorAnimationStateMachineBehaviourEventsListeners(T4 majorAnimationStateMachineBehaviour)
        {
            base.RemoveMajorAnimationStateMachineBehaviourEventsListeners(majorAnimationStateMachineBehaviour);
            majorAnimationStateMachineBehaviour.LaunchingStateEntered.RemoveListener(OnLaunchingAnimationStateEntered);
        }

        protected override void SetupMajorAnimationStateMachineBehaviour(T4 majorAnimationStateMachineBehaviour)
        {
            base.SetupMajorAnimationStateMachineBehaviour(majorAnimationStateMachineBehaviour);
            majorAnimationStateMachineBehaviour.LaunchingStateEntered.AddListener(OnLaunchingAnimationStateEntered);
        }

        protected void Idle()
        {
            AnimatorInfo.SetParameter(AnimatorInfo.GetIdleParameter());
        }

        public void Resume()
        {
            Resumed.Invoke();
        }

        public void Suspend()
        {
            float currentAnimatorSpeed = AnimatorInfo.Animator.speed;

            eventsListenersService.AddUnsubscribingEventListener(() => AnimatorInfo.Animator.speed = currentAnimatorSpeed, 
                new EventsWithUnsubscribingListeners(Resumed, Destroyed));

            AnimatorInfo.Animator.speed = 0;
        }

        public virtual bool TryRun()
        {
            if (animationStateService.IsStateOfTag(AnimatorInfo.GetCurrentAnimatorStateInfo(AnimatorInfo.GetMajorLayer()), PrimaryAnimationStateTag.Launching))
            {
                Idle();

                return true;
            }
            else
                return false;
        }

        protected virtual void OnLaunchingAnimationStateEntered()
        {
            AnimatedlyLaunched.Invoke(gameObject);
        }
    }
}

namespace GameScene.Behaviours.FieldObject.Info
{
    public abstract class BaseFieldObjectAnimatorInfo<T1, T2, T3, T4> : BaseMaterializedObjectAnimatorInfo<T1, T2, T3, T4> 
        where T1 : SinglePartAnimationStateMachinesBehavioursDescription<T2> where T2 : LaunchingAnimationStateMachineBehaviour where T3 : Enum where T4 : Enum
    {
        public BaseFieldObjectAnimatorInfo(Animator animator) : base(animator) { }
        
        public abstract T4 GetIdleParameter();
    }
}

namespace GameScene.Behaviours.FieldObject.Info.Interfaces
{
    public interface IFieldObjectAnimationInfo<T> where T : Enum
    {
        T EndStateTag { get; }
    }
}

namespace GameScene.Behaviours.FieldObject.Interfaces
{
    public interface IHoldableFieldObjectBehaviour<T> where T : UnityEventBase, new()
    {
        AnimationPassingEvents<T> HoldingAnimation { get; }
    }

    public interface ILaunchableFieldObjectBehaviour
    {
        MaterializedObjectBehaviourEvent AnimatedlyLaunched { get; }

        bool TryRun();
    }

    public interface IRetainableFieldObjectBehaviour
    {
        void Resume();

        void Suspend();
    }
}