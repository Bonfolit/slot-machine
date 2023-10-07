using BonLib.DependencyInjection;
using BonLib.Events;
using BonLib.Pooling;
using Core.Config;
using Core.Runtime.Data;
using Core.Runtime.Events.Gameplay;
using Core.Runtime.Helpers;
using Core.Runtime.Managers;
using UnityEngine;

namespace Core.Runtime.Gameplay.Slot
{
    [System.Serializable]
    public class Slot
    {
        private int m_index;
        public int Index => m_index;

        private float m_lowerBoundOffset;

        private float m_verticalOffset;
        
        private SlotSpriteContainer m_slotSpriteContainer;

        private Transform m_selfTransform;
        private Transform m_anchorTransform;
        
        [SerializeField]
        private SlotType m_type;
        public SlotType Type => m_type;

        private SlotConfig m_config;

        private PoolObject m_rentedSpriteRendererPoolObject;
        private SpriteRenderer m_spriteRenderer;

        public Slot(int index)
        {
            m_index = index;
        }

        public void Initialize(Transform anchorTransform, SlotType type, SlotConfig config, SlotSpriteContainer slotSpriteContainer, PoolObject spriteRendererPoolObject)
        {
            m_anchorTransform = anchorTransform;
            m_type = type;
            m_config = config;
            m_slotSpriteContainer = slotSpriteContainer;
            
            m_rentedSpriteRendererPoolObject = PrefabPool.Rent(spriteRendererPoolObject);
            m_selfTransform = m_rentedSpriteRendererPoolObject.transform;
            m_spriteRenderer = (SpriteRenderer)m_rentedSpriteRendererPoolObject.CustomReference;
            m_spriteRenderer.sprite = m_slotSpriteContainer.GetSprite(m_type);

            m_selfTransform.localScale = new Vector3(m_config.SlotDimensions.x, m_config.SlotDimensions.y, 1f);

            m_verticalOffset = SlotHelper.CalculateVerticalOffset(m_index, config);
            m_lowerBoundOffset = SlotHelper.CalculateVerticalOffset(0, m_config);
            
            SetPosition(new Vector3(0f, m_verticalOffset, 0f));
        }

        public void SetPosition(Vector3 localPos)
        {
            var pos = m_anchorTransform.TransformPoint(localPos);
            m_selfTransform.position = pos;
        }

        public void SetSlide(float slideAmount)
        {
            float modOffset = (slideAmount + m_verticalOffset) - m_lowerBoundOffset;
            modOffset = (modOffset % m_config.ColumnTotalHeight + m_config.ColumnTotalHeight) % m_config.ColumnTotalHeight;
            modOffset += m_lowerBoundOffset;

            SetPosition(new Vector3(0f, modOffset, 0f));
        }
    }

}