using Core.Config;
using Core.Misc;
using Core.Runtime.Gameplay.Slot;
using UnityEngine;

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

        public static CombinationCounter[] GetCombinationCounters(in SlotCombination[] combinations,
            SlotCombinationTable table)
        {
            var rowCount = combinations.Length;
            
            var totalCombinationCount = table.SlotCombinations.Count;

            var counters = new CombinationCounter[totalCombinationCount];
            var blockWidths = new float[totalCombinationCount];

            for (var i = 0; i < totalCombinationCount; i++)
            {
                var blockWidth = ((float)rowCount / 100f) / table.SlotCombinations[i].Probability;
                blockWidths[i] = blockWidth;
            }
            
            for (int i = 0; i < rowCount; i++)
            {
                var combinationIndex = combinations[i].GetCombinationIndexFrom(table);
                var width = blockWidths[combinationIndex];
                var blockIndex = (int)((float)i / width);

                if (counters[combinationIndex].BlockCounters == null)
                {
                    counters[combinationIndex].BlockCounters = new int[Mathf.RoundToInt(table.SlotCombinations[combinationIndex].Probability * 100f)];
                }
                
                counters[combinationIndex].AddCounter(blockIndex, 1);
            }

            return counters;
        }
        
        public static CombinationCounter[] GetCombinationCounters(in int[] combinationIndices,
            SlotCombinationTable table)
        {
            var rowCount = combinationIndices.Length;
            
            var totalCombinationCount = table.SlotCombinations.Count;

            var counters = new CombinationCounter[totalCombinationCount];
            var blockWidths = new float[totalCombinationCount];

            for (var i = 0; i < totalCombinationCount; i++)
            {
                var blockWidth = ((float)rowCount / 100f) / table.SlotCombinations[i].Probability;
                blockWidths[i] = blockWidth;
            }
            
            for (int i = 0; i < rowCount; i++)
            {
                var combinationIndex = combinationIndices[i];
                var width = blockWidths[combinationIndex];
                var blockIndex = (int)((float)i / width);

                if (counters[combinationIndex].BlockCounters == null)
                {
                    counters[combinationIndex].BlockCounters = new int[Mathf.RoundToInt(table.SlotCombinations[combinationIndex].Probability * 100f)];
                }
                
                counters[combinationIndex].AddCounter(blockIndex, 1);
            }

            return counters;
        }


        private static int GetCombinationIndexFrom(this SlotCombination combination, SlotCombinationTable table)
        {
            return table.GetSlotCombinationIndex(in combination);
        }
    }
}