using System;
using System.Collections;
using System.Collections.Generic;
using GameScene.Behaviours.Ball.Info;
using GameScene.Managers.Game.Settings;
using GameScene.Managers.Reservoir.Settings;
using GameScene.Managers.Rotators.Settings;
using GameScene.Services.Ball.Data;
using GameScene.Services.Ball.Enums;
using GameScene.Services.Field.Data;
using GameScene.Services.Game.Data;
using GameScene.Services.Game.Descriptions;
using GameScene.Services.Game.Info;
using GameScene.Services.Game.Settings;
using GameScene.Services.Platform;
using GameScene.Services.Platform.Data;
using ServicesLocators;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GameScene.Services.Game
{
    public partial class GameLogicService
    {
        private partial class PathToReservoirGenerator
        {
            private readonly LastPathPointOperator lastPathPointOperator;

            private readonly NeighboringPlatformsFinder neighboringPlatformsFinder;

            private readonly PathPointsExplorer pathPointsExplorer;

            private readonly RotationPathPointsGenerator rotationPathPointsGenerator;

            private readonly VariativePathPointsMoveDirectionsItemsPickingService variativePathPointsMoveDirectionsItemsPickingService;

            public PathToReservoirGenerator()
            {
                lastPathPointOperator = new LastPathPointOperator();
                neighboringPlatformsFinder = SharedSceneServicesLocator.GetService<NeighboringPlatformsFinder>();
                pathPointsExplorer = new PathPointsExplorer();
                rotationPathPointsGenerator = new RotationPathPointsGenerator();
                variativePathPointsMoveDirectionsItemsPickingService = new VariativePathPointsMoveDirectionsItemsPickingService();
            }

            private static int RandomizeBoundaryPathLength(PathToReservoirLengthSettings pathLengthSettings)
            {
                return UnityEngine.Random.Range(pathLengthSettings.Minimal, pathLengthSettings.Maximal + 1);
            }

            private bool TryGeneratePathPoint(ref Vector2Int currentPathPoint, int dimensionHalfPlatformsCount, ISet<Vector2Int> forbiddenPositions,
                ICollection<MoveDirection> pathMoveDirections)
            {
                IList<NeighboringPlatformData> neighboringPointsData = neighboringPlatformsFinder.GetNeighboringPlatformsData(currentPathPoint, dimensionHalfPlatformsCount,
                    forbiddenPositions);

                if (neighboringPointsData.Count > 0)
                {
                    int generatedPathPointIndex = UnityEngine.Random.Range(0, neighboringPointsData.Count);
                    NeighboringPlatformData generatedPathPointData = neighboringPointsData[generatedPathPointIndex];

                    forbiddenPositions.Add(currentPathPoint = generatedPathPointData.Position);
                    pathMoveDirections.Add(generatedPathPointData.MoveDirection);

                    return true;
                }
                else
                    return false;
            }

            private bool TryGeneratePossibleReservoirPositions(Vector2Int lastPathPoint, int dimensionHalfPlatformsCount, ISet<Vector2Int> forbiddenReservoirPositions,
                ICollection<Vector2Int> possibleReservoirPositions)
            {
                foreach (Vector2Int possibleReservoirPosition in neighboringPlatformsFinder.GetNeighboringPlatformsPositionsIteratively(lastPathPoint,
                    dimensionHalfPlatformsCount, forbiddenReservoirPositions))
                    possibleReservoirPositions.Add(possibleReservoirPosition);

                return possibleReservoirPositions.Count > 0;
            }

            private IEnumerator GeneratePathInfoIteratively(Vector2Int startPathPoint, int dimensionHalfPlatformsCount, PathToReservoirLengthSettings pathLengthSettings,
                Action<PathToReservoirGeneratedInfo> pathToReservoirGeneratedInfoExtractor)
            {
                bool isNextPathPointsGeneratingPossible;
                Vector2Int currentPathPoint = startPathPoint;
                (int boundary, int current) iterationNumber = (RandomizeBoundaryPathLength(pathLengthSettings), 1);
                ISet<Vector2Int> forbiddenPositions = new HashSet<Vector2Int>(new Vector2Int[] { startPathPoint });
                PathToReservoirGeneratedInfo pathToReservoirGeneratedInfo = new PathToReservoirGeneratedInfo();

                do
                {
                    isNextPathPointsGeneratingPossible = TryGeneratePathPoint(ref currentPathPoint, dimensionHalfPlatformsCount, forbiddenPositions,
                        pathToReservoirGeneratedInfo.MoveDirections);

                    yield return pathToReservoirGeneratedInfo;
                } while (isNextPathPointsGeneratingPossible && (++iterationNumber.current <= iterationNumber.boundary));

                if ((pathToReservoirGeneratedInfo.Length < iterationNumber.boundary) || !TryGeneratePossibleReservoirPositions(currentPathPoint, dimensionHalfPlatformsCount,
                    forbiddenPositions, pathToReservoirGeneratedInfo.PossibleReservoirPositions))
                    lastPathPointOperator.SetLastPathPointAsPossibleReservoirPosition(ref currentPathPoint, pathToReservoirGeneratedInfo.MoveDirections, forbiddenPositions,
                        pathToReservoirGeneratedInfo.PossibleReservoirPositions);

                pathToReservoirGeneratedInfo.SetupPathPoints(new PathToReservoirPointsGeneratedData(currentPathPoint, forbiddenPositions));
                pathToReservoirGeneratedInfoExtractor.Invoke(pathToReservoirGeneratedInfo);
            }

            public IEnumerator GeneratePathIteratively(PathToReservoirGenerativeInfo pathToReservoirGenerativeInfo, PathToReservoirSettings pathToReservoirSettings,
                Action<PathToReservoirData> pathToReservoirDataExtractor)
            {
                PathToReservoirGeneratedInfo pathToReservoirGeneratedInfo = null;
                IDictionary<int, RotationType> rotationPathPointsTypes = null;
                PathToReservoirGeneratedEntitiesSettings pathToReservoirGeneratedEntitiesSettings = null;
                PathToReservoirObservingInfo pathToReservoirObservingInfo;
                PathToReservoirOpeningInfo pathToReservoirOpeningInfo;

                yield return GeneratePathInfoIteratively(pathToReservoirGenerativeInfo.StartPathPoint, pathToReservoirGenerativeInfo.DimensionHalfPlatformsCount,
                    pathToReservoirSettings.Length, pathToReservoirGeneratedInfoParameter => pathToReservoirGeneratedInfo = pathToReservoirGeneratedInfoParameter);

                yield return rotationPathPointsGenerator.GenerateRotationPathPointsIteratively(pathToReservoirGeneratedInfo.Length,
                    pathToReservoirSettings.RotationPointsPercentage, rotationPathPointsTypesParameter => rotationPathPointsTypes = rotationPathPointsTypesParameter);

                yield return pathPointsExplorer.ExplorePathIteratively(pathToReservoirGenerativeInfo, pathToReservoirGeneratedInfo, rotationPathPointsTypes,
                    pathToReservoirGeneratedEntitiesSettingsParameter => pathToReservoirGeneratedEntitiesSettings = pathToReservoirGeneratedEntitiesSettingsParameter);

                pathToReservoirOpeningInfo = new PathToReservoirOpeningInfo(pathToReservoirGenerativeInfo.StartPathPoint, pathToReservoirGeneratedInfo.MoveDirections);
                pathToReservoirObservingInfo = new PathToReservoirObservingInfo(pathToReservoirOpeningInfo, variativePathPointsMoveDirectionsItemsPickingService.CreateVariationDataPickingRoutineExtractor(pathToReservoirGeneratedInfo.ReachableNonVariativePathPoints));
                pathToReservoirDataExtractor(new PathToReservoirData(pathToReservoirObservingInfo, pathToReservoirGeneratedEntitiesSettings));
            }

            private enum PathToReservoirPointsRetrievingConstraint
            {
                WithLastPoint,

                WithoutLastPoint
            }

            private class PathToReservoirGeneratedInfo
            {
                private PathToReservoirPointsGeneratedData pathToReservoirPointsGeneratedData;

                private IList<MoveDirection> moveDirections;

                private ICollection<Vector2Int> possibleReservoirPositions;

                public PathToReservoirGeneratedInfo()
                {
                    pathToReservoirPointsGeneratedData = null;
                }

                public int Length
                {
                    get
                    {
                        return MoveDirections.Count;
                    }
                }

                public IList<MoveDirection> MoveDirections
                {
                    get
                    {
                        if (moveDirections == null)
                            moveDirections = new List<MoveDirection>();

                        return moveDirections;
                    }
                }

                public ICollection<Vector2Int> PossibleReservoirPositions
                {
                    get
                    {
                        if (possibleReservoirPositions == null)
                            possibleReservoirPositions = new List<Vector2Int>();

                        return possibleReservoirPositions;
                    }
                }

                public ISet<Vector2Int> ForbiddenActivatedPlatformsPositions
                {
                    get
                    {
                        return pathToReservoirPointsGeneratedData.Retrieve(PathToReservoirPointsRetrievingConstraint.WithoutLastPoint);
                    }
                }

                public ISet<Vector2Int> ReachableNonVariativePathPoints
                {
                    get
                    {
                        return pathToReservoirPointsGeneratedData.Retrieve(PathToReservoirPointsRetrievingConstraint.WithLastPoint);
                    }
                }

                public void SetupPathPoints(PathToReservoirPointsGeneratedData pathToReservoirPointsGeneratedData)
                {
                    this.pathToReservoirPointsGeneratedData = pathToReservoirPointsGeneratedData;
                }
            }

            private class PathToReservoirPointsGeneratedData
            {
                private readonly int amount;

                public PathToReservoirPointsGeneratedData(Vector2Int last, ISet<Vector2Int> overall)
                {
                    Last = last;
                    Overall = overall;
                    amount = Overall.Count;
                }

                private Vector2Int Last { get; set; }

                private ISet<Vector2Int> Overall { get; set; }

                public ISet<Vector2Int> Retrieve(PathToReservoirPointsRetrievingConstraint constraint)
                {
                    switch (constraint)
                    {
                        case PathToReservoirPointsRetrievingConstraint.WithLastPoint:
                            {
                                if (Overall.Count == amount - 1)
                                    Overall.Add(Last);
                            }
                            break;
                        default:
                            {
                                if (Overall.Count == amount)
                                    Overall.Remove(Last);
                            }
                            break;
                    }

                    return Overall;
                }
            }
        }
    }
}

