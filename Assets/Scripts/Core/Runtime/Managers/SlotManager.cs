using System;
using System.Collections;
using BonLib.DependencyInjection;
using BonLib.Events;
using BonLib.Managers;
using BonLib.Pooling;
using Core.Config;
using Core.Runtime.Events.Gameplay;
using Core.Runtime.Gameplay.Slot;
using Core.Runtime.Solvers;
using NaughtyAttributes;
using UnityEngine;

namespace Core.Runtime.Managers
{

    public class SlotManager : Manager<SlotManager>,
        IEventHandler<SlotMachineSpunEvent>
    {
        private SaveManager m_saveManager;
        private SlotCombinationTable m_table;
        public SlotCombinationTable Table => m_table ??= 
            Resources.Load<SlotCombinationTable>("Data/SlotCombinationTable");
        
        private SlotConfig m_config;
        public SlotConfig Config => m_config ??=
            Resources.Load<SlotConfig>("Config/SlotConfig");

        [SerializeField]
        private SlotCombination[] m_nextCombinations;

        public override void ResolveDependencies()
        {
            base.ResolveDependencies();

            m_saveManager = DI.Resolve<SaveManager>();
        }

        public override void SubscribeToEvents()
        {
            base.SubscribeToEvents();
            
            EventManager.AddListener<SlotMachineSpunEvent>(this);
        }

        public override void Initialize()
        {
            base.Initialize();
            
            SetSlotCombinations();
        }

        private void SetSlotCombinations()
        {
            if (m_saveManager.Data.NextCombinations != null)
            {
                m_nextCombinations = m_saveManager.Data.NextCombinations;
            }
            else
            {
                m_nextCombinations = SlotSolver.Solve(Table, Config.CombinationBufferAmount, 10000);

                var updatedEvt = new SlotCombinationsUpdatedEvent(m_nextCombinations);
                EventManager.SendEvent(ref updatedEvt);
            }
        }

        public void OnEventReceived(ref SlotMachineSpunEvent evt)
        {
            var nextCombination = m_saveManager.Data.NextCombinations[0];
            SlotSolver.QueueNewCombination(Table, ref m_saveManager.Data.NextCombinations);
            
            var selectedEvent = new CombinationSelectedEvent(nextCombination);
            EventManager.SendEvent(ref selectedEvent);

        }
    }

}