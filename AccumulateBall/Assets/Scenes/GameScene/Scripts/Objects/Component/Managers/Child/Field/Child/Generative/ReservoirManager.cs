using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GameScene.Behaviours.Reservoir;
using GameScene.Behaviours.Reservoir.Enums;
using GameScene.Managers.Entity.Settings.Interfaces;
using GameScene.Managers.Field;
using GameScene.Managers.Field.Interfaces;
using GameScene.Managers.Field.Settings;
using GameScene.Managers.Reservoir.Settings;
using GameScene.Services.Reservoir;
using GameScene.Services.Reservoir.Settings;
using UnityEngine;
using UnityEngine.Events;

namespace GameScene.Managers.Reservoir
{
    public class ReservoirManager : BaseChildFieldEntityGenerativeManager<ReservoirBehaviour, ReservoirSettings, ReservoirSubstanceSettings, ReservoirSubstanceColorsSettings, SubstanceColorType>, 
        ICheckableGeneratedFieldObjectManager<SubstanceColorType>
    {
        private readonly AvailableReservoirSettingsGenerator availableReservoirSettingsGenerator;

        public ReservoirManager()
        {
            availableReservoirSettingsGenerator = new AvailableReservoirSettingsGenerator();
            ReservoirFreezed = new UnityEvent();
        }

        public UnityEvent ReservoirFreezed { get; private set; }

        private static SubstanceColorType GenerateSubstanceColorType(Vector2Int reservoirPosition, Func<Vector2Int, ICollection<SubstanceColorType>> substanceColorsTypesAccessFunction)
        {
            ICollection<SubstanceColorType> availableSubstanceColorsTypes = substanceColorsTypesAccessFunction(reservoirPosition);

            return availableSubstanceColorsTypes.ElementAt(UnityEngine.Random.Range(0, availableSubstanceColorsTypes.Count));
        }

        SubstanceColorType ICheckableGeneratedFieldObjectManager<SubstanceColorType>.GetObjectCheckableInfo()
        {
            return EntityInfo.Object.GetComponent<ReservoirBehaviour>().CreateCheckableData();
        }

        protected override void RemoveObjectBehaviourEventsListeners(ReservoirBehaviour objectBehaviour)
        {
            base.RemoveObjectBehaviourEventsListeners(objectBehaviour);
            objectBehaviour.AnimatedlyFreezed.RemoveListener(OnReservoirAnimatedlyFreezed);
        }

        protected override IEnumerator ProcessGeneratedObjectMinorPartAnimatedlyDisappearedIteratively()
        {
            yield return base.ProcessGeneratedObjectMinorPartAnimatedlyDisappearedIteratively();

            ObjectPartsFlushed.Invoke();
        }

        protected override ReservoirBehaviour CreateAndInitiallySetupObjectBehaviour(GameObject obj, Vector2Int objectPosition, params object[] parameters)
        {
            ReservoirBehaviour reservoirBehaviour = base.CreateAndInitiallySetupObjectBehaviour(obj, objectPosition, parameters);

            reservoirBehaviour.Setup((SubstanceColorType)parameters[0]);
            reservoirBehaviour.AnimatedlyFreezed.AddListener(OnReservoirAnimatedlyFreezed);

            return reservoirBehaviour;
        }

        public void BeginReservoirSubstanceFlushing()
        {
            EntityInfo.Object.GetComponent<ReservoirBehaviour>().BeginSubstanceFlushing();
        }

        public void GenerateReservoir(GameObject field, IDictionary<Vector2Int, GameObject> freePlatforms, GeneratedPossibleReservoirSettings generatedReservoirSettings)
        {
            void SetupSubstance(ReservoirSubstanceSetupInfo reservoirSubstanceSetupInfo)
            {
                materializedObjectElementColorService.SetElementColor(reservoirSubstanceSetupInfo.Part, entityObjectSettings.PartSettings.Colors.GetSettings(reservoirSubstanceSetupInfo.ColorType));
            }

            AvailableReservoirSettings availableReservoirSettings = availableReservoirSettingsGenerator.GenerateAvailableReservoirSettings(generatedReservoirSettings);

            GenerateObject(freePlatforms, availableReservoirSettings.Positions, customObjectSetupParameterExtractor : 
                reservoirPositionParameter => GenerateSubstanceColorType(reservoirPositionParameter, availableReservoirSettings.SubstanceColorsTypesAccessFunction),
                customAdditionalObjectSetupAction : (Action<GameObject, object>)((reservoirParameter, substanceColorTypeParameter) => SetupSubstance(new ReservoirSubstanceSetupInfo(reservoirParameter, entityObjectSettings, 
                (SubstanceColorType)substanceColorTypeParameter))));
        }

        private void OnReservoirAnimatedlyFreezed()
        {
            ReservoirFreezed.Invoke();
        }

        private class ReservoirSubstanceSetupInfo : EntityPartSetupInfo<ReservoirSubstanceSettings>
        {
            public ReservoirSubstanceSetupInfo(GameObject reservoir, IPartialEntityObjectSettings<ReservoirSubstanceSettings> reservoirSettings, SubstanceColorType colorType) :
                base(reservoir, reservoirSettings)
            {
                ColorType = colorType;
            }

            public SubstanceColorType ColorType;
        }
    }
}

namespace GameScene.Managers.Reservoir.Settings
{
    [Serializable]
    public struct ReservoirSubstanceColorsSettings : IUnitedlyGettableEntityCategorySettings<Color, SubstanceColorType>
    {
        [SerializeField]
        private Color blue;

        [SerializeField]
        private Color green;

        [SerializeField]
        private Color orange;

        [SerializeField]
        private Color red;

        public Color GetSettings(SubstanceColorType category)
        {
            switch (category)
            {
                case SubstanceColorType.Blue:
                    return blue;
                case SubstanceColorType.Green:
                    return green;
                case SubstanceColorType.Orange:
                    return orange;
                default:
                    return red;
            }
        }
    }

    public class GeneratedMultipleReservoirSettings
    {
        private IList<Vector2Int> positions;

        private ICollection<SubstanceColorType> subtanceColorsTypes;

        public IList<Vector2Int> Positions
        {
            get
            {
                if (positions == null)
                    positions = new List<Vector2Int>();

                return positions;
            }
        }

        public ICollection<SubstanceColorType> SubtanceColorsTypes
        {
            get
            {
                if (subtanceColorsTypes == null)
                    subtanceColorsTypes = new List<SubstanceColorType>();

                return subtanceColorsTypes;
            }
        }
    }

    public class GeneratedPossibleReservoirSettings
    {
        private IDictionary<Vector2Int, SubstanceColorType> singlePositionalSubstanceColorsTypes;

        public GeneratedPossibleReservoirSettings()
        {
            MultipleSettings = new GeneratedMultipleReservoirSettings();
        }

        public IDictionary<Vector2Int, SubstanceColorType> SinglePositionalSubstanceColorsTypes
        {
            get
            {
                if (singlePositionalSubstanceColorsTypes == null)
                    singlePositionalSubstanceColorsTypes = new Dictionary<Vector2Int, SubstanceColorType>();

                return singlePositionalSubstanceColorsTypes;
            }
        }

        public GeneratedMultipleReservoirSettings MultipleSettings { get; private set; }
    }

    [Serializable]
    public class ReservoirSettings : GeneratedFieldObjectSettings<ReservoirSubstanceSettings, ReservoirSubstanceColorsSettings, SubstanceColorType> { }

    [Serializable]
    public class ReservoirSubstanceSettings : GeneratedFieldObjectPartSettings<ReservoirSubstanceColorsSettings, SubstanceColorType> { }
}