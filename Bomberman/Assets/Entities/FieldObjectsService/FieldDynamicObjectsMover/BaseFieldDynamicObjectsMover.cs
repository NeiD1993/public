using Assets.Entities.FieldObjects;
using Assets.Entities.FieldObjects.FieldObject.FieldObjectBehaviour;
using Assets.Entities.FieldObjects.FieldObject.FieldObjectBehaviour.MovingBehaviour;
using Assets.Entities.FieldObjects.FieldObject.FieldObjectsTypes;
using Assets.Entities.FieldObjectsService.BaseFieldObjectService;
using Assets.Scripts.Behaviour.MoveableFieldObjectBehaviour;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Entities.FieldObjectsService.FieldObjectsMover
{
    abstract class BaseFieldDynamicObjectsMover : BaseChangableFieldObjectsService
    {
        public BaseFieldDynamicObjectsMover(Field field) : base(field) { }

        protected static float? GetRotationAngleByMoveDirection(MoveDirection fieldObjectMoveDirection, MoveDirection moveDirection)
        {
            switch (fieldObjectMoveDirection)
            {
                case MoveDirection.Up:
                    return GetRotationAngleByMoveDirection(moveDirection, new Dictionary<MoveDirection, float>() { { MoveDirection.Left, -90 }, 
                                                                                                                   { MoveDirection.Down, -180 }, 
                                                                                                                   { MoveDirection.Right, 90 } });
                case MoveDirection.Left:
                    return GetRotationAngleByMoveDirection(moveDirection, new Dictionary<MoveDirection, float>() { { MoveDirection.Down, -90 }, 
                                                                                                                   { MoveDirection.Right, -180 }, 
                                                                                                                   { MoveDirection.Up, 90 } });
                case MoveDirection.Down:
                    return GetRotationAngleByMoveDirection(moveDirection, new Dictionary<MoveDirection, float>() { { MoveDirection.Right, -90 }, 
                                                                                                                   { MoveDirection.Up, -180 }, 
                                                                                                                   { MoveDirection.Left, 90 } });
                case MoveDirection.Right:
                    return GetRotationAngleByMoveDirection(moveDirection, new Dictionary<MoveDirection, float>() { { MoveDirection.Up, -90 }, 
                                                                                                                   { MoveDirection.Left, -180 }, 
                                                                                                                   { MoveDirection.Down, 90 } });
            }

            return null;
        }

        protected static float? GetRotationAngleByMoveDirection(MoveDirection moveDirection, IDictionary<MoveDirection, float> moveDirectionsRotationAngles)
        {
            foreach (KeyValuePair<MoveDirection, float> moveDirectionRotationAngle in moveDirectionsRotationAngles)
            {
                if (moveDirectionRotationAngle.Key == moveDirection)
                    return moveDirectionRotationAngle.Value;
            }

            return null;
        }

        public static void MoveChildGameObject(GameObject childGameObject, GameObject parentGameObject)
        {
            if ((childGameObject != null) && (parentGameObject != null) && (childGameObject.transform.parent == parentGameObject.transform))
                childGameObject.transform.position = parentGameObject.transform.position;
        }

        public static void MoveText(Text text, GameObject relativeGameObject)
        {
            if ((text != null) && (relativeGameObject != null))
                text.transform.position = Camera.main.WorldToScreenPoint(relativeGameObject.transform.position) +
                                          Camera.main.ScreenToWorldPoint(new Vector3(text.rectTransform.sizeDelta.x / 2, text.rectTransform.sizeDelta.y / 2, 0));
        }

        public static Vector2 GetIntegerFieldIndexesByMoveDirection(float horizontalIndex, float verticalIndex, float indexesStep, MoveDirection moveDirection, out Vector2 moveDirectionIncrement)
        {
            int horizontalIndexInteger = 0;
            int verticalIndexInteger = 0;
            moveDirectionIncrement = Vector2.zero;

            switch (moveDirection)
            {
                case MoveDirection.Down:
                    {
                        verticalIndexInteger = (int)Mathf.Ceil(verticalIndex);
                        moveDirectionIncrement = new Vector2(0, -indexesStep);
                    }
                    break;
                case MoveDirection.Left:
                    {
                        horizontalIndexInteger = (int)Mathf.Ceil(horizontalIndex);
                        moveDirectionIncrement = new Vector2(-indexesStep, 0);
                    }
                    break;
                case MoveDirection.Right:
                    {
                        horizontalIndexInteger = (int)Mathf.Floor(horizontalIndex);
                        moveDirectionIncrement = new Vector2(indexesStep, 0);
                    }
                    break;
                case MoveDirection.Up:
                    {
                        verticalIndexInteger = (int)Mathf.Floor(verticalIndex);
                        moveDirectionIncrement = new Vector2(0, indexesStep);
                    }
                    break;
            }

            if ((moveDirection == MoveDirection.Down) || (moveDirection == MoveDirection.Up))
                horizontalIndexInteger = (int)horizontalIndex;
            else
                verticalIndexInteger = (int)verticalIndex;

            return new Vector2(horizontalIndexInteger, verticalIndexInteger);
        }

        protected void CollectPowerUps(Vector2 fieldIndexes)
        {
            if (Field.PowerUps.ContainsKey(fieldIndexes))
            {
                PlayerBehaviour playerBehaviour = Field.FieldObjectsComponentsGetter.GetPlayerBehaviour<PlayerBehaviour>();
                PowerUpTypes powerUpType = Field.PowerUps[fieldIndexes].PowerUpType;
                GameObject lightGameObject;

                if ((powerUpType == PowerUpTypes.SpeedIncreasing) || (powerUpType == PowerUpTypes.WallPass))
                {
                    PlayerFieldObjectMovingBehaviour playerFieldObjectMovingBehaviour = playerBehaviour.GetFieldObjectBehaviour<PlayerFieldObjectMovingBehaviour>();

                    if (powerUpType == PowerUpTypes.SpeedIncreasing)
                    {
                        if (playerFieldObjectMovingBehaviour.CanIncreaseMovingSpeed())
                        {
                            playerFieldObjectMovingBehaviour.MovingSpeed++;
                            Field.FieldObjectsDestroyer.DestroyPowerUp(fieldIndexes);
                            lightGameObject = Field.FieldDynamicObjectsGenerator.CreateLight("Moving Speed");
                            playerFieldObjectMovingBehaviour.PlayFieldObjectSound(Field, lightGameObject, "Speed Increasing Power Up Pick Up");
                        }
                    }
                    else
                    {
                        if (!playerFieldObjectMovingBehaviour.IsWallPassed)
                        {
                            playerFieldObjectMovingBehaviour.IsWallPassed = true;
                            Field.FieldObjectsDestroyer.DestroyPowerUp(fieldIndexes);
                            lightGameObject = Field.FieldDynamicObjectsGenerator.CreateLight("Wall Pass");
                            playerFieldObjectMovingBehaviour.PlayFieldObjectSound(Field, lightGameObject, "Wall Pass Power Up Pick Up");
                        }
                    }
                }
                else if ((powerUpType == PowerUpTypes.ExplosionWaveDistanceIncreasing) || (powerUpType == PowerUpTypes.MaxPlacedBombsCountIncreasing))
                {
                    PlayerFieldObjectBombBehaviour playerFieldObjectBombBehaviour = playerBehaviour.GetFieldObjectBehaviour<PlayerFieldObjectBombBehaviour>();

                    if ((powerUpType == PowerUpTypes.ExplosionWaveDistanceIncreasing))
                    {
                        if (playerFieldObjectBombBehaviour.CanIncreaseExplosionWaveDistance())
                        {
                            playerFieldObjectBombBehaviour.ExplosionWaveDistance++;
                            Field.FieldObjectsDestroyer.DestroyPowerUp(fieldIndexes);
                            lightGameObject = Field.FieldDynamicObjectsGenerator.CreateLight("Explosion Wave Distance");
                            playerFieldObjectBombBehaviour.PlayFieldObjectSound(Field, lightGameObject, "Explosion Wave Distance Increasing Power Up Pick Up");
                        }
                    }
                    else
                    {
                        playerFieldObjectBombBehaviour.MaxPlacedBombsCount++;
                        Field.FieldObjectsDestroyer.DestroyPowerUp(fieldIndexes);
                        lightGameObject = Field.FieldDynamicObjectsGenerator.CreateLight("Remained Bombs");
                        playerFieldObjectBombBehaviour.PlayFieldObjectSound(Field, lightGameObject, "Max Bomb Place Count Power Up Pick Up");
                    }
                }
            }
        }

        protected void MoveNonEmptyFieldGameObject(float startHorizontalIndex, float endHorizontalIndex, float startVerticalIndex, float endVerticalIndex, GameObject nonEmptyFieldGameObject)
        {
            Vector3 localScale = Field.WallPrefab.transform.localScale;
            if (nonEmptyFieldGameObject != null)
                nonEmptyFieldGameObject.transform.position += new Vector3((endHorizontalIndex - startHorizontalIndex) * localScale.x, 0, (endVerticalIndex - startVerticalIndex) * localScale.z);
        }

        protected int GetPossibleStepsCount(Vector2 startIndexes, Vector2 incrementStep, int stepsCount, FieldObjectType fieldObjectType, bool isDestroyable = true)
        {
            bool isFieldObjectEmptyThroughtMoveDirection = true;
            int possibleStepsCount = 0;
            int startIndexX;
            int startIndexY;
            Vector2 playerPosition;
            Vector2 repeatedStartIndexes = startIndexes;

            while ((possibleStepsCount < stepsCount) && (isFieldObjectEmptyThroughtMoveDirection))
            {
                startIndexes += incrementStep;
                startIndexX = (int)startIndexes.x;
                startIndexY = (int)startIndexes.y;

                if (Field.IsIndexesOutOfFieldIndexesRanges(startIndexX, startIndexY) || 
                    (Field.FieldObjects[startIndexX][startIndexY].ObjectType != FieldObjectType.Empty))
                {
                    switch (fieldObjectType)
                    {
                        case FieldObjectType.Enemy:
                            {
                                if ((Field.FieldObjects[startIndexX][startIndexY].ObjectType == FieldObjectType.BreakableWall) ||
                                    (Field.FieldObjects[startIndexX][startIndexY].ObjectType == FieldObjectType.UnbreakableWall))
                                    isFieldObjectEmptyThroughtMoveDirection = false;
                                else
                                {
                                    if (Field.FieldObjects[startIndexX][startIndexY].ObjectType == FieldObjectType.Player)
                                    {
                                        if (isDestroyable)
                                        {
                                            Field.FieldObjectsDestroyer.DestroyGameObject(startIndexX, startIndexY, FieldObjectType.Player,
                                                                                          Field.FieldObjects[(int)repeatedStartIndexes.x][(int)repeatedStartIndexes.y].GameObject);
                                        }
                                        possibleStepsCount++;
                                    }
                                    else if (Field.FieldObjects[startIndexX][startIndexY].ObjectType == FieldObjectType.PlayerAndBreakableWall)
                                    {
                                        if (!isDestroyable)
                                            possibleStepsCount++;
                                        else
                                            isFieldObjectEmptyThroughtMoveDirection = false;
                                    }
                                    else if ((Field.FieldObjects[startIndexX][startIndexY].ObjectType == FieldObjectType.Enemy) && !isDestroyable)
                                        possibleStepsCount++;
                                    else
                                        isFieldObjectEmptyThroughtMoveDirection = false;
                                }
                            }
                            break;
                        default:
                            {
                                if ((Field.FieldObjects[startIndexX][startIndexY].ObjectType == FieldObjectType.Enemy) && isDestroyable)
                                {
                                    playerPosition = Field.PlayerPosition.Value;
                                    Field.FieldObjectsDestroyer.DestroyGameObject((int)playerPosition.x, (int)playerPosition.y, fieldObjectType);
                                    isFieldObjectEmptyThroughtMoveDirection = false;
                                }
                                else if (Field.FieldObjects[startIndexX][startIndexY].ObjectType == FieldObjectType.BreakableWall)
                                {
                                    if (Field.FieldObjectsStatistics.CurrentPlayerWallPass)
                                    {
                                        possibleStepsCount++;
                                        CollectPowerUps(startIndexes);
                                    }
                                    else
                                        isFieldObjectEmptyThroughtMoveDirection = false;
                                }
                                else if (Field.FieldObjects[startIndexX][startIndexY].ObjectType == FieldObjectType.UnbreakableWall)
                                    isFieldObjectEmptyThroughtMoveDirection = false;
                            }
                            break;
                    }
                }
                else
                {
                    possibleStepsCount++;
                    if ((fieldObjectType == FieldObjectType.Player) || (fieldObjectType == FieldObjectType.PlayerAndBreakableWall))
                        CollectPowerUps(startIndexes);
                }
            }

            return possibleStepsCount;
        }

        protected void TryMoveOnMoveDirectionIncrement(float endHorizontalIndex, float endVerticalIndex, float indexesStep, MoveDirection moveDirection, GameObject nonEmptyFieldGameObject, 
                                                       ref bool tryMoveNonEmptyFieldObjectOnFloatIndexes, ref Vector2 startIndexes)
        {
            Vector2 moveDirectionIncrement;
            Vector2 fieldIndexes = GetIntegerFieldIndexesByMoveDirection(endHorizontalIndex, endVerticalIndex, indexesStep, moveDirection, out moveDirectionIncrement);
            int horizontalFieldIndex = (int)fieldIndexes.x;
            int verticalFieldIndex = (int)fieldIndexes.y;

            if (!field.IsIndexesOutOfFieldIndexesRanges(horizontalFieldIndex, verticalFieldIndex) &&
                (field.FieldObjects[horizontalFieldIndex][verticalFieldIndex].ObjectType != FieldObjectType.UnbreakableWall))
            {
                MoveNonEmptyFieldGameObject(startIndexes.x, endHorizontalIndex, startIndexes.y, endVerticalIndex, nonEmptyFieldGameObject);
                tryMoveNonEmptyFieldObjectOnFloatIndexes = true;
            }

            startIndexes += moveDirectionIncrement;
        }

        public bool TryMoveNonEmptyFieldObjectOnFloatIndexes(ref Vector2 startIndexes, float indexesStep, MoveDirection moveDirection, GameObject nonEmptyFieldGameObject)
        {
            bool tryMoveNonEmptyFieldObjectOnFloatIndexes = false;
            float startIndexesX = startIndexes.x;
            float startIndexesY = startIndexes.y;

            switch (moveDirection)
            {
                case MoveDirection.Down:
                    TryMoveOnMoveDirectionIncrement(startIndexesX, startIndexesY - indexesStep, indexesStep, moveDirection, nonEmptyFieldGameObject, ref tryMoveNonEmptyFieldObjectOnFloatIndexes, 
                                                    ref startIndexes);
                    break;
                case MoveDirection.Left:
                    TryMoveOnMoveDirectionIncrement(startIndexesX - indexesStep, startIndexesY, indexesStep, moveDirection, nonEmptyFieldGameObject, ref tryMoveNonEmptyFieldObjectOnFloatIndexes, 
                                                    ref startIndexes);
                    break;
                case MoveDirection.Right:
                    TryMoveOnMoveDirectionIncrement(startIndexesX + indexesStep, startIndexesY, indexesStep, moveDirection, nonEmptyFieldGameObject, ref tryMoveNonEmptyFieldObjectOnFloatIndexes, 
                                                    ref startIndexes);
                    break;
                case MoveDirection.Up:
                    TryMoveOnMoveDirectionIncrement(startIndexesX, startIndexesY + indexesStep, indexesStep, moveDirection, nonEmptyFieldGameObject, ref tryMoveNonEmptyFieldObjectOnFloatIndexes, 
                                                    ref startIndexes);
                    break;
            }

            return tryMoveNonEmptyFieldObjectOnFloatIndexes;
        }

        public Nullable<Vector2> GetFieldEmptyObjectIndexes(int horizontalIndex, int verticalIndex, MoveDirection moveDirection, int moveDirectionSpeed, FieldObjectType fieldObjectType, 
                                                            bool isDestroyable = true)
        {
            Nullable<Vector2> fieldEmptyObjectIndexes = null;

            if ((Field != null) && !Field.IsIndexesOutOfFieldIndexesRanges(horizontalIndex, verticalIndex))
            {
                int possibleStepsCount;
                Vector2 fieldPosition = new Vector2(horizontalIndex, verticalIndex);

                switch (moveDirection)
                {
                    case MoveDirection.Down:
                        {
                            possibleStepsCount = GetPossibleStepsCount(fieldPosition, Vector2.down, moveDirectionSpeed, fieldObjectType, isDestroyable);
                            if (possibleStepsCount > 0)
                                fieldEmptyObjectIndexes = new Vector2(horizontalIndex, verticalIndex - possibleStepsCount);
                        }
                        break;

                    case MoveDirection.Left:
                        {
                            possibleStepsCount = GetPossibleStepsCount(fieldPosition, Vector2.left, moveDirectionSpeed, fieldObjectType, isDestroyable);
                            if (possibleStepsCount > 0)
                                fieldEmptyObjectIndexes = new Vector2(horizontalIndex - possibleStepsCount, verticalIndex);
                        }
                        break;
                    case MoveDirection.None:
                        {
                            if (Field.FieldObjects[horizontalIndex][verticalIndex].ObjectType == FieldObjectType.Empty)
                                fieldEmptyObjectIndexes = new Vector2(horizontalIndex, verticalIndex);
                        }
                        break;
                    case MoveDirection.Right:
                        {
                            possibleStepsCount = GetPossibleStepsCount(fieldPosition, Vector2.right, moveDirectionSpeed, fieldObjectType, isDestroyable);
                            if (possibleStepsCount > 0)
                                fieldEmptyObjectIndexes = new Vector2(horizontalIndex + possibleStepsCount, verticalIndex);
                        }
                        break;
                    case MoveDirection.Up:
                        {
                            possibleStepsCount = GetPossibleStepsCount(fieldPosition, Vector2.up, moveDirectionSpeed, fieldObjectType, isDestroyable);
                            if (possibleStepsCount > 0)
                                fieldEmptyObjectIndexes = new Vector2(horizontalIndex, verticalIndex + possibleStepsCount);
                        }
                        break;
                }
            }

            return fieldEmptyObjectIndexes;
        }

        protected abstract void TryRotateMoveableDynamicObject(GameObject moveableDynamicObject, ref MoveDirection fieldObjectMoveDirection, MoveDirection rotationMoveDirection);

        public abstract MoveDirection GetComplexPriorityMoveDirection(Vector2 fieldIndexes, int moveDirectionSpeed, FieldObjectType fieldObjectType);

        public abstract MoveDirection GetPriorityMoveDirection(Vector2 fieldIndexes, int moveDirectionSpeed, FieldObjectType fieldObjectType);

        public abstract MoveDirection GetSimplePriorityMoveDirection(Vector2 fieldIndexes, int moveDirectionSpeed, FieldObjectType fieldObjectType);

        public abstract void RotateFieldGameObject(GameObject fieldGameObject, Vector3 rotationAngle);

        public abstract bool MoveNonEmptyFieldObject(ref Vector2 nonEmptyPosition, ref MoveDirection fieldObjectMoveDirection, MoveDirection moveDirection, int moveDirectionSpeed, FieldObjectType fieldObjectType);
    }
}
