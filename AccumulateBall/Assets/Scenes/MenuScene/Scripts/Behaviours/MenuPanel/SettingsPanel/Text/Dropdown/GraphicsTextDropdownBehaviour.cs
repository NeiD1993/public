using System;
using MenuScene.Services.Subscription.Enums;
using UnityEngine;
using static UnityEngine.UI.Dropdown;

namespace MenuScene.Behaviours.TextDropdown
{
    public class GraphicsTextDropdownBehaviour : BaseTextDropdownBehaviour
    {
        protected override void AddOptions()
        {
            foreach (string qualitySettingName in QualitySettings.names)
                Control.options.Add(new OptionData(qualitySettingName));
        }

        protected override int GetDefaultValue()
        {
            return QualitySettings.GetQualityLevel();
        }

        public override void ProcessValueAdmitted(int value)
        {
            applicationEventsListenersService.EditEventListener(ApplicationEventType.Quitting, this, (Action)(() => QualitySettings.SetQualityLevel(value)));
        }
    }
}