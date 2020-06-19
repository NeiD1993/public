using GameScene.Managers.Entity.Settings;
using GameScene.Objects.Interfaces;
using UnityEngine.Events;

namespace GameScene.Managers.Entity
{
    public abstract class BaseNotifiedlyDestroyableEntityManager<T> : BaseEntityManager<T>, INotifiedlyDestroyableObject where T : SimpleEntityObjectSettings
    {
        public BaseNotifiedlyDestroyableEntityManager()
        {
            Destroyed = new UnityEvent();
        }

        public UnityEvent Destroyed { get; private set; }

        protected virtual void OnDestroy()
        {
            Destroyed.Invoke();
        }
    }
}