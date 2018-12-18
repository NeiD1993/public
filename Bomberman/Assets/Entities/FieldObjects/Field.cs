using Assets.Entities.FieldObjects.FieldObject;
using System;
using System.Collections.Generic;
using UnityEngine;
using Assets.Entities.FieldObjectsService.FieldGenerator.FieldStaticObjectsGenerator;
using Assets.Entities.FieldObjectsService.FieldObjectsMover;
using Assets.Entities.FieldObjectsService.FieldObjectsDestroyer;
using Assets.Entities.FieldObjects.FieldObject.FieldObjectsTypes;
using Assets.Entities.FieldObjectsService;
using Assets.Entities.FieldObjecsService.PlayerFieldObjectStatistics;

namespace Assets.Entities.FieldObjects
{
    class Field : ScriptableObject
    {
        private static readonly int maxHorizontalSize = 100;

        private static readonly int maxVerticalSize = 60;

        private static readonly int minHorizontalSize = 4;

        private static readonly int minVerticalSize = 4;

        private static readonly int minDestroyableActionDelay = 1;

        private static readonly int maxDestroyableActionDelay = 5;

        protected static readonly string explosionsGameObjectName = "Explosions";

        protected static readonly string fieldGameObjectName = "Field";

        protected static readonly string fieldGameObjectsName = "Field Game Objects";

        public static readonly string explosionWaveGameObjectName = "Explosion Wave";

        public static readonly string powerUpsGameObjectName = "Power Ups";

        private int horizontalSize;

        private int verticalSize;

        protected int destroyableActionDelay;

        protected int enemiesCount;

        protected BaseFieldObjectsDestroyer fieldObjectsDestroyer;

        protected BaseFieldDynamicObjectsGenerator fieldDynamicObjectsGenerator;

        protected BaseFieldStaticObjectsGenerator fieldStaticObjectsGenerator;

        protected BaseFieldDynamicObjectsMover fieldDynamicObjectsMover;

        protected FieldObjectsComponentsGetter fieldObjectsComponentsGetter;

        protected FieldObjectsStatistics fieldObjectsStatistics;

        protected GameObject explosionsGameObject;

        protected GameObject fieldGameObject;

        protected GameObject fieldGameObjects;

        protected GameObject floorGameObject;

        protected GameObject powerUpsGameObject;

        protected GameObject lightPrefab;

        protected GameObject resultsTextPrefab;

        protected GameObject textPrefab;

        protected GameObject wallPrefab;

        protected Material killEnemyTextMaterial;

        protected Nullable<Vector2> playerPosition;

        protected IList<Vector2> freePositions;

        protected IDictionary<Vector2, PowerUp> powerUps;

        protected IDictionary<string, Material> lightMaterials;

        protected IDictionary<string, AudioClip> fieldObjectsSounds;

        protected IDictionary<string, Material> resultsTextsMaterials;

        protected FieldObject.FieldObject[][] fieldObjects;

        protected IDictionary<Vector2, GameObject> bombGameObjects = new Dictionary<Vector2, GameObject>();

        public int HorizontalSize
        {
            get
            {
                return horizontalSize;
            }

            set
            {
                if (horizontalSize != value)
                {
                    horizontalSize = ((value >= minHorizontalSize) && (value <= maxHorizontalSize)) ? value : minHorizontalSize;
                    if (VerticalSize != 0)
                        Reset();
                }
            }
        }

        public int VerticalSize
        {
            get
            {
                return verticalSize;
            }

            set
            {
                if (verticalSize != value)
                {
                    verticalSize = ((value >= minVerticalSize) && (value <= maxVerticalSize)) ? value : minVerticalSize;
                    if (HorizontalSize != 0)
                        Reset();
                }
            }
        }

        public int DestroyableActionDelay
        {
            get
            {
                return destroyableActionDelay;
            }

            set
            {
                destroyableActionDelay = ((value >= minDestroyableActionDelay) && (value <= maxDestroyableActionDelay)) ? value : minDestroyableActionDelay;
            }
        }

        public int EnemiesCount
        {
            get
            {
                return enemiesCount;
            }

            set
            {
                if ((enemiesCount != value) && (value >= 0))
                    enemiesCount = value;
            }
        }

        public BaseFieldObjectsDestroyer FieldObjectsDestroyer
        {
            get
            {
                return fieldObjectsDestroyer;
            }

            set
            {
                if ((fieldObjectsDestroyer == null) && (value != null))
                {
                    fieldObjectsDestroyer = value;
                    fieldObjectsDestroyer.Field = this;
                }
            }
        }

