using System;
using MenuScene.Services.Subscription.Enums;
using Settings;
using UnityEngine;

namespace MenuScene.Behaviours.TextSlider
{
    public class TargetFrameRateTextSliderBehaviour : BaseTextSliderBehaviour
    {
        protected override void ProcessValueAdmittedInternally()
        {
            int value = (int)Value;

            Application.targetFrameRate = value;
            applicationEventsListenersService.EditEventListener(ApplicationEventType.Quitting, this, (Action)(() => PlayerPrefs.SetInt(nameof(Application.targetFrameRate), value)));
        }

        protected override void SetBoundaryValuesInternally()
        {
            Control.minValue = TargetFrameRateSettings.Lowest;
            Control.maxValue = TargetFrameRateSettings.Highest;
        }

        protected override float GetDefaultValue()
        {
            return Application.targetFrameRate;
        }
    }
}