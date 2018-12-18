using Assets.Entities.FieldObjects;
using Assets.Scripts.Behaviour.ContinuedBehaviour;
using UnityEngine;

namespace Assets.Entities.FieldObjectsService.FieldGenerator.FieldStaticObjectsGenerator
{
    abstract class BaseFieldStaticObjectsGenerator : BaseFieldObjectsGenerator
    {
        protected static byte maxBreakableWallsPercentage = 50;

        protected static byte minBreakableWallsPercentage = 5;

        private byte breakableWallsPercentage;

        public BaseFieldStaticObjectsGenerator(byte breakableWallsPercentage, Field field) : base(field)
        {
            BreakableWallsPercentage = breakableWallsPercentage;
        }

        public byte BreakableWallsPercentage
        {
            get
            {
                return breakableWallsPercentage;
            }

            protected set
            {
                breakableWallsPercentage = ((breakableWallsPercentage != value) && ((value >= minBreakableWallsPercentage) && (value <= maxBreakableWallsPercentage))) ? value : minBreakableWallsPercentage;
            }
        }

        protected void AddFloor(GameObject floorPrefab, GameObject parentGameObject, Material floorMaterial)
        {
            GameObject floor = CreateGameObject(floorPrefab, parentGameObject, floorMaterial);
            Vector3 localScale = Field.WallPrefab.transform.localScale;

            floor.transform.localScale = new Vector3((Field.HorizontalSize * localScale.x) / 2, 1, (Field.VerticalSize * localScale.z) / 2);
        }

        protected void AddStatistics(GameObject statisticsPrefab, GameObject parentGameObject)
        {
            AddBehaviourToFieldObject<StatisticsBehaviour>(statisticsPrefab);
            CreateGameObject(statisticsPrefab, parentGameObject);
        }

        public abstract void GenerateField(GameObject floorPrefab, GameObject helpTextPrefab, GameObject statisticsPrefab, Material breakableWallMaterial, Material floorMaterial, Material unbreakableWallMaterial, 
                                           int[] powerUpsGeneratedCount, GameObject[] powerUpPrefabs, Vector3 powerUpsRotationAngle, float powerUpDelay);
    }
}
