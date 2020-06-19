using System;
using System.Collections;
using GameScene.Behaviours.AnimationStateMachine.ScoreItem;
using GameScene.Behaviours.AnimationStateMachine.ScoreItem.Enums;
using GameScene.Behaviours.AnimationStateMachine.ScoreItem.Events.Enums;
using GameScene.Behaviours.Control;
using GameScene.Behaviours.Control.Info;
using GameScene.Behaviours.MaterializedObject.Descriptions;
using GameScene.Behaviours.MaterializedObject.Info;
using GameScene.Behaviours.MaterializedObject.Info.Interfaces;
using GameScene.Behaviours.ScoreItem.Characteristics;
using GameScene.Behaviours.ScoreItem.Characteristics.Info;
using GameScene.Behaviours.ScoreItem.Characteristics.Info.Enums;
using GameScene.Behaviours.ScoreItem.Enums;
using GameScene.Behaviours.ScoreItem.Events;
using GameScene.Behaviours.ScoreItem.Events.Arguments;
using GameScene.Behaviours.ScoreItem.Info;
using GameScene.Services.Content.Characteristics;
using GameScene.Services.Content.Characteristics.Interfaces.Data;
using GameScene.Services.Content.Data;
using GameScene.Services.Subscription.Events;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace GameScene.Behaviours.ScoreItem
{
    public class ScoreItemBehaviour : BaseControlBehaviour<ScoreItemAnimatorInfo, 
        UnenumerableNonSinglePartAnimationStateMachinesBehavioursDescription<ScoreItemFundamentAnimationStateMachineBehaviour, ScoreItemRateAnimationStateMachineBehaviour>, 
        ScoreItemFundamentAnimationStateMachineBehaviour, ScoreItemBehaviourSetupInfo, ScoreItemCharacteristics, ScoreItemAnimatorControllerLayer, 
        ScoreItemAnimatorControllerParameter>
    {
        private static readonly (float minorAnimationStateMachinePreExiting, (float first, float repeated) inputCalibrating) animationFragmentsTime;

        static ScoreItemBehaviour()
        {
            animationFragmentsTime = (0.4f, (0.5f, 1));
        }

        public ScoreItemBehaviour()
        {
            InformingStatus = InformingStatus.NonActive;
            AnimatedlyPreparedForInforming = new ScoreItemAnimatedlyPreparedForInformingEvent();
        }

        protected Text InformationElement { get; set; }

        public InformingStatus InformingStatus { get; private set; }

        public ScoreItemAnimatedlyPreparedForInformingEvent AnimatedlyPreparedForInforming { get; private set; }

        private void ActualizeInformationElementText()
        {
            InformationElement.text = Characteristics.InformationCharacteristics.Content;
        }

        private void BeginRateFading()
        {
            AnimatorInfo.SetParameter(ScoreItemAnimatorControllerParameter.IsFading);
        }

        private void BeginRateTuning()
        {
            AnimatorInfo.SetParameter(ScoreItemAnimatorControllerParameter.IsTuning);
        }

        private void BeginTermination()
        {
            AnimatorInfo.SetParameter(ScoreItemAnimatorControllerParameter.IsTermination);
        }

        protected override void RemoveMajorAnimationStateMachineBehaviourEventsListeners(ScoreItemFundamentAnimationStateMachineBehaviour majorAnimationStateMachineBehaviour)
        {
            base.RemoveMajorAnimationStateMachineBehaviourEventsListeners(majorAnimationStateMachineBehaviour);
            majorAnimationStateMachineBehaviour.PostInformingStateEntered.RemoveListener(OnPostInformingAnimationStateEntered);
            majorAnimationStateMachineBehaviour.InputCalibratingStateEntered.RemoveListener(OnInputCalibratingAnimationStateEntered);
        }

        protected void SetupInformationElement(Text informationElement)
        {
            InformationElement = informationElement;
            ActualizeInformationElementText();
            Characteristics.InformationCharacteristics.Refreshed.AddListener(OnInformationCharacteristicsRefreshed);
        }

        protected override void SetupAnimationStateMachinesBehaviours()
        {
            base.SetupAnimationStateMachinesBehaviours();
            SetupMinorAnimationStateMachineBehaviour(AnimatorInfo.AnimationStateMachinesBehavioursDescription.Minor);
        }

        protected override void SetupMajorAnimationStateMachineBehaviour(ScoreItemFundamentAnimationStateMachineBehaviour majorAnimationStateMachineBehaviour)
        {
            base.SetupMajorAnimationStateMachineBehaviour(majorAnimationStateMachineBehaviour);
            majorAnimationStateMachineBehaviour.PostInformingStateEntered.AddListener(OnPostInformingAnimationStateEntered);
            majorAnimationStateMachineBehaviour.InputCalibratingStateEntered.AddListener(OnInputCalibratingAnimationStateEntered);
        }

        protected void SetupMinorAnimationStateMachineBehaviour(ScoreItemRateAnimationStateMachineBehaviour minorAnimationStateMachineBehaviour)
        {
            eventsListenersService.AddOrdinaryEventListener(() => OnMinorAnimationStateMachineDestroyed(minorAnimationStateMachineBehaviour),
                minorAnimationStateMachineBehaviour.Destroyed);
            minorAnimationStateMachineBehaviour.StateMachinePreExiting.AddListener(OnMinorAnimationStateMachinePreExiting);
            minorAnimationStateMachineBehaviour.BlinkingAnimationStateEnteredAfterTuningWithConfiguring
                .AddListener(OnRateBlinkingAnimationStateEnteredAfterTuningWithConfiguring);
        }

        protected override ScoreItemAnimatorInfo CreateAnimatorInfo(Animator animator)
        {
            return new ScoreItemAnimatorInfo(animator);
        }

        public void BeginCalibrating()
        {
            BeginRateTuning();
            AnimatorInfo.SetParameter(ScoreItemAnimatorControllerParameter.IsCalibrating);
        }

        public void ChangeRateFlashType(FlashType flashType)
        {
            ScoreItemAnimatorControllerParameter flashingModeAnimatorControllerParameter;

            switch (flashType)
            {
                case FlashType.Blink:
                    flashingModeAnimatorControllerParameter = ScoreItemAnimatorControllerParameter.IsBlinking;
                    break;
                default:
                    flashingModeAnimatorControllerParameter = ScoreItemAnimatorControllerParameter.IsFlickering;
                    break;
            }

            AnimatorInfo.SetParameter(flashingModeAnimatorControllerParameter);
        }

        public override void Setup(ScoreItemBehaviourSetupInfo setupParameter)
        {
            base.Setup(setupParameter);
            SetupInformationElement(setupParameter.InformationElement);
        }

        public new IEnumerator BeginHiding()
        {
            BeginTermination();
            BeginRateFading();

            yield return new WaitUntil(() => InformingStatus == InformingStatus.NonActive);

            base.BeginHiding();
        }

        public IEnumerator BeginInformingIteratively()
        {
            AnimatorInfo.SetParameter(ScoreItemAnimatorControllerParameter.IsInforming);

            yield return new WaitUntil(() => animationStateService.IsStateOfTag(AnimatorInfo.GetCurrentAnimatorStateInfo(AnimatorInfo.GetMajorLayer()),
                ScoreItemFundamentAnimationStateTag.Informing));
        }

        public IEnumerator ConfigureRateIteratively(ScoreItemCharacteristicsConfiguringActionInfo characteristicsConfiguringActionInfo)
        {
            ScoreItemCharacteristicsConfiguringActionData characteristicsConfiguringActionData = new ScoreItemCharacteristicsConfiguringActionData();
            ScoreItemRateAnimationStateMachineBehaviour minorAnimationStateMachineBehaviour = AnimatorInfo.AnimationStateMachinesBehavioursDescription.Minor;

            yield return characteristicsConfiguringActionInfo.PerformingTypeRetrievingRoutineExtractor(Characteristics, performingTypeParameter =>
            characteristicsConfiguringActionData.PerformingType = performingTypeParameter);

            characteristicsConfiguringActionData.Action = () => characteristicsConfiguringActionInfo.Action(Characteristics,
                characteristicsConfiguringActionData.PerformingType.Value);

            if (characteristicsConfiguringActionData.PerformingType == ScoreItemCharacteristicsConfiguringActionPerformingType.Fulfilling)
            {
                eventsListenersService.AddUnsubscribingEventListener(characteristicsConfiguringActionData.Action,
                    new EventsWithUnsubscribingListeners(minorAnimationStateMachineBehaviour.OutputConfiguringStateEntered, minorAnimationStateMachineBehaviour.Destroyed));
                AnimatorInfo.SetParameter(ScoreItemAnimatorControllerParameter.IsConfiguring);

                yield return new WaitUntil(() =>
                {
                    ScoreItemAnimatorControllerLayer layer = AnimatorInfo.GetMinorLayer();

                    return animationStateService.IsStateOfTag(AnimatorInfo.GetCurrentAnimatorStateInfo(layer), ScoreItemRateAnimationStateTag.OutputConfiguring) && 
                    AnimatorInfo.IsCurrentStateAnimationFragmentPlayed(layer);
                });
            }
            else
                characteristicsConfiguringActionData.Action();
        }

        private void OnInformationCharacteristicsRefreshed()
        {
            ActualizeInformationElementText();
        }

        private void OnInputCalibratingAnimationStateEntered(ScoreItemFundamentInputCalibratingAnimationStateEnteringType inputCalibratingAnimationStateEnteringType)
        {
            IEnumerator ProcessInputCalibratingAnimationStateEnteredIteratively()
            {
                (float inputCalibratingAnimationFragmentTime, bool isPreparedForInformingFirstly) GetProcessingParameters()
                {
                    (float first, float repeated) inputCalibratingAnimationFragmentsTime = animationFragmentsTime.inputCalibrating;

                    switch (inputCalibratingAnimationStateEnteringType)
                    {
                        case ScoreItemFundamentInputCalibratingAnimationStateEnteringType.Firstly:
                            return (inputCalibratingAnimationFragmentsTime.first, true);
                        default:
                            return (inputCalibratingAnimationFragmentsTime.repeated, false);
                    }
                }

                (float inputCalibratingAnimationFragmentTime, bool isPreparedForInformingFirstly) processingParameters = GetProcessingParameters();

                yield return new WaitUntil(() => AnimatorInfo.IsCurrentStateAnimationFragmentPlayed(AnimatorInfo.GetMajorLayer(), 
                    processingParameters.inputCalibratingAnimationFragmentTime));

                InformingStatus = InformingStatus.Prepared;
                AnimatedlyPreparedForInforming.Invoke(new ScoreItemAnimatedlyPreparedForInformingEventArgument(gameObject,
                    processingParameters.isPreparedForInformingFirstly));
            }

            StartCoroutine(ProcessInputCalibratingAnimationStateEnteredIteratively());
        }

        private void OnMinorAnimationStateMachineDestroyed(ScoreItemRateAnimationStateMachineBehaviour minorAnimationStateMachineBehaviour)
        {
            eventsListenersService.RemoveOrdinaryEventListener(minorAnimationStateMachineBehaviour.Destroyed);
            minorAnimationStateMachineBehaviour.StateMachinePreExiting.RemoveListener(OnMinorAnimationStateMachinePreExiting);
            minorAnimationStateMachineBehaviour.BlinkingAnimationStateEnteredAfterTuningWithConfiguring
                .RemoveListener(OnRateBlinkingAnimationStateEnteredAfterTuningWithConfiguring);
        }

        private void OnMinorAnimationStateMachinePreExiting()
        {
            IEnumerator ProcessMinorAnimationStateMachinePreExitingIteratively()
            {
                yield return new WaitUntil(() => AnimatorInfo.IsCurrentStateAnimationFragmentPlayed(AnimatorInfo.GetMinorLayer(), 
                    animationFragmentsTime.minorAnimationStateMachinePreExiting));

                InformingStatus = InformingStatus.NonActive;
            }

            StartCoroutine(ProcessMinorAnimationStateMachinePreExitingIteratively());
        }

        private void OnPostInformingAnimationStateEntered()
        {
            InformingStatus = InformingStatus.NonActive;
        }

        private void OnRateBlinkingAnimationStateEnteredAfterTuningWithConfiguring()
        {
            InformingStatus = InformingStatus.Active;
        }

        protected override void OnDestroy()
        {
            Characteristics.InformationCharacteristics.Refreshed.RemoveListener(OnInformationCharacteristicsRefreshed);
            base.OnDestroy();
        }

        private class ScoreItemCharacteristicsConfiguringActionData
        {
            public ScoreItemCharacteristicsConfiguringActionData()
            {
                Action = null;
                PerformingType = null;
            }

            public ScoreItemCharacteristicsConfiguringActionPerformingType? PerformingType { get; set; }

            public UnityAction Action { get; set; }
        }
    }
}

