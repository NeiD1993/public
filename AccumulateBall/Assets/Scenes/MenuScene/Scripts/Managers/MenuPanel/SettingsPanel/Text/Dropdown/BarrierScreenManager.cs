using System;
using MenuScene.Managers.TextDropdown.Info;
using MenuScene.Managers.TextDropdown.Settings;
using UnityEngine;
using UnityEngine.UI;

namespace MenuScene.Managers.TextDropdown
{
    class BarrierScreenManager : MonoBehaviour
    {
        [SerializeField]
        private BarrierScreenSettings screenSettings;

        public BarrierScreenManager()
        {
            ScreenInstance = null;
            ScreenSetupInfo = null;
        }

        private GameObject ScreenInstance { get; set; }

        public BarrierScreenSetupInfo? ScreenSetupInfo { get; set; }

        private void OnDisable()
        {
            Destroy(ScreenInstance);
        }

        private void OnEnable()
        {
            void RefreshContentTextSample(bool isReset = false)
            {
                string ObtainMessage()
                {
                    BarrierMessageSettings messageSettings = screenSettings.MessageSettings;

                    return string.Concat(messageSettings.Prefix, ScreenSetupInfo.Value.MessageRoot, messageSettings.Postfix);
                }

                screenSettings.SamplesSettings.ContentText.text = isReset ? string.Empty : ObtainMessage();
            }

            RefreshContentTextSample();
            ScreenInstance = Instantiate(screenSettings.SamplesSettings.Screen);
            RefreshContentTextSample(true);
            ScreenInstance.name = screenSettings.InstanceName;
            ScreenInstance.transform.SetParent(ScreenSetupInfo.Value.Parent.transform, false);
            ScreenInstance.SetActive(true);
        }
    }
}

namespace MenuScene.Managers.TextDropdown.Info
{
    public struct BarrierScreenSetupInfo
    {
        public BarrierScreenSetupInfo(string messageRoot, GameObject parent)
        {
            MessageRoot = messageRoot;
            Parent = parent;
        }

        public string MessageRoot { get; private set; }

        public GameObject Parent { get; private set; }
    }
}

namespace MenuScene.Managers.TextDropdown.Settings
{
    [Serializable]
    public struct BarrierMessageSettings
    {
        [SerializeField]
        private string postfix;

        [SerializeField]
        private string prefix;

        public string Postfix
        {
            get
            {
                return postfix;
            }
        }

        public string Prefix
        {
            get
            {
                return prefix;
            }
        }
    }

    [Serializable]
    public struct BarrierScreenSamplesSettings
    {
        [SerializeField]
        private GameObject screen;

        [SerializeField]
        private Text contentText;

        public GameObject Screen
        {
            get
            {
                return screen;
            }
        }

        public Text ContentText
        {
            get
            {
                return contentText;
            }
        }
    }

    [Serializable]
    public struct BarrierScreenSettings
    {
        [SerializeField]
        private BarrierMessageSettings messageSettings;

        [SerializeField]
        private BarrierScreenSamplesSettings samplesSettings;

        [SerializeField]
        private string instanceName;

        public BarrierMessageSettings MessageSettings
        {
            get
            {
                return messageSettings;
            }
        }

        public BarrierScreenSamplesSettings SamplesSettings
        {
            get
            {
                return samplesSettings;
            }
        }

        public string InstanceName
        {
            get
            {
                return instanceName;
            }
        }
    }
}