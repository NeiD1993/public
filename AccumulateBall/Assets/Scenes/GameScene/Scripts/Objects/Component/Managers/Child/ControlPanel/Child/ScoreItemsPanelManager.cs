using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GameScene.Behaviours.ScoreItem;
using GameScene.Behaviours.ScoreItem.Characteristics;
using GameScene.Behaviours.ScoreItem.Characteristics.Info;
using GameScene.Behaviours.ScoreItem.Characteristics.Info.Enums;
using GameScene.Behaviours.ScoreItem.Enums;
using GameScene.Behaviours.ScoreItem.Events.Arguments;
using GameScene.Behaviours.ScoreItem.Info;
using GameScene.Managers.ControlPanel;
using GameScene.Managers.ControlPanel.Info;
using GameScene.Managers.ControlPanel.Interfaces;
using GameScene.Managers.ControlPanel.Settings;
using GameScene.Managers.ControlPanel.Settings.Interfaces;
using GameScene.Managers.Entity.Interfaces;
using GameScene.Managers.Entity.Settings;
using GameScene.Managers.Entity.Settings.Interfaces;
using GameScene.Managers.ScoreItems.Settings;
using GameScene.Services.Colors;
using GameScene.Services.Content.Data;
using GameScene.Services.ControlPanel.Enums;
using GameScene.Services.Game.Characteristics.Details;
using GameScene.Services.Game.Enums;
using GameScene.Services.Game.Units;
using GameScene.Services.ScoreItems;
using ServicesLocators;
using UnityEngine;
using UnityEngine.UI;

