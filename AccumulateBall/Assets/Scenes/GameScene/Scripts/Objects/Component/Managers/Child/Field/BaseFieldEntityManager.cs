using GameScene.Managers.Entity;
using GameScene.Managers.Entity.Data;
using GameScene.Managers.Entity.Settings;
using GameScene.Managers.Field.Data;
using GameScene.Managers.Field.Enums;
using GameScene.Managers.Field.Info;
using GameScene.Managers.Field.Interfaces;
using UnityEngine.Events;

namespace GameScene.Managers.Field
{
    public abstract class BaseFieldEntityManager<T1, T2, T3, T4> : BaseNotifiedlyDestroyableEntityManager<T1>, IStandardFieldEntityManager where T1 : SimpleEntityObjectSettings
        where T2 : BaseFieldEntityInfo<T3>, new() where T3 : BaseEntityLockableStatusData<FieldEntityPrimalStatusData> where T4 : UnityEventBase, new()
    {
        public BaseFieldEntityManager()
        {
            SetupEntityInfo();
            EntityPrimalStatusChanged = new T4();
        }

        protected T2 EntityInfo { get; set; }

        public T4 EntityPrimalStatusChanged { get; private set; }

        public abstract FieldEntityPrimalStatusData SummaryEntityPrimalStatus { get; }

        protected abstract void ResumeEntityInternally();

        protected abstract void SuspendEntityInternally();

        protected virtual void SetupEntityInfo()
        {
            EntityInfo = new T2();
        }

        public void ResumeEntity()
        {
            ResumeEntityInternally();
            EntityInfo.Suspension = FieldEntitySuspensionStatus.NotSuspended;
        }

        public void SuspendEntity()
        {
            SuspendEntityInternally();
            EntityInfo.Suspension = FieldEntitySuspensionStatus.Suspended;
        }

        public void Unlock()
        {
            EntityInfo.PrimalStatusData.Unlock();
        }
    }
}

namespace GameScene.Managers.Field.Data
{
    public struct FieldEntityPrimalStatusData
    {
        public FieldEntityPrimalStatusData(FieldEntityAvailabilityStatus availability = FieldEntityAvailabilityStatus.Unavailable,
            FieldEntityFinalizationStatus finalization = FieldEntityFinalizationStatus.NotReadyForFinalization)
        {
            Availability = availability;
            Finalization = finalization;
        }

        public FieldEntityAvailabilityStatus Availability { get; private set; }

        public FieldEntityFinalizationStatus Finalization { get; private set; }

        public FieldEntityPrimalStatusData GetWithChangedAvailability(FieldEntityAvailabilityStatus availability)
        {
            return new FieldEntityPrimalStatusData(availability, Finalization);
        }
    }
}

namespace GameScene.Managers.Field.Enums
{
    public enum FieldEntityAvailabilityStatus
    {
        Available = 1,

        Unavailable = 0
    }

    public enum FieldEntityFinalizationStatus
    {
        NotReadyForFinalization = 0,

        ReadyForFinalization = 1
    }

    public enum FieldEntitySuspensionStatus
    {
        NotSuspended,

        Suspended
    }
}

namespace GameScene.Managers.Field.Info
{
    public abstract class BaseFieldEntityInfo<T> where T : BaseEntityLockableStatusData<FieldEntityPrimalStatusData>
    {
        public BaseFieldEntityInfo(T primalStatusData)
        {
            PrimalStatusData = primalStatusData;
            Suspension = FieldEntitySuspensionStatus.NotSuspended;
        }

        public FieldEntitySuspensionStatus Suspension { get; set; }

        public T PrimalStatusData { get; private set; }
    }
}

namespace GameScene.Managers.Field.Interfaces
{
    public interface IAcceptableFieldEntityManager<T> where T : UnityEventBase
    {
        T EntityAccepted { get; }
    }
    
    public interface INotifiedlyBannableFieldEntityManager
    {
        UnityEvent EntityBanned { get; }

        UnityEvent EntityPreBanned { get; }
    }

    public interface INotifiedlyUnbannableFieldEntityManager
    {
        UnityEvent EntityUnbanned { get; }
    }

    public interface IStandardFieldEntityManager
    {
        FieldEntityPrimalStatusData SummaryEntityPrimalStatus { get; }

        void ResumeEntity();

        void SuspendEntity();

        void Unlock();
    }
}