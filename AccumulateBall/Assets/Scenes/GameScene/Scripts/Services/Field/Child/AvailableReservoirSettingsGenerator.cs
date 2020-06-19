using System;
using System.Collections.Generic;
using GameScene.Behaviours.Reservoir.Enums;
using GameScene.Managers.Reservoir.Settings;
using GameScene.Services.Reservoir.Settings;
using UnityEngine;

namespace GameScene.Services.Reservoir
{
    public class AvailableReservoirSettingsGenerator
    {
        private static AvailableReservoirSettings GetMultyPositionalAvailableReservoirSettings(GeneratedPossibleReservoirSettings generatedReservoirSettings)
        {
            return new AvailableReservoirSettings(generatedReservoirSettings.MultipleSettings.Positions, reservoirPositionParameter =>
            generatedReservoirSettings.MultipleSettings.SubtanceColorsTypes);
        }

        private static AvailableReservoirSettings GetSinglePositionalAvailableReservoirSettings(GeneratedPossibleReservoirSettings generatedReservoirSettings)
        {
            return new AvailableReservoirSettings(generatedReservoirSettings.SinglePositionalSubstanceColorsTypes.Keys,
                reservoirPositionParameter => new SubstanceColorType[] { generatedReservoirSettings.SinglePositionalSubstanceColorsTypes[reservoirPositionParameter] });
        }

        public AvailableReservoirSettings GenerateAvailableReservoirSettings(GeneratedPossibleReservoirSettings generatedReservoirSettings)
        {
            ReservoirSettingsType generatedReservoirSettingsType = (ReservoirSettingsType)UnityEngine.Random.Range(0, Enum.GetValues(typeof(ReservoirSettingsType)).Length);

            switch (generatedReservoirSettingsType)
            {
                case ReservoirSettingsType.MultyPositional:
                    {
                        if (generatedReservoirSettings.MultipleSettings.SubtanceColorsTypes.Count > 0)
                            return GetMultyPositionalAvailableReservoirSettings(generatedReservoirSettings);
                        else
                            return GetSinglePositionalAvailableReservoirSettings(generatedReservoirSettings);
                    }
                default:
                    {
                        if (generatedReservoirSettings.SinglePositionalSubstanceColorsTypes.Values.Count > 0)
                            return GetSinglePositionalAvailableReservoirSettings(generatedReservoirSettings);
                        else
                            return GetMultyPositionalAvailableReservoirSettings(generatedReservoirSettings);
                    }
            }
        }

        private enum ReservoirSettingsType
        {
            MultyPositional,

            SinglePositional
        }
    }
}

namespace GameScene.Services.Reservoir.Settings
{
    public struct AvailableReservoirSettings
    {
        public AvailableReservoirSettings(ICollection<Vector2Int> positions, Func<Vector2Int, ICollection<SubstanceColorType>> substanceColorsTypesAccessFunction)
        {
            Positions = positions;
            SubstanceColorsTypesAccessFunction = substanceColorsTypesAccessFunction;
        }

        public ICollection<Vector2Int> Positions { get; private set; }

        public Func<Vector2Int, ICollection<SubstanceColorType>> SubstanceColorsTypesAccessFunction { get; private set; }
    }
}