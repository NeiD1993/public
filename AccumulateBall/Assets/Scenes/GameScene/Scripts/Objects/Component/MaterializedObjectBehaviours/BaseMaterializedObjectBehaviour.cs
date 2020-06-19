using System;
using System.Collections.Generic;
using System.Linq;
using GameScene.Behaviours.AnimationStateMachine.Exiting;
using GameScene.Behaviours.MaterializedObject.Data;
using GameScene.Behaviours.MaterializedObject.Descriptions;
using GameScene.Behaviours.MaterializedObject.Descriptions.Interfaces;
using GameScene.Behaviours.MaterializedObject.Info;
using GameScene.Behaviours.MaterializedObject.Info.Interfaces;
using GameScene.Behaviours.MaterializedObject.Interfaces;
using GameScene.Objects;
using GameScene.Objects.Interfaces;
using GameScene.Services.Animation;
using ServicesLocators;
using UnityEngine;
using UnityEngine.Events;

namespace GameScene.Behaviours.MaterializedObject
{
    public abstract class BaseMaterializedObjectBehaviour<T1, T2, T3, T4, T5, T6> : BaseComponentObject, INotifiedlyHideableMaterializedObjectBehaviour<T1>, 
        INotifiedlyDestroyableObject where T1 : UnityEventBase, new() where T2 : BaseMaterializedObjectAnimatorInfo<T3, T4, T5, T6> 
        where T3 : SinglePartAnimationStateMachinesBehavioursDescription<T4> where T4 : MajorExitingAnimationStateMachineBehaviour where T5 : Enum where T6 : Enum
    {
        protected AnimationStateService animationStateService;

        public BaseMaterializedObjectBehaviour()
        {
            Destroyed = new UnityEvent();
            AnimatedlyDisappeared = new T1();
        }

        protected T2 AnimatorInfo { get; set; }

        public UnityEvent Destroyed { get; private set; }

        public T1 AnimatedlyDisappeared { get; protected set; }

        protected abstract T2 CreateAnimatorInfo(Animator animator);

        protected override void ObtainSharingRelatedServices()
        {
            base.ObtainSharingRelatedServices();
            animationStateService = SharedSceneServicesLocator.GetService<AnimationStateService>();
        }

        protected override void ProcessStart()
        {
            AnimatorInfo = CreateAnimatorInfo(gameObject.GetComponent<Animator>());
            SetupAnimationStateMachinesBehaviours();
        }

        protected virtual void RemoveMajorAnimationStateMachineBehaviourEventsListeners(T4 majorAnimationStateMachineBehaviour)
        {
            eventsListenersService.RemoveOrdinaryEventListener(majorAnimationStateMachineBehaviour.Destroyed);
            majorAnimationStateMachineBehaviour.StateMachineExiting.RemoveListener(OnMajorAnimationStateMachineExiting);
        }

        protected virtual void SetupAnimationStateMachinesBehaviours()
        {
            SetupMajorAnimationStateMachineBehaviour(AnimatorInfo.AnimationStateMachinesBehavioursDescription.Major);
        }

        protected virtual void SetupMajorAnimationStateMachineBehaviour(T4 majorAnimationStateMachineBehaviour)
        {
            eventsListenersService.AddOrdinaryEventListener(() => RemoveMajorAnimationStateMachineBehaviourEventsListeners(majorAnimationStateMachineBehaviour),
                majorAnimationStateMachineBehaviour.Destroyed);
            majorAnimationStateMachineBehaviour.StateMachineExiting.AddListener(OnMajorAnimationStateMachineExiting);
        }

        public void BeginHiding()
        {
            AnimatorInfo.SetParameter(AnimatorInfo.GetDisappearanceParameter());
        }

        protected abstract void OnMajorAnimationStateMachineExiting();

        protected virtual void OnDestroy()
        {
            Destroyed.Invoke();
        }
    }
}

namespace GameScene.Behaviours.MaterializedObject.Data
{
    public class AnimatorControllerPrimaryData<T1, T2> where T1 : Enum where T2 : Enum
    {
        public AnimatorControllerPrimaryData(Animator animator)
        {
            SetupLayersIndexes(animator);
            SetupParametersIds(animator);
        }

        public IDictionary<T1, int> LayersIndexes { get; private set; }

