using MenuScene.Behaviours.TextSlider;
using UnityEngine;
using UnityEngine.UI;

namespace MenuScene.Components.TextUI
{
    [AddComponentMenu("UI/Slider - Text")]
    public class TextSlider : Slider
    {
        private Text Text { get; set; }

        private void ChangeText(float value)
        {
            Text.text = value.ToString();
        }

        protected override void Start()
        {
            void SetupText()
            {
                Text = gameObject.GetComponentInChildren<Text>();
                ChangeText(value);
            }

            base.Start();
            SetupText();
            onValueChanged.AddListener(OnValueChanged);
        }

        private void OnValueChanged(float value)
        {
            if (Application.isPlaying)
                GetComponent<BaseTextSliderBehaviour>().ProcessValueAdmitted();

            ChangeText(value);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            onValueChanged.RemoveListener(OnValueChanged);
        }
    }
}