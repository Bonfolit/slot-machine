using BonLib.Events;
using Core.Runtime.Gameplay.Slot;

namespace Core.Runtime.Events.Gameplay
{

    public struct SlotMachineSpinEvent : IEvent
    {
        public bool IsConsumed { get; set; }
        
        public SlotCombination Combination;

        public SlotMachineSpinEvent(SlotCombination combination) : this()
        {
            Combination = combination;
        }
    }

}