using BonLib.Events;
using Core.Runtime.Gameplay.Slot;

namespace Core.Runtime.Events.Gameplay
{

    public struct CombinationSelectedEvent : IEvent
    {
        public bool IsConsumed { get; set; }

        public SlotCombination Combination;

        public CombinationSelectedEvent(SlotCombination combination) : this()
        {
            Combination = combination;
        }
    }

}