using System;
using System.Collections;
using System.Collections.Generic;

namespace GameScene.Services.Routines
{
    public abstract class BaseMultipleRoutinesExecutor : BaseRoutinesExecutor
    {
        public abstract IEnumerator ExecuteRoutinesIteratively();
    }

    public abstract class BaseRoutinesExecutor
    {
        protected readonly Queue<IEnumerator> routines;

        public BaseRoutinesExecutor()
        {
            routines = new Queue<IEnumerator>();
        }
    }

    public abstract class BaseWaitingSingleRountinesExecutor<T> : BaseRoutinesExecutor
    {
        protected Func<T, bool> defaultRoutineExecutionConditionFunction;
        
        protected Func<bool> GetRoutineExecutionConditionFunction(T routineExecutionConditionInfo, Func<bool> waitingConditionFunction = null)
        {
            if (waitingConditionFunction != null)
                return () => defaultRoutineExecutionConditionFunction(routineExecutionConditionInfo) && waitingConditionFunction();
            else
                return () => defaultRoutineExecutionConditionFunction(routineExecutionConditionInfo);
        }
    }
}

namespace GameScene.Services.Routines.Interfaces
{
    public interface IWaitingSingleRoutinesExecutor
    {
        IEnumerator ExecuteRoutineIteratively(IEnumerator routine, Func<bool> waitingConditionFunction = null);
    }

    public interface IWaitingSingleRoutinesExecutor<T> where T : struct
    {
        IEnumerator ExecuteRoutineIteratively(IEnumerator routine, T routineInfo, Func<bool> waitingConditionFunction = null);
    }
}