namespace GameScene.Managers.ScoreItems
{
    public class ScoreItemsPanelManager : BaseChildControlEntityManager<ScoreItemBehaviour, ScoreItemSettings, ScoreItemRectTransformSettings,
        ScoreItemRectTransformPositionSettings, ChildControlEntityInfo<ScoreItemCategory>, ScoreItemCategory, ScoreItemBehaviourSetupInfo>, ICalibratableControlEntityManager,
        IDisableableControlEntityManager, IIntensifiableControlEntityManager, IIterativelyRunnableEntityManager,
        IRefreshableControlEntityManager<GameStageContentCharacteristicsDetails, GameStageContentUnits<int, TimeSpan, int>>
    {
        private readonly ScoreItemInformationExtractionService scoreItemInformationExtractionService;

        private readonly ScoreItemsSummaryInformingStatusFormer scoreItemsSummaryInformingStatusFormer;

        private MaterializedObjectElementColorService materializedObjectElementColorService;

        public ScoreItemsPanelManager()
        {
            scoreItemInformationExtractionService = new ScoreItemInformationExtractionService();
            scoreItemsSummaryInformingStatusFormer = new ScoreItemsSummaryInformingStatusFormer();
        }

        private void ChangeScoreItemsFlashType(FlashType flashType, bool isSafetyChanging = true)
        {
            Action scoreItemsFlashTypeChangingAction = () =>
            {
                foreach (GameObject scoreItem in EntityInfo.Controls.Values)
                    scoreItem.GetComponent<ScoreItemBehaviour>().ChangeRateFlashType(flashType);
            };

            if (isSafetyChanging)
            {
                if (scoreItemsSummaryInformingStatusFormer.FormSummaryStatus(EntityInfo.Controls.Values) == InformingStatus.Active)
                    scoreItemsFlashTypeChangingAction();
            }
            else
                scoreItemsFlashTypeChangingAction();
        }

        private void CreateScoreItem(ScoreItemCategory scoreItemCategory)
        {
            int scoreItemPositionIndex = entityObjectSettings.RectTransformSettings.PositionSettings.PositionIndexes.GetSettings(scoreItemCategory);
            ScoreItemTextControlInformationSettings scoreItemTextControlInformationSettings = entityObjectSettings.PartSettings.InformationSettings
                .GetSettings(scoreItemCategory);
            ContentExtractionData<object, string> scoreItemInformationExtractionData = scoreItemInformationExtractionService
                .EstablishContentExtractionData(scoreItemTextControlInformationSettings.Blank, scoreItemTextControlInformationSettings.CompositionSettings);
            Func<object, ControlBehaviourInstantiatingInfo> scoreItemBehaviourInstantiatingInfoExtractor =
                scoreItemTextControlSetupInfoParameter => new ControlBehaviourInstantiatingInfo(scoreItemCategory,
                new ScoreItemBehaviourSetupInfo(new ScoreItemCharacteristics(new ScoreItemInformationCharacteristics(scoreItemInformationExtractionData)),
                ((ScoreItemTextControlSetupInfo)scoreItemTextControlSetupInfoParameter).InformationElement));

            CreateControl(new ControlInstantiatingInfo(scoreItemPositionIndex, scoreItemCategory.ToString(), entityObjectSettings.Prefab,
                entityObjectSettings.AnimatorController), (Action<GameObject, object>)((scoreItem, scoreItemTextControlSetupInfoParameter) =>
                InitiallySetupTextControl((ScoreItemTextControlSetupInfo)scoreItemTextControlSetupInfoParameter)), scoreItemBehaviourInstantiatingInfoExtractor, 
                scoreItemParameter => new ScoreItemTextControlSetupInfo(scoreItemParameter, entityObjectSettings, scoreItemCategory));
        }

        private void InitiallySetupTextControl(ScoreItemTextControlSetupInfo scoreItemTextControlSetupInfo)
        {
            void SetupTextControlInformationElementFont(Text scoreItemTextControlInformationElement,
                ScoreItemTextControlInformationFontSettings scoreItemTextControlInformationFontSettings)
            {
                scoreItemTextControlInformationElement.font = scoreItemTextControlInformationFontSettings.Font;
                scoreItemTextControlInformationElement.fontSize = scoreItemTextControlInformationFontSettings.Size;
            }

            SetupControlRectTransformSettings(scoreItemTextControlSetupInfo.Part, entityObjectSettings.PartSettings.RectTransformSettings, rectTransformParameter =>
            {
                rectTransformParameter.localPosition = new Vector3(rectTransformParameter.localPosition.x, entityObjectSettings.PartSettings.Displacement,
                    rectTransformParameter.localPosition.y);
                rectTransformParameter.sizeDelta = entityObjectSettings.PartSettings.RectTransformSettings.Size;
            }, false);

            ScoreItemTextControlInformationSettings scoreItemTextControlInformationSettings = entityObjectSettings.PartSettings.InformationSettings
                .GetSettings(scoreItemTextControlSetupInfo.Category);

            materializedObjectElementColorService.SetElementColor(scoreItemTextControlSetupInfo.InformationElement, scoreItemTextControlInformationSettings.Color);
            SetupTextControlInformationElementFont(scoreItemTextControlSetupInfo.InformationElement, scoreItemTextControlInformationSettings.FontSettings);
        }

        protected override void ObtainSharingRelatedServices()
        {
            base.ObtainSharingRelatedServices();
            materializedObjectElementColorService = SharedSceneServicesLocator.GetService<MaterializedObjectElementColorService>();
        }

        protected override void RemoveControlBehaviourEventsListeners(ScoreItemBehaviour controlBehaviour)
        {
            base.RemoveControlBehaviourEventsListeners(controlBehaviour);
            eventsListenersService.RemoveOrdinaryEventListener(controlBehaviour.AnimatedlyPreparedForInforming);
        }

        protected override PanelType GetPanelType()
        {
            return PanelType.ScoreItemsPanel;
        }

        protected override ScoreItemBehaviour CreateAndInitiallySetupControlBehaviour(GameObject control, ControlBehaviourInstantiatingInfo controlBehaviourInstantiatingInfo)
        {
            ScoreItemBehaviour scoreItemBehaviour = base.CreateAndInitiallySetupControlBehaviour(control, controlBehaviourInstantiatingInfo);

            eventsListenersService.AddOrdinaryEventListener(scoreItemAnimatedlyPreparedForInformingEventArgumentParameter =>
            OnScoreItemAnimatedlyPreparedForInforming(scoreItemAnimatedlyPreparedForInformingEventArgumentParameter, controlBehaviourInstantiatingInfo.Category),
            scoreItemBehaviour.AnimatedlyPreparedForInforming);

            return scoreItemBehaviour;
        }

        public void EnableEntity()
        {
            ChangeScoreItemsFlashType(FlashType.Blink);
        }

        public void DisableEntity()
        {
            ChangeScoreItemsFlashType(FlashType.Flicker);
        }

        public void IntensifyEntity()
        {
            ChangeScoreItemsFlashType(FlashType.Blink, false);
        }

        public void RefreshEntityPartially(GameStageContentCharacteristicsDetails refreshingParameter)
        {
            ScoreItemCategory GetScoreItemToRefreshCategory(GameStageContentUnitGenus gameStageContentGenus)
            {
                switch (gameStageContentGenus)
                {
                    case GameStageContentUnitGenus.AchievedPoints:
                        return ScoreItemCategory.Points;
                    case GameStageContentUnitGenus.LeftTime:
                        return ScoreItemCategory.Time;
                    default:
                        return ScoreItemCategory.Steps;
                }
            }

            GameObject scoreItemToRefresh = EntityInfo.Controls[GetScoreItemToRefreshCategory(refreshingParameter.Genus)];

            StartCoroutine(scoreItemToRefresh.GetComponent<ScoreItemBehaviour>().Characteristics.InformationCharacteristics.Refresh(refreshingParameter.Content));
        }

        public IEnumerator CalibrateEntityIteratively()
        {
            foreach (GameObject scoreItem in EntityInfo.Controls.Values)
                scoreItem.GetComponent<ScoreItemBehaviour>().BeginCalibrating();

            yield return new WaitForFixedUpdate();

            yield return new WaitUntil(() => scoreItemsSummaryInformingStatusFormer.FormSummaryStatus(EntityInfo.Controls.Values) == InformingStatus.Prepared);
        }

        public IEnumerator CreateScoreItemsIteratively()
        {
            foreach (ScoreItemCategory scoreItemCategory in Enum.GetValues(typeof(ScoreItemCategory)))
            {
                CreateScoreItem(scoreItemCategory);

                yield return new WaitUntil(() => EntityInfo.Controls.ContainsKey(scoreItemCategory));
            }
        }

        public IEnumerator HideScoreItemsIteratively()
        {
            KeyValuePair<ScoreItemCategory, GameObject> scoreItemNode;

            while (EntityInfo.Controls.Count > 0)
            {
                scoreItemNode = EntityInfo.Controls.First();

                yield return scoreItemNode.Value.GetComponent<ScoreItemBehaviour>().BeginHiding();

                yield return new WaitUntil(() => !EntityInfo.Controls.ContainsKey(scoreItemNode.Key));
            }
        }

        public IEnumerator RefreshEntityCompletelyIteratively(GameStageContentUnits<int, TimeSpan, int> refreshingParameter)
        {
            object GetScoreItemInformationCharacteristicsRefreshingParameter(ScoreItemCategory scoreItemCategory, GameStageContentUnits<int, TimeSpan,
                int> gameStageExtractableData)
            {
                switch (scoreItemCategory)
                {
                    case ScoreItemCategory.Points:
                        return refreshingParameter.AchievedPoints;
                    case ScoreItemCategory.Steps:
                        return refreshingParameter.RemainedSteps;
                    default:
                        return refreshingParameter.LeftTime;
                }
            }

            ScoreItemCharacteristicsConfiguringActionInfo GetScoreItemCharacteristicsConfiguringActionInfo(ScoreItemInformationCharacteristicsRefreshingInfo
                informationCharacteristicsRefreshingInfo)
            {
                IEnumerator GetConfiguringActionPerformingTypeIteratively(ScoreItemInformationCharacteristics informationCharacteristics,
                    Action<ScoreItemCharacteristicsConfiguringActionPerformingType> performingTypeExtractor)
                {
                    StartCoroutine(informationCharacteristics.Refresh(informationCharacteristicsRefreshingInfo.Parameter, refreshingDataParameter =>
                    informationCharacteristicsRefreshingInfo.Data = refreshingDataParameter));

                    yield return new WaitUntil(() => informationCharacteristicsRefreshingInfo.Data != null);

                    performingTypeExtractor(informationCharacteristicsRefreshingInfo.Data.Result ? ScoreItemCharacteristicsConfiguringActionPerformingType.Fulfilling :
                        ScoreItemCharacteristicsConfiguringActionPerformingType.Refusing);
                }

                return new ScoreItemCharacteristicsConfiguringActionInfo((scoreItemCharacteristicsParameter, scoreItemCharacteristicsConfigurationActionPerformingType) =>
                {
                    informationCharacteristicsRefreshingInfo.Data.PerformFurtherProceeding = scoreItemCharacteristicsConfigurationActionPerformingType ==
                    ScoreItemCharacteristicsConfiguringActionPerformingType.Fulfilling;
                }, (scoreItemCharacteristicsParameter, performingTypeExtractorParameter) =>
                GetConfiguringActionPerformingTypeIteratively(scoreItemCharacteristicsParameter.InformationCharacteristics, performingTypeExtractorParameter));
            }

            ScoreItemInformationCharacteristicsRefreshingInfo scoreItemInformationCharacteristicsRefreshingInfo = new ScoreItemInformationCharacteristicsRefreshingInfo();

            foreach (KeyValuePair<ScoreItemCategory, GameObject> scoreItemNode in EntityInfo.Controls)
            {
                scoreItemInformationCharacteristicsRefreshingInfo.Parameter = GetScoreItemInformationCharacteristicsRefreshingParameter(scoreItemNode.Key, 
                    refreshingParameter);

                yield return scoreItemNode.Value.GetComponent<ScoreItemBehaviour>()
                    .ConfigureRateIteratively(GetScoreItemCharacteristicsConfiguringActionInfo(scoreItemInformationCharacteristicsRefreshingInfo));
            }
        }

        public IEnumerator RunEntityIteratively()
        {
            foreach (GameObject scoreItem in EntityInfo.Controls.Values)
                yield return scoreItem.GetComponent<ScoreItemBehaviour>().BeginInformingIteratively();
        }

        private void OnScoreItemAnimatedlyPreparedForInforming(ScoreItemAnimatedlyPreparedForInformingEventArgument scoreItemAnimatedlyPreparedForInformingEventArgument,
            ScoreItemCategory scoreItemCategory)
        {
            if (scoreItemAnimatedlyPreparedForInformingEventArgument.IsPreparedForInformingFirstly)
            {
                EntityInfo.Controls.Add(scoreItemCategory, scoreItemAnimatedlyPreparedForInformingEventArgument.ScoreItem);

                if (EntityInfo.Controls.Count == Enum.GetValues(typeof(ScoreItemCategory)).Length)
                    EntityActivated.Invoke();
            }
        }

        private class ScoreItemTextControlSetupInfo : EntityPartSetupInfo<ScoreItemTextControlSettings>
        {
            public ScoreItemTextControlSetupInfo(GameObject scoreItem, IPartialEntityObjectSettings<ScoreItemTextControlSettings> scoreItemSettings, 
                ScoreItemCategory category) : base(scoreItem, scoreItemSettings)
            {
                Category = category;
                InformationElement = Part.GetComponent<Text>();
            }

            public ScoreItemCategory Category { get; private set; }

            public Text InformationElement { get; private set; }
        }
    }
}