namespace GameScene.Behaviours.ScoreItem.Characteristics
{
    public class ScoreItemCharacteristics
    {
        public ScoreItemCharacteristics(ScoreItemInformationCharacteristics informationCharacteristics)
        {
            InformationCharacteristics = informationCharacteristics;
        }

        public ScoreItemInformationCharacteristics InformationCharacteristics { get; private set; }
    }

    public class ScoreItemInformationCharacteristics : BaseExtractableContentWithParameterizedIterativeRefreshingCharacteristics<object, object, string>
    {
        public ScoreItemInformationCharacteristics(ContentExtractionData<object, string> contentExtractionData) : base(contentExtractionData) { }

        protected override object GetChangeableExtractionParameter(object refreshingParameter = null)
        {
            return refreshingParameter;
        }
    }
}

namespace GameScene.Behaviours.ScoreItem.Characteristics.Info
{
    public struct ScoreItemCharacteristicsConfiguringActionInfo
    {
        public ScoreItemCharacteristicsConfiguringActionInfo(Action<ScoreItemCharacteristics, ScoreItemCharacteristicsConfiguringActionPerformingType> action,
            Func<ScoreItemCharacteristics, Action<ScoreItemCharacteristicsConfiguringActionPerformingType>, IEnumerator> performingTypeRetrievingRoutineExtractor)
        {
            Action = action;
            PerformingTypeRetrievingRoutineExtractor = performingTypeRetrievingRoutineExtractor;
        }

