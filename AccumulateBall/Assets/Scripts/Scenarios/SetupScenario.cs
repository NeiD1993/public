using ServicesLocators;
using Settings;
using UnityEngine;

namespace Scanarios
{
    public static class SetupScenario
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Setup()
        {
            Application.targetFrameRate = PlayerPrefs.GetInt(nameof(Application.targetFrameRate), TargetFrameRateSettings.Average);
            SharedSceneServicesLocator.Setup();
        }
    }
}