namespace GameScene.Managers.ScoreItems.Settings
{
    [Serializable]
    public struct ScoreItemTextControlChangeableInformationSectionSettings
    {
        [SerializeField]
        private string format;

        [SerializeField]
        private string identifier;

        public string Format
        {
            get
            {
                return format;
            }
        }

        public string Identifier
        {
            get
            {
                return identifier;
            }
        }
    }

    [Serializable]
    public struct ScoreItemTextControlInformationCompositionSettings
    {
        [SerializeField]
        private ScoreItemTextControlChangeableInformationSectionSettings changeableSectionSettings;

        [SerializeField]
        private string pattern;

        public ScoreItemTextControlChangeableInformationSectionSettings ChangeableSectionSettings
        {
            get
            {
                return changeableSectionSettings;
            }
        }

        public string Pattern
        {
            get
            {
                return pattern;
            }
        }
    }

    [Serializable]
    public struct ScoreItemTextControlInformationFontSettings : ISizeableEntityComponentSettings<int>
    {
        [SerializeField]
        private int size;

        [SerializeField]
        private Font font;

        public int Size
        {
            get
            {
                return size;
            }
        }

        public Font Font
        {
            get
            {
                return font;
            }
        }
    }

    [Serializable]
    public struct ScoreItemTextControlInformationSettings
    {
        [SerializeField]
        private Color color;

