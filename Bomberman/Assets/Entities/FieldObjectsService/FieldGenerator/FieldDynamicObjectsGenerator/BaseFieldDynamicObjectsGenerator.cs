using Assets.Entities.FieldObjects;
using Assets.Entities.FieldObjects.FieldObject.FieldObjectBehaviour.MovingBehaviour;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Entities.FieldObjectsService.FieldGenerator.FieldStaticObjectsGenerator
{
    abstract class BaseFieldDynamicObjectsGenerator : BaseFieldObjectsGenerator
    {
        public BaseFieldDynamicObjectsGenerator(Field field) : base(field) { }

        protected static FieldObjectMovingBehaviourType CreateMovingBehaviourForMoveableDynamicObject<FieldObjectMovingBehaviourType>(MoveDirection moveDirection, int? movingSpeed)
            where FieldObjectMovingBehaviourType : FieldObjectMovingBehaviour
        {
            if (movingSpeed != null)
            {
                FieldObjectMovingBehaviourType fieldObjectBehaviour = ScriptableObject.CreateInstance<FieldObjectMovingBehaviourType>();
                ((FieldObjectMovingBehaviourType)fieldObjectBehaviour).Init(moveDirection, movingSpeed.Value);
                return fieldObjectBehaviour;
            }

            return null;
        }
        public abstract GameObject CreateBomb(GameObject bombPrefab, GameObject explosionWavePartPrefab, Vector2 freePosition, byte bombDelay, int explosionWaveDistance, float explosionWaveDelay,
                                              float explosionWaveStep);

        public abstract void CreateEnemy(int enemiesCount, GameObject[] enemyPrefabs, int movingSpeed, byte delay);

        public abstract void CreateExplosionWave(GameObject explosionWavePartPrefab, Vector2 bombPosition, int explosionWaveDistance, float explosionWaveDelay, float explosionWaveStep);

        public abstract void CreatePlayer(GameObject playerPrefab, GameObject bombPrefab, GameObject explosionWavePartPrefab, int movingSpeed, byte bombDelay, int explosionWaveDistance,
                                          float explosionWaveDelay, float explosionWaveStep, int maxPlacedBombsCount);

        public abstract GameObject CreateLight(string materialTag, string killedEnemyTag = null, int? killedEnemyPoints = null);

        public abstract void CreateResultsText(string resultsText);
    }
}
