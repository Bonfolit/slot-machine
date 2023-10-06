using BonLib.Pooling;
using Core.Runtime.Data;
using Core.Runtime.Managers;
using UnityEngine;

namespace Core.Runtime.Gameplay.Slot
{
    public class SlotColumn : MonoBehaviour
    {
        private Slot[] m_slots;
        public void Initialize(SlotSpriteContainer container, SlotLayoutConfig layoutConfig, PoolObject spriteRendererPoolObject)
        {
            m_slots = new[] { new Slot(), new Slot(), new Slot(), new Slot(), new Slot() };

            for (var i = 0; i < m_slots.Length; i++)
            {
                m_slots[i].Initialize(transform, (SlotType)i, layoutConfig, container, spriteRendererPoolObject);

                var localPos = new Vector3(0f, -i * layoutConfig.VerticalOffset, 0f);
                m_slots[i].SetPosition(localPos);
            }
        }
    }

}