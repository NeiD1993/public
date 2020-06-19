using System;
using System.Collections;
using GameScene.Managers.ButtonsPanel;
using GameScene.Managers.ButtonsPanel.Enums;
using GameScene.Managers.ControlPanel.Enums;
using GameScene.Managers.ControlPanel.Events;
using GameScene.Managers.ControlPanel.Info;
using GameScene.Managers.ControlPanel.Interfaces;
using GameScene.Managers.Entity.Settings;
using GameScene.Managers.ScoreItems;
using GameScene.Services.ControlPanel.Enums;
using GameScene.Services.ControlPanel.Info;
using GameScene.Services.Game.Characteristics.Details;
using GameScene.Services.Game.Units;
using GameScene.Services.Managers;
using ServicesLocators;
using UnityEngine.Events;

namespace GameScene.Managers.ControlPanel
{
    public class ControlPanelManager : BaseControlEntityManager<SimpleEntityObjectSettings, BasicControlEntityInfo>,
        IDisableableControlEntityManager<ControlPanelDisablingType, ControlPanelWaitForDisablingEvent>, ICalibratableControlEntityManager, IIntensifiableControlEntityManager, 
        INotifiedlyDestroyableByObjectsControlEntityManager, INotifiedlyRunnableControlEntityManager, IStatusChangingControlEntityManager<ControlPanelStatus>,
        IRefreshableControlEntityManager<GameStageContentCharacteristicsDetails, GameStageContentUnits<int, TimeSpan, int>>
    {
        private EntityManagersAccessor entityManagersAccessor;

        public ControlPanelManager()
        {
            EntityEnabled = new UnityEvent();
            EntityObjectsDestroyed = new UnityEvent();
            EntityRunned = new UnityEvent();
            EntityWaitForDisablingEvent = new ControlPanelWaitForDisablingEvent();
        }

        private ButtonsPanelManager ButtonsPanelManager { get; set; }

        private ScoreItemsPanelManager ScoreItemsPanelManager { get; set; }

        public UnityEvent EntityEnabled { get; private set; }

        public UnityEvent EntityObjectsDestroyed { get; private set; }

        public UnityEvent EntityRunned { get; private set; }

        public ControlPanelWaitForDisablingEvent EntityWaitForDisablingEvent { get; private set; }

        private void ObtainChildControlEntityManagers()
        {
            ButtonsPanelManager = entityManagersAccessor.GetManager<ButtonsPanelManager>();
            ScoreItemsPanelManager = entityManagersAccessor.GetManager<ScoreItemsPanelManager>();
        }

        private void SetupButtonsPanelManager()
        {
            ButtonsPanelManager.Destroyed.AddListener(OnButtonsPanelManagerDestroyed);
            ButtonsPanelManager.EntityActivated.AddListener(OnButtonsActivated);
            ButtonsPanelManager.EntityEnabled.AddListener(OnButtonsEnabled);
            ButtonsPanelManager.EntityObjectsDestroyed.AddListener(OnButtonsDestroyed);
            ButtonsPanelManager.EntityRunned.AddListener(OnButtonsRunned);
            ButtonsPanelManager.EntityWaitForDisablingEvent.AddListener(OnButtonsWaitForDisabling);
        }

        private void SetupChildControlEntityManagers()
        {
            SetupButtonsPanelManager();
            SetupScoreItemsPanelManager();
        }

        private void SetupScoreItemsPanelManager()
        {
            ScoreItemsPanelManager.Destroyed.AddListener(OnScoreItemsPanelManagerDestroyed);
            ScoreItemsPanelManager.EntityActivated.AddListener(OnScoreItemsActivated);
        }

        protected override void PerpetrateStartProcessing()
        {
            SetupChildControlEntityManagers();
            base.PerpetrateStartProcessing();
        }

        protected override void ObtainSharingRelatedServices()
        {
            base.ObtainSharingRelatedServices();
            entityManagersAccessor = SharedSceneServicesLocator.GetService<EntityManagersAccessor>();
        }

        protected override void PreProcessStart()
        {
            base.PreProcessStart();
            ObtainChildControlEntityManagers();
        }

        protected override PanelInfo GetPanelInfo()
        {
            return new PanelInfo(PanelType.ControlPanel, entityObjectSettings.InstanceName);
        }

        protected override Func<bool> GetCanConfigureConditionFunction()
        {
            return () => (ButtonsPanelManager?.IsConfigurated).Value && (ScoreItemsPanelManager?.IsConfigurated).Value;
        }

        public void CancelEntityDisabling()
        {
            ButtonsPanelManager.CancelEntityDisabling();
        }

        public void ChangeEntityStatus(ControlPanelStatus entityStatus)
        {
            ButtonsPanelStatus buttonsStatus;

            switch (entityStatus)
            {
                case ControlPanelStatus.CompletelyDisabled:
                    buttonsStatus = ButtonsPanelStatus.Disabled;
                    break;
                case ControlPanelStatus.DisabledForFinalization:
                    buttonsStatus = ButtonsPanelStatus.KeepingEnabled;
                    break;
                default:
                    buttonsStatus = ButtonsPanelStatus.FullEnabled;
                    break;
            }

            ButtonsPanelManager.ChangeEntityStatus(buttonsStatus);
        }

        public void InitializeControlPanel()
        {
            ButtonsPanelManager.CreateButtons(ButtonsPanelType.Opening);
        }

        public void IntensifyEntity()
        {
            ScoreItemsPanelManager.IntensifyEntity();
        }

        public void PerformEntityDisabling()
        {
            ButtonsPanelManager.PerformEntityDisabling();
            ScoreItemsPanelManager.DisableEntity();
        }

        public void RefreshEntityPartially(GameStageContentCharacteristicsDetails refreshingParameter)
        {
            ScoreItemsPanelManager.RefreshEntityPartially(refreshingParameter);
        }

        public IEnumerator BeginControlPanelRunningIteratively(ControlPanelRunningType controlPanelRunningType)
        {
            yield return ScoreItemsPanelManager.RunEntityIteratively();

            if (controlPanelRunningType == ControlPanelRunningType.Wholly)
                ButtonsPanelManager.CreateButtons(ButtonsPanelType.Process);
            else
                EntityRunned.Invoke();
        }

        public IEnumerator CalibrateEntityIteratively()
        {
            yield return ScoreItemsPanelManager.CalibrateEntityIteratively();
        }

        public IEnumerator RefreshEntityCompletelyIteratively(GameStageContentUnits<int, TimeSpan, int> refreshingParameter)
        {
            yield return ScoreItemsPanelManager.RefreshEntityCompletelyIteratively(refreshingParameter);
        }

        private void OnButtonsActivated()
        {
            StartCoroutine(ScoreItemsPanelManager.CreateScoreItemsIteratively());
        }

        private void OnButtonsDestroyed()
        {
            IEnumerator ProcessButtonsDestroyedIteratively()
            {
                yield return ScoreItemsPanelManager.HideScoreItemsIteratively();

                EntityObjectsDestroyed.Invoke();
            }

            StartCoroutine(ProcessButtonsDestroyedIteratively());
        }

        private void OnButtonsEnabled()
        {
            ScoreItemsPanelManager.EnableEntity();
            EntityEnabled.Invoke();
        }

        private void OnButtonsPanelManagerDestroyed()
        {
            ButtonsPanelManager.Destroyed.RemoveListener(OnButtonsPanelManagerDestroyed);
            ButtonsPanelManager.EntityActivated.RemoveListener(OnButtonsActivated);
            ButtonsPanelManager.EntityEnabled.RemoveListener(OnButtonsEnabled);
            ButtonsPanelManager.EntityObjectsDestroyed.RemoveListener(OnButtonsDestroyed);
            ButtonsPanelManager.EntityRunned.RemoveListener(OnButtonsRunned);
            ButtonsPanelManager.EntityWaitForDisablingEvent.RemoveListener(OnButtonsWaitForDisabling);
        }

        private void OnButtonsRunned()
        {
            EntityRunned.Invoke();
        }

        private void OnButtonsWaitForDisabling(ButtonsPanelDisablingType buttonsDisablingType)
        {
            ControlPanelDisablingType controlPanelDisablingType;

            switch (buttonsDisablingType)
            {
                case ButtonsPanelDisablingType.WithoutPreservation:
                    controlPanelDisablingType = ControlPanelDisablingType.WithButtonsDestroying;
                    break;
                default:
                    controlPanelDisablingType = ControlPanelDisablingType.WithoutButtonsDestroying;
                    break;
            }

            EntityWaitForDisablingEvent.Invoke(controlPanelDisablingType);
        }

        private void OnScoreItemsActivated()
        {
            EntityActivated.Invoke();
        }

        private void OnScoreItemsPanelManagerDestroyed()
        {
            ScoreItemsPanelManager.Destroyed.RemoveListener(OnScoreItemsPanelManagerDestroyed);
            ScoreItemsPanelManager.EntityActivated.RemoveListener(OnScoreItemsActivated);
        }
    }
}

namespace GameScene.Managers.ControlPanel.Enums
{
    public enum ControlPanelDisablingType
    {
        WithButtonsDestroying,

        WithoutButtonsDestroying
    }

    public enum ControlPanelRunningType
    {
        Wholly,

        WithoutButtons
    }

    public enum ControlPanelStatus
    {
        CompletelyDisabled,

        DisabledForFinalization,

        Enabled
    }
}

namespace GameScene.Managers.ControlPanel.Events
{
    public class ControlPanelWaitForDisablingEvent : UnityEvent<ControlPanelDisablingType> { }
}