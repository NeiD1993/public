using System;
using System.Collections;
using GameScene.Services.Routines;
using GameScene.Services.Routines.Interfaces;
using UnityEngine;

namespace GameScene.Services.Field
{
    public class FieldEntityStatusChangingRoutinesExecutor : BaseWaitingSingleRountinesExecutor<IEnumerator>, IWaitingSingleRoutinesExecutor
    {
        public FieldEntityStatusChangingRoutinesExecutor()
        {
            defaultRoutineExecutionConditionFunction = routine => routines.Peek() == routine;
        }

        public IEnumerator ExecuteRoutineIteratively(IEnumerator routine, Func<bool> waitingConditionFunction = null)
        {
            routines.Enqueue(routine);

            yield return new WaitUntil(GetRoutineExecutionConditionFunction(routine, waitingConditionFunction));

            yield return routine;

            routines.Dequeue();
        }
    }
}