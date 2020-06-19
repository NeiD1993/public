using System;
using System.Collections;
using System.Collections.Generic;
using GameScene.Behaviours.Button;
using GameScene.Behaviours.Button.Characteristics;
using GameScene.Behaviours.Button.Enums;
using GameScene.Behaviours.Control.Info;
using GameScene.Managers.ButtonsPanel.Data;
using GameScene.Managers.ButtonsPanel.Enums;
using GameScene.Managers.ButtonsPanel.Events;
using GameScene.Managers.ButtonsPanel.Info;
using GameScene.Managers.ButtonsPanel.Settings;
using GameScene.Managers.ControlPanel;
using GameScene.Managers.ControlPanel.Info;
using GameScene.Managers.ControlPanel.Interfaces;
using GameScene.Managers.ControlPanel.Settings;
using GameScene.Managers.ControlPanel.Settings.Interfaces;
using GameScene.Managers.Entity.Actions;
using GameScene.Managers.Entity.Data;
using GameScene.Managers.Entity.Data.Interfaces;
using GameScene.Managers.Entity.Settings;
using GameScene.Services.Buttons;
using GameScene.Services.Buttons.Enums;
using GameScene.Services.Buttons.Info;
using GameScene.Services.ControlPanel.Enums;
using GameScene.Services.Subscription.Events;
using UnityEngine;
using UnityEngine.Events;

