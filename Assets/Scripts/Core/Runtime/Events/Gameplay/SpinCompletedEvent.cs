using BonLib.Events;
using Core.Runtime.Gameplay.Slot;

namespace Core.Runtime.Events.Gameplay
{

    public struct SpinCompletedEvent : IEvent
    {
        public bool IsConsumed { get; set; }

        public SlotCombination CurrentCombination;

        public SpinCompletedEvent(SlotCombination currentCombination) : this()
        {
            CurrentCombination = currentCombination;
        }
    }

}