using UnityEngine;
using UnityEngine.Serialization;

namespace Core.Config
{

    [CreateAssetMenu(fileName = "SlotConfig", menuName = "Config/Slot Config", order = 0)]
    public class SlotConfig : ScriptableObject
    {
        [FormerlySerializedAs("ColumnHeight")] public int ColumnSize;
        public Vector2 SlotDimensions;
        public float VerticalOffset;
        public int MarkerIndex;
        public int CombinationBufferAmount;

        public float ColumnTotalHeight => (float)ColumnSize * VerticalOffset;
    }

}