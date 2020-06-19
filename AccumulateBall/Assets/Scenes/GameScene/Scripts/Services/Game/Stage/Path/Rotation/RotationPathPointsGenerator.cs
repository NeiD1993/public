using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
                private readonly BoundaryRotationPathPointsAmountRandomizer boundaryRotationPathPointsAmountRandomizer;

                private readonly PathPointsNumbersIntervalAccessor pathPointsNumbersIntervalAccessor;

                public RotationPathPointsGenerator()
                {
                    pathPointsNumbersIntervalAccessor = new PathPointsNumbersIntervalAccessor();
                    boundaryRotationPathPointsAmountRandomizer = new BoundaryRotationPathPointsAmountRandomizer();
                }

                private static bool TryAddNewPathPointsNumbersInterval(LinkedListNode<PathPointsNumbersInterval> pathPointsNumbersIntervalToAddAfter,
                    PathPointsNumbersInterval forbiddenPathPointsNumbersInterval, LinkedList<PathPointsNumbersInterval> pathPointsNumbersIntervals)
                {
                    PathPointsNumbersInterval newPathPointsNumbersInterval = new PathPointsNumbersInterval(forbiddenPathPointsNumbersInterval.End + 1, 
                        pathPointsNumbersIntervalToAddAfter.Value.End);

                    if (newPathPointsNumbersInterval.Length >= 1)
                    {
                        pathPointsNumbersIntervals.AddAfter(pathPointsNumbersIntervalToAddAfter, newPathPointsNumbersInterval);
                        return true;
                    }
                    else
                        return false;
                }

                private static bool TryUpdateExistingPathPointsNumbersIntervalLength(LinkedListNode<PathPointsNumbersInterval> existingPathPointsNumbersInterval,
                    PathPointsNumbersInterval forbiddenPathPointsNumbersInterval, LinkedList<PathPointsNumbersInterval> pathPointsNumbersIntervals)
                {
                    existingPathPointsNumbersInterval.Value.End = forbiddenPathPointsNumbersInterval.Start - 1;

                    if (existingPathPointsNumbersInterval.Value.Length < 1)
                    {
                        pathPointsNumbersIntervals.Remove(existingPathPointsNumbersInterval);
                        return false;
                    }
                    else
                        return true;
                }

                private static RotationType GenerateRotationPointType(IDictionary<RotationType, int> rotationPathPointsAmountDataDictionary)
                {
                    ICollection<RotationType> possibleRotationTypes = rotationPathPointsAmountDataDictionary.Keys;
                    RotationType generatedRotationType = possibleRotationTypes.ElementAt(UnityEngine.Random.Range(0, possibleRotationTypes.Count));

                    if (--rotationPathPointsAmountDataDictionary[generatedRotationType] == 0)
                        rotationPathPointsAmountDataDictionary.Remove(generatedRotationType);

                    return generatedRotationType;
                }

                private static PathPointsNumbersInterval CreateForbiddenPathPointsNumbersInterval(int rotationPathPointNumber, 
                    PathPointsNumbersInterval pathPointsNumbersInterval)
                {
                    int startPointNumber = (rotationPathPointNumber > pathPointsNumbersInterval.Start) ? rotationPathPointNumber - 1 : pathPointsNumbersInterval.Start;
                    int endPointNumber = (rotationPathPointNumber < pathPointsNumbersInterval.End) ? rotationPathPointNumber + 1 : pathPointsNumbersInterval.End;

                    return new PathPointsNumbersInterval(startPointNumber, endPointNumber);
                }

                private IEnumerator GenerateRotationPathPointTypeIteratively(int pathPointsNumbersIntervalIndex, 
                    LinkedList<PathPointsNumbersInterval> pathPointsNumbersIntervals, IDictionary<RotationType, int> rotationPathPointsAmountDataDictionary, 
                    Action<KeyValuePair<int, RotationType>> rotationPathPointTypeExtractor)
                {
                    LinkedListNode<PathPointsNumbersInterval> pathPointsNumbersInterval = null;

                    yield return pathPointsNumbersIntervalAccessor.GetPathPointsNumbersIntervalByIndexIteratively(pathPointsNumbersIntervalIndex, pathPointsNumbersIntervals,
                            pathPointsNumbersIntervalParameter => pathPointsNumbersInterval = pathPointsNumbersIntervalParameter);

                    int generatedRotationPathPointNumber = UnityEngine.Random.Range(pathPointsNumbersInterval.Value.Start, pathPointsNumbersInterval.Value.End + 1);
                    PathPointsNumbersInterval forbiddenPathPointsNumbersInterval = CreateForbiddenPathPointsNumbersInterval(generatedRotationPathPointNumber, 
                        pathPointsNumbersInterval.Value);

                    TryAddNewPathPointsNumbersInterval(pathPointsNumbersInterval, forbiddenPathPointsNumbersInterval, pathPointsNumbersIntervals);
                    TryUpdateExistingPathPointsNumbersIntervalLength(pathPointsNumbersInterval, forbiddenPathPointsNumbersInterval, pathPointsNumbersIntervals);

                    rotationPathPointTypeExtractor(new KeyValuePair<int, RotationType>(generatedRotationPathPointNumber, 
                        GenerateRotationPointType(rotationPathPointsAmountDataDictionary)));
                }

                public IEnumerator GenerateRotationPathPointsIteratively(int pathLength, PathToReservoirRotationPointsPercentageSettings rotationPointsPercentageSettings, 
                    Action<IDictionary<int, RotationType>> rotationPathPointsTypesExtractor)
                {
                    IDictionary<RotationType, int> boundaryRotationPathPointsAmountDataDictionary = boundaryRotationPathPointsAmountRandomizer
                        .RandomizeBoundaryRotationPathPointsAmount(pathLength, rotationPointsPercentageSettings).CopyToDictionary();
                    IDictionary<int, RotationType> rotationPathPointsTypes = new Dictionary<int, RotationType>();

                    if (boundaryRotationPathPointsAmountDataDictionary.Count != 0)
                    {
                        KeyValuePair<int, RotationType> rotationPathPointTypeItem;
                        LinkedList<PathPointsNumbersInterval> pathPointsNumbersIntervals = new LinkedList<PathPointsNumbersInterval>();

                        pathPointsNumbersIntervals.AddFirst(new PathPointsNumbersInterval(2, pathLength - 1));

                        while ((pathPointsNumbersIntervals.Count != 0) && (boundaryRotationPathPointsAmountDataDictionary.Count > 0))
                        {
                            yield return GenerateRotationPathPointTypeIteratively(UnityEngine.Random.Range(0, pathPointsNumbersIntervals.Count), pathPointsNumbersIntervals,
                                boundaryRotationPathPointsAmountDataDictionary, rotationPathPointTypeItemParameter => 
                                rotationPathPointTypeItem = rotationPathPointTypeItemParameter);

                            rotationPathPointsTypes.Add(rotationPathPointTypeItem);

                            yield return rotationPathPointsTypes;
                        }
                    }

                    rotationPathPointsTypesExtractor(rotationPathPointsTypes);
                }

                private class PathPointsNumbersInterval
                {
                    public PathPointsNumbersInterval(int start, int end)
                    {
                        Start = start;
                        End = end;
                    }

                    public int Start { get; private set; }

                    public int End { get; set; }

                    public int Length
                    {
                        get
                        {
                            return End - (Start - 1);
                        }
                    }
                }
            }
        }
    }
}