        public Action<ScoreItemCharacteristics, ScoreItemCharacteristicsConfiguringActionPerformingType> Action { get; private set; }

        public Func<ScoreItemCharacteristics, Action<ScoreItemCharacteristicsConfiguringActionPerformingType>, IEnumerator> PerformingTypeRetrievingRoutineExtractor
        { get; private set; }
    }

    public class ScoreItemInformationCharacteristicsRefreshingInfo
    {
        public ScoreItemInformationCharacteristicsRefreshingInfo()
        {
            Data = null;
            Parameter = null;
        }

        public ContentRefreshingData Data { get; set; }

        public object Parameter { get; set; }
    }
}

namespace GameScene.Behaviours.ScoreItem.Characteristics.Info.Enums
{
    public enum ScoreItemCharacteristicsConfiguringActionPerformingType
    {
        Fulfilling,

        Refusing
    }
}

namespace GameScene.Behaviours.ScoreItem.Enums
{
    public enum FlashType
    {
        Blink,

        Flicker
    }

    public enum InformingStatus
    {
        Active,

        NonActive,

        Prepared
    }

    public enum ScoreItemAnimatorControllerLayer
    {
        Fundament,

        Rate
    }

    public enum ScoreItemAnimatorControllerParameter
    {
        IsBlinking,

