using MenuScene.Behaviours.TextControl;
using MenuScene.Behaviours.TextControl.Interfaces;
using UnityEngine.UI;

namespace MenuScene.Behaviours.TextDropdown
{
    public abstract class BaseTextDropdownBehaviour : BaseSelectableTextControlBehaviour<Dropdown, int>, IAdmittableSelectableTextControlBehaviour<int>
    {
        protected override int Value
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

        protected abstract void AddOptions();

        protected override void SetupControl()
        {
            AddOptions();
            base.SetupControl();
        }

        public abstract void ProcessValueAdmitted(int value);
    }
}