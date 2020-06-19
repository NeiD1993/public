using System;
using System.Collections;
using System.Collections.Generic;
using GameScene.Services.Buttons.Enums;
using GameScene.Services.Buttons.Info;
using GameScene.Services.Routines;
using GameScene.Services.Routines.Interfaces;
using UnityEngine;

namespace GameScene.Services.Buttons
{
    public class ButtonsPanelStatusChangingRoutinesExecutor : BaseWaitingSingleRountinesExecutor<ButtonsPanelStatusChangingRoutineExecutionConditionInfo>, 
        IWaitingSingleRoutinesExecutor<ButtonsPanelStatusChangingRoutineInfo>
    {
        private readonly IDictionary<Guid, ButtonsPanelStatusChangingRoutineExecutionData> routinesExecutionData;

        public ButtonsPanelStatusChangingRoutinesExecutor()
        {
            defaultRoutineExecutionConditionFunction = routineExecutionConditionInfo => (routines.Peek() == routineExecutionConditionInfo.Routine) && 
            (routinesExecutionData[routineExecutionConditionInfo.Guid].State != ButtonsPanelStatusChangingRoutineState.WaitForExecution);
            routinesExecutionData = new Dictionary<Guid, ButtonsPanelStatusChangingRoutineExecutionData>();
        }

        private void AddRoutineData(IEnumerator routine, ButtonsPanelStatusChangingRoutineInfo routineInfo)
        {
            routines.Enqueue(routine);
            routinesExecutionData.Add(routineInfo.Guid, new ButtonsPanelStatusChangingRoutineExecutionData(routine, routineInfo.State));
        }

        private void RemoveRoutineData(Guid routineGuid)
        {
            routinesExecutionData.Remove(routineGuid);
            routines.Dequeue();
        }

        public void CancelRoutineExecution(Guid routineGuid)
        {
            routinesExecutionData[routineGuid].State = ButtonsPanelStatusChangingRoutineState.CanceledForExecution;
        }

        public void PrepareRoutineForExecution(Guid routineGuid)
        {
            routinesExecutionData[routineGuid].State = ButtonsPanelStatusChangingRoutineState.PreparedForExecution;
        }

        public IEnumerator ExecuteRoutineIteratively(IEnumerator routine, ButtonsPanelStatusChangingRoutineInfo routineInfo, Func<bool> waitingConditionFunction = null)
        {
            AddRoutineData(routine, routineInfo);

            yield return new WaitUntil(GetRoutineExecutionConditionFunction(new ButtonsPanelStatusChangingRoutineExecutionConditionInfo(routine, routineInfo.Guid), 
                waitingConditionFunction));

            if (routinesExecutionData[routineInfo.Guid].State == ButtonsPanelStatusChangingRoutineState.PreparedForExecution)
                yield return routine;

            RemoveRoutineData(routineInfo.Guid);
        }

        private class ButtonsPanelStatusChangingRoutineExecutionData
        {
            public ButtonsPanelStatusChangingRoutineExecutionData(IEnumerator routine, ButtonsPanelStatusChangingRoutineState state)
            {
                Routine = routine;
                State = state;
            }

            public IEnumerator Routine { get; private set; }

            public ButtonsPanelStatusChangingRoutineState State { get; set; }
        }
    }
}

namespace GameScene.Services.Buttons.Enums
{
    public enum ButtonsPanelStatusChangingRoutineState
    {
        CanceledForExecution,

        PreparedForExecution,

        WaitForExecution
    }

    public enum ButtonsPanelStatusChangingRoutineProcessingType
    {
        Cancelling,

        Performing
    }
}

namespace GameScene.Services.Buttons.Info
{
    public struct ButtonsPanelStatusChangingRoutineExecutionConditionInfo
    {
        public ButtonsPanelStatusChangingRoutineExecutionConditionInfo(IEnumerator routine, Guid guid)
        {
            Routine = routine;
            Guid = guid;
        }

        public Guid Guid { get; private set; }

        public IEnumerator Routine { get; private set; }
    }

    public struct ButtonsPanelStatusChangingRoutineInfo
    {
        public ButtonsPanelStatusChangingRoutineInfo(Guid guid, ButtonsPanelStatusChangingRoutineState state)
        {
            Guid = guid;
            State = state;
        }

        public ButtonsPanelStatusChangingRoutineState State { get; set; }

        public Guid Guid { get; private set; }
    }
}