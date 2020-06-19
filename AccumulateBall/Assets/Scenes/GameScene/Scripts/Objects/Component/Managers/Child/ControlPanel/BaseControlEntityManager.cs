using System;
using System.Collections;
using GameScene.Managers.ControlPanel.Info;
using GameScene.Managers.Entity;
using GameScene.Managers.Entity.Interfaces;
using GameScene.Managers.Entity.Settings;
using GameScene.Services.ControlPanel;
using GameScene.Services.ControlPanel.Info;
using ServicesLocators;
using UnityEngine;
using UnityEngine.Events;

namespace GameScene.Managers.ControlPanel
{
    public abstract class BaseControlEntityManager<T1, T2> : BaseNotifiedlyDestroyableEntityManager<T1>, IActivatableEntityManager where T1 : SimpleEntityObjectSettings
        where T2 : BasicControlEntityInfo, new()
    {
        private PanelsAccessor panelsAccessor;

        public BaseControlEntityManager()
        {
            EntityInfo = new T2();
            EntityActivated = new UnityEvent();
        }

        protected T2 EntityInfo { get; set; }

        public override GameObject Entity
        {
            get
            {
                return EntityInfo.Panel;
            }
        }

        public UnityEvent EntityActivated { get; private set; }

        protected abstract PanelInfo GetPanelInfo();

        protected override void ObtainSharingRelatedServices()
        {
            base.ObtainSharingRelatedServices();
            panelsAccessor = SharedSceneServicesLocator.GetService<PanelsAccessor>();
        }

        protected override void PreProcessStart()
        {
            base.PreProcessStart();
            EntityInfo.Panel = panelsAccessor.GetPanel(GetPanelInfo());
        }
    }
}

namespace GameScene.Managers.ControlPanel.Info
{
    public class BasicControlEntityInfo
    {
        public GameObject Panel { get; set; }
    }
}

namespace GameScene.Managers.ControlPanel.Interfaces
{
    public interface ICalibratableControlEntityManager
    {
        IEnumerator CalibrateEntityIteratively();
    }

    public interface IDisableableControlEntityManager
    {
        void EnableEntity();

        void DisableEntity();
    }

    public interface IDisableableControlEntityManager<T1, T2> where T1 : Enum where T2 : UnityEvent<T1>
    {
        UnityEvent EntityEnabled { get; }

        T2 EntityWaitForDisablingEvent { get; }

        void CancelEntityDisabling();

        void PerformEntityDisabling();
    }

    public interface IIntensifiableControlEntityManager
    {
        void IntensifyEntity();
    }

    public interface INotifiedlyDestroyableByObjectsControlEntityManager
    {
        UnityEvent EntityObjectsDestroyed { get; }
    }

    public interface INotifiedlyRunnableControlEntityManager
    {
        UnityEvent EntityRunned { get; }
    }

    public interface IRefreshableControlEntityManager<T1, T2>
    {
        void RefreshEntityPartially(T1 refreshingParameter);

        IEnumerator RefreshEntityCompletelyIteratively(T2 refreshingParameter);
    }

    public interface IStatusChangingControlEntityManager<T> where T : Enum
    {
        void ChangeEntityStatus(T entityStatus);
    }
}