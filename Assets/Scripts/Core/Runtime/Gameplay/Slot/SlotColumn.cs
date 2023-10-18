using Core.Config;
using UnityEngine;

namespace Core.Runtime.Gameplay.Slot
{
    public class SlotColumn : MonoBehaviour
    {
        [SerializeField]
        private float m_slideAmount;
        public float SlideAmount => m_slideAmount;

        private float m_columnTotalHeight;

        [SerializeField]
        private Slot[] m_slots;
        
        public void Initialize(SlotConfig config)
        {
            m_columnTotalHeight = config.ColumnTotalHeight;

            for (int i = 0; i < config.ColumnSize; i++)
            {
                m_slots[i].Initialize(config);
            }
        }

        public void SetSlide(float slideAmount)
        {
            m_slideAmount = slideAmount % m_columnTotalHeight;

            for (var i = 0; i < m_slots.Length; i++)
            {
                m_slots[i].SetSlide(m_slideAmount);
            }
        }

        public void SetBlur(bool setBlur)
        {
            for (var i = 0; i < m_slots.Length; i++)
            {
                m_slots[i].SetBlur(setBlur);
            }
        }
    }

}