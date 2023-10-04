using BonLib.Events;

namespace Core.Runtime.Events.SceneManagement
{

    public struct SceneLoadedEvent : IEvent
    {
        public bool IsConsumed { get; set; }

        public int SceneIndex;

        public SceneLoadedEvent(int sceneIndex) : this()
        {
            SceneIndex = sceneIndex;
        }
    }

}