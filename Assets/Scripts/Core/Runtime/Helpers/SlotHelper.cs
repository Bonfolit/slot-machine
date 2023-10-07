using Core.Config;
using Core.Runtime.Gameplay.Slot;

namespace Core.Runtime.Helpers
{

    public static class SlotHelper
    {
        public static float CalculateVerticalOffset(int slotIndex, SlotConfig config)
        {
            return (slotIndex - config.MarkerIndex) * config.VerticalOffset;
            // Start from the topmost element's position, then subtract from it as we go up in index
            return ((((float)config.ColumnSize - 1) / 2) - slotIndex) * config.VerticalOffset;
        }
    }

}