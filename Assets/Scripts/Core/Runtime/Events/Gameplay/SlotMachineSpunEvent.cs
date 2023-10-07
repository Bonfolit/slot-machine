using BonLib.Events;

namespace Core.Runtime.Events.Gameplay
{

    public struct SlotMachineSpunEvent : IEvent
    {
        public bool IsConsumed { get; set; }
    }

}