using BonLib.Events;
using Core.Runtime.Events.Gameplay;
using UnityEngine;

namespace Core.Runtime.Gameplay.VFX
{

    public class ParticleController : MonoBehaviour
    {
        [SerializeField]
        private ParticleSystem m_particle;

        [SerializeField]
        private int m_burstCount;

        public void SetBurstCount(int count)
        {
            m_burstCount = count;
        }

        public void Play()
        {
            var emission = m_particle.emission;
            var burst = emission.GetBurst(0);
            var curve = burst.count;
            curve.constant = m_burstCount;
            burst.count = curve;
            emission.SetBurst(0, burst);
            
            m_particle.Play();
        }
    }

}