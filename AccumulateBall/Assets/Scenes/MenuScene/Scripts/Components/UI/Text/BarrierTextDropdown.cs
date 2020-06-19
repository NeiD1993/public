using MenuScene.Behaviours.TextDropdown;
using MenuScene.Components.UI;
using MenuScene.Managers.TextDropdown;
using MenuScene.Managers.TextDropdown.Info;
using UnityEngine;
using UnityEngine.UI;

namespace MenuScene.Components.TextUI
{
    [AddComponentMenu("UI/Dropdown - Barrier Text")]
    public class BarrierTextDropdown : Dropdown
    {
        private BarrierScreenManager barrierScreenManager;

        public BarrierTextDropdown()
        {
            barrierScreenManager = null;
            ResetClickedValue();
            DropdownListCanvas = null;
            Blocker = null;
        }

        private int ClickedValue { get; set; }

        private BarrierScreenManager BarrierScreenManager
        {
            get
            {
                if (barrierScreenManager == null)
                    barrierScreenManager = gameObject.GetComponent<BarrierScreenManager>();

                return barrierScreenManager;
            }
        }

        private Canvas DropdownListCanvas { get; set; }

        private GameObject Blocker { get; set; }

        private void ProcessItemClicked(GameObject item)
        {
            int itemIndex = item.transform.GetSiblingIndex() - 1;

            if (itemIndex != value)
            {
                Blocker.GetComponent<Button>().enabled = false;
                Blocker.GetComponent<Canvas>().sortingOrder = DropdownListCanvas.sortingOrder + 1;

                ClickedValue = itemIndex;
                SetupBarrierScreenManager(new BarrierScreenSetupInfo(options[ClickedValue].text, Blocker), true);
            }
        }

        private void ResetClickedValue()
        {
            ClickedValue = -1;
        }

        private void SetupBarrierScreenManager(BarrierScreenSetupInfo? screenSetupInfo = null, bool enabled = false)
        {
            BarrierScreenManager.ScreenSetupInfo = screenSetupInfo;
            BarrierScreenManager.enabled = enabled;
        }

        protected override void DestroyItem(DropdownItem item)
        {
            base.DestroyItem(item);
            ((NotifiedlyClickableToggle)item.toggle).Clicked.RemoveListener(ProcessItemClicked);
        }

        protected override DropdownItem CreateItem(DropdownItem itemTemplate)
        {
            DropdownItem item = base.CreateItem(itemTemplate);

            ((NotifiedlyClickableToggle)item.toggle).Clicked.AddListener(ProcessItemClicked);

            return item;
        }

        protected override GameObject CreateBlocker(Canvas rootCanvas)
        {
            return Blocker = base.CreateBlocker(rootCanvas);
        }

        protected override GameObject CreateDropdownList(GameObject template)
        {
            GameObject dropdownList = base.CreateDropdownList(template);

            DropdownListCanvas = dropdownList.GetComponent<Canvas>();

            return dropdownList;
        }

        public void AdmitClickedValue()
        {
            GetComponent<BaseTextDropdownBehaviour>().ProcessValueAdmitted(ClickedValue);
        }

        public new void Hide()
        {
            base.Hide();
            ResetClickedValue();
            SetupBarrierScreenManager();
        }
    }
}