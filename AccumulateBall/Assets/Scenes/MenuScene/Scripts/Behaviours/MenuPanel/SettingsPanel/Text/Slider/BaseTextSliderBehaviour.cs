using MenuScene.Behaviours.TextControl;
using MenuScene.Behaviours.TextControl.Interfaces;
using UnityEngine.UI;

namespace MenuScene.Behaviours.TextSlider
{
    public abstract class BaseTextSliderBehaviour : BaseSelectableTextControlBehaviour<Slider, float>, IAdmittableSelectableTextControlBehaviour
    {
        public BaseTextSliderBehaviour()
        {
            AreBoundaryValuesSet = false;
        }

        private bool AreBoundaryValuesSet { get; set; }

        protected override float Value
        {
            get
            {
                return Control.value;
            }

            set
            {
                Control.value = value;
            }
        }

        private void SetBoundaryValues()
        {
            SetBoundaryValuesInternally();
            AreBoundaryValuesSet = true;
        }

        protected abstract void ProcessValueAdmittedInternally();

        protected abstract void SetBoundaryValuesInternally();

        protected override void SetupControl()
        {
            SetBoundaryValues();
            base.SetupControl();
        }

        public void ProcessValueAdmitted()
        {
            if (AreBoundaryValuesSet)
                ProcessValueAdmittedInternally();
        }
    }
}