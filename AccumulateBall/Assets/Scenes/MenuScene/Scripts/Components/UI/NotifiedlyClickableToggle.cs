using MenuScene.Components.UI.Interfaces;
using MenuScene.Components.UI.Interfaces.Events;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MenuScene.Components.UI
{
    [AddComponentMenu("UI/Toggle - Notifiedly Clickable")]
    public class NotifiedlyClickableToggle : Toggle, INotifiedlyClickableUIComponent
    {
        public NotifiedlyClickableToggle()
        {
            Clicked = new UIComponentClickedEvent();
        }

        public UIComponentClickedEvent Clicked { get; private set; }

        public override void OnPointerClick(PointerEventData eventData)
        {
            Clicked.Invoke(gameObject);
        }
    }
}