        public IDictionary<T2, int> ParametersIndexes { get; private set; }

        private void SetupLayersIndexes(Animator animator)
        {
            LayersIndexes = new Dictionary<T1, int>();

            foreach (T1 layer in Enum.GetValues(typeof(T1)))
                LayersIndexes.Add(layer, animator.GetLayerIndex(layer.ToString()));
        }

        private void SetupParametersIds(Animator animator)
        {
            ParametersIndexes = new Dictionary<T2, int>();

            Type parametersType = typeof(T2);

            for (int i = 0; i < animator.parameterCount; i++)
                ParametersIndexes.Add((T2)Enum.Parse(parametersType, animator.parameters[i].name), i);
        }
    }
}

namespace GameScene.Behaviours.MaterializedObject.Descriptions
{
    public abstract class NonSinglePartAnimationStateMachinesBehavioursDescription<T1, T2> : SinglePartAnimationStateMachinesBehavioursDescription<T1>
        where T1 : MajorExitingAnimationStateMachineBehaviour where T2 : MinorExitingAnimationStateMachineBehaviour
    {
        protected readonly IEnumerable<T2> minor;

        public NonSinglePartAnimationStateMachinesBehavioursDescription(Animator animator) : base(animator)
        {
            minor = animator.GetBehaviours<T2>();
        }
    }

    public class EnumerableNonSinglePartAnimationStateMachinesBehavioursDescription<T1, T2> : NonSinglePartAnimationStateMachinesBehavioursDescription<T1, T2>,
        IEnumerableAnimationStateMachinesBehavioursDescription<T2> where T1 : MajorExitingAnimationStateMachineBehaviour
        where T2 : MinorExitingAnimationStateMachineBehaviour
    {
        public EnumerableNonSinglePartAnimationStateMachinesBehavioursDescription(Animator animator) : base(animator) { }

        public IEnumerable<T2> Minor
        {
            get
            {
                return minor;
            }
        }
    }

    public class SinglePartAnimationStateMachinesBehavioursDescription<T> where T : MajorExitingAnimationStateMachineBehaviour
    {
        public SinglePartAnimationStateMachinesBehavioursDescription(Animator animator)
        {
            Major = animator.GetBehaviour<T>();
        }

        public T Major { get; private set; }
    }

    public class UnenumerableNonSinglePartAnimationStateMachinesBehavioursDescription<T1, T2> : NonSinglePartAnimationStateMachinesBehavioursDescription<T1, T2>,
        IUnenumerableAnimationStateMachinesBehavioursDescription<T2> where T1 : MajorExitingAnimationStateMachineBehaviour
        where T2 : MinorExitingAnimationStateMachineBehaviour
    {
        public UnenumerableNonSinglePartAnimationStateMachinesBehavioursDescription(Animator animator) : base(animator) { }

        public T2 Minor
        {
            get
            {
                return minor.Single();
            }
        }
    }
}

namespace GameScene.Behaviours.MaterializedObject.Descriptions.Interfaces
{
    public interface IEnumerableAnimationStateMachinesBehavioursDescription<T> where T : MinorExitingAnimationStateMachineBehaviour
    {
        IEnumerable<T> Minor { get; }
    }

    public interface IUnenumerableAnimationStateMachinesBehavioursDescription<T> where T : MinorExitingAnimationStateMachineBehaviour
    {
        T Minor { get; }
    }
}

namespace GameScene.Behaviours.MaterializedObject.Events
{
    public class AnimationPassingEvents<T> where T : UnityEventBase, new()
    {
        public AnimationPassingEvents()
        {
            Started = new UnityEvent();
            Ended = new T();
        }

        public UnityEvent Started { get; private set; }

        public T Ended { get; private set; }
    }

    public class MaterializedObjectBehaviourEvent : UnityEvent<GameObject> { }
}