        [SerializeField]
        private ScoreItemTextControlInformationFontSettings fontSettings;

        [SerializeField]
        private ScoreItemTextControlInformationCompositionSettings compositionSettings;

        [SerializeField]
        private string blank;

        public Color Color
        {
            get
            {
                return color;
            }
        }

        public ScoreItemTextControlInformationFontSettings FontSettings
        {
            get
            {
                return fontSettings;
            }
        }

        public ScoreItemTextControlInformationCompositionSettings CompositionSettings
        {
            get
            {
                return compositionSettings;
            }
        }

        public string Blank
        {
            get
            {
                return blank;
            }
        }
    }

    public abstract class BaseScoreItemCategorySettings<T> : IUnitedlyGettableEntityCategorySettings<T, ScoreItemCategory> where T : struct
    {
        [SerializeField]
        protected T pointsScoreItem;

        [SerializeField]
        protected T stepsScoreItem;

        [SerializeField]
        protected T timeScoreItem;

        public T GetSettings(ScoreItemCategory category)
        {
            switch (category)
            {
                case ScoreItemCategory.Points:
                    return pointsScoreItem;
                case ScoreItemCategory.Steps:
                    return stepsScoreItem;
                default:
                    return timeScoreItem;
            }
        }
    }

    [Serializable]
    public class ScoreItemCategoryTextControlInformationSettings : BaseScoreItemCategorySettings<ScoreItemTextControlInformationSettings> { }

