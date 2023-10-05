using System;
using System.Collections;
using BonLib.Events;
using BonLib.Managers;
using Core.Runtime.Events.Gameplay;
using Core.Runtime.Gameplay.Slot;
using Core.Runtime.Solvers;
using UnityEngine;

namespace Core.Runtime.Managers
{

    public class SlotManager : Manager<SlotManager>,
        IEventHandler<RequestPlayerDataEvent>
    {
        private SlotCombinationTable m_table;

        [SerializeField]
        private SlotCombination[] m_nextCombinations;

        public override void SubscribeToEvents()
        {
            base.SubscribeToEvents();
            
            EventManager.AddListener<RequestPlayerDataEvent>(this, Priority.Low);
        }

        public override void PreInitialize()
        {
            base.PreInitialize();
            
            m_table = Resources.Load<SlotCombinationTable>("Data/SlotCombinationTable");
        }

        public override void Initialize()
        {
            base.Initialize();

            var evt = new RequestPlayerDataEvent();
            EventManager.SendEvent(ref evt);
        }

        public void OnEventReceived(ref RequestPlayerDataEvent evt)
        {
            if (evt.Data.NextCombinations != null)
            {
                m_nextCombinations = evt.Data.NextCombinations;
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