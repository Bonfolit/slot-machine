using BonLib.Events;
using Core.Runtime.Gameplay.Slot;

namespace Core.Runtime.Events.Gameplay
{

    public struct SlotCombinationsUpdatedEvent : IEvent
    {
        public bool IsConsumed { get; set; }

        public SlotCombination[] SlotCombinations;

        public SlotCombinationsUpdatedEvent(SlotCombination[] slotCombinations) : this()
        {
            SlotCombinations = slotCombinations;
        }
    }

}