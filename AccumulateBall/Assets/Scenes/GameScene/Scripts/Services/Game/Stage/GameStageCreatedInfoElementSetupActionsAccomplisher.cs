using System;
using System.Collections.Generic;
using GameScene.Services.Game.Actions;
using GameScene.Services.Game.Actions.Enums;
using GameScene.Services.Game.Actions.Info;

namespace GameScene.Services.Game
{
    public class GameStageCreatedInfoElementSetupActionsAccomplisher<T> where T : class 
    {
        private static Func<T, Action<T>, Action> actionExtractor;

        private readonly Queue<GameStageCreatedInfoElementSetupActionInfo> actionsInfo;

        static GameStageCreatedInfoElementSetupActionsAccomplisher()
        {
            actionExtractor = (gameStageCreatedInfoElement, action) => () => action(gameStageCreatedInfoElement);
        }

        public GameStageCreatedInfoElementSetupActionsAccomplisher(T gameStageCreatedInfoElement, GameStageCreatedInfoElementSetupActions<T> actions)
        {
            actionsInfo = new Queue<GameStageCreatedInfoElementSetupActionInfo>(new GameStageCreatedInfoElementSetupActionInfo[] {
                new GameStageCreatedInfoElementSetupActionInfo(GameStageCreatedInfoElementSetupActionType.Creating, actionExtractor(gameStageCreatedInfoElement,
                actions.Creating)),
                new GameStageCreatedInfoElementSetupActionInfo(GameStageCreatedInfoElementSetupActionType.Disposing, actionExtractor(gameStageCreatedInfoElement,
                actions.Disposing))
            });
        }

        public void Accomplish(GameStageCreatedInfoElementSetupActionType actionType)
        {
            GameStageCreatedInfoElementSetupActionInfo upcomingActionInfo = actionsInfo.Peek();

            if (upcomingActionInfo.Type == actionType)
            {
                upcomingActionInfo.Action();
                actionsInfo.Dequeue();
            }
        }
    }
}