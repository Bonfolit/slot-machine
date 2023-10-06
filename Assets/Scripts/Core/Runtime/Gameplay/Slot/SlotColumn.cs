using System;
using System.Collections.Generic;
using BonLib.DependencyInjection;
using BonLib.Events;
using BonLib.Pooling;
using Core.Config;
using Core.Runtime.Data;
using Core.Runtime.Events.Gameplay;
using Core.Runtime.Managers;
using UnityEngine;

namespace Core.Runtime.Gameplay.Slot
{
    public class SlotColumn : MonoBehaviour
    {
        private EventManager m_eventManager;
        
        private float m_slideAmount;

        public float speed;

        private LinkedList<Slot> m_slotLinkedList;
        [SerializeField]
        private Slot[] m_slots;
        public void Initialize(SlotSpriteContainer container, SlotLayoutConfig layoutConfig, PoolObject spriteRendererPoolObject)
        {
            m_eventManager = DI.Resolve<EventManager>();
            
            m_slots = new Slot[layoutConfig.ColumnSize];

            m_slotLinkedList = new LinkedList<Slot>();
            for (int i = 0; i < layoutConfig.ColumnSize; i++)
            {
                m_slots[i] = new Slot(i);

                if (i == 0)
                {
                    m_slotLinkedList.AddFirst(new Slot(i));
                }
                else
                {
                    m_slotLinkedList.AddLast(new Slot(i));
                }
            }

            for (var i = 0; i < m_slots.Length; i++)
            {
                m_slots[i].Initialize(transform, (SlotType)i, layoutConfig, container, spriteRendererPoolObject);

                var localPos = new Vector3(0f, -i * layoutConfig.VerticalOffset, 0f);
                // m_slots[i].SetPosition(localPos);
            }
        }

        public void SetSlide(float slideAmount)
        {
            m_slideAmount = slideAmount;

            for (var i = 0; i < m_slots.Length; i++)
            {
                m_slots[i].SetSlide(m_slideAmount);
            }
        }

        private void Update()
        {
            if (m_eventManager == null)
            {
                return;
            }
            
            m_slideAmount += Time.deltaTime * speed;

            for (var i = 0; i < m_slots.Length; i++)
            {
                m_slots[i].SetSlide(m_slideAmount);
            }
        }
    }

}