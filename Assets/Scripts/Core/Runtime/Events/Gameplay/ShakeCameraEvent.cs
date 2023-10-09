using BonLib.Events;

namespace Core.Runtime.Events.Gameplay
{

    public struct ShakeCameraEvent : IEvent
    {
        public bool IsConsumed { get; set; }
    }

}