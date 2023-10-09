using System.IO;
using BonLib.Events;
using BonLib.Managers;
using Core.Data;
using Core.Runtime.Events.Gameplay;
using Core.Runtime.Gameplay.Slot;
using UnityEngine;

namespace Core.Runtime.Managers
{
    public class SaveManager : Manager<SaveManager>
    {
        private string m_dataFilePath;
        
        [SerializeField]
        private GameData m_data;
        public GameData Data => m_data ??= ScriptableObject.CreateInstance<GameData>();


        public override void PreInitialize()
        {
            base.PreInitialize();
            
            m_dataFilePath = Path.Combine(Application.persistentDataPath, "gameData.json");
            
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
                var evt = new SetSlotCombinationsEvent(Data, true);
                EventManager.SendEvent(ref evt);
                
                SaveUserData();
            }
        }
        
        private void SaveUserData()
        {
            string json = JsonUtility.ToJson(Data);

            File.WriteAllText(m_dataFilePath, json);
        }

        public SlotCombination GetNextCombination()
        {
            var combination = Data.GetNextCombination();

            if (Data.ReachedFinalCombination())
            {
                var evt = new SetSlotCombinationsEvent(Data);
                EventManager.SendEvent(ref evt);
            }
            
            SaveUserData();
            
            return combination;
        }
        
        public SlotCombination GetLastCombination()
        {
            return Data.LastCombination;
        }
    }
}