namespace GameScene.Managers.ButtonsPanel
{
    public class ButtonsPanelManager : BaseChildControlEntityManager<ButtonBehaviour, ButtonSettings, ButtonRectTransformSettings, ButtonRectTransformPositionSettings,
        ButtonsPanelInfo, ButtonCategory, CharacteristicalControlBehaviourSetupInfo<ButtonCharacteristics>>, 
        IDisableableControlEntityManager<ButtonsPanelDisablingType, ButtonsPanelWaitForDisablingEvent>, INotifiedlyDestroyableByObjectsControlEntityManager, 
        INotifiedlyRunnableControlEntityManager, IStatusChangingControlEntityManager<ButtonsPanelStatus>
    {
        private readonly ButtonInfoService buttonInfoService;

        public ButtonsPanelManager()
        {
            buttonInfoService = new ButtonInfoService();
            ButtonsPanelReadyForDisabling = new ButtonsPanelReadyForDisablingEvent();
            EntityEnabled = new UnityEvent();
            EntityObjectsDestroyed = new UnityEvent();
            EntityRunned = new UnityEvent();
            EntityWaitForDisablingEvent = new ButtonsPanelWaitForDisablingEvent();
        }

        private ButtonsPanelReadyForDisablingEvent ButtonsPanelReadyForDisabling { get; set; }

        public UnityEvent EntityEnabled { get; private set; }

        public UnityEvent EntityObjectsDestroyed { get; private set; }

        public UnityEvent EntityRunned { get; private set; }

        public ButtonsPanelWaitForDisablingEvent EntityWaitForDisablingEvent { get; private set; }

        private static ISet<ButtonType> GetButtonsTypesToCreate(ButtonsPanelType buttonsPanelType)
        {
            ISet<ButtonType> buttonsTypes = new HashSet<ButtonType>();

            switch (buttonsPanelType)
            {
                case ButtonsPanelType.Opening:
                    buttonsTypes.Add(ButtonType.Start);
                    break;
                case ButtonsPanelType.Process:
                    {
                        buttonsTypes.Add(ButtonType.Stop);
                        buttonsTypes.Add(ButtonType.Pause);
                    }
                    break;
                case ButtonsPanelType.ProcessInterruption:
                    buttonsTypes.Add(ButtonType.Continue);
                    break;
                default:
                    buttonsTypes.Add(ButtonType.Pause);
                    break;
            }

            return buttonsTypes;
        }

        private void BeginButtonsHiding()
        {
            foreach (GameObject control in EntityInfo.Controls.Values)
                control.GetComponent<ButtonBehaviour>().BeginHiding();

            EntityInfo.Type = ButtonsPanelType.None;
        }

        private void CreateButton(ButtonType type, bool isDefaultPosition, ISet<ButtonType> buttonsTypesToCreate)
        {
            ButtonInstantiatingInfo buttonInstantiatingInfo = buttonInfoService.GetButtonInstantiatingInfo(entityObjectSettings, type, isDefaultPosition);
            Func<ControlBehaviourInstantiatingInfo> buttonBehaviourInstantiatingInfoExtractor = () => 
            new ControlBehaviourInstantiatingInfo(
                (type == ButtonType.Continue) || (type == ButtonType.Pause) ? ButtonCategory.Keeping : ButtonCategory.Forming, 
                new CharacteristicalControlBehaviourSetupInfo<ButtonCharacteristics>(new ButtonCharacteristics(type)), 
                new object[] { buttonsTypesToCreate });

            CreateControl(new ControlInstantiatingInfo(
                buttonInstantiatingInfo.PositionIndex, type.ToString(), buttonInstantiatingInfo.Prefab, buttonInstantiatingInfo.AnimatorController), 
                (Action<GameObject>)EndButtonInitialSetup,
                buttonBehaviourInstantiatingInfoExtractor);
        }

        private void EndButtonInitialSetup(GameObject button)
        {
            button.AddComponent<CapsuleCollider>();

            CapsuleCollider capsuleCollider = button.GetComponent<CapsuleCollider>();

            capsuleCollider.isTrigger = true;
            capsuleCollider.radius = entityObjectSettings.CapsuleColliderSettings.Radius;
            capsuleCollider.height = entityObjectSettings.CapsuleColliderSettings.Height;
        }

        private void ProcessDisablingButtonClicked(GameObject disablingButton)
        {
            void ProcessDisablingButtonClickedInternally(ButtonsPanelDisablingType buttonsPanelDisablingType, Action<ButtonsPanelStatus> buttonsPanelStatusChangedAction)
            {
                StartCoroutine(EntityInfo.StatusData.SetStatusIteratively(ButtonsPanelStatus.Disabled, ButtonsPanelStatusUpdateType.WaitForConfirmation,
                    new EntityStatusChangingActions<ButtonsPanelStatus>(
                        buttonsPanelStatusChangedAction,
                        () => EntityInfo.StatusData.Unlock()),
                    buttonsPanelStatusSettingRoutineGuidParameter =>
                    {
                        eventsListenersService.AddUnsubscribingEventListener(buttonsPanelStatusSettingRoutineProcessingTypeParameter =>
                        EntityInfo.StatusData.ChangeSettingRoutineProcessingType(buttonsPanelStatusSettingRoutineProcessingTypeParameter,
                        buttonsPanelStatusSettingRoutineGuidParameter),
                        new EventsWithUnsubscribingListeners<ButtonsPanelStatusChangingRoutineProcessingType>(ButtonsPanelReadyForDisabling, Destroyed));
                        EntityWaitForDisablingEvent.Invoke(buttonsPanelDisablingType);
                    }));
            }

            ButtonBehaviour disablingButtonBehaviour = disablingButton.GetComponent<ButtonBehaviour>();

            if (disablingButtonBehaviour.Characteristics.Type == ButtonType.Pause)
                ProcessDisablingButtonClickedInternally(ButtonsPanelDisablingType.WithPreservation, buttonsPanelStatusParameter =>
                {
                    disablingButtonBehaviour.BeginHiding();
                    StartCoroutine(ProcessButtonsPanelStatusChangedIterativelyWithUnlocking());
                });
            else
                ProcessDisablingButtonClickedInternally(ButtonsPanelDisablingType.WithoutPreservation, buttonsPanelStatusParameter => BeginButtonsHiding());
        }

        private IEnumerator ProcessButtonsPanelStatusChangedIteratively()
        {
            ButtonBehaviour buttonBehaviour;
            Func<ButtonBehaviour, IEnumerator> buttonsPanelStatusChangedRoutineExtractor;
            Func<ButtonType, bool> isButtonShouldBeProcessedConditionFunction = buttonTypeParameter => true;

            if (EntityInfo.StatusData.Status == ButtonsPanelStatus.KeepingEnabled)
                isButtonShouldBeProcessedConditionFunction = buttonTypeParameter => buttonTypeParameter != ButtonType.Stop;
            else
                isButtonShouldBeProcessedConditionFunction = buttonTypeParameter => true;

            switch (EntityInfo.StatusData.Status)
            {
                case ButtonsPanelStatus.Disabled:
                    buttonsPanelStatusChangedRoutineExtractor = buttonBehaviourParameter => buttonBehaviourParameter.DisableIteratively();
                    break;
                default:
                    buttonsPanelStatusChangedRoutineExtractor = buttonBehaviourParameter => buttonBehaviourParameter.EnableIteratively();
                    break;
            }

            foreach (GameObject button in EntityInfo.Controls.Values)
            {
                buttonBehaviour = button.GetComponent<ButtonBehaviour>();

                if (isButtonShouldBeProcessedConditionFunction(buttonBehaviour.Characteristics.Type))
                    yield return buttonsPanelStatusChangedRoutineExtractor(buttonBehaviour);
            }
        }

        private IEnumerator ProcessButtonsPanelStatusChangedIterativelyWithUnlocking()
        {
            yield return ProcessButtonsPanelStatusChangedIteratively();

            EntityInfo.StatusData.Unlock();
        }

        protected override void PreProcessStart()
        {
            base.PreProcessStart();
            eventsListenersService.AddOrdinaryEventListener(() => StartCoroutine(ProcessButtonsPanelStatusChangedIteratively()),
                EntityInfo.StatusData.NonIterativelyChanged);
        }

        protected override void RemoveControlBehaviourEventsListeners(ButtonBehaviour controlBehaviour)
        {
            base.RemoveControlBehaviourEventsListeners(controlBehaviour);
            eventsListenersService.RemoveOrdinaryEventListener(controlBehaviour.AnimatedlyAppeared);
            controlBehaviour.Clicked.RemoveListener(OnButtonClicked);
        }

        protected override PanelType GetPanelType()
        {
            return PanelType.ButtonsPanel;
        }

        protected override ButtonBehaviour CreateAndInitiallySetupControlBehaviour(GameObject control, ControlBehaviourInstantiatingInfo controlBehaviourInstantiatingInfo)
        {
            ButtonBehaviour buttonBehaviour = base.CreateAndInitiallySetupControlBehaviour(control, controlBehaviourInstantiatingInfo);

            eventsListenersService.AddOrdinaryEventListener(appearedButtonParameter => OnButtonAnimatedlyAppeared(appearedButtonParameter,
                controlBehaviourInstantiatingInfo.Category, (ISet<ButtonType>)controlBehaviourInstantiatingInfo.Parameters[0]), buttonBehaviour.AnimatedlyAppeared);
            buttonBehaviour.Clicked.AddListener(OnButtonClicked);

            return buttonBehaviour;
        }

        public void CancelEntityDisabling()
        {
            ButtonsPanelReadyForDisabling.Invoke(ButtonsPanelStatusChangingRoutineProcessingType.Cancelling);
        }

        public void ChangeEntityStatus(ButtonsPanelStatus entityStatus)
        {
            StartCoroutine(EntityInfo.StatusData.SetStatusIteratively(entityStatus, ButtonsPanelStatusUpdateType.Immediately, 
                new EntityStatusChangingActions<ButtonsPanelStatus>(buttonsPanelStatusParameter => 
                StartCoroutine(ProcessButtonsPanelStatusChangedIterativelyWithUnlocking()), () => EntityInfo.StatusData.Unlock()), null, 
                () => EntityInfo.Controls.Count > 0));
        }

        public void CreateButtons(ButtonsPanelType buttonsType)
        {
            bool isDefaultButtonsPosition = buttonsType == ButtonsPanelType.Opening;
            ISet<ButtonType> buttonsTypesToCreate = GetButtonsTypesToCreate(buttonsType);

            foreach (ButtonType buttonType in buttonsTypesToCreate)
                CreateButton(buttonType, isDefaultButtonsPosition, buttonsTypesToCreate);

            EntityInfo.Type = buttonsType;
        }

        public void PerformEntityDisabling()
        {
            ButtonsPanelReadyForDisabling.Invoke(ButtonsPanelStatusChangingRoutineProcessingType.Performing);
        }

        private void OnButtonAnimatedlyAppeared(GameObject button, ButtonCategory buttonCategory, ISet<ButtonType> buttonsTypesToCreate)
        {
            ButtonBehaviour buttonBehaviour = button.GetComponent<ButtonBehaviour>();

            buttonsTypesToCreate.Remove(buttonBehaviour.Characteristics.Type);
            EntityInfo.Controls.Add(buttonCategory, button);

            if (buttonsTypesToCreate.Count == 0)
            {
                EntityInfo.StatusData.Status = (EntityInfo.Type == ButtonsPanelType.Opening) ? ButtonsPanelStatus.FullEnabled :
                    ((EntityInfo.Type == ButtonsPanelType.Process) ? ButtonsPanelStatus.KeepingEnabled : EntityInfo.StatusData.PreviousStatus.Value);

                if (EntityInfo.Type == ButtonsPanelType.Process)
                    EntityRunned.Invoke();
                else if (EntityInfo.Type == ButtonsPanelType.RestInterruption)
                    EntityEnabled.Invoke();
            }
        }

        private void OnButtonClicked(GameObject button)
        {
            ButtonBehaviour buttonBehaviour = button.GetComponent<ButtonBehaviour>();

            if ((buttonBehaviour.Characteristics.Type == ButtonType.Continue) || (buttonBehaviour.Characteristics.Type == ButtonType.Start))
            {
                buttonBehaviour.BeginHiding();
                EntityInfo.StatusData.Status = ButtonsPanelStatus.Disabled;
            }
            else
                ProcessDisablingButtonClicked(button);
        }

        protected override void OnControlAnimatedlyDisappeared(ButtonCategory controlCategory)
        {
            IEnumerator ProcessStopButtonDisappearedIteratively()
            {
                yield return new WaitUntil(() => EntityInfo.Controls.Count == 0);

                EntityObjectsDestroyed.Invoke();
            }

            Action GetButtonDisappearedCustomAdditionalAction()
            {
                if (!((EntityInfo.Type == ButtonsPanelType.None) && (controlCategory == ButtonCategory.Keeping)))
                {
                    ButtonType buttonType = EntityInfo.Controls[controlCategory].GetComponent<ButtonBehaviour>().Characteristics.Type;

                    switch (buttonType)
                    {
                        case ButtonType.Continue:
                            return () => CreateButtons(ButtonsPanelType.RestInterruption);
                        case ButtonType.Pause:
                            return () => CreateButtons(ButtonsPanelType.ProcessInterruption);
                        case ButtonType.Start:
                            return () => EntityActivated.Invoke();
                        default:
                            return () => StartCoroutine(ProcessStopButtonDisappearedIteratively());
                    }
                }
                else
                    return null;
            }

            Action buttonDestroyingCustomAdditionalAction = GetButtonDisappearedCustomAdditionalAction();

            base.OnControlAnimatedlyDisappeared(controlCategory);
            buttonDestroyingCustomAdditionalAction?.Invoke();
        }

        protected override void OnDestroy()
        {
            eventsListenersService.RemoveOrdinaryEventListener(EntityInfo.StatusData.NonIterativelyChanged);
            base.OnDestroy();
        }

        private class ButtonsPanelReadyForDisablingEvent : UnityEvent<ButtonsPanelStatusChangingRoutineProcessingType> { }
    }
}

