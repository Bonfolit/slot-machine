using System.IO;
using BonLib.Events;
using BonLib.Managers;
using Core.Runtime.Data;
using Core.Runtime.Events.Gameplay;
using Core.Runtime.Gameplay.Slot;
using UnityEngine;

namespace Core.Runtime.Managers
{

    public class SaveManager : Manager<SaveManager>,
        IEventHandler<SpinCompletedEvent>,
        IEventHandler<SlotCombinationsUpdatedEvent>,
        IEventHandler<CombinationSelectedEvent>
    {
        private string m_dataFilePath;
        
        [SerializeField]
        private PlayerData m_data;
        public PlayerData Data => m_data ??= ScriptableObject.CreateInstance<PlayerData>();

        public override void SubscribeToEvents()
        {
            base.SubscribeToEvents();
            
            EventManager.AddListener<SpinCompletedEvent>(this);
            EventManager.AddListener<SlotCombinationsUpdatedEvent>(this, Priority.Critical);
            EventManager.AddListener<CombinationSelectedEvent>(this, Priority.Critical);
        }

        public override void PreInitialize()
        {
            base.PreInitialize();
            
            m_dataFilePath = Path.Combine(Application.persistentDataPath, "playerData.json");
            
            LoadUserData();
        }
        
        private void LoadUserData()
        {
            if (File.Exists(m_dataFilePath))
            {
                string json = File.ReadAllText(m_dataFilePath);

                JsonUtility.FromJsonOverwrite(json, Data);
            }
            else
            {
                Data.LastCombination = new SlotCombination(SlotType.A, SlotType.Bonus, SlotType.Jackpot);
                SaveUserData();
            }
        }
        
        private void SaveUserData()
        {
            string json = JsonUtility.ToJson(Data);

            File.WriteAllText(m_dataFilePath, json);
        }

        public void OnEventReceived(ref SpinCompletedEvent evt)
        {
            Data.LastCombination = evt.CurrentCombination;
            
            SaveUserData();
        }

        public void OnEventReceived(ref SlotCombinationsUpdatedEvent evt)
        {
            Data.NextCombinations = evt.SlotCombinations;
            
            SaveUserData();
        }

        public void OnEventReceived(ref CombinationSelectedEvent evt)
        {
            Data.LastCombination = evt.Combination;
            
            SaveUserData();
        }
    }
}