    [Serializable]
    public class ScoreItemRectTransformPositionIndexesSettings : BaseScoreItemCategorySettings<int> { }

    [Serializable]
    public class ScoreItemRectTransformPositionSettings : BaseControlRectTransformPositionSettings,
        IIndexableControlRectTransformPositionSettings<ScoreItemRectTransformPositionIndexesSettings>
    {
        [SerializeField]
        private ScoreItemRectTransformPositionIndexesSettings positionIndexes;

        public ScoreItemRectTransformPositionIndexesSettings PositionIndexes
        {
            get
            {
                return positionIndexes;
            }
        }
    }

    [Serializable]
    public class ScoreItemRectTransformSettings : BaseControlRectTransformSettings, IPositionableControlRectTransformSettings<ScoreItemRectTransformPositionSettings>
    {
        [SerializeField]
        private ScoreItemRectTransformPositionSettings positionSettings;

        public ScoreItemRectTransformPositionSettings PositionSettings
        {
            get
            {
                return positionSettings;
            }
        }
    }

    [Serializable]
    public class ScoreItemSettings : OwnedEntityObjectSettings, IPartialEntityObjectSettings<ScoreItemTextControlSettings>,
        IRectTransformableControlSettings<ScoreItemRectTransformSettings>, ISingleAnimatorControllerEntityObjectSettings, ISinglePrefabEntityObjectSettings
    {
        [SerializeField]
        private string rootName;

        [SerializeField]
        private GameObject prefab;

        [SerializeField]
        private RuntimeAnimatorController animatorController;

        [SerializeField]
        private ScoreItemRectTransformSettings rectTransformSettings;

        [SerializeField]
        private ScoreItemTextControlSettings partSettings;

        public string RootName
        {
            get
            {
                return rootName;
            }
        }

        public GameObject Prefab
        {
            get
            {
                return prefab;
            }
        }

        public RuntimeAnimatorController AnimatorController
        {
            get
            {
                return animatorController;
            }
        }

        public ScoreItemRectTransformSettings RectTransformSettings
        {
            get
            {
                return rectTransformSettings;
            }
        }

        public ScoreItemTextControlSettings PartSettings
        {
            get
            {
                return partSettings;
            }
        }
    }

    [Serializable]
    public class ScoreItemTextControlRectTransformSettings : BaseControlRectTransformSettings, ISizeableEntityComponentSettings<Vector2>
    {
        [SerializeField]
        private Vector2 size;

        public Vector2 Size
        {
            get
            {
                return size;
            }
        }
    }

    [Serializable]
    public class ScoreItemTextControlSettings : DisplaceableEntityObjectSettings, IRectTransformableControlSettings<ScoreItemTextControlRectTransformSettings>
    {
        [SerializeField]
        private ScoreItemTextControlRectTransformSettings rectTransformSettings;

        [SerializeField]
        private ScoreItemCategoryTextControlInformationSettings informationSettings;

        public ScoreItemTextControlRectTransformSettings RectTransformSettings
        {
            get
            {
                return rectTransformSettings;
            }
        }

        public ScoreItemCategoryTextControlInformationSettings InformationSettings
        {
            get
            {
                return informationSettings;
            }
        }
    }
}