namespace GameScene.Managers.ButtonsPanel.Data
{
    public class ButtonsPanelLockableStatusData : BaseEntityRoutinelyLockableStatusData<ButtonsPanelStatus, ButtonsPanelStatusChangingRoutinesExecutor>,
        IIterativelyGettableEntityStatusData<ButtonsPanelStatus>, INonIterativelySettableEntityStatusData,
        IWaitedlyTypicallyIterativelySettableEntityStatusData<ButtonsPanelStatus, ButtonsPanelStatusUpdateType, Guid>
    {
        public ButtonsPanelLockableStatusData() : base(ButtonsPanelStatus.Disabled)
        {
            PreviousStatus = null;
            NonIterativelyChanged = new UnityEvent();
        }

        public ButtonsPanelStatus? PreviousStatus { get; private set; }

        public UnityEvent NonIterativelyChanged { get; private set; }

        protected override bool TrySetStatus(ButtonsPanelStatus value)
        {
            ButtonsPanelStatus previousStatus = Status;
            bool result = base.TrySetStatus(value);

            if (result)
            {
                PreviousStatus = previousStatus;
                NonIterativelyChanged.Invoke();
            }

            return result;
        }

        public void ChangeSettingRoutineProcessingType(ButtonsPanelStatusChangingRoutineProcessingType settingRoutineProcessingType, Guid settingRoutineGuid)
        {
            switch (settingRoutineProcessingType)
            {
                case ButtonsPanelStatusChangingRoutineProcessingType.Cancelling:
                    routinesExecutor.CancelRoutineExecution(settingRoutineGuid);
                    break;
                default:
                    routinesExecutor.PrepareRoutineForExecution(settingRoutineGuid);
                    break;
            }
        }

        public IEnumerator GetStatusIteratively(Action<ButtonsPanelStatus> statusExtractedAction)
        {
            yield return routinesExecutor.ExecuteRoutineIteratively(WaitToGetStatusIteratively(statusExtractedAction), new ButtonsPanelStatusChangingRoutineInfo(Guid.NewGuid(),
                ButtonsPanelStatusChangingRoutineState.PreparedForExecution));
        }

        public IEnumerator SetStatusIteratively(ButtonsPanelStatus value, ButtonsPanelStatusUpdateType setterType,
            EntityStatusChangingActions<ButtonsPanelStatus> statusChangingActions, Action<Guid> setterInfoReceivedAction = null, Func<bool> waitingConditionFunction = null)
        {
            ButtonsPanelStatusChangingRoutineInfo buttonsPanelStatusSettingRoutineInfo = new ButtonsPanelStatusChangingRoutineInfo(Guid.NewGuid(),
                setterType == ButtonsPanelStatusUpdateType.Immediately ? ButtonsPanelStatusChangingRoutineState.PreparedForExecution :
                ButtonsPanelStatusChangingRoutineState.WaitForExecution);

            setterInfoReceivedAction?.Invoke(buttonsPanelStatusSettingRoutineInfo.Guid);

            yield return routinesExecutor.ExecuteRoutineIteratively(WaitToSetStatusIteratively(value, statusChangingActions), buttonsPanelStatusSettingRoutineInfo,
                waitingConditionFunction);
        }
    }
}

