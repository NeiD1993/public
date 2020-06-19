using System;
using System.Collections;
using GameScene.Managers.Entity.Actions;
using GameScene.Managers.Entity.Interfaces;
using GameScene.Managers.Entity.Settings;
using GameScene.Managers.Entity.Settings.Interfaces;
using GameScene.Services.Routines;
using UnityEngine;
using UnityEngine.Events;

namespace GameScene.Managers.Entity
{
    public abstract class BaseEntityManager<T1> : BaseManager, IStandardEntityManager where T1 : SimpleEntityObjectSettings
    {
        [SerializeField]
        protected T1 entityObjectSettings;

        public abstract GameObject Entity { get; }
        
        protected void AttachToEntity(GameObject childEntity)
        {
            childEntity.transform.parent = Entity.transform;
        }

        protected class EntityPartSetupInfo<T2> where T2 : SimpleEntityObjectSettings
        {
            public EntityPartSetupInfo(GameObject entity, IPartialEntityObjectSettings<T2> entityObjectSettings)
            {
                Part = entity.transform.Find(entityObjectSettings.RootName).Find(entityObjectSettings.PartSettings.InstanceName).gameObject;
            }

            public GameObject Part { get; private set; }
        }
    }
}

namespace GameScene.Managers.Entity.Actions
{
    public struct EntityStatusChangingActions<T>
    {
        public EntityStatusChangingActions(Action<T> changedAction, Action unchangedAction)
        {
            ChangedAction = changedAction;
            UnchangedAction = unchangedAction;
        }

        public Action UnchangedAction { get; private set; }

        public Action<T> ChangedAction { get; private set; }
    }
}

namespace GameScene.Managers.Entity.Data
{
    public abstract class BaseEntityLockableStatusData<T>
    {
        private T status;

        public BaseEntityLockableStatusData(T status)
        {
            this.status = status;
            IsLocked = false;
        }

        private bool IsLocked { get; set; }

        public T Status
        {
            get
            {
                return status;
            }

            set
            {
                TrySetStatus(value);
            }
        }

        protected virtual bool TrySetStatus(T value)
        {
            if (!value.Equals(status))
            {
                status = value;

                return true;
            }

            return false;
        }

        protected IEnumerator PerformLockingStatusAction(Action statusAction)
        {
            IsLocked = true;
            statusAction();

            yield return new WaitUntil(() => IsLocked == false);
        }

        protected IEnumerator WaitToSetStatusIteratively(T value, EntityStatusChangingActions<T> statusChangingActions)
        {
            yield return PerformLockingStatusAction(() =>
            {
                if (TrySetStatus(value))
                    statusChangingActions.ChangedAction(Status);
                else
                    statusChangingActions.UnchangedAction();
            });
        }

        public void Unlock()
        {
            IsLocked = false;
        }
    }

    public abstract class BaseEntityRoutinelyLockableStatusData<T1, T2> : BaseEntityLockableStatusData<T1> where T2 : BaseRoutinesExecutor, new()
    {
        protected readonly T2 routinesExecutor;

        public BaseEntityRoutinelyLockableStatusData(T1 status) : base(status)
        {
            Status = status;
            routinesExecutor = new T2();
        }

        protected IEnumerator WaitToGetStatusIteratively(Action<T1> statusExtractedAction)
        {
            yield return PerformLockingStatusAction(() => statusExtractedAction(Status));
        }
    }
}

namespace GameScene.Managers.Entity.Data.Interfaces
{
    public interface IIterativelyGettableEntityStatusData<T>
    {
        IEnumerator GetStatusIteratively(Action<T> statusExtractedAction);
    }

    public interface IIterativelySettableEntityStatusData<T>
    {
        IEnumerator SetStatusIteratively(T value);
    }

    public interface INonIterativelySettableEntityStatusData
    {
        UnityEvent NonIterativelyChanged { get; }
    }

    public interface IWaitedlyIterativelySettableEntityStatusData<T>
    {
        IEnumerator SetStatusIteratively(T value, Func<bool> waitingConditionFunction, EntityStatusChangingActions<T> statusChangingActions);
    }

    public interface IWaitedlyTypicallyIterativelySettableEntityStatusData<T1, T2, T3> where T2 : Enum where T3 : struct
    {
        IEnumerator SetStatusIteratively(T1 value, T2 setterType, EntityStatusChangingActions<T1> statusChangingActions, Action<T3> setterInfoReceivedAction = null,
            Func<bool> waitingConditionFunction = null);
    }
}

namespace GameScene.Managers.Entity.Interfaces
{
    public interface IActivatableEntityManager
    {
        UnityEvent EntityActivated { get; }
    }

    public interface IIterativelyRunnableEntityManager
    {
        IEnumerator RunEntityIteratively();
    }

    public interface IStandardEntityManager
    {
        GameObject Entity { get; }
    }
}

namespace GameScene.Managers.Entity.Settings
{
    [Serializable]
    public class DisplaceableEntityObjectSettings : SimpleEntityObjectSettings
    {
        [SerializeField]
        private float displacement;

        public float Displacement
        {
            get
            {
                return displacement;
            }
        }
    }

    [Serializable]
    public class OwnedEntityObjectSettings : SimpleEntityObjectSettings, IOwnedEntityObjectSettings
    {
        [SerializeField]
        private string ownerInstanceName;

        public string OwnerInstanceName
        {
            get
            {
                return ownerInstanceName;
            }
        }
    }

    [Serializable]
    public class SimpleEntityObjectSettings
    {
        [SerializeField]
        private string instanceName;

        public string InstanceName
        {
            get
            {
                return instanceName;
            }
        }
    }
}

namespace GameScene.Managers.Entity.Settings.Interfaces
{
    public interface IDelayedEntityObjectSettings<T> where T : struct
    {
        T Delays { get; }
    }

    public interface IOwnedEntityObjectSettings
    {
        string OwnerInstanceName { get; }
    }

    public interface IPartialEntityObjectSettings<T> where T : SimpleEntityObjectSettings
    {
        string RootName { get; }

        T PartSettings { get; }
    }

    public interface ISingleAnimatorControllerEntityObjectSettings
    {
        RuntimeAnimatorController AnimatorController { get; }
    }

    public interface ISinglePrefabEntityObjectSettings
    {
        GameObject Prefab { get; }
    }

    public interface ISizeableEntityComponentSettings<T> where T : struct
    {
        T Size { get; }
    }

    public interface IUnitedlyGettableEntityCategorySettings<T1, T2> where T2 : Enum
    {
        T1 GetSettings(T2 category);
    }
}