using Core.Config;
using Core.Helpers;
using UnityEngine;

namespace Core.Runtime.Gameplay.Slot
{
    [System.Serializable]
    public class Slot : MonoBehaviour
    {
        // private static int BLUR_TEXTURE_ID = Shader.PropertyToID("_BlurTex");
        // private static int MAIN_TEXTURE_ID = Shader.PropertyToID("_MainTex");
        // private static int BLUR_START_TIME_ID = Shader.PropertyToID("_FadeStartTime");
        // private static int BLUR_DURATION_ID = Shader.PropertyToID("_FadeDuration");
        // private static int BLUR_DIRECTION_ID = Shader.PropertyToID("_FadeDirection");

        private float m_lowerBoundOffset;

        private float m_verticalOffset;
        
        private Transform m_selfTransform;
        
        [SerializeField]
        private SlotType m_type;
        public SlotType Type => m_type;

        private SlotConfig m_config;

        [SerializeField]
        private SpriteRenderer m_spriteRenderer;

        [SerializeField]
        private SpriteRenderer m_blurSpriteRenderer;

        public void Initialize(SlotConfig config)
        {
            m_config = config;
            m_selfTransform = transform;
            
            SetBlur(false);

            m_selfTransform.localScale = new Vector3(m_config.SlotDimensions.x, m_config.SlotDimensions.y, 1f);

            m_verticalOffset = SlotHelper.CalculateVerticalOffset((int)Type, config);
            m_lowerBoundOffset = SlotHelper.CalculateVerticalOffset(0, m_config);
            
            SetPosition(new Vector3(0f, m_verticalOffset, 0f));
        }

        public void SetPosition(Vector3 localPos)
        {
            m_selfTransform.localPosition = localPos;
        }

        public void SetSlide(float slideAmount)
        {
            float modOffset = (slideAmount + m_verticalOffset) - m_lowerBoundOffset;
            modOffset = (modOffset % m_config.ColumnTotalHeight + m_config.ColumnTotalHeight) % m_config.ColumnTotalHeight;
            modOffset += m_lowerBoundOffset;

            SetPosition(new Vector3(0f, modOffset, 0f));
        }
        
        public void SetBlur(bool setBlur)
        {
            m_spriteRenderer.enabled = !setBlur;
            m_blurSpriteRenderer.enabled = setBlur;
        }

    }

}