namespace GameScene.Managers.ButtonsPanel.Enums
{
    public enum ButtonsPanelDisablingType
    {
        WithoutPreservation,

        WithPreservation
    }

    public enum ButtonsPanelStatus
    {
        Disabled,

        FullEnabled,

        KeepingEnabled
    }

    public enum ButtonsPanelStatusUpdateType
    {
        Immediately,

        WaitForConfirmation
    }

    public enum ButtonsPanelType
    {
        None,

        Opening,

        Process,

        ProcessInterruption,

        RestInterruption
    }
}

namespace GameScene.Managers.ButtonsPanel.Events
{
    public class ButtonsPanelWaitForDisablingEvent : UnityEvent<ButtonsPanelDisablingType> { }
}

namespace GameScene.Managers.ButtonsPanel.Info
{
    public class ButtonsPanelInfo : ChildControlEntityInfo<ButtonCategory>
    {
        public ButtonsPanelInfo()
        {
            Type = ButtonsPanelType.None;
            StatusData = new ButtonsPanelLockableStatusData();
        }

        public ButtonsPanelType Type { get; set; }

        public ButtonsPanelLockableStatusData StatusData { get; private set; }
    }
}

namespace GameScene.Managers.ButtonsPanel.Settings
{
    [Serializable]
    public struct ButtonCapsuleColliderSettings
    {
        [SerializeField]
        private float radius;

