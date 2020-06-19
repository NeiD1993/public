using MenuScene.Components.UI.Interfaces.Events;
using UnityEngine;
using UnityEngine.Events;

namespace MenuScene.Components.UI.Interfaces
{
    public interface INotifiedlyClickableUIComponent
    {
        UIComponentClickedEvent Clicked { get; }
    }
}

namespace MenuScene.Components.UI.Interfaces.Events
{
    public class UIComponentClickedEvent : UnityEvent<GameObject> { }
}