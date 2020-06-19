using System;
using System.Collections.Generic;
using Loaders.Enums;
using ServicesLocators.Attributes;
using UnityEngine.SceneManagement;

namespace ServicesLocators
{
    public static class SharedSceneServicesLocator
    {
        private static readonly SharedSceneServicesData servicesData;

        static SharedSceneServicesLocator()
        {
            servicesData = new SharedSceneServicesData();
        }

        public static void Setup()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.sceneUnloaded += OnSceneUnloaded;
        }

        public static T GetService<T>()
        {
            Type serviceType = typeof(T);

            servicesData.AddService(serviceType);

            return (T)servicesData[serviceType];
        }

        private static void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
        {
            servicesData.SceneKind = (SceneKind)Enum.Parse(typeof(SceneKind), scene.name);
        }

        private static void OnSceneUnloaded(Scene scene)
        {
            servicesData.SceneKind = null;
            servicesData.ClearServices();
        }

        private class SharedSceneServicesData
        {
            private IDictionary<Type, object> services;

            public SharedSceneServicesData()
            {
                SceneKind = null;
                services = new Dictionary<Type, object>();
            }

            public SceneKind? SceneKind { get; set; }

            public object this[Type serviceType]
            {
                get
                {
                    if (services.ContainsKey(serviceType))
                        return services[serviceType];
                    else
                        return null;
                }
            }

            public void AddService(Type serviceType)
            {
                SharedSceneService serviceAttribute;

                if (((serviceAttribute = (SharedSceneService)Attribute.GetCustomAttribute(serviceType, typeof(SharedSceneService))) != null) && 
                    serviceAttribute.SceneKind == SceneKind && !services.ContainsKey(serviceType))
                    services.Add(serviceType, Activator.CreateInstance(serviceType));
            }

            public void ClearServices()
            {
                services.Clear();
            }
        }
    }
}

namespace ServicesLocators.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class SharedSceneService : Attribute
    {
        public SharedSceneService(SceneKind sceneKind)
        {
            SceneKind = sceneKind;
        }

        public SceneKind SceneKind { get; private set; }
    }
}