        [SerializeField]
        private float height;

        public float Radius
        {
            get
            {
                return radius;
            }
        }

        public float Height
        {
            get
            {
                return height;
            }
        }
    }

    [Serializable]
    public struct ButtonPrefabsSettings
    {
        [SerializeField]
        private FormingButtonPrefabsSettings formingButtons;

        [SerializeField]
        private KeepingButtonPrefabsSettings keepingButtons;

        public FormingButtonPrefabsSettings FormingButtons
        {
            get
            {
                return formingButtons;
            }
        }

        public KeepingButtonPrefabsSettings KeepingButtons
        {
            get
            {
                return keepingButtons;
            }
        }
    }

    [Serializable]
    public struct FormingButtonPrefabsSettings
    {
        [SerializeField]
        private GameObject startButton;

        [SerializeField]
        private GameObject stopButton;

        public GameObject StartButton
        {
            get
            {
                return startButton;
            }
        }

        public GameObject StopButton
        {
            get
            {
                return stopButton;
            }
        }
    }

    [Serializable]
    public struct KeepingButtonPrefabsSettings
    {
        [SerializeField]
        private GameObject continueButton;

        [SerializeField]
        private GameObject pauseButton;

        public GameObject ContinueButton
        {
            get
            {
                return continueButton;
            }
        }

