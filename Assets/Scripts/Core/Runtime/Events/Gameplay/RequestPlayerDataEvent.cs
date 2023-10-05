using BonLib.Events;
using Core.Runtime.Data;

namespace Core.Runtime.Events.Gameplay
{

    public struct RequestPlayerDataEvent : IEvent
    {
        public bool IsConsumed { get; set; }
        
        public PlayerData Data;

        public RequestPlayerDataEvent(PlayerData data) : this()
        {
            Data = data;
        }
    }

}