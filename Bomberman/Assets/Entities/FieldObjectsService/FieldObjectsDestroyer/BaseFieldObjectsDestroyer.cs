using Assets.Entities.FieldObjects;
using Assets.Entities.FieldObjects.FieldObject.FieldObjectsTypes;
using Assets.Entities.FieldObjectsService.BaseFieldObjectService;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Entities.FieldObjectsService.FieldObjectsDestroyer
{
    abstract class BaseFieldObjectsDestroyer : BaseChangableFieldObjectsService
    {
        public static readonly int maxObjectSecondsLifetime = 10;

        public static readonly int minObjectSecondsLifetime = 2;

        private int objectSecondsLifetime;

        public BaseFieldObjectsDestroyer(Field field, int objectSecondsLifetime) : base(field) 
        {
            ObjectSecondsLifetime = objectSecondsLifetime;
        }

        public int ObjectSecondsLifetime
        {
            get
            {
                return objectSecondsLifetime;
            }

            set
            {
                objectSecondsLifetime = ((value >= minObjectSecondsLifetime) && (value <= maxObjectSecondsLifetime)) ? value : minObjectSecondsLifetime;
            }
        }

        public abstract IEnumerator DestroyGameObjectInTimer(GameObject gameObject);

        public abstract void DestroyBomb(Vector2 powerUpIndexes);

        public abstract void DestroyGameObject(int horizontalIndex, int verticalIndex, FieldObjectType fieldObjectType, GameObject destroyerGameObject = null);

        public abstract void DestroyPowerUp(Vector2 powerUpIndexes);

        public abstract void DestroyLight();

        public abstract void DestroyExplosionWaveGameObject(GameObject explosionWaveGameObject);

        public abstract void DestroyGameObjectBetweenFieldIndexes(Vector2 startFieldIndexes, Vector2 endFieldIndexes, MoveDirection moveDirection);
    }
}
