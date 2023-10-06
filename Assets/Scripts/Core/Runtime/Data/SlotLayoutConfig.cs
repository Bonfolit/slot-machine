using Core.Runtime.Gameplay.Slot;
using UnityEngine;

namespace Core.Runtime.Data
{

    [CreateAssetMenu(fileName = "SlotLayoutConfig", menuName = "Config/Slot Layout Config", order = 0)]
    public class SlotLayoutConfig : ScriptableObject
    {
        public Vector2 SlotDimensions;
        public float VerticalOffset;
    }

}