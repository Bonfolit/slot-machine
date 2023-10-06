using UnityEngine;
using UnityEngine.Serialization;

namespace Core.Config
{

    [CreateAssetMenu(fileName = "SlotLayoutConfig", menuName = "Config/Slot Layout Config", order = 0)]
    public class SlotLayoutConfig : ScriptableObject
    {
        [FormerlySerializedAs("ColumnHeight")] public int ColumnSize;
        public Vector2 SlotDimensions;
        public float VerticalOffset;
        public int MarkerIndex;
        public float ModHeight;

        public float ColumnTotalHeight => (float)ColumnSize * VerticalOffset;
    }

}