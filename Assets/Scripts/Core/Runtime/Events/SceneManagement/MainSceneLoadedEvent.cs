using BonLib.Events;

namespace Core.Runtime.Events.SceneManagement
{

    public struct MainSceneLoadedEvent : IEvent
    {
        public bool IsConsumed { get; set; }
    }

}