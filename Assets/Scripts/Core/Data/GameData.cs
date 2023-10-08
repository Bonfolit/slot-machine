using Core.Runtime.Gameplay.Slot;
using UnityEngine;

namespace Core.Data
{
    [CreateAssetMenu(fileName = "GameData", menuName = "Data/Game Data", order = 0)]
    public class GameData : ScriptableObject
    {
        public SlotCombination LastCombination;

        public SlotCombination[] NextCombinations;
    }

}