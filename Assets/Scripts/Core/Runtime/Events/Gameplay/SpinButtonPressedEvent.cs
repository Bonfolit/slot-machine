using BonLib.Events;

namespace Core.Runtime.Events.Gameplay
{

    public struct SpinButtonPressedEvent : IEvent
    {
        public bool IsConsumed { get; set; }
    }

}