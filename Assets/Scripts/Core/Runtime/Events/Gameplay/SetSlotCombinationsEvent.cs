using BonLib.Events;
using Core.Data;

namespace Core.Runtime.Events.Gameplay
{

    public struct SetSlotCombinationsEvent : IEvent
    {
        public bool IsConsumed { get; set; }

        public GameData GameData;

        public bool InitialGeneration;

        public SetSlotCombinationsEvent(GameData gameData, bool initialGeneration = false) : this()
        {
            GameData = gameData;
            InitialGeneration = initialGeneration;
        }
    }

}