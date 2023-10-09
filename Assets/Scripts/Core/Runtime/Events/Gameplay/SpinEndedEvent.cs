using BonLib.Events;
using Core.Runtime.Gameplay.Slot;

namespace Core.Runtime.Events.Gameplay
{

    public struct SpinEndedEvent : IEvent
    {
        public bool IsConsumed { get; set; }

        public SlotCombination Combination;

        public SpinEndedEvent(SlotCombination combination) : this()
        {
            Combination = combination;
        }
    }

}