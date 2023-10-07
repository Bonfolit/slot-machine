using System;
using System.Collections.Generic;
using Core.Data;
using DG.Tweening;
using UnityEngine;

namespace Core.Config
{

    [CreateAssetMenu(fileName = "SlotAnimationConfig", menuName = "Config/Slot Animation Config", order = 0)]
    public class SlotAnimationConfig : ScriptableObject
    {
        public List<SlotAnimation> Animations;

        public float StopOffsetPerColumn;

        public SlotAnimationData GetAnimationData(SlotAnimationType type)
        {
            foreach (var slotAnimation in Animations)
            {
                if (type.Equals(slotAnimation.Type))
                {
                    return slotAnimation.Data;
                }
            }

            throw new Exception($"Could not find animation data for {type}");
        }
    }

    [System.Serializable]
    public struct SlotAnimation
    {
        public SlotAnimationType Type;
        public SlotAnimationData Data;
    }

    public enum SlotAnimationType
    {
        Quick,
        Normal,
        Slow
    }

}