using System;
using System.Collections;
using System.Collections.Generic;
using GameScene.Behaviours.Ball.Enums;
using GameScene.Behaviours.Ball.Info;
using GameScene.Managers.Rotators.Settings;
using GameScene.Services.Ball;
using GameScene.Services.Ball.Data;
using GameScene.Services.Ball.Enums;
using GameScene.Services.Ball.Info;
using GameScene.Services.Field;
using GameScene.Services.Field.Data;
using GameScene.Services.Game.Info;
using GameScene.Services.Game.Settings;
using GameScene.Services.Platform;
using ServicesLocators;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GameScene.Services.Game
{
    public partial class GameLogicService
    {
        private partial class PathToReservoirGenerator
        {
            private class PathPointsExplorer
            {
                private readonly AxisService axisService; 

                private readonly FieldEntityBindingService fieldEntityBindingService;

                private readonly NeighboringPlatformsFinder neighboringPlatformsFinder;

                private readonly PositionService positionService;

                private readonly RotationsDataService rotationsDataService;

                private readonly RotationService rotationService;

                public PathPointsExplorer()
                {
                    axisService = SharedSceneServicesLocator.GetService<AxisService>();
                    fieldEntityBindingService = SharedSceneServicesLocator.GetService<FieldEntityBindingService>();
                    neighboringPlatformsFinder = SharedSceneServicesLocator.GetService<NeighboringPlatformsFinder>();
                    positionService = SharedSceneServicesLocator.GetService<PositionService>();
                    rotationsDataService = SharedSceneServicesLocator.GetService<RotationsDataService>();
                    rotationService = SharedSceneServicesLocator.GetService<RotationService>();
                }

                private void AddActivatedPlatformsPositions(Vector2Int possibleReservoirPosition, int dimensionHalfPlatformsCount, 
                    ISet<Vector2Int> forbiddenActivatedPlatformsPositions, IDictionary<Vector2Int, ISet<Vector2Int>> activatedPlatformsPositionsNearReservoirs)
                {
                    activatedPlatformsPositionsNearReservoirs[possibleReservoirPosition] = new HashSet<Vector2Int>();

                    foreach (Vector2Int activatedPlatformPosition in neighboringPlatformsFinder.GetNeighboringPlatformsPositionsIteratively(possibleReservoirPosition, 
                        dimensionHalfPlatformsCount,
                           forbiddenActivatedPlatformsPositions))
                        activatedPlatformsPositionsNearReservoirs[possibleReservoirPosition].Add(activatedPlatformPosition);
                }

                private void AddPathPossibleSettingsForUnfilledCardinalPoint(Vector2Int possibleReservoirPosition, CardinalPoint unfilledBallCardinalPoint,
                    PathToReservoirPossibleSettings pathPossibleSettings)
                {
                    pathPossibleSettings.GeneratedReservoirSettings.SinglePositionalSubstanceColorsTypes.Add(possibleReservoirPosition,
                        fieldEntityBindingService.GetSubstanceColorTypeByCardinalPoint(unfilledBallCardinalPoint));
                    pathPossibleSettings.BallRotationsData.Add(new BallRotationsDataKey(possibleReservoirPosition, unfilledBallCardinalPoint), 
                        new RotationsData(0, RotationType.None));
                }

                private bool TryAddPossibleMultiplePositionsReservoirSubstanceColorsTypes(IEnumerable<CardinalPoint> unfilledCardinalPoints, 
                    PathToReservoirPossibleSettings pathPossibleSettings)
                {
                    if (pathPossibleSettings.GeneratedReservoirSettings.MultipleSettings.SubtanceColorsTypes.Count == 0)
                    {
                        foreach (CardinalPoint unfilledCardinalPoint in unfilledCardinalPoints)
                            pathPossibleSettings.GeneratedReservoirSettings.MultipleSettings.SubtanceColorsTypes
                                .Add(fieldEntityBindingService.GetSubstanceColorTypeByCardinalPoint(unfilledCardinalPoint));

                        return true;
                    }
                    else
                        return false;
                }

                private IEnumerator AddPathPossibleSettingsForFilledCardinalPointIteratively(Vector2Int possibleReservoirPosition, RotationsInfo ballRotationsInfo,
                    PathToReservoirPossibleSettings pathPossibleSettings)
                {
                    CardinalPoint unfilledCardinalPoint;
                    RotationsData rotationsData;

                    TryAddPossibleMultiplePositionsReservoirSubstanceColorsTypes(ballRotationsInfo.CardinalPointsInfo.Unfilled, pathPossibleSettings);
                    pathPossibleSettings.GeneratedReservoirSettings.MultipleSettings.Positions.Add(possibleReservoirPosition);

                    foreach (KeyValuePair<CardinalPoint, RotationsData> rotationsDataItem in 
                        rotationsDataService.GetRotationsDataBetweenCardinalPointsIteratively(ballRotationsInfo))
                    {
                        unfilledCardinalPoint = rotationsDataItem.Key;
                        rotationsData = rotationsDataItem.Value;
                        pathPossibleSettings.BallRotationsData.Add(new BallRotationsDataKey(possibleReservoirPosition, unfilledCardinalPoint), rotationsData);

                        yield return pathPossibleSettings;
                    }
                }

                private IEnumerator CreatePathPossibleSettingsIteratively(Vector2Int lastPathPoint, int dimensionHalfPlatformsCount,
                    BallPathToReservoirExploringInfo ballPathExploringInfo, IEnumerable<Vector2Int> possibleReservoirPositions,
                    ISet<Vector2Int> forbiddenActivatedPlatformsPositions, Action<PathToReservoirPossibleSettings> pathPossibleSettingsExtractor)
                {
                    CardinalPoint ballCardinalPoint;
                    PathToReservoirPossibleSettings pathPossibleSettings = new PathToReservoirPossibleSettings();

                    foreach (Vector2Int possibleReservoirPosition in possibleReservoirPositions)
                    {
                        ballCardinalPoint = ballPathExploringInfo.Info.PositioningData
                            .CardinalPoints[positionService.GetMoveDirectionBetweenPositions(lastPathPoint, possibleReservoirPosition)];

                        if (ballPathExploringInfo.Info.CardinalPointsFillingData.HalfFilledCardinalPoints.Contains(ballCardinalPoint))
                            AddPathPossibleSettingsForUnfilledCardinalPoint(possibleReservoirPosition, ballCardinalPoint, pathPossibleSettings);
                        else
                            yield return AddPathPossibleSettingsForFilledCardinalPointIteratively(possibleReservoirPosition,
                                new RotationsInfo(new CardinalPointsRotationsInfo(ballPathExploringInfo.Info.CardinalPointsFillingData.HalfFilledCardinalPoints, 
                                ballCardinalPoint), ballPathExploringInfo.InitialTotalOrientation), pathPossibleSettings);

                        AddActivatedPlatformsPositions(possibleReservoirPosition, dimensionHalfPlatformsCount, forbiddenActivatedPlatformsPositions,
                            pathPossibleSettings.ActivatedPlatformsPositionsNearReservoirs);

                        yield return pathPossibleSettings;
                    }

                    pathPossibleSettingsExtractor(pathPossibleSettings);
                }

                public IEnumerator ExplorePathIteratively(PathToReservoirGenerativeInfo pathToReservoirGenerativeInfo, 
                    PathToReservoirGeneratedInfo pathToReservoirGeneratedInfo, IDictionary<int, RotationType> rotationPathPointsTypes, 
                    Action<PathToReservoirGeneratedEntitiesSettings> pathToReservoirGeneratedEntitiesSettingsExtractor)
                {
                    MoveDirection currentMoveDirection;
                    Vector2Int currentPathPoint = pathToReservoirGenerativeInfo.StartPathPoint;
                    BallPathToReservoirExploringInfo ballPathExploringInfo = new BallPathToReservoirExploringInfo(pathToReservoirGenerativeInfo.BallInfo);
                    PathToReservoirGeneratedEntitiesSettings pathToReservoirGeneratedEntitiesSettings = new PathToReservoirGeneratedEntitiesSettings();

                    for (int i = 1; i <= pathToReservoirGeneratedInfo.MoveDirections.Count; i++)
                    {
                        currentMoveDirection = pathToReservoirGeneratedInfo.MoveDirections[i - 1];
                        currentPathPoint = positionService.MovePosition(currentPathPoint, currentMoveDirection);
                        axisService.Reverse(axisService.GetTypeByMoveDirection(currentMoveDirection), pathToReservoirGenerativeInfo.BallInfo.PositioningData);

                        if ((pathToReservoirGeneratedEntitiesSettings.GeneratedRotatorsSettings.Count < rotationPathPointsTypes.Count) && 
                            rotationPathPointsTypes.ContainsKey(i))
                        {
                            rotationService.Rotate(rotationPathPointsTypes[i], pathToReservoirGenerativeInfo.BallInfo.PositioningData.CardinalPoints);
                            pathToReservoirGeneratedEntitiesSettings.GeneratedRotatorsSettings.Add(new GeneratedRotatorSettings(currentPathPoint, rotationPathPointsTypes[i]));
                        }

                        yield return pathToReservoirGeneratedEntitiesSettings;
                    }

                    yield return CreatePathPossibleSettingsIteratively(currentPathPoint, pathToReservoirGenerativeInfo.DimensionHalfPlatformsCount, ballPathExploringInfo, 
                        pathToReservoirGeneratedInfo.PossibleReservoirPositions, pathToReservoirGeneratedInfo.ForbiddenActivatedPlatformsPositions, 
                        pathPossibleSettingsParameter => pathToReservoirGeneratedEntitiesSettings.PossibleSettings = pathPossibleSettingsParameter);

                    pathToReservoirGeneratedEntitiesSettingsExtractor(pathToReservoirGeneratedEntitiesSettings);
                }

                private struct BallPathToReservoirExploringInfo
                {
                    public BallPathToReservoirExploringInfo(BallInfo initialInfo)
                    {
                        InitialTotalOrientation = initialInfo.PositioningData.TotalOrientation;
                        Info = initialInfo;
                    }

                    public Orientation InitialTotalOrientation { get; private set; }

                    public BallInfo Info { get; private set; }
                }
            }
        }
    }
}