using System;
using System.Collections;
using BonLib.DependencyInjection;
using BonLib.Events;
using BonLib.Managers;
using BonLib.Pooling;
using Core.Runtime.Events.Gameplay;
using Core.Runtime.Gameplay.Slot;
using Core.Runtime.Solvers;
using NaughtyAttributes;
using UnityEngine;

namespace Core.Runtime.Managers
{

    public class SlotManager : Manager<SlotManager>
    {
        private SaveManager m_saveManager;
        private SlotCombinationTable m_table;

        [SerializeField]
        private SlotCombination[] m_nextCombinations;

        public override void ResolveDependencies()
        {
            base.ResolveDependencies();

            m_saveManager = DI.Resolve<SaveManager>();
        }

        public override void PreInitialize()
        {
            base.PreInitialize();
            m_table = Resources.Load<SlotCombinationTable>("Data/SlotCombinationTable");
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
                m_nextCombinations = SlotSolver.Solve(m_table, 100, 10000);

                var updatedEvt = new SlotCombinationsUpdatedEvent(m_nextCombinations);
                EventManager.SendEvent(ref updatedEvt);
            }
        }
    }

}