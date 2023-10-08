using System.Collections.Generic;
using Core.Runtime.Gameplay.Slot;
using UnityEngine;
using UnityEngine.Serialization;

namespace Core.Data
{

    [CreateAssetMenu(fileName = "SlotSpriteContainer", menuName = "Data/Slot Sprite Container", order = 0)]
    public class SlotSpriteContainer : ScriptableObject
    {
        public List<SlotSprites> SlotSpritePairs;

        public SlotSprites GetSprites(SlotType type)
        {
            for (var i = 0; i < SlotSpritePairs.Count; i++)
            {
                if (SlotSpritePairs[i].Type == type)
                {
                    return SlotSpritePairs[i];
                }
            }

            return default;
        }
    }

    [System.Serializable]
    public struct SlotSprites
    {
        public SlotType Type;
        public Sprite Sprite;
        public Texture BlurTexture;
    }

}