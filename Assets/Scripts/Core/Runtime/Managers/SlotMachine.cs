using System;
using BonLib.DependencyInjection;
using BonLib.Managers;
using BonLib.Pooling;
using Core.Config;
using Core.Runtime.Data;
using Core.Runtime.Events.Gameplay;
using Core.Runtime.Gameplay.Slot;
using NaughtyAttributes;
using UnityEngine;

namespace Core.Runtime.Managers
{

    public class SlotMachine : Manager<SlotMachine>
    {
        private SaveManager m_saveManager;
        
        private SlotSpriteContainer m_slotSpriteContainer;
        public SlotSpriteContainer SlotSpriteContainer => m_slotSpriteContainer ??=
            Resources.Load<SlotSpriteContainer>("Data/SlotSpriteContainer");
        
        private SlotConfig m_config;
        public SlotConfig Config => m_config ??=
            Resources.Load<SlotConfig>("Config/SlotConfig");

        [SerializeField]
        private PoolObject m_spriteRendererPoolObject;
        
        [SerializeField]
        private SlotColumn[] m_slotColumns;

        public override void ResolveDependencies()
        {
            base.ResolveDependencies();

            m_saveManager = DI.Resolve<SaveManager>();
        }

        public override void LateInitialize()
        {
            base.LateInitialize();
            
            for (var i = 0; i < m_slotColumns.Length; i++)
            {
                m_slotColumns[i].Initialize(SlotSpriteContainer, Config, m_spriteRendererPoolObject);
            }
            
            SetCombination(m_saveManager.Data.LastCombination);
        }

        private void SetCombination(SlotCombination combination)
        {
            for (var i = 0; i < m_slotColumns.Length; i++)
            {
                var slot = combination.SlotTypes[i];
                var slideCount = (Config.MarkerIndex - (int)slot) % Config.ColumnSize;
                var slideAmount = Config.VerticalOffset * slideCount;

                m_slotColumns[i].SetSlide(slideAmount);
            }
        }

        [Button]
        public void Spin()
        {
            var nextCombination = m_saveManager.Data.NextCombinations[0];
            
            SetCombination(nextCombination);

            var remainingCombinations = new SlotCombination[Config.CombinationBufferAmount];
            Array.Copy(m_saveManager.Data.NextCombinations, 1, remainingCombinations, 0, 99);
            
            var selectedEvent = new CombinationSelectedEvent(nextCombination);
            EventManager.SendEvent(ref selectedEvent);

            var updateEvent = new SlotCombinationsUpdatedEvent(remainingCombinations);
            EventManager.SendEvent(ref updateEvent);
        }
    }

}