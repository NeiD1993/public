using System;
using System.Collections;
using GameScene.Behaviours.AnimationStateMachine.Exiting;
using GameScene.Behaviours.AnimationStateMachine.Launching;
using GameScene.Behaviours.FieldObject.Info;
using GameScene.Behaviours.FieldObject.Interfaces;
using GameScene.Behaviours.MaterializedObject.Descriptions;
using GameScene.Behaviours.MaterializedObject.Events;
using GameScene.Behaviours.MaterializedObject.Info.Interfaces;
using UnityEngine;
using UnityEngine.Events;

namespace GameScene.Behaviours.FieldObject
{
    public abstract class BaseGeneratedFieldObjectBehaviour<T1, T2, T3, T4, T5, T6> : BaseFieldObjectBehaviour<UnityEvent, T1, EnumerableNonSinglePartAnimationStateMachinesBehavioursDescription<T2, T3>, T2, T5, T6>, 
        IPartialGeneratedFieldObjectBehaviour where T1 : BaseGeneratedFieldObjectAnimatorInfo<T2, T3, T5, T6> where T2 : LaunchingAnimationStateMachineBehaviour 
        where T3 : MinorExitingAnimationStateMachineBehaviour where T5 : Enum where T6 : Enum
    {
        public BaseGeneratedFieldObjectBehaviour()
        {
            MinorPartAnimatedDisappearance = new AnimationPassingEvents<UnityEvent>();
        }

        protected T4 HallmarksInfo { get; set; }

        public AnimationPassingEvents<UnityEvent> MinorPartAnimatedDisappearance { get; private set; }

        protected virtual void PostProcessMinorAnimationStateMachinePreExiting(T3 minorAnimationStateMachineBehaviour)
        {
            MinorPartAnimatedDisappearance.Ended.Invoke();
        }

        protected virtual void PreProcessMinorAnimationStateMachinePreExiting(T3 minorAnimationStateMachineBehaviour)
        {
            MinorPartAnimatedDisappearance.Started.Invoke();
        }

        protected virtual void RemoveMinorAnimationStateMachinesBehavioursEventsListeners(T3 minorAnimationStateMachineBehaviour)
        {
            eventsListenersService.RemoveOrdinaryEventListener(minorAnimationStateMachineBehaviour.Destroyed);
            eventsListenersService.RemoveOrdinaryEventListener(minorAnimationStateMachineBehaviour.StateMachinePreExiting);
        }

        protected override void SetupAnimationStateMachinesBehaviours()
        {
            base.SetupAnimationStateMachinesBehaviours();

            foreach (T3 minorAnimationStateMachineBehaviour in AnimatorInfo.AnimationStateMachinesBehavioursDescription.Minor)
                SetupMinorAnimationStateMachinesBehaviour(minorAnimationStateMachineBehaviour);
        }

        protected virtual void SetupMinorAnimationStateMachinesBehaviour(T3 minorAnimationStateMachineBehaviour)
        {
            eventsListenersService.AddOrdinaryEventListener(() => OnMinorAnimationStateMachineDestroyed(minorAnimationStateMachineBehaviour), 
                minorAnimationStateMachineBehaviour.Destroyed);
            eventsListenersService.AddOrdinaryEventListener(() => OnMinorAnimationStateMachinePreExiting(minorAnimationStateMachineBehaviour), 
                minorAnimationStateMachineBehaviour.StateMachinePreExiting);
        }

        private void OnMinorAnimationStateMachineDestroyed(T3 minorAnimationStateMachineBehaviour)
        {
            RemoveMinorAnimationStateMachinesBehavioursEventsListeners(minorAnimationStateMachineBehaviour);
        }

        private void OnMinorAnimationStateMachinePreExiting(T3 minorAnimationStateMachineBehaviour)
        {
            IEnumerator ProcessMinorAnimationStateMachinePreExitingIteratively()
            {
                PreProcessMinorAnimationStateMachinePreExiting(minorAnimationStateMachineBehaviour);

                yield return new WaitUntil(() =>
                {
                    T5 layer = default;

                    if (AnimatorInfo is BaseMinorlyMultiLayeredGeneratedFieldObjectAnimatorInfo<T2, T3, T5, T6> minorlyMultiLayeredAnimatorInfo)
                        layer = minorlyMultiLayeredAnimatorInfo.GetMinorLayer(minorAnimationStateMachineBehaviour);
                    else if (AnimatorInfo is BaseMinorlySingleLayeredGeneratedFieldObjectAnimatorInfo<T2, T3, T5, T6> minorlySingleLayeredAnimatorInfo)
                        layer = minorlySingleLayeredAnimatorInfo.GetMinorLayer();

                    return AnimatorInfo.IsCurrentStateAnimationFragmentPlayed(layer);
                });

                PostProcessMinorAnimationStateMachinePreExiting(minorAnimationStateMachineBehaviour);
            }

            StartCoroutine(ProcessMinorAnimationStateMachinePreExitingIteratively());
        }

        protected override void OnMajorAnimationStateMachineExiting()
        {
            AnimatedlyDisappeared.Invoke();
        }
    }
}

