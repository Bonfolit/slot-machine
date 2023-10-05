using System.Collections.Generic;
using UnityEngine;

namespace Core.Runtime.Gameplay.Slot
{
    [CreateAssetMenu(fileName = "SlotCombinationTable", menuName = "Slot Combination Table", order = 0)]
    public class SlotCombinationTable : ScriptableObject
    {
        [SerializeField]
        public List<SlotCombinationProbability> SlotCombinations;

        public int GetSlotCombinationIndex(in SlotCombination combination)
        {
            for (var i = 0; i < SlotCombinations.Count; i++)
            {
                if (SlotCombinations[i].Combination.Equals(combination))
                {
                    return i;
                }
            }

            return -1;
        }
    }
}