using System;
using System.Collections.Generic;
using GameScene.Managers.Game.Settings;
using GameScene.Services.Ball.Enums;

namespace GameScene.Services.Game
{
    public partial class GameLogicService
    {
        private partial class PathToReservoirGenerator
        {
            private partial class RotationPathPointsGenerator
            {
                private class BoundaryRotationPathPointsAmountRandomizer
                {
                    private static int RandomizeCommonBoundaryRotationPathPointsCount(int pathLength,
                        PathToReservoirRotationPointsPercentageSettings rotationPathPointsPercentageSettings)
                    {
                        int extremeRotationPathPointsCount = (int)Math.Floor(((float)pathLength) / 2) - 1;

                        return (int)(extremeRotationPathPointsCount * UnityEngine.Random.Range(rotationPathPointsPercentageSettings.Minimal, 
                            rotationPathPointsPercentageSettings.Maximal));
                    }

                    private static (RotationType type, int boundaryCount) RandomizeRotationPathPointsCategory(int commonBoundaryRotationPathPointsCount)
                    {
                        RotationType[] rotationPathPointsTypes = new RotationType[] { RotationType.Clockwise, RotationType.CounterClockwise };

                        return (rotationPathPointsTypes[UnityEngine.Random.Range(0, rotationPathPointsTypes.Length)], 
                            UnityEngine.Random.Range(0, commonBoundaryRotationPathPointsCount + 1));
                    }

                    public PathToReservoirBoundaryRotationPointsAmountData RandomizeBoundaryRotationPathPointsAmount(int pathLength, 
                        PathToReservoirRotationPointsPercentageSettings rotationPathPointsPercentageSettings)
                    {
                        int commonBoundaryRotationPathPointsCount = RandomizeCommonBoundaryRotationPathPointsCount(pathLength, rotationPathPointsPercentageSettings);
                        (int clockwise, int counterClockwise) boundaryRotationPathPointsAmount = default;
                        (RotationType type, int boundaryCount) rotationPathPointsCategoryItem = RandomizeRotationPathPointsCategory(commonBoundaryRotationPathPointsCount);

                        switch (rotationPathPointsCategoryItem.type)
                        {
                            case RotationType.Clockwise:
                                boundaryRotationPathPointsAmount = (rotationPathPointsCategoryItem.boundaryCount, commonBoundaryRotationPathPointsCount - 
                                    rotationPathPointsCategoryItem.boundaryCount);
                                break;
                            case RotationType.CounterClockwise:
                                boundaryRotationPathPointsAmount = (commonBoundaryRotationPathPointsCount - rotationPathPointsCategoryItem.boundaryCount, 
                                    rotationPathPointsCategoryItem.boundaryCount);
                                break;
                        }

                        return new PathToReservoirBoundaryRotationPointsAmountData(boundaryRotationPathPointsAmount);
                    }
                }

                private struct PathToReservoirBoundaryRotationPointsAmountData
                {
                    public PathToReservoirBoundaryRotationPointsAmountData((int clockwise, int counterClockwise) amount)
                    {
                        Clockwise = amount.clockwise;
                        CounterClockwise = amount.counterClockwise;
                    }

                    public int Clockwise { get; set; }

                    public int CounterClockwise { get; set; }

                    public IDictionary<RotationType, int> CopyToDictionary()
                    {
                        IDictionary<RotationType, int> dataDictionary = new Dictionary<RotationType, int>();
                        Action<RotationType, int> copyToDictionaryAction = (pointsTypeParameter, pointsAmountParameter) =>
                        {
                            if (pointsAmountParameter > 0)
                                dataDictionary.Add(pointsTypeParameter, pointsAmountParameter);
                        };

                        copyToDictionaryAction(RotationType.Clockwise, Clockwise);
                        copyToDictionaryAction(RotationType.CounterClockwise, CounterClockwise);

                        return dataDictionary;
                    }
                }
            }
        }
    }
}