        public BaseFieldDynamicObjectsGenerator FieldDynamicObjectsGenerator
        {
            get
            {
                return fieldDynamicObjectsGenerator;
            }

            set
            {
                if ((fieldDynamicObjectsGenerator == null) && (value != null))
                {
                    fieldDynamicObjectsGenerator = value;
                    fieldDynamicObjectsGenerator.Field = this;
                }
            }
        }

        public BaseFieldStaticObjectsGenerator FieldStaticObjectsGenerator
        {
            get
            {
                return fieldStaticObjectsGenerator;
            }

            set
            {
                if ((fieldStaticObjectsGenerator == null) && (value != null))
                {
                    fieldStaticObjectsGenerator = value;
                    fieldStaticObjectsGenerator.Field = this;
                }
            }
        }

        public BaseFieldDynamicObjectsMover FieldDynamicObjectsMover
        {
            get
            {
                return fieldDynamicObjectsMover;
            }

            set
            {
                if ((fieldDynamicObjectsMover == null) && (value != null))
                {
                    fieldDynamicObjectsMover = value;
                    fieldDynamicObjectsMover.Field = this;
                }
            }
        }

        public FieldObjectsComponentsGetter FieldObjectsComponentsGetter
        {
            get
            {
                return fieldObjectsComponentsGetter;
            }

            set
            {
                if ((fieldObjectsComponentsGetter == null) && (value != null))
                {
                    fieldObjectsComponentsGetter = value;
                    fieldObjectsComponentsGetter.Field = this;
                }
            }
        }

        public FieldObjectsStatistics FieldObjectsStatistics
        {
            get
            {
                return fieldObjectsStatistics;
            }

            set
            {
                if ((fieldObjectsStatistics == null) && (value != null))
                {
                    fieldObjectsStatistics = value;
                    fieldObjectsStatistics.Field = this;
                }
            }
        }

        public GameObject ExplosionsGameObject
        {
            get
            {
                if (explosionsGameObject == null)
                {
                    explosionsGameObject = new GameObject(explosionsGameObjectName);
                    explosionsGameObject.transform.parent = FieldGameObject.transform;
                }
                return explosionsGameObject;
            }
        }

        public GameObject FieldGameObject
        {
            get
            {
                if (fieldGameObject == null)
                    fieldGameObject = new GameObject(fieldGameObjectName);
                return fieldGameObject;
            }
        }

        public GameObject FieldGameObjects
        {
            get
            {
                if (fieldGameObjects == null)
                {
                    fieldGameObjects = new GameObject(fieldGameObjectsName);
                    fieldGameObjects.transform.parent = FieldGameObject.transform;
                }
                return fieldGameObjects;
            }
        }

        public GameObject FloorGameObject
        {
            get
            {
                return floorGameObject;
            }

            set
            {
                if ((floorGameObject != value) && (value != null))
                    floorGameObject = value;
            }
        }

        public GameObject PowerUpsGameObject
        {
            get
            {
                if (powerUpsGameObject == null)
                {
                    powerUpsGameObject = new GameObject(powerUpsGameObjectName);
                    powerUpsGameObject.transform.parent = FieldGameObject.transform;
                }
                return powerUpsGameObject;
            }
        }

        public GameObject LightPrefab
        {
            get
            {
                return lightPrefab;
            }

            set
            {
                if ((lightPrefab != value) && (value != null))
                    lightPrefab = value;
            }
        }

        public GameObject ResultsTextPrefab
        {
            get
            {
                return resultsTextPrefab;
            }

            set
            {
                if ((resultsTextPrefab != value) && (value != null))
                    resultsTextPrefab = value;
            }
        }

        public GameObject TextPrefab
        {
            get
            {
                return textPrefab;
            }

            set
            {
                if ((textPrefab != value) && (value != null))
                    textPrefab = value;
            }
        }

        public GameObject WallPrefab
        {
            get
            {
                return wallPrefab;
            }

            set
            {
                if ((wallPrefab != value) && (value != null))
                {
                    wallPrefab = value;
                    Reset();
                }
            }
        }

        public Material KillEnemyTextMaterial
        {
            get
            {
                return killEnemyTextMaterial;
            }

            set
            {
                if ((killEnemyTextMaterial != value) && (value != null))
                    killEnemyTextMaterial = value;
            }
        }

