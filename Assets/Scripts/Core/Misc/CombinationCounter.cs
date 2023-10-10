using System.Runtime.InteropServices;

namespace Core.Misc
{

    [StructLayout(LayoutKind.Sequential, Pack=1)]
    public struct CombinationCounter
    {
        public int[] BlockCounters;

        public void AddCounter(int blockIndex, int amount)
        {
            BlockCounters[blockIndex] += amount;
        }

        public void Reset()
        {
            for (var i = 0; i < BlockCounters.Length; i++)
            {
                BlockCounters[i] = 0;
            }
        }
    }

}