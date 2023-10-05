using System.Runtime.InteropServices;

namespace Core.Runtime.Gameplay.Slot
{
    [System.Serializable]
    [StructLayout(LayoutKind.Sequential, Pack=1)]
    public struct SlotCombinationProbability
    {
        public SlotCombination Combination;
        public float Probability;
    }

}