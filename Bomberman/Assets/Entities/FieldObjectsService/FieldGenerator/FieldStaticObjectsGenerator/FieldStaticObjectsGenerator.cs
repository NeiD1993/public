using Assets.Entities.FieldObjects;
using Assets.Entities.FieldObjects.FieldObject;
using Assets.Entities.FieldObjects.FieldObject.FieldObjectBehaviour;
using Assets.Entities.FieldObjects.FieldObject.FieldObjectsTypes;
using Assets.Entities.FieldObjectsService.FieldGenerator.FieldStaticObjectsGenerator;
using Assets.Scripts.Behaviour.ContinuedBehaviour;
using System;
using UnityEngine;

namespace Assets.Entities.FieldObjectsService.FieldStaticObjectsGenerator
{
    class FieldStaticObjectsGenerator : BaseFieldStaticObjectsGenerator
    {
        protected static int minHorizontalSize = 5;

        protected static int minVerticalSize = 5;

        public FieldStaticObjectsGenerator(byte breakableWallsPercentage, Field field) : base(breakableWallsPercentage, field) { }

        protected static bool IsIndexForUnbreakableWall(int index)
        {
            return (index % 2 == 0);
        }

        protected void GenerateBreakableWalls(Material breakableWallMaterial)
        {
            int horizontalIndex;
            int horizontalSize = Field.HorizontalSize;
            int verticalIndex;
            int verticalSize = Field.VerticalSize;
            int unplacedBreakableWallsCount = ((horizontalSize * verticalSize - (2 * (horizontalSize + verticalSize - 2) + (horizontalSize / 2 - 1) * (verticalSize / 2 - 1))) *
                                               BreakableWallsPercentage) / 100;

            while (unplacedBreakableWallsCount > 0)
            {
                horizontalIndex = randomGenerator.Next(1, horizontalSize - 1);
                verticalIndex = randomGenerator.Next(1, verticalSize - 1);
                if (Field.FieldObjects[horizontalIndex][verticalIndex].ObjectType == FieldObjectType.Empty)
                {
                    ChangeGameObjectOnField(horizontalIndex, verticalIndex, FieldObjectType.BreakableWall, Field.WallPrefab, breakableWallMaterial);
                    RemoveFreePosition(new Vector2(horizontalIndex, verticalIndex));
                    unplacedBreakableWallsCount--;
                }
            }
        }

        protected void GenerateUnbreakableWalls(Material unbreakableWallMaterial)
        {
            int horizontalSize = Field.HorizontalSize;
            int verticalSize = Field.VerticalSize;

            for (int i = 0; i < horizontalSize; i++)
            {
                for (int j = 0; j < verticalSize; j++)
                {
                    if ((((i == 0) || (i == horizontalSize - 1)) || ((j == 0) || (j == verticalSize - 1))) || ((IsIndexForUnbreakableWall(i) && IsIndexForUnbreakableWall(j))))
                        ChangeGameObjectOnField(i, j, FieldObjectType.UnbreakableWall, Field.WallPrefab, unbreakableWallMaterial);
                    else
                        AddFreePosition(new Vector2(i, j));
                }
            }
        }