        IsCalibrating,

        IsConfiguring,

        IsDisappearance,

        IsFading,

        IsFlickering,

        IsInforming,

        IsTermination,

        IsTuning
    }

    public enum ScoreItemCategory
    {
        Points = 1,

        Steps = 2,

        Time = 0
    }
}

namespace GameScene.Behaviours.ScoreItem.Events
{
    public class ScoreItemAnimatedlyPreparedForInformingEvent : UnityEvent<ScoreItemAnimatedlyPreparedForInformingEventArgument> { }
}

namespace GameScene.Behaviours.ScoreItem.Events.Arguments
{
    public struct ScoreItemAnimatedlyPreparedForInformingEventArgument
    {
        public ScoreItemAnimatedlyPreparedForInformingEventArgument(GameObject scoreItem, bool isPreparedForInformingFirstly)
        {
            ScoreItem = scoreItem;
            IsPreparedForInformingFirstly = isPreparedForInformingFirstly;
        }

        public bool IsPreparedForInformingFirstly { get; private set; }

        public GameObject ScoreItem { get; private set; }
    }
}

namespace GameScene.Behaviours.ScoreItem.Info
{
    public class ScoreItemAnimatorInfo : BaseMaterializedObjectAnimatorInfo<UnenumerableNonSinglePartAnimationStateMachinesBehavioursDescription<ScoreItemFundamentAnimationStateMachineBehaviour, ScoreItemRateAnimationStateMachineBehaviour>, 
        ScoreItemFundamentAnimationStateMachineBehaviour, ScoreItemAnimatorControllerLayer, ScoreItemAnimatorControllerParameter>, IMinorlySingleLayeredMaterializedObjectAnimatorInfo<ScoreItemAnimatorControllerLayer>
    {
        public ScoreItemAnimatorInfo(Animator animator) : base(animator) { }

        protected override UnenumerableNonSinglePartAnimationStateMachinesBehavioursDescription<ScoreItemFundamentAnimationStateMachineBehaviour, ScoreItemRateAnimationStateMachineBehaviour> CreateAnimationStateMachinesBehavioursDescription(Animator animator)
        {
            return new UnenumerableNonSinglePartAnimationStateMachinesBehavioursDescription<ScoreItemFundamentAnimationStateMachineBehaviour, ScoreItemRateAnimationStateMachineBehaviour>(animator);
        }

        public override ScoreItemAnimatorControllerLayer GetMajorLayer()
        {
            return ScoreItemAnimatorControllerLayer.Fundament;
        }

        public ScoreItemAnimatorControllerLayer GetMinorLayer()
        {
            return ScoreItemAnimatorControllerLayer.Rate;
        }

        public override ScoreItemAnimatorControllerParameter GetDisappearanceParameter()
        {
            return ScoreItemAnimatorControllerParameter.IsDisappearance;
        }
    }

    public class ScoreItemBehaviourSetupInfo : CharacteristicalControlBehaviourSetupInfo<ScoreItemCharacteristics>
    {
        public ScoreItemBehaviourSetupInfo(ScoreItemCharacteristics characteristics, Text informationElement) : base(characteristics)
        {
            InformationElement = informationElement;
        }

        public Text InformationElement { get; private set; }
    }
}