namespace GameScene.Services.Game.Data
{
    public struct PathToReservoirData
    {
        public PathToReservoirData(PathToReservoirObservingInfo observingInfo, PathToReservoirGeneratedEntitiesSettings generatedEntitiesSettings)
        {
            ObservingInfo = observingInfo;
            GeneratedEntitiesSettings = generatedEntitiesSettings;
        }

        public int Length
        {
            get
            {
                return ObservingInfo.Opening.MoveDirections.Count;
            }
        }

        public PathToReservoirObservingInfo ObservingInfo { get; private set; }

        public PathToReservoirGeneratedEntitiesSettings GeneratedEntitiesSettings { get; private set; }
    }
}

namespace GameScene.Services.Game.Info
{
    public struct PathToReservoirGenerativeInfo
    {
        public PathToReservoirGenerativeInfo(Vector2Int startPathPoint, int dimensionHalfPlatformsCount, BallInfo ballInfo)
        {
            StartPathPoint = startPathPoint;
            DimensionHalfPlatformsCount = dimensionHalfPlatformsCount;
            BallInfo = ballInfo;
        }

        public int DimensionHalfPlatformsCount { get; private set; }

        public Vector2Int StartPathPoint { get; private set; }

        public BallInfo BallInfo { get; private set; }
    }

