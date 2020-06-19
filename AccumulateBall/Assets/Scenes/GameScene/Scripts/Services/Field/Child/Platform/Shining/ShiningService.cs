using System;
using GameScene.Services.Colors;
using GameScene.Services.Platform.Enums;
using GameScene.Services.Platform.Info;
using ServicesLocators;
using UnityEngine;

namespace GameScene.Services.Platform
{
    public partial class ShiningService : BaseSharedService
    {
        private readonly MaterializedObjectElementColorService materializedObjectElementColorService;

        private readonly ShiningGradientService shiningGradientService;

        public ShiningService()
        {
            materializedObjectElementColorService = SharedSceneServicesLocator.GetService<MaterializedObjectElementColorService>();
            shiningGradientService = new ShiningGradientService();
        }

        private ShiningColorsInfo CreateShiningColorsInfo(GameObject illuminator, Color incomingColor)
        {
            Color outgoingColor = materializedObjectElementColorService.GetElementColor(illuminator);

            return new ShiningColorsInfo(incomingColor, outgoingColor);
        }

        public Action<float> CreateShiningAction(GameObject illuminator, ShiningInfo info)
        {
            Gradient gradient = shiningGradientService.CreateShiningGradient(info.Type, CreateShiningColorsInfo(illuminator, info.IncomingColor));

            return (timeParameter) => materializedObjectElementColorService.SetElementColor(illuminator, gradient.Evaluate(timeParameter));
        }

        private struct ShiningColorsInfo
        {
            public ShiningColorsInfo(Color incoming, Color outgoing)
            {
                Incoming = incoming;
                Outgoing = outgoing;
            }

            public Color Incoming { get; private set; }

            public Color Outgoing { get; private set; }
        }
    }
}

namespace GameScene.Services.Platform.Enums
{
    public enum ShiningType
    {
        Mirrored,

        Unmirrored
    }
}

namespace GameScene.Services.Platform.Info
{
    public struct ShiningInfo
    {
        public ShiningInfo(ShiningType type, Color incomingColor)
        {
            Type = type;
            IncomingColor = incomingColor;
        }

        public ShiningType Type { get; private set; }

        public Color IncomingColor { get; private set; }
    }
}