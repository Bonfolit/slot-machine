using System;
using System.Collections.Generic;
using Core.Data;
using DG.Tweening;
using UnityEngine;

namespace Core.Config
{

    [CreateAssetMenu(fileName = "ColumnAnimationConfig", menuName = "Config/Column Animation Config", order = 0)]
    public class ColumnAnimationConfig : ScriptableObject
    {
        public List<ColumnAnimation> Animations;

        public int StartOffsetPerColumnInMs;

        public ColumnAnimationData GetAnimationData(ColumnAnimationType type)
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
    public struct ColumnAnimation
    {
        public ColumnAnimationType Type;
        public ColumnAnimationData Data;
    }

    public enum ColumnAnimationType
    {
        Quick,
        Normal,
        Slow
    }

}