        public Nullable<Vector2> PlayerPosition
        {
            get
            {
                return playerPosition;
            }

            set
            {
                FieldObjectType fieldObjectType = fieldObjects[(int)value.Value.x][(int)value.Value.y].ObjectType;

                if ((fieldObjectType == FieldObjectType.Player) || (fieldObjectType == FieldObjectType.PlayerAndBreakableWall))
                    playerPosition = value;
            }
        }

        public IList<Vector2> FreePositions
        {
            get
            {
                if (freePositions == null)
                    freePositions = new List<Vector2>();
                return freePositions;
            }
        }

        public IDictionary<Vector2, PowerUp> PowerUps
        {
            get
            {
                if (powerUps == null)
                    powerUps = new Dictionary<Vector2, PowerUp>();
                return powerUps;
            }
        }

        public IDictionary<string, Material> LightMaterials
        {
            get
            {
                return lightMaterials;
            }

            set
            {
                if ((lightMaterials != value) && (value != null))
                    lightMaterials = value;
            }
        }

        public IDictionary<string, AudioClip> FieldObjectsSounds
        {
            get
            {
                return fieldObjectsSounds;
            }

            set
            {
                if ((fieldObjectsSounds != value) && (value != null))
                    fieldObjectsSounds = value;
            }
        }

        public IDictionary<string, Material> ResultsTextsMaterials
        {
            get
            {
                return resultsTextsMaterials;
            }

            set
            {
                if ((resultsTextsMaterials != value) && (value != null))
                    resultsTextsMaterials = value;
            }
        }

        public FieldObject.FieldObject[][] FieldObjects
        {
            get
            {
                return fieldObjects;
            }
        }

        protected void Reset()
        {
            fieldObjects = new FieldObject.FieldObject[HorizontalSize][];

            for (int i = 0; i < HorizontalSize; i++)
            {
                fieldObjects[i] = new FieldObject.FieldObject[VerticalSize];
                for (int j = 0; j < VerticalSize; j++)
                    fieldObjects[i][j] = new FieldObject.FieldObject(FieldObjectType.Empty, this, null);
            }

            if (FreePositions.Count > 0)
                FreePositions.Clear();
        }

        public void AddBombGameObject(int horizontalIndex, int verticalIndex, GameObject bombGameObject)
        {
            try
            {
                bombGameObjects.Add(new Vector2(horizontalIndex, verticalIndex), bombGameObject);
            }
            catch
            {
                return;
            }
        }

        public void ResetPlayerPosition()
        {
            if (PlayerPosition != null)
            {
                Vector2 playerPosition = PlayerPosition.Value;
                FieldObject.FieldObject playerFieldObject = FieldObjects[(int)playerPosition.x][(int)playerPosition.y];

                if (((playerFieldObject.ObjectType == FieldObjectType.Empty) && (playerFieldObject.GameObject == null)) ||
                    ((playerFieldObject.ObjectType == FieldObjectType.BreakableWall) && (playerFieldObject.GameObject.transform.childCount == 0)))
                    this.playerPosition = null;
            }
        }

        public bool IsBombGameObjectsContainsKey(int horizontalIndex, int verticalIndex)
        {
            return bombGameObjects.ContainsKey(new Vector2(horizontalIndex, verticalIndex));
        }

        public bool IsIndexesOutOfFieldIndexesRanges(int horizontalIndex, int verticalIndex)
        {
            return !(((horizontalIndex > 0) && (horizontalIndex < HorizontalSize)) && ((verticalIndex > 0) && (verticalIndex < VerticalSize)));
        }

        public int GetBombGameObjectsCount()
        {
            return bombGameObjects.Count;
        }

        public GameObject GetBombGameObject(Vector2 bombGameObjectIndexes)
        {
            if (IsBombGameObjectsContainsKey((int)bombGameObjectIndexes.x, (int)bombGameObjectIndexes.y))
                return bombGameObjects[bombGameObjectIndexes];

            return null;
        }

        public GameObject RemoveBombGameObject(int horizontalIndex, int verticalIndex)
        {
            GameObject bombGameObject = null;
            Vector2 fieldIndexes = new Vector2(horizontalIndex, verticalIndex);

            if (IsBombGameObjectsContainsKey(horizontalIndex, verticalIndex))
            {
                bombGameObject = bombGameObjects[fieldIndexes];
                bombGameObjects.Remove(new Vector2(horizontalIndex, verticalIndex));
            }

            return bombGameObject;
        }
    }
}