namespace GameScene.Behaviours.FieldObject.Info
{
    public abstract class BaseGeneratedFieldObjectAnimatorInfo<T1, T2, T3, T4> : BaseFieldObjectAnimatorInfo<EnumerableNonSinglePartAnimationStateMachinesBehavioursDescription<T1, T2>, T1, T3, T4>
        where T1 : LaunchingAnimationStateMachineBehaviour where T2 : MinorExitingAnimationStateMachineBehaviour where T3 : Enum where T4 : Enum
    {
        public BaseGeneratedFieldObjectAnimatorInfo(Animator animator) : base(animator) { }

        protected override EnumerableNonSinglePartAnimationStateMachinesBehavioursDescription<T1, T2> CreateAnimationStateMachinesBehavioursDescription(Animator animator)
        {
            return new EnumerableNonSinglePartAnimationStateMachinesBehavioursDescription<T1, T2>(animator);
        }
    }

    public abstract class BaseMinorlyMultiLayeredGeneratedFieldObjectAnimatorInfo<T1, T2, T3, T4> : BaseGeneratedFieldObjectAnimatorInfo<T1, T2, T3, T4>, IMinorlyMultiLayeredMaterializedObjectAnimatorInfo<T2, T3>
        where T1 : LaunchingAnimationStateMachineBehaviour where T2 : MinorExitingAnimationStateMachineBehaviour where T3 : Enum where T4 : Enum
    {
        public BaseMinorlyMultiLayeredGeneratedFieldObjectAnimatorInfo(Animator animator) : base(animator) { }

        public abstract T3 GetMinorLayer(T2 minorAnimationStateMachineBehaviour);
    }

    public abstract class BaseMinorlySingleLayeredGeneratedFieldObjectAnimatorInfo<T1, T2, T3, T4> : BaseGeneratedFieldObjectAnimatorInfo<T1, T2, T3, T4>, IMinorlySingleLayeredMaterializedObjectAnimatorInfo<T3> 
        where T1 : LaunchingAnimationStateMachineBehaviour where T2 : MinorExitingAnimationStateMachineBehaviour where T3 : Enum where T4 : Enum
    {
        public BaseMinorlySingleLayeredGeneratedFieldObjectAnimatorInfo(Animator animator) : base(animator) { }

        public abstract T3 GetMinorLayer();
    }
}

namespace GameScene.Behaviours.FieldObject.Interfaces
{
    public interface ICheckableGeneratedFieldObjectBehaviour<T>
    {
        T CreateCheckableData();
    }

    public interface ICheckableGeneratedFieldObjectBehaviour<T1, T2> where T2 : struct
    {
        T1 CreateCheckableData(T2 parameter);
    }

    public interface IPartialGeneratedFieldObjectBehaviour
    {
        AnimationPassingEvents<UnityEvent> MinorPartAnimatedDisappearance { get; }
    }
}