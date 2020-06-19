using System;
using System.Collections;
using System.Collections.Generic;

namespace GameScene.Services.Game
{
    public partial class GameLogicService
    {
        private partial class PathToReservoirGenerator
        {
            private partial class RotationPathPointsGenerator
            {
                private class PathPointsNumbersIntervalAccessor
                {
                    private static IEnumerator GetIntermediatePathPointsNumbersIntervalByIndexIteratively(int intermediatePathPointsNumbersIntervalIndex,
                        LinkedList<PathPointsNumbersInterval> pathPointsNumbersIntervals,
                        Action<LinkedListNode<PathPointsNumbersInterval>> intermediatePathPointsNumbersIntervalExtractor)
                    {
                        PathPointsNumbersIntervalAccessPattern pathPointsNumbersIntervalAccessPattern =
                            (intermediatePathPointsNumbersIntervalIndex <= ((pathPointsNumbersIntervals.Count - 1) / 2)) ?
                            PathPointsNumbersIntervalAccessPattern.FirstToLast : PathPointsNumbersIntervalAccessPattern.LastToFirst;
                        int pathPointsNumbersIntervalsLeftToOverstep = (pathPointsNumbersIntervalAccessPattern == PathPointsNumbersIntervalAccessPattern.FirstToLast) ?
                            intermediatePathPointsNumbersIntervalIndex : (pathPointsNumbersIntervals.Count - (intermediatePathPointsNumbersIntervalIndex + 1));
                        Func<LinkedListNode<PathPointsNumbersInterval>, LinkedListNode<PathPointsNumbersInterval>> pathPointsNumbersIntervalAccessAction =
                            GetPathPointsNumbersIntervalAccessFunction(pathPointsNumbersIntervalAccessPattern);
                        LinkedListNode<PathPointsNumbersInterval> intermediatePathPointsNumbersInterval = intermediatePathPointsNumbersInterval =
                            (pathPointsNumbersIntervalAccessPattern == PathPointsNumbersIntervalAccessPattern.LastToFirst) ? pathPointsNumbersIntervals.Last :
                            pathPointsNumbersIntervals.First;

                        while (pathPointsNumbersIntervalsLeftToOverstep > 0)
                        {
                            intermediatePathPointsNumbersInterval = pathPointsNumbersIntervalAccessAction(intermediatePathPointsNumbersInterval);
                            pathPointsNumbersIntervalsLeftToOverstep--;

                            yield return intermediatePathPointsNumbersInterval;
                        }

                        intermediatePathPointsNumbersIntervalExtractor(intermediatePathPointsNumbersInterval);
                    }

                    private static LinkedListNode<PathPointsNumbersInterval> GetBorderPathPointsNumbersIntervalByIndex(int borderPathPointsNumbersIntervalIndex,
                        LinkedList<PathPointsNumbersInterval> pathPointsNumbersIntervals)
                    {
                        LinkedListNode<PathPointsNumbersInterval> borderPathPointsNumbersInterval = null;

                        if (borderPathPointsNumbersIntervalIndex == 0)
                            borderPathPointsNumbersInterval = pathPointsNumbersIntervals.First;
                        else if (borderPathPointsNumbersIntervalIndex == pathPointsNumbersIntervals.Count - 1)
                            borderPathPointsNumbersInterval = pathPointsNumbersIntervals.Last;

                        return borderPathPointsNumbersInterval;
                    }

                    private static Func<LinkedListNode<PathPointsNumbersInterval>, LinkedListNode<PathPointsNumbersInterval>>
                        GetPathPointsNumbersIntervalAccessFunction(PathPointsNumbersIntervalAccessPattern pathPointsNumbersIntervalAccessPattern)
                    {
                        if (pathPointsNumbersIntervalAccessPattern == PathPointsNumbersIntervalAccessPattern.FirstToLast)
                            return pathPointsNumbersIntervalParameter => pathPointsNumbersIntervalParameter.Next;
                        else
                            return pathPointsNumbersIntervalParameter => pathPointsNumbersIntervalParameter.Previous;
                    }

                    public IEnumerator GetPathPointsNumbersIntervalByIndexIteratively(int pathPointsNumbersIntervalIndex, 
                        LinkedList<PathPointsNumbersInterval> pathPointsNumbersIntervals, 
                        Action<LinkedListNode<PathPointsNumbersInterval>> pathPointsNumbersIntervalExtractor)
                    {
                        if ((pathPointsNumbersIntervalIndex > 0) && (pathPointsNumbersIntervalIndex < pathPointsNumbersIntervals.Count - 1))
                            yield return GetIntermediatePathPointsNumbersIntervalByIndexIteratively(pathPointsNumbersIntervalIndex, pathPointsNumbersIntervals,
                                pathPointsNumbersIntervalExtractor);
                        else
                            pathPointsNumbersIntervalExtractor(GetBorderPathPointsNumbersIntervalByIndex(pathPointsNumbersIntervalIndex, pathPointsNumbersIntervals));
                    }

                    private enum PathPointsNumbersIntervalAccessPattern
                    {
                        FirstToLast,

                        LastToFirst
                    }
                }
            }
        }
    }
}