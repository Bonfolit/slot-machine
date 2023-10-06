using Core.Config;
using Core.Runtime.Gameplay.Slot;

namespace Core.Runtime.Helpers
{

    public static class SlotHelper
    {
        public static float CalculateVerticalOffset(int slotIndex, SlotLayoutConfig layoutConfig)
        {
            return (slotIndex - layoutConfig.MarkerIndex) * layoutConfig.VerticalOffset;
            // Start from the topmost element's position, then subtract from it as we go up in index
            return ((((float)layoutConfig.ColumnSize - 1) / 2) - slotIndex) * layoutConfig.VerticalOffset;
        }
    }

}