    public struct PathToReservoirObservingInfo
    {
        public PathToReservoirObservingInfo(PathToReservoirOpeningInfo opening, Func<Vector2Int, IDictionary<Vector2Int, MoveDirection>, Action<PickedVariativePathPointsDataDescription<Vector2Int, MoveDirection>?>, IEnumerator> variativePathPointsDataPickingRoutineExtractor)
        {
            Opening = opening;
            VariativePathPointsDataPickingRoutineExtractor = variativePathPointsDataPickingRoutineExtractor;
        }

        public PathToReservoirOpeningInfo Opening { get; private set; }

        public Func<Vector2Int, IDictionary<Vector2Int, MoveDirection>, Action<PickedVariativePathPointsDataDescription<Vector2Int, MoveDirection>?>, IEnumerator> VariativePathPointsDataPickingRoutineExtractor { get; private set; }
    }

    public struct PathToReservoirOpeningInfo
    {
        public PathToReservoirOpeningInfo(Vector2Int startPathPoint, IList<MoveDirection> moveDirections)
        {
            StartPathPoint = startPathPoint;
            MoveDirections = moveDirections;
        }

        public Vector2Int StartPathPoint { get; private set; }

        public IList<MoveDirection> MoveDirections { get; private set; }
    }
}

namespace GameScene.Services.Game.Settings
{
    public class PathToReservoirGeneratedEntitiesSettings
    {
        private ICollection<GeneratedRotatorSettings> generatedRotatorsSettings;

        public ICollection<GeneratedRotatorSettings> GeneratedRotatorsSettings
        {
            get
            {
                if (generatedRotatorsSettings == null)
                    generatedRotatorsSettings = new List<GeneratedRotatorSettings>();

                return generatedRotatorsSettings;
            }
        }

        public PathToReservoirPossibleSettings PossibleSettings { get; set; }
    }

    public class PathToReservoirPossibleSettings
    {
        private IDictionary<BallRotationsDataKey, RotationsData> ballRotationsData;

        private IDictionary<Vector2Int, ISet<Vector2Int>> activatedPlatformsPositionsNearReservoirs;

        public PathToReservoirPossibleSettings()
        {
            GeneratedReservoirSettings = new GeneratedPossibleReservoirSettings();
        }

        public GeneratedPossibleReservoirSettings GeneratedReservoirSettings { get; private set; }

        public IDictionary<BallRotationsDataKey, RotationsData> BallRotationsData
        {
            get
            {
                if (ballRotationsData == null)
                    ballRotationsData = new Dictionary<BallRotationsDataKey, RotationsData>();

                return ballRotationsData;
            }
        }

        public IDictionary<Vector2Int, ISet<Vector2Int>> ActivatedPlatformsPositionsNearReservoirs
        {
            get
            {
                if (activatedPlatformsPositionsNearReservoirs == null)
                    activatedPlatformsPositionsNearReservoirs = new Dictionary<Vector2Int, ISet<Vector2Int>>();

                return activatedPlatformsPositionsNearReservoirs;
            }
        }
    }
}