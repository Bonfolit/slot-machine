using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;

namespace Core.Data
{
    [CreateAssetMenu(fileName = "ColumnAnimationData", menuName = "Data/Column Animation Data", order = 0)]
    public class ColumnAnimationData : ScriptableObject
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