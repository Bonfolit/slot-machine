using Core.Runtime.Gameplay.Slot;
using UnityEngine;
using UnityEngine.Serialization;

namespace Core.Data
{
    [CreateAssetMenu(fileName = "GameData", menuName = "Data/Game Data", order = 0)]
    public class GameData : ScriptableObject
    {
        public int CombinationIndex;
        public SlotCombination LastCombination;

        public SlotCombination[] Combinations;

        public bool ReachedFinalCombination()
        {
            return CombinationIndex == Combinations.Length - 1;
        }

        public SlotCombination GetNextCombination()
        {
            return LastCombination = Combinations[++CombinationIndex];
        }
    }

}