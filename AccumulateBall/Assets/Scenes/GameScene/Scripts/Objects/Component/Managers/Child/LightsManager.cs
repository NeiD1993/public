using System;
using System.Collections;
using GameScene.Managers.Entity;
using GameScene.Managers.Entity.Settings;
using GameScene.Managers.Entity.Settings.Interfaces;
using GameScene.Managers.Light.Enums;
using GameScene.Managers.Light.Settings;
using UnityEngine;

namespace GameScene.Managers.Light
{
    public class LightsManager : BaseEntityManager<LightSettings>
    {
        private GameObject lights;

        public override GameObject Entity
        {
            get
            {
                return lights;
            }
        }

        private void SpawnLight(LightInfoSettings lightInfoSettings, int fieldDimension)
        {
            GameObject light = Instantiate(entityObjectSettings.Prefab, GetLightDistanceTransform(fieldDimension, lightInfoSettings.Transform), Quaternion.identity);

            light.name = string.Concat(lightInfoSettings.NamePrefix, entityObjectSettings.InstanceName);

            AttachToEntity(light);

            light.transform.rotation = Quaternion.Euler(lightInfoSettings.Rotation);
        }

        private Vector3 GetLightDistanceTransform(int fieldDimension, Vector3 transform)
        {
            return fieldDimension * entityObjectSettings.Distance * transform;
        }

        public IEnumerator DespawnLightsIteratively()
        {
            GameObject light;

            while (Entity.transform.childCount > 0)
            {
                light = Entity.transform.GetChild(0).gameObject;
                Destroy(light);

                yield return new WaitUntil(() => light == null);

                if (Entity.transform.childCount > 0)
                    yield return new WaitForSeconds(entityObjectSettings.Delays.Despawning);
            }

            Destroy(lights);
        }

        public IEnumerator SpawnLightsIteratively(int fieldDimension)
        {
            int lightCategoryIndex = 0;
            Array lightCategories = Enum.GetValues(typeof(LightCategory));

            lights = new GameObject(entityObjectSettings.OwnerInstanceName);

            foreach(LightCategory lightCategory in lightCategories)
            {
                SpawnLight(entityObjectSettings.InfoSettings.GetSettings(lightCategory), fieldDimension);

                if (lightCategoryIndex++ < lightCategories.Length - 1)
                    yield return new WaitForSeconds(entityObjectSettings.Delays.Spawning);
            }
        }
    }
}

namespace GameScene.Managers.Light.Enums
{
    public enum LightCategory
    {
        NorthEast,

        NorthWest,

        SouthEast,

        SouthWest
    }
}

namespace GameScene.Managers.Light.Settings
{
    [Serializable]
    public struct LightCategoryInfoSettings : IUnitedlyGettableEntityCategorySettings<LightInfoSettings, LightCategory>
    {
        [SerializeField]
        private LightInfoSettings northEastLight;

        [SerializeField]
        private LightInfoSettings northWestLight;

        [SerializeField]
        private LightInfoSettings southEastLight;

        [SerializeField]
        private LightInfoSettings southWestLight;

        public LightInfoSettings GetSettings(LightCategory category)
        {
            switch (category)
            {
                case LightCategory.NorthEast:
                    return northEastLight;
                case LightCategory.NorthWest:
                    return northWestLight;
                case LightCategory.SouthEast:
                    return southEastLight;
                default:
                    return southWestLight;
            }
        }
    }

    [Serializable]
    public struct LightDelaysSettings
    {
        [SerializeField]
        private float despawning;

        [SerializeField]
        private float spawning;

        public float Despawning
        {
            get
            {
                return despawning;
            }
        }

        public float Spawning
        {
            get
            {
                return spawning;
            }
        }
    }

    [Serializable]
    public struct LightInfoSettings
    {
        [SerializeField]
        private string namePrefix;

        [SerializeField]
        private Vector3 transform;

        [SerializeField]
        private Vector3 rotation;

        public string NamePrefix
        {
            get
            {
                return namePrefix;
            }
        }

        public Vector3 Transform
        {
            get
            {
                return transform;
            }
        }

        public Vector3 Rotation
        {
            get
            {
                return rotation;
            }
        }
    }

    [Serializable]
    public class LightSettings : OwnedEntityObjectSettings, IDelayedEntityObjectSettings<LightDelaysSettings>
    {
        [SerializeField]
        private float distance;

        [SerializeField]
        private LightCategoryInfoSettings infoSettings;

        [SerializeField]
        private LightDelaysSettings delays;

        [SerializeField]
        private GameObject prefab;

        public float Distance
        {
            get
            {
                return distance;
            }
        }

        public LightCategoryInfoSettings InfoSettings
        {
            get
            {
                return infoSettings;
            }
        }

        public LightDelaysSettings Delays
        {
            get
            {
                return delays;
            }
        }

        public GameObject Prefab
        {
            get
            {
                return prefab;
            }
        }
    }
}