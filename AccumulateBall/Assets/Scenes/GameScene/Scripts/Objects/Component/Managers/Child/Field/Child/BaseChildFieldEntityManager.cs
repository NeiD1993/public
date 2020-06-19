using System;
using System.Collections;
using GameScene.Behaviours.FieldObject.Interfaces;
using GameScene.Behaviours.MaterializedObject.Interfaces;
using GameScene.Managers.Entity.Actions;
using GameScene.Managers.Entity.Data;
using GameScene.Managers.Entity.Data.Interfaces;
using GameScene.Managers.Entity.Settings;
using GameScene.Managers.Entity.Settings.Interfaces;
using GameScene.Managers.Field.Data;
using GameScene.Managers.Field.Info;
using GameScene.Managers.Field.Settings;
using GameScene.Objects.Interfaces;
using UnityEngine;
using UnityEngine.Events;

namespace GameScene.Managers.Field
{
    public abstract class BaseChildFieldEntityManager<T1, T2, T3, T4> : BaseFieldEntityManager<T2, T3, ChildFieldEntityPrimalStatusData, UnityEvent> 
        where T1 : MonoBehaviour, INotifiedlyDestroyableObject, IHideableMaterializedObjectBehaviour, ILaunchableFieldObjectBehaviour where T2 : FieldObjectSettings 
        where T3 : ChildFieldEntityInfo, new() where T4 : UnityEventBase, new()
    {
        public BaseChildFieldEntityManager()
        {
            EntityLaunched = new UnityEvent();
            EntityPlaced = new T4();
        }

        public override FieldEntityPrimalStatusData SummaryEntityPrimalStatus
        {
            get
            {
                return EntityInfo.PrimalStatusData.Status;
            }
        }

        public UnityEvent EntityLaunched { get; private set; }

        public T4 EntityPlaced { get; private set; }

        protected virtual void RemoveObjectBehaviourEventsListeners(T1 objectBehaviour)
        {
            eventsListenersService.RemoveOrdinaryEventListener(objectBehaviour.Destroyed);
            eventsListenersService.RemoveOrdinaryEventListener(objectBehaviour.AnimatedlyLaunched);
        }

        protected override void SetupEntityInfo()
        {
            base.SetupEntityInfo();
            EntityInfo.PrimalStatusData.NonIterativelyChanged.AddListener(OnEntityPrimalStatusNonIterativelyChanged);
        }

        protected virtual T1 CreateAndInitiallySetupObjectBehaviour(GameObject obj, Vector2Int objectPosition, params object[] parameters)
        {
            obj.AddComponent<T1>();

            T1 objectBehaviour = obj.GetComponent<T1>();

            eventsListenersService.AddOrdinaryEventListener(() => RemoveObjectBehaviourEventsListeners(objectBehaviour), objectBehaviour.Destroyed);
            eventsListenersService.AddOrdinaryEventListener(objectParameter => OnObjectAnimatedlyLaunched(objectParameter, objectPosition), 
                objectBehaviour.AnimatedlyLaunched);

            return objectBehaviour;
        }

        protected abstract void OnObjectAnimatedlyLaunched(GameObject obj, Vector2Int objectPosition);

        protected override void OnDestroy()
        {
            EntityInfo.PrimalStatusData.NonIterativelyChanged.RemoveListener(OnEntityPrimalStatusNonIterativelyChanged);
            base.OnDestroy();
        }

        private void OnEntityPrimalStatusNonIterativelyChanged()
        {
            EntityPrimalStatusChanged.Invoke();
        }        
    }
}

namespace GameScene.Managers.Field.Data
{
    public class ChildFieldEntityPrimalStatusData : BaseEntityLockableStatusData<FieldEntityPrimalStatusData>, 
        IIterativelySettableEntityStatusData<FieldEntityPrimalStatusData>, INonIterativelySettableEntityStatusData
    {
        public ChildFieldEntityPrimalStatusData() : base(new FieldEntityPrimalStatusData())
        {
            NonIterativelyChanged = new UnityEvent();
        }

        public UnityEvent NonIterativelyChanged { get; private set; }

        public IEnumerator SetStatusIteratively(FieldEntityPrimalStatusData value)
        {
            yield return WaitToSetStatusIteratively(value, 
                new EntityStatusChangingActions<FieldEntityPrimalStatusData>(entityPrimalStatusParameter => NonIterativelyChanged.Invoke(), () => Unlock()));
        }
    }
}

namespace GameScene.Managers.Field.Info
{
    public class ChildFieldEntityInfo : BaseFieldEntityInfo<ChildFieldEntityPrimalStatusData>
    {
        public ChildFieldEntityInfo() : base(new ChildFieldEntityPrimalStatusData()) { }
    }
}

namespace GameScene.Managers.Field.Interfaces
{
    public interface INotifiedlyDestroyableByObjectChildFieldEntityManager<T> where T : UnityEventBase
    {
        T EntityObjectDestroyed { get; }
    }
}

namespace GameScene.Managers.Field.Settings
{
    [Serializable]
    public class FieldObjectSettings : DisplaceableEntityObjectSettings, ISinglePrefabEntityObjectSettings
    {
        [SerializeField]
        private GameObject prefab;

        public GameObject Prefab
        {
            get
            {
                return prefab;
            }
        }
    }
}