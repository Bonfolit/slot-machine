using System;
using BonLib.DependencyInjection;
using BonLib.Events;
using UnityEngine;

namespace Core.Runtime.Managers
{

    public class GameManager : MonoBehaviour
    {
        private EventManager m_eventManager;
        
        private void Awake()
        {
            DontDestroyOnLoad(this.gameObject);
            
            DI.Bind(this);
            m_eventManager = new EventManager(64);
            DI.Bind<EventManager>(m_eventManager);
        }
    }

}