using BonLib.Events;

namespace Core.Runtime.Events.Gameplay
{

    public struct SlotMachineStopEvent : IEvent
    {
        public bool IsConsumed { get; set; }
    }

}