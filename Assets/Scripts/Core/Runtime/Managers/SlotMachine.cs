using BonLib.Managers;
using BonLib.Pooling;
using Core.Runtime.Data;
using Core.Runtime.Gameplay.Slot;
using UnityEngine;

namespace Core.Runtime.Managers
{

    public class SlotMachine : Manager<SlotMachine>
    {
        private SlotSpriteContainer m_slotSpriteContainer;
        public SlotSpriteContainer SlotSpriteContainer => m_slotSpriteContainer ??=
            Resources.Load<SlotSpriteContainer>("Data/SlotSpriteContainer");
        
        private SlotLayoutConfig m_layoutConfig;
        public SlotLayoutConfig LayoutConfig => m_layoutConfig ??=
            Resources.Load<SlotLayoutConfig>("Config/SlotLayoutConfig");

        [SerializeField]
        private PoolObject m_spriteRendererPoolObject;
        
        [SerializeField]
        private SlotColumn[] m_slotColumns;
        
        public override void LateInitialize()
        {
            base.LateInitialize();

            foreach (var column in m_slotColumns)
            {
                column.Initialize(SlotSpriteContainer, LayoutConfig, m_spriteRendererPoolObject);
            }
        }
    }

}