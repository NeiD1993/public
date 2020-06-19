using System;
using System.Collections.Generic;
using System.Linq;
using GameScene.Services.Platform.Enums;
using UnityEngine;

namespace GameScene.Services.Platform
{
    public partial class ShiningService : BaseSharedService
    {
        private class ShiningGradientService
        {
            private static void SetShiningGradientKeys(Gradient gradient, IEnumerable<ShiningGradientKeyCreatingInfo> keysCreatingInfo)
            {
                T[] CreateShiningGradientKeys<T>(Func<ShiningGradientKeyCreatingInfo, T> keyCreatingFunction) where T : struct
                {
                    return keysCreatingInfo.Select(keyCreatingFunction).ToArray();
                }

                gradient.SetKeys(CreateShiningGradientKeys((keyCreatingInfoParameter) => new GradientColorKey(keyCreatingInfoParameter.Color, keyCreatingInfoParameter.Time)), 
                    CreateShiningGradientKeys((keyCreatingInfoParameter) => new GradientAlphaKey(keyCreatingInfoParameter.Color.a, keyCreatingInfoParameter.Time)));
            }

            private static IEnumerable<ShiningGradientKeyCreatingInfo> GetShiningGradientKeysCreatingInfo(ShiningType type, ShiningColorsInfo colorsInfo)
            {
                IEnumerable<ShiningGradientKeyCreatingInfo> GetMirroredShiningGradientKeysCreatingInfo()
                {
                    Color outgoingColor = colorsInfo.Outgoing;

                    return new ShiningGradientKeyCreatingInfo[]
                    {
                        new ShiningGradientKeyCreatingInfo(outgoingColor, 0),
                        new ShiningGradientKeyCreatingInfo(colorsInfo.Incoming, 0.5f),
                        new ShiningGradientKeyCreatingInfo(outgoingColor, 1)
                    };
                }

                IEnumerable<ShiningGradientKeyCreatingInfo> GetUnmirroredShiningGradientKeysCreatingInfo()
                {
                    return new ShiningGradientKeyCreatingInfo[]
                    {
                        new ShiningGradientKeyCreatingInfo(colorsInfo.Outgoing, 0),
                        new ShiningGradientKeyCreatingInfo(colorsInfo.Incoming, 1)
                    };
                }

                switch (type)
                {
                    case ShiningType.Mirrored:
                        return GetMirroredShiningGradientKeysCreatingInfo();
                    default:
                        return GetUnmirroredShiningGradientKeysCreatingInfo();
                }
            }

            public Gradient CreateShiningGradient(ShiningType type, ShiningColorsInfo colorsInfo)
            {
                Gradient gradient = new Gradient();

                SetShiningGradientKeys(gradient, GetShiningGradientKeysCreatingInfo(type, colorsInfo));

                return gradient;
            }

            private struct ShiningGradientKeyCreatingInfo
            {
                public ShiningGradientKeyCreatingInfo(Color color, float time)
                {
                    Color = color;
                    Time = time;
                }

                public float Time { get; private set; }

                public Color Color { get; private set; }
            }
        }
    }
}