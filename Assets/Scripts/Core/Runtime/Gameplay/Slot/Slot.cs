using BonLib.DependencyInjection;
using BonLib.Pooling;
using Core.Runtime.Data;
using Core.Runtime.Managers;
using UnityEngine;

namespace Core.Runtime.Gameplay.Slot
{
    [System.Serializable]
    public class Slot
    {
        private SlotSpriteContainer m_slotSpriteContainer;

        private Transform m_selfTransform;
        private Transform m_anchorTransform;
        
        [SerializeField]
        private SlotType m_type;
        public SlotType Type => m_type;

        private SlotLayoutConfig m_layoutConfig;

        private PoolObject m_rentedSpriteRendererPoolObject;
        private SpriteRenderer m_spriteRenderer;

        public Slot()
        {
        }

        public void Initialize(Transform anchorTransform, SlotType type, SlotLayoutConfig layoutConfig, SlotSpriteContainer slotSpriteContainer, PoolObject spriteRendererPoolObject)
        {
            m_anchorTransform = anchorTransform;
            m_type = type;
            m_layoutConfig = layoutConfig;
            m_slotSpriteContainer = slotSpriteContainer;
            m_rentedSpriteRendererPoolObject = PrefabPool.Rent(spriteRendererPoolObject);
            m_selfTransform = m_rentedSpriteRendererPoolObject.transform;
            m_spriteRenderer = (SpriteRenderer)m_rentedSpriteRendererPoolObject.CustomReference;
            m_spriteRenderer.sprite = m_slotSpriteContainer.GetSprite(m_type);

            m_selfTransform.localScale = new Vector3(m_layoutConfig.SlotDimensions.x, m_layoutConfig.SlotDimensions.y, 1f);
        }

        public void SetPosition(Vector3 localPos)
        {
            var pos = m_anchorTransform.TransformPoint(localPos);
            m_selfTransform.position = pos;
        }
    }

}