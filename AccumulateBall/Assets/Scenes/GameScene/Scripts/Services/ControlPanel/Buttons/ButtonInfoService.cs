using GameScene.Behaviours.Button.Enums;
using GameScene.Managers.ButtonsPanel.Settings;
using GameScene.Services.Buttons.Info;
using UnityEngine;

namespace GameScene.Services.Buttons
{
    public class ButtonInfoService
    {
        private static int GetButtonPositionIndex(ButtonRectTransformSettings rectTransformSettings, ButtonType type, bool isDefaultPosition = false)
        {
            int buttonPositionIndex;

            if (isDefaultPosition)
                buttonPositionIndex = 0;
            else
            {
                ButtonRectTransformPositionIndexesSettings positionIndexesSettings = rectTransformSettings.PositionSettings.PositionIndexes;

                buttonPositionIndex = (type == ButtonType.Continue) || (type == ButtonType.Pause) ? positionIndexesSettings.KeepingButton : 
                    positionIndexesSettings.FormingButton;
            }

            return buttonPositionIndex;
        }

        private static GameObject GetButtonPrefab(ButtonPrefabsSettings prefabsSettings, ButtonType type)
        {
            switch (type)
            {
                case ButtonType.Continue:
                    return prefabsSettings.KeepingButtons.ContinueButton;
                case ButtonType.Pause:
                    return prefabsSettings.KeepingButtons.PauseButton;
                case ButtonType.Start:
                    return prefabsSettings.FormingButtons.StartButton;
                default:
                    return prefabsSettings.FormingButtons.StopButton;
            }
        }

        public ButtonInstantiatingInfo GetButtonInstantiatingInfo(ButtonSettings buttonSettings, ButtonType type, bool isDefaultPosition = false)
        {
            return new ButtonInstantiatingInfo(GetButtonPrefab(buttonSettings.Prefabs, type), GetButtonAnimatorController(type, buttonSettings.AnimatorControllers), 
                GetButtonPositionIndex(buttonSettings.RectTransformSettings, type, isDefaultPosition));
        }

        public RuntimeAnimatorController GetButtonAnimatorController(ButtonType type, ButtonAnimatorControllerSettings animatorControllerSettings)
        {
            return (type == ButtonType.Continue) || (type == ButtonType.Pause) ? animatorControllerSettings.KeepingButton : animatorControllerSettings.FormingButton;
        }
    }
}

namespace GameScene.Services.Buttons.Info
{
    public struct ButtonInstantiatingInfo
    {
        public ButtonInstantiatingInfo(GameObject prefab, RuntimeAnimatorController animatorController, int positionIndex)
        {
            Prefab = prefab;
            AnimatorController = animatorController;
            PositionIndex = positionIndex;
        }

        public int PositionIndex { get; private set; }

        public GameObject Prefab { get; private set; }

        public RuntimeAnimatorController AnimatorController { get; private set; }
    }
}