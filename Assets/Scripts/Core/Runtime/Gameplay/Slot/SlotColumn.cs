﻿using BonLib.Pooling;
using Core.Config;
using Core.Data;
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
        public void Initialize(SlotSpriteContainer container, SlotConfig config, PoolObject spriteRendererPoolObject, float columnTotalHeight)
        {
            m_columnTotalHeight = columnTotalHeight;

            for (int i = 0; i < config.ColumnSize; i++)
            {
                m_slots[i].Initialize(transform, (SlotType)i, config, container);
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

        public void BlurSlots(float duration)
        {
            Debug.LogWarning("BLUR");
            for (var i = 0; i < m_slots.Length; i++)
            {
                m_slots[i].Blur(duration);
            }
        }
        
        public void UnblurSlots(float duration)
        {
            Debug.LogWarning("UNBLUR");

            for (var i = 0; i < m_slots.Length; i++)
            {
                m_slots[i].Unblur(duration);
            }
        }
    }

}