using BonLib.Pooling;
using Core.Config;
using Core.Data;
using Core.Helpers;
using UnityEngine;

namespace Core.Runtime.Gameplay.Slot
{
    [System.Serializable]
    public class Slot
    {
        private static int BLUR_TEXTURE_ID = Shader.PropertyToID("_BlurTex");
        private static int BLUR_START_TIME_ID = Shader.PropertyToID("_BlurStartTime");
        private static int BLUR_DURATION_ID = Shader.PropertyToID("_BlurDuration");
        private static int BLUR_DIRECTION_ID = Shader.PropertyToID("_BlurDirection");
        
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

        private MaterialPropertyBlock m_mpb;

        public Slot(int index)
        {
            m_index = index;

            m_mpb = new MaterialPropertyBlock();
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

            var slotSprites = m_slotSpriteContainer.GetSprites(m_type);
            m_spriteRenderer.sprite = slotSprites.Sprite;
            m_mpb.SetTexture(BLUR_TEXTURE_ID, slotSprites.BlurTexture);
            m_spriteRenderer.SetPropertyBlock(m_mpb);

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

        public void Blur(float duration)
        {
            m_mpb.SetFloat(BLUR_START_TIME_ID, Time.time);
            m_mpb.SetFloat(BLUR_DURATION_ID, duration);
            m_mpb.SetFloat(BLUR_DIRECTION_ID, 1f);
            m_spriteRenderer.SetPropertyBlock(m_mpb);
        }
        
        public void Unblur(float duration)
        {
            m_mpb.SetFloat(BLUR_START_TIME_ID, Time.time + duration);
            m_mpb.SetFloat(BLUR_DURATION_ID, duration);
            m_mpb.SetFloat(BLUR_DIRECTION_ID, -1f);
            m_spriteRenderer.SetPropertyBlock(m_mpb);
        }
    }

}