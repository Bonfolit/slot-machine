using BonLib.Pooling;
using Core.Config;
using Core.Data;
using Core.Helpers;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;

namespace Core.Runtime.Gameplay.Slot
{
    [System.Serializable]
    public class Slot : MonoBehaviour
    {
        private static int BLUR_TEXTURE_ID = Shader.PropertyToID("_BlurTex");
        private static int MAIN_TEXTURE_ID = Shader.PropertyToID("_MainTex");
        private static int BLUR_START_TIME_ID = Shader.PropertyToID("_FadeStartTime");
        private static int BLUR_DURATION_ID = Shader.PropertyToID("_FadeDuration");
        private static int BLUR_DIRECTION_ID = Shader.PropertyToID("_FadeDirection");
        
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
        [SerializeField]
        private SpriteRenderer m_spriteRenderer;

        [SerializeField]
        private SpriteRenderer m_blurSpriteRenderer;

        // private MaterialPropertyBlock m_mpb;

        public void Initialize(Transform anchorTransform, SlotType type, SlotConfig config, SlotSpriteContainer slotSpriteContainer)
        {
            // m_mpb = new MaterialPropertyBlock();
            m_selfTransform = transform;
            m_index = (int)Type;

            m_anchorTransform = anchorTransform;
            m_type = type;
            m_config = config;
            m_slotSpriteContainer = slotSpriteContainer;
            
            // m_mpb.SetFloat(BLUR_DIRECTION_ID, 1);
            // m_blurSpriteRenderer.SetPropertyBlock(m_mpb);
            
            Unblur(0f);

            m_selfTransform.localScale = new Vector3(m_config.SlotDimensions.x, m_config.SlotDimensions.y, 1f);

            m_verticalOffset = SlotHelper.CalculateVerticalOffset(m_index, config);
            m_lowerBoundOffset = SlotHelper.CalculateVerticalOffset(0, m_config);
            
            SetPosition(new Vector3(0f, m_verticalOffset, 0f));
        }

        public void SetPosition(Vector3 localPos)
        {
            // var pos = m_anchorTransform.TransformPoint(localPos);
            m_selfTransform.localPosition = localPos;
        }

        public void SetSlide(float slideAmount)
        {
            float modOffset = (slideAmount + m_verticalOffset) - m_lowerBoundOffset;
            modOffset = (modOffset % m_config.ColumnTotalHeight + m_config.ColumnTotalHeight) % m_config.ColumnTotalHeight;
            modOffset += m_lowerBoundOffset;

            SetPosition(new Vector3(0f, modOffset, 0f));
        }

        public float debugBlurDuration;
        [Button]
        public void TestBlur()
        {
            Blur(debugBlurDuration);
        }
        [Button]
        public void TestUnblur()
        {
            Unblur(debugBlurDuration);
        }

        public float debugAlpha;

        [Button]
        public void TestAlpha()
        {
            var color = m_spriteRenderer.color;
            color.a = debugAlpha;
            m_spriteRenderer.color = color;
        }
        public void Blur(float duration)
        {
            m_spriteRenderer.enabled = false;
            m_blurSpriteRenderer.enabled = true;
            
            /*
            m_mpb.SetFloat(BLUR_DURATION_ID, duration);
            m_mpb.SetFloat(BLUR_START_TIME_ID, Time.time);
            m_mpb.SetFloat(BLUR_DIRECTION_ID, 1f);
            m_spriteRenderer.SetPropertyBlock(m_mpb);

            m_mpb.SetFloat(BLUR_START_TIME_ID, Time.time + duration);
            // m_mpb.SetFloat(BLUR_START_TIME_ID, Time.time);
            m_mpb.SetFloat(BLUR_DIRECTION_ID, -1f);
            m_blurSpriteRenderer.SetPropertyBlock(m_mpb);
            */
        }
        
        public void Unblur(float duration)
        {
            m_spriteRenderer.enabled = true;
            m_blurSpriteRenderer.enabled = false;

            /*
            m_mpb.SetFloat(BLUR_DURATION_ID, duration);
            m_mpb.SetFloat(BLUR_START_TIME_ID, Time.time + duration);
            // m_mpb.SetFloat(BLUR_START_TIME_ID, Time.time);
            m_mpb.SetFloat(BLUR_DIRECTION_ID, -1f);
            m_spriteRenderer.SetPropertyBlock(m_mpb);
            
            m_mpb.SetFloat(BLUR_START_TIME_ID, Time.time);
            m_mpb.SetFloat(BLUR_DIRECTION_ID, 1f);
            m_blurSpriteRenderer.SetPropertyBlock(m_mpb);
            */
        }
    }

}