namespace GameScene.Behaviours.MaterializedObject.Info
{
    public abstract class BaseMaterializedObjectAnimatorInfo<T1, T2, T3, T4> : IParameterizedlySettableMaterializedObjectAnimatorInfo<T4>, 
        IParameterizedlySettableMaterializedObjectAnimatorInfo<T4, bool>, IParameterizedlySettableMaterializedObjectAnimatorInfo<T4, int> 
        where T1 : SinglePartAnimationStateMachinesBehavioursDescription<T2> where T2 : MajorExitingAnimationStateMachineBehaviour where T3 : Enum where T4 : Enum
    {
        public BaseMaterializedObjectAnimatorInfo(Animator animator)
        {
            Animator = animator;
            AnimatorControllerPrimaryData = new AnimatorControllerPrimaryData<T3, T4>(Animator);
            AnimationStateMachinesBehavioursDescription = CreateAnimationStateMachinesBehavioursDescription(Animator);
        }

        protected AnimatorControllerPrimaryData<T3, T4> AnimatorControllerPrimaryData { get; set; }

        public Animator Animator { get; private set; }

        public T1 AnimationStateMachinesBehavioursDescription { get; private set; }

        private void SetParameter(T4 parameter, AnimatorControllerParameterType type, Action<AnimatorControllerParameter> settingAction)
        {
            AnimatorControllerParameter GetParameter()
            {
                int parameterIndex = AnimatorControllerPrimaryData.ParametersIndexes[parameter];

                return Animator.GetParameter(parameterIndex);
            }

            AnimatorControllerParameter animatorControllerParameter = GetParameter();

            if (animatorControllerParameter.type == type)
                settingAction(animatorControllerParameter);
        }

        protected abstract T1 CreateAnimationStateMachinesBehavioursDescription(Animator animator);

        public void SetParameter(T4 parameter)
        {
            SetParameter(parameter, AnimatorControllerParameterType.Trigger, animatorControllerParameter => Animator.SetTrigger(animatorControllerParameter.nameHash));
        }

        public void SetParameter(T4 parameter, bool value)
        {
            SetParameter(parameter, AnimatorControllerParameterType.Bool, animatorControllerParameter => Animator.SetBool(animatorControllerParameter.nameHash, value));
        }

        public void SetParameter(T4 parameter, int value)
        {
            SetParameter(parameter, AnimatorControllerParameterType.Int, animatorControllerParameter => Animator.SetInteger(animatorControllerParameter.nameHash, value));
        }

        public abstract T3 GetMajorLayer();

        public abstract T4 GetDisappearanceParameter();

        public bool IsCurrentStateAnimationFragmentPlayed(T3 layer, float fragmentTime = 1)
        {
            return GetCurrentStateAnimationFragmentTime(layer) >= fragmentTime;
        }

        public int GetCurrentStateAnimationLoopsCount(T3 layer)
        {
            return (int)GetCurrentStateAnimationFragmentTime(layer);
        }

        public float GetCurrentStateAnimationFragmentTime(T3 layer)
        {
            return GetCurrentAnimatorStateInfo(layer).normalizedTime;
        }

        public AnimatorStateInfo GetCurrentAnimatorStateInfo(T3 layer)
        {
            return Animator.GetCurrentAnimatorStateInfo(AnimatorControllerPrimaryData.LayersIndexes[layer]);
        }
    }
}

namespace GameScene.Behaviours.MaterializedObject.Info.Interfaces
{
    public interface IMinorlyMultiLayeredMaterializedObjectAnimatorInfo<T1, T2> where T1 : MinorExitingAnimationStateMachineBehaviour where T2 : Enum
    {
        T2 GetMinorLayer(T1 minorAnimationStateMachineBehaviour);
    }

    public interface IMinorlySingleLayeredMaterializedObjectAnimatorInfo<T> where T : Enum
    {
        T GetMinorLayer();
    }

    public interface IParameterizedlySettableMaterializedObjectAnimatorInfo<T> where T : Enum
    {
        void SetParameter(T parameter);
    }

    public interface IParameterizedlySettableMaterializedObjectAnimatorInfo<T1, T2> where T1 : Enum where T2 : struct
    {
        void SetParameter(T1 parameter, T2 value);
    }
}

namespace GameScene.Behaviours.MaterializedObject.Interfaces
{
    public interface IHideableMaterializedObjectBehaviour
    {
        void BeginHiding();
    }

    public interface ISetupableMaterializedObjectBehaviour<T>
    {
        void Setup(T setupParameter);
    }

    public interface INotifiedlyHideableMaterializedObjectBehaviour<T> : IHideableMaterializedObjectBehaviour where T : UnityEventBase
    {
        T AnimatedlyDisappeared { get; }
    }
}