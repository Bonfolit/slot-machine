using Core.Runtime.Gameplay.Slot;
using UnityEngine;

namespace Core.Data
{
    [CreateAssetMenu(fileName = "PlayerData", menuName = "Data/Player Data", order = 0)]
    public class PlayerData : ScriptableObject
    {
        public SlotCombination LastCombination;

        public SlotCombination[] NextCombinations;
    }

}