        protected void GeneratePowerUps(int[] powerUpsGeneratedCount, GameObject[] powerUpPrefabs, Vector3 powerUpsRotationAngle, float powerUpDelay)
        {
            bool isPowerUpsGenerated = false;
            int horizontalIndex;
            int horizontalSize = Field.HorizontalSize;
            int verticalIndex;
            int verticalSize = Field.VerticalSize;
            int[] createdPowerUpsCount = new int[powerUpsGeneratedCount.Length];
            Vector2 powerUpFieldIndexes;
            GameObject powerUpClone;

            while (!isPowerUpsGenerated)
            {
                horizontalIndex = randomGenerator.Next(1, horizontalSize - 1);
                verticalIndex = randomGenerator.Next(1, verticalSize - 1);

                if (Field.FieldObjects[horizontalIndex][verticalIndex].ObjectType == FieldObjectType.BreakableWall)
                {
                    powerUpFieldIndexes = new Vector2(horizontalIndex, verticalIndex);

                    if (!Field.PowerUps.ContainsKey(powerUpFieldIndexes))
                    {
                        int powerUpIndex = GetCreatablePowerUpIndex(powerUpsGeneratedCount, createdPowerUpsCount);

                        PowerUpFieldObjectBehaviour powerUpFieldObjectBehaviour = CreateFieldObjectBehaviour<PowerUpFieldObjectBehaviour>();

                        powerUpFieldObjectBehaviour.Init(powerUpsRotationAngle);
                        AddContinuedFieldObjectBehaviour<PowerUpBehaviour>(powerUpPrefabs[powerUpIndex], powerUpFieldObjectBehaviour, powerUpFieldIndexes, powerUpDelay);
                        powerUpClone = CreateGameObject(powerUpPrefabs[powerUpIndex], Field.PowerUpsGameObject);
                        ChangeGameObjectPosition(horizontalIndex, verticalIndex, powerUpClone);
                        Field.PowerUps.Add(powerUpFieldIndexes, new PowerUp((PowerUpTypes)powerUpIndex, Field, powerUpClone));
                        createdPowerUpsCount[powerUpIndex]++;

                        isPowerUpsGenerated = IsPowerUpsGenerated(powerUpsGeneratedCount, createdPowerUpsCount);
                    }
                }
            }
        }

        protected bool IsPowerUpsGenerated(int[] powerUpsGeneratedCount, int[] createdPowerUpsCount)
        {
            for (int i = 0; i < powerUpsGeneratedCount.Length; i++)
            {
                if (powerUpsGeneratedCount[i] != createdPowerUpsCount[i])
                    return false;
            }

            return true;
        }

        protected int GetCreatablePowerUpIndex(int[] powerUpsGeneratedCount, int[] createdPowerUpsCount)
        {
            bool isCreatablePowerUpIndex = false;
            int creatablePowerUpIndex = -1;

            while (!isCreatablePowerUpIndex)
            {
                creatablePowerUpIndex = randomGenerator.Next(0, Enum.GetValues(typeof(PowerUpTypes)).Length);
                if (createdPowerUpsCount[creatablePowerUpIndex] < powerUpsGeneratedCount[creatablePowerUpIndex])
                    isCreatablePowerUpIndex = true;
            }

            return creatablePowerUpIndex;
        }

        public override void GenerateField(GameObject floorPrefab, GameObject helpTextPrefab, GameObject statisticsPrefab, Material breakableWallMaterial, Material floorMaterial, 
                                           Material unbreakableWallMaterial, int[] powerUpsGeneratedCount, GameObject[] powerUpPrefabs, Vector3 powerUpsRotationAngle, float powerUpDelay)
        {
            if (Field != null)
            {
                int horizontalSize = Field.HorizontalSize;
                int verticalSize = Field.VerticalSize;
                GameObject wallPrefab = Field.WallPrefab;

                if ((wallPrefab != null) && (((Field.HorizontalSize % 2) != 0) && ((Field.VerticalSize % 2) != 0)) && (floorPrefab != null) && (breakableWallMaterial != null) && 
                    (floorMaterial != null) && (unbreakableWallMaterial != null))
                {
                    AddFloor(floorPrefab, Field.FieldGameObject, floorMaterial);
                    AddStatistics(statisticsPrefab, Field.FieldGameObject);
                    CreateGameObject(helpTextPrefab, Field.FieldGameObject);
                    GenerateUnbreakableWalls(unbreakableWallMaterial);
                    GenerateBreakableWalls(breakableWallMaterial);
                    GeneratePowerUps(powerUpsGeneratedCount, powerUpPrefabs, powerUpsRotationAngle, powerUpDelay);
                }
            }
        }
    }
}
