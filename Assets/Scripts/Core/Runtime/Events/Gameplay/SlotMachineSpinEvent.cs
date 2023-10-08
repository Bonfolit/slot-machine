using BonLib.Events;

namespace Core.Runtime.Events.Gameplay
{

    public struct SlotMachineSpinEvent : IEvent
    {
        public bool IsConsumed { get; set; }
    }

}