        public GameObject PauseButton
        {
            get
            {
                return pauseButton;
            }
        }
    }

    public abstract class BaseButtonCategorySettings<T>
    {
        [SerializeField]
        private T formingButton;

        [SerializeField]
        private T keepingButton;

        public T FormingButton
        {
            get
            {
                return formingButton;
            }
        }

        public T KeepingButton
        {
            get
            {
                return keepingButton;
            }
        }
    }

    [Serializable]
    public class ButtonAnimatorControllerSettings : BaseButtonCategorySettings<RuntimeAnimatorController> { }

    [Serializable]
    public class ButtonRectTransformPositionIndexesSettings : BaseButtonCategorySettings<int> { }

    [Serializable]
    public class ButtonRectTransformPositionSettings : BaseControlRectTransformPositionSettings, 
        IIndexableControlRectTransformPositionSettings<ButtonRectTransformPositionIndexesSettings>
    {
        [SerializeField]
        private ButtonRectTransformPositionIndexesSettings positionIndexes;

        public ButtonRectTransformPositionIndexesSettings PositionIndexes
        {
            get
            {
                return positionIndexes;
            }
        }
    }

    [Serializable]
    public class ButtonRectTransformSettings : BaseControlRectTransformSettings, IPositionableControlRectTransformSettings<ButtonRectTransformPositionSettings>
    {
        [SerializeField]
        private ButtonRectTransformPositionSettings positionSettings;

        public ButtonRectTransformPositionSettings PositionSettings
        {
            get
            {
                return positionSettings;
            }
        }
    }

    [Serializable]
    public class ButtonSettings : OwnedEntityObjectSettings, IRectTransformableControlSettings<ButtonRectTransformSettings>
    {
        [SerializeField]
        private ButtonCapsuleColliderSettings capsuleColliderSettings;

        [SerializeField]
        private ButtonPrefabsSettings prefabs;

        [SerializeField]
        private ButtonAnimatorControllerSettings animatorControllers;

        [SerializeField]
        private ButtonRectTransformSettings rectTransformSettings;

        public ButtonAnimatorControllerSettings AnimatorControllers
        {
            get
            {
                return animatorControllers;
            }
        }

        public ButtonCapsuleColliderSettings CapsuleColliderSettings
        {
            get
            {
                return capsuleColliderSettings;
            }
        }

        public ButtonPrefabsSettings Prefabs
        {
            get
            {
                return prefabs;
            }
        }

        public ButtonRectTransformSettings RectTransformSettings
        {
            get
            {
                return rectTransformSettings;
            }
        }
    }
}