using Assets.Entities.FieldObjects;
using Assets.Entities.FieldObjects.FieldObject;
using Assets.Entities.FieldObjects.FieldObject.FieldObjectsTypes;
using Assets.Entities.FieldObjectsService.FieldObjectsMover;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Entities.FieldObjectsService.FieldDynamicObjectsMover
{
    class FieldDynamicObjectsMover : BaseFieldDynamicObjectsMover
    {
        private static readonly int maxValueClosedPathCellsPercentage = 100;

        private static readonly int minValueClosedPathCellsPercentage = 5;

        private int maxClosedPathCellsPercentage;

        public FieldDynamicObjectsMover(Field field, int maxClosedPathCellsPercentage) : base(field) 
        {
            MaxClosedPathCellsPercentage = maxClosedPathCellsPercentage;
        }

        public int MaxClosedPathCellsPercentage
        {
            get
            {
                return maxClosedPathCellsPercentage;
            }

            set
            {
                maxClosedPathCellsPercentage = ((Field != null) && ((value >= minValueClosedPathCellsPercentage) && (value <= maxValueClosedPathCellsPercentage))) ? 
                    value : minValueClosedPathCellsPercentage;
            }
        }

        protected override void TryRotateMoveableDynamicObject(GameObject moveableDynamicObject, ref MoveDirection fieldObjectMoveDirection, MoveDirection rotationMoveDirection)
        {
            if (fieldObjectMoveDirection != rotationMoveDirection)
            {
                float? rotationAngle = GetRotationAngleByMoveDirection(fieldObjectMoveDirection, rotationMoveDirection);
                if ((rotationAngle != null) && (moveableDynamicObject != null))
                {
                    moveableDynamicObject.transform.Rotate(new Vector3(0, rotationAngle.Value, 0));
                    fieldObjectMoveDirection = rotationMoveDirection;
                }
            }
        }

        public override bool MoveNonEmptyFieldObject(ref Vector2 nonEmptyPosition, ref MoveDirection fieldObjectMoveDirection, MoveDirection moveDirection, int moveDirectionSpeed, 
                                                     FieldObjectType fieldObjectType)
        {
            int nonEmptyPositionX = (int)nonEmptyPosition.x;
            int nonEmptyPositionY = (int)nonEmptyPosition.y;

            if ((field != null) && !field.IsIndexesOutOfFieldIndexesRanges(nonEmptyPositionX, nonEmptyPositionY) && (moveDirectionSpeed >= 1))
            {
                Nullable<Vector2> fieldEmptyObjectIndexes = GetFieldEmptyObjectIndexes(nonEmptyPositionX, nonEmptyPositionY, moveDirection, moveDirectionSpeed, fieldObjectType);

                if (fieldEmptyObjectIndexes != null)
                {
                    Vector2 fieldObjectEmptyIndexesValue = fieldEmptyObjectIndexes.Value;
                    int emptyPositionX = (int)fieldObjectEmptyIndexesValue.x;
                    int emptyPositionY = (int)fieldObjectEmptyIndexesValue.y;
                    FieldObject[][] fieldObjects = field.FieldObjects;
                    FieldObject nonEmptyFieldObject = fieldObjects[nonEmptyPositionX][nonEmptyPositionY];
                    FieldObject emptyFieldObject = fieldObjects[emptyPositionX][emptyPositionY];
                    GameObject moveableRotatableGameObject;

                    if ((emptyFieldObject.ObjectType == FieldObjectType.BreakableWall) && (nonEmptyFieldObject.ObjectType == FieldObjectType.Player))
                    {
                        moveableRotatableGameObject = nonEmptyFieldObject.GameObject;
                        emptyFieldObject.ObjectType = FieldObjectType.PlayerAndBreakableWall;
                        nonEmptyFieldObject.ObjectType = FieldObjectType.Empty;
                        moveableRotatableGameObject.transform.parent = emptyFieldObject.GameObject.transform;
                        AddFreePosition(nonEmptyPosition);
                    }
                    else if ((emptyFieldObject.ObjectType == FieldObjectType.BreakableWall) && (nonEmptyFieldObject.ObjectType == FieldObjectType.PlayerAndBreakableWall))
                    {
                        emptyFieldObject.ObjectType = FieldObjectType.PlayerAndBreakableWall;
                        nonEmptyFieldObject.ObjectType = FieldObjectType.BreakableWall;
                        moveableRotatableGameObject = nonEmptyFieldObject.GameObject.transform.GetChild(0).gameObject;
                        moveableRotatableGameObject.transform.parent = emptyFieldObject.GameObject.transform;
                    }
                    else if ((emptyFieldObject.ObjectType == FieldObjectType.Empty) && (nonEmptyFieldObject.ObjectType == FieldObjectType.PlayerAndBreakableWall))
                    {
                        moveableRotatableGameObject = nonEmptyFieldObject.GameObject.transform.GetChild(0).gameObject;
                        moveableRotatableGameObject.transform.parent = Field.FieldGameObjects.transform;
                        emptyFieldObject.GameObject = moveableRotatableGameObject;
                        emptyFieldObject.ObjectType = FieldObjectType.Player;
                        nonEmptyFieldObject.ObjectType = FieldObjectType.BreakableWall;
                        RemoveFreePosition(fieldObjectEmptyIndexesValue);
                    }
                    else
                    {
                        moveableRotatableGameObject = nonEmptyFieldObject.GameObject;
                        emptyFieldObject.ObjectType = nonEmptyFieldObject.ObjectType;
                        nonEmptyFieldObject.ObjectType = FieldObjectType.Empty;
                        emptyFieldObject.GameObject = moveableRotatableGameObject;
                        AddFreePosition(nonEmptyPosition);
                        RemoveFreePosition(fieldObjectEmptyIndexesValue);
                    }

                    MoveNonEmptyFieldGameObject(nonEmptyPositionX, emptyPositionX, nonEmptyPositionY, emptyPositionY, moveableRotatableGameObject);

                    TryRotateMoveableDynamicObject(moveableRotatableGameObject, ref fieldObjectMoveDirection, moveDirection);

                    nonEmptyPosition = fieldObjectEmptyIndexesValue;
                    Field.PlayerPosition = fieldObjectEmptyIndexesValue;

                    return true;
                }

                return false;
            }

            else return false;
        }

        public override MoveDirection GetComplexPriorityMoveDirection(Vector2 fieldIndexes, int moveDirectionSpeed, FieldObjectType fieldObjectType) 
        {
            if (Field.PlayerPosition != null)
                return AStarPathFinder.FindAStarPriorityMoveDirection(moveDirectionSpeed, fieldIndexes, Field.PlayerPosition.Value, Field, fieldObjectType, 
                                                                      (MaxClosedPathCellsPercentage * Field.FreePositions.Count) / 100);
            else
                return MoveDirection.None;
        }

        public override MoveDirection GetPriorityMoveDirection(Vector2 fieldIndexes, int moveDirectionSpeed, FieldObjectType fieldObjectType)
        {
            if ((field != null) && (moveDirectionSpeed >= 1))
            {
                Nullable<Vector2> playerPosition = field.PlayerPosition;

                if (playerPosition != null)
                {
                    int fieldIndexesX = (int)fieldIndexes.x;
                    int fieldIndexesY = (int)fieldIndexes.y;
                    Vector2 distanceVector;
                    Vector2 playerPositionValue = playerPosition.Value;
                    Nullable<Vector2> endFieldPosition;
                    Dictionary<MoveDirection, float> moveDirectionsDistances = new Dictionary<MoveDirection, float>();

                    foreach (MoveDirection moveDirection in Enum.GetValues(typeof(MoveDirection)))
                    {
                        endFieldPosition = GetFieldEmptyObjectIndexes(fieldIndexesX, fieldIndexesY, moveDirection, moveDirectionSpeed, fieldObjectType);
                        if (endFieldPosition != null)
                        {
                            distanceVector = playerPositionValue - endFieldPosition.Value;
                            moveDirectionsDistances.Add(moveDirection, Mathf.Sqrt(Mathf.Pow(Mathf.Abs(distanceVector.x), 2) + Mathf.Pow(Mathf.Abs(distanceVector.y), 2)));
                        }
                    }

                    if (moveDirectionsDistances.Count > 0)
                        return moveDirectionsDistances.OrderBy(moveDirectionDistance => moveDirectionDistance.Value).First().Key;
                }
            }

            return MoveDirection.None;
        }

        public override MoveDirection GetSimplePriorityMoveDirection(Vector2 fieldIndexes, int moveDirectionSpeed, FieldObjectType fieldObjectType)
        {
            if (Field.PlayerPosition != null)
            {
                Array moveDirectionValues = Enum.GetValues(typeof(MoveDirection));
                Nullable<Vector2> moveDirectionIndexes = null;
                MoveDirection priorityMoveDirection = MoveDirection.None;
                List<MoveDirection> checkedMoveDirections = new List<MoveDirection>();

                while ((checkedMoveDirections.Count < moveDirectionValues.Length - 1) && (moveDirectionIndexes == null))
                {
                    priorityMoveDirection = (MoveDirection)randomGenerator.Next(0, moveDirectionValues.Length + 1);
                    if ((priorityMoveDirection != MoveDirection.None) && !checkedMoveDirections.Contains(priorityMoveDirection))
                        moveDirectionIndexes = GetFieldEmptyObjectIndexes((int)fieldIndexes.x, (int)fieldIndexes.y, priorityMoveDirection, moveDirectionSpeed, fieldObjectType);
                    checkedMoveDirections.Add(priorityMoveDirection);
                }

                return priorityMoveDirection;
            }
            else
                return MoveDirection.None;
        }

        public override void RotateFieldGameObject(GameObject fieldGameObject, Vector3 rotationAngle)
        {
            if (fieldGameObject != null)
                fieldGameObject.transform.Rotate(rotationAngle);
        }

        protected class AStarPathFinder
        {
            protected static float GetHeuristicEstimatePathLengthFromCellToGoal(Vector2 startCellIndexes, Vector2 endCellIndexes)
            {
                return Mathf.Abs(endCellIndexes.x - startCellIndexes.x) + Mathf.Abs(endCellIndexes.y - startCellIndexes.y);
            }

            protected static IList<PathCell> GetPathCellNeighbourCells(int moveDirectionSpeed, PathCell pathCell, Vector2 endCellIndexes, Field field, FieldObjectType fieldObjectType)
            {
                Nullable<Vector2> pathCellIndexes;
                PathCell pathCellNeighbourCell;
                IList<PathCell> pathCellNeighbourCells = new List<PathCell>();

                foreach(MoveDirection moveDirection in Enum.GetValues(typeof(MoveDirection)))
                {
                    if (moveDirection != MoveDirection.None)
                    {
                        pathCellIndexes = field.FieldDynamicObjectsMover.GetFieldEmptyObjectIndexes((int)pathCell.Indexes.x, (int)pathCell.Indexes.y, moveDirection, moveDirectionSpeed, fieldObjectType, 
                                                                                                    false);
                        if (pathCellIndexes != null)
                        {
                            pathCellNeighbourCell = new PathCell(pathCellIndexes.Value, moveDirection, pathCell, pathCell.PathLengthFromStartToCell + moveDirectionSpeed,
                                                                 GetHeuristicEstimatePathLengthFromCellToGoal(pathCellIndexes.Value, endCellIndexes));
                            pathCellNeighbourCells.Add(pathCellNeighbourCell);
                        }
                    }
                }

                return pathCellNeighbourCells;
            }

            protected static MoveDirection GetAStarFirstPriorityMoveDirectionForPathCell(PathCell pathCell, Vector2 startCellIndexes)
            {
                PathCell previousPathCell = null;
                PathCell currentPathCell = pathCell;

                if (pathCell != null)
                {
                    while (currentPathCell.Indexes != startCellIndexes)
                    {
                        previousPathCell = currentPathCell;
                        currentPathCell = currentPathCell.CellCameFrom;
                    }
                }

                if (previousPathCell != null)
                    return previousPathCell.MoveDirection;
                else
                    return MoveDirection.None;
            }

            public static MoveDirection FindAStarPriorityMoveDirection(int moveDirectionSpeed, Vector2 startCellIndexes, Vector2 endCellIndexes, Field field, FieldObjectType fieldObjectType, 
                                                                       int maxClosedPathCellsCount)
            {
                PathCell openPathCell;
                PathCell currentPathCell = null;
                IList<PathCell> closedPathCells = new List<PathCell>();
                IList<PathCell> openedPathCells = new List<PathCell>();
                Func<PathCell> choosePathCellFunc = () => openedPathCells.OrderBy(pathCell => pathCell.EstimateFullPathLength).FirstOrDefault();

                openedPathCells.Add(new PathCell(startCellIndexes, MoveDirection.None, null, 0, GetHeuristicEstimatePathLengthFromCellToGoal(startCellIndexes, endCellIndexes)));

                while ((openedPathCells.Count > 0) && (closedPathCells.Count <= maxClosedPathCellsCount))
                {
                    currentPathCell = choosePathCellFunc();

                    if (currentPathCell.Indexes == endCellIndexes)
                        break;

                    openedPathCells.Remove(currentPathCell);
                    closedPathCells.Add(currentPathCell);

                    foreach (PathCell pathCellNeighbourCell in GetPathCellNeighbourCells(moveDirectionSpeed, currentPathCell, endCellIndexes, field, fieldObjectType))
                    {
                        Func<PathCell, bool> pathCellIndexesPredicate = pathCell => (pathCell.Indexes == pathCellNeighbourCell.Indexes);

                        if (!(closedPathCells.Count(pathCellIndexesPredicate) > 0))
                        {
                            openPathCell = openedPathCells.FirstOrDefault(pathCellIndexesPredicate);

                            if (openPathCell == null)
                                openedPathCells.Add(pathCellNeighbourCell);
                            else
                            {
                                if (openPathCell.PathLengthFromStartToCell > pathCellNeighbourCell.PathLengthFromStartToCell)
                                {
                                    openPathCell.CellCameFrom = currentPathCell;
                                    openPathCell.PathLengthFromStartToCell = pathCellNeighbourCell.PathLengthFromStartToCell;
                                }
                            }
                        }
                    }
                }

                return GetAStarFirstPriorityMoveDirectionForPathCell(choosePathCellFunc(), startCellIndexes);
            }

            protected class PathCell
            {
                public PathCell(Vector2 indexes, MoveDirection moveDirection, PathCell cellCameFrom, float pathLengthFromStartToCell, float heuristicEstimatePathLengthFromCellToGoal)
                {
                    Indexes = indexes;
                    MoveDirection = moveDirection;
                    CellCameFrom = cellCameFrom;
                    PathLengthFromStartToCell = pathLengthFromStartToCell;
                    HeuristicEstimatePathLengthFromCellToGoal = heuristicEstimatePathLengthFromCellToGoal;
                }

                public float HeuristicEstimatePathLengthFromCellToGoal { get; set; }

                public float PathLengthFromStartToCell { get; set; }

                public MoveDirection MoveDirection { get; set; }

                public PathCell CellCameFrom { get; set; }

                public Vector2 Indexes { get; set; }

                public float EstimateFullPathLength
                {
                    get
                    {
                        return (PathLengthFromStartToCell + HeuristicEstimatePathLengthFromCellToGoal);
                    }
                }
            }
        }
    }
}
