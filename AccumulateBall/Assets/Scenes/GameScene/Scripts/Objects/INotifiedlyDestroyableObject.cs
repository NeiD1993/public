using UnityEngine.Events;

namespace GameScene.Objects.Interfaces
{
    public interface INotifiedlyDestroyableObject
    {
        UnityEvent Destroyed { get; }
    }
}