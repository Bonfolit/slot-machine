using Core.Config;
using Core.Runtime.Gameplay.Slot;

namespace Core.Helpers
{
    public static class SlotHelper
    {
        public static float CalculateVerticalOffset(int slotIndex, SlotConfig config)
        {
            return (slotIndex - config.MarkerIndex) * config.VerticalOffset;
        }

        public static bool IsMatch(this SlotCombination combination)
        {
            var isMatch = true;
            var slotType = combination.SlotTypes[0];
            for (var i = 1; i < combination.SlotTypes.Length; i++)
            {
                if (!slotType.Equals(combination.SlotTypes[i]))
                {
                    isMatch = false;
                    break;
                }
            }

            return isMatch;
        }
    }
}