using System;
using BonLib.Events;
using BonLib.Managers;
using Core.Config;
using Core.Runtime.Events.Gameplay;
using Core.Runtime.Gameplay.VFX;
using UnityEngine;
using UnityEngine.Serialization;

namespace Core.Runtime.Managers
{

    public class ParticleManager : Manager<ParticleManager>,
        IEventHandler<SpinEndedEvent>
    {
        private ParticleConfig m_particleConfig;
        public ParticleConfig ParticleConfig => m_particleConfig ??=
            Resources.Load<ParticleConfig>("Config/ParticleConfig");
        
        [SerializeField]
        private ParticleController m_coinParticleController;
        
        [SerializeField]
        private ParticleController m_blastParticleController;

        public override void SubscribeToEvents()
        {
            base.SubscribeToEvents();
            
            EventManager.AddListener<SpinEndedEvent>(this);
        }

        public void OnEventReceived(ref SpinEndedEvent evt)
        {
            if (evt.Combination.IsMatch)
            {
                var slotType = evt.Combination.SlotTypes[0];
                var burstCount = ParticleConfig.GetBurstCount(slotType);
                
                m_coinParticleController.SetBurstCount(burstCount);
                m_coinParticleController.Play();
                m_blastParticleController.Play();
            }
        }
    }

}