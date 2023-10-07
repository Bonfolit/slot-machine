using Core.Config;

namespace Core.Helpers
{
    public static class SlotHelper
    {
        public static float CalculateVerticalOffset(int slotIndex, SlotConfig config)
        {
            return (slotIndex - config.MarkerIndex) * config.VerticalOffset;
        }
    }
}