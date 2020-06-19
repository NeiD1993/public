using GameScene.Managers;
using GameScene.Managers.Entity.Interfaces;
using UnityEngine;

namespace GameScene.Services.Managers
{
    public class EntityManagersAccessor : BaseSharedService
    {
        public T GetManager<T>() where T : BaseManager, IStandardEntityManager
        {
            return GameObject.Find(typeof(T).Name).GetComponent<T>();
        }
    }
}