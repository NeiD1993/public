using System;
using GameScene.Managers.Platforms.Settings;
using UnityEngine;

namespace GameScene.Services.Platform
{
    public class PlatformsRadianceColorsToner
    {
        private Func<int, Color> Toner { get; set; }

        public void ResetToning()
        {
            Toner = null;
        }

        public void SetupToning(PlatformNonNeutralRoofIlluminatorColorsSettings platformNonNeutralRoofIlluminatorColorsSettings, int tonesAmount)
        {
            Color toneShift = (platformNonNeutralRoofIlluminatorColorsSettings.Warm - platformNonNeutralRoofIlluminatorColorsSettings.Cold) / (tonesAmount - 1);

            Toner = toneIndexParameter => platformNonNeutralRoofIlluminatorColorsSettings.Cold + toneIndexParameter * toneShift;
        }

        public Color Tone(int toneIndex)
        {
            return Toner(toneIndex);
        }
    }
}