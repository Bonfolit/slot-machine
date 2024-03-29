﻿using BonLib.DependencyInjection;
using BonLib.Events;
using BonLib.Managers;
using Core.Config;
using Core.Data;
using Core.Runtime.Events.Gameplay;
using Core.Runtime.Gameplay.Slot;
using Core.Solvers;
using UnityEngine;

namespace Core.Runtime.Managers
{

    public class SlotManager : Manager<SlotManager>,
        IEventHandler<SetSlotCombinationsEvent>
    {
        private SaveManager m_saveManager;
        private SlotCombinationTable m_table;
        public SlotCombinationTable Table => m_table ??= 
            Resources.Load<SlotCombinationTable>("Data/SlotCombinationTable");
        
        private SlotConfig m_config;
        public SlotConfig Config => m_config ??=
            Resources.Load<SlotConfig>("Config/SlotConfig");

        [SerializeField]
        private SlotMachine m_slotMachine;

        public override void ResolveDependencies()
        {
            base.ResolveDependencies();

            m_saveManager = DI.Resolve<SaveManager>();
        }

        public override void SubscribeToEvents()
        {
            base.SubscribeToEvents();
            
            EventManager.AddListener<SetSlotCombinationsEvent>(this);
        }

        public override void Initialize()
        {
            base.Initialize();
            
            m_slotMachine.Init();
        }

        public void OnEventReceived(ref SetSlotCombinationsEvent evt)
        {
            SetCombinations(evt.GameData, evt.InitialGeneration);
        }

        public void SetCombinations(GameData gameData, bool initialGeneration)
        {
            gameData.Combinations = SlotSolver.Solve(
                Table, 
                Config.CombinationBufferAmount, 
                Config.IterationLimit, 
                Config.LossThreshold);

            if (initialGeneration)
            {
                gameData.LastCombination = Config.InitialCombination;
            }

            gameData.CombinationIndex = -1;
        }

        public SlotCombination GetNextCombination()
        {
            return m_saveManager.GetNextCombination();
        }

        public SlotCombination GetLastCombination()
        {
            return m_saveManager.GetLastCombination();
        }
    }

}