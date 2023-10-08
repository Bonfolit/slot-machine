using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;

namespace Core.Data
{
    [CreateAssetMenu(fileName = "SlotAnimationData", menuName = "Data/Slot Animation Data", order = 0)]
    public class SlotAnimationData : ScriptableObject
    {
        public int InitialSlideCount;
        public float InitialDuration;
        public Ease InitialEase;

        [MinMaxSlider(2f, 20f)]
        public Vector2Int LoopSpinRange;
        public float LoopDuration; 
        public Ease LoopEase;
        
        public int StopSlideCount;
        public float StopDuration;
        public Ease StopEase;
    }
}