using System.Collections.Generic;
using Core.Runtime.Gameplay.Slot;
using UnityEngine;

namespace Core.Runtime.Data
{

    [CreateAssetMenu(fileName = "SlotSpriteContainer", menuName = "Data/Slot Sprite Container", order = 0)]
    public class SlotSpriteContainer : ScriptableObject
    {
        public List<SlotSpritePair> SlotSpritePairs;

        public Sprite GetSprite(SlotType type)
        {
            for (var i = 0; i < SlotSpritePairs.Count; i++)
            {
                if (SlotSpritePairs[i].Type == type)
                {
                    return SlotSpritePairs[i].Sprite;
                }
            }

            return null;
        }
    }

    [System.Serializable]
    public struct SlotSpritePair
    {
        public SlotType Type;
        public Sprite Sprite;
    }

}