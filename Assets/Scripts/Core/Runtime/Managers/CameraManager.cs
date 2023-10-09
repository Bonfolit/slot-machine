using BonLib.Events;
using BonLib.Managers;
using Core.Config;
using Core.Runtime.Events.Gameplay;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;

namespace Core.Runtime.Managers
{

    public class CameraManager : Manager<CameraManager>,
        IEventHandler<SpinEndedEvent>
    {
        private CameraConfig m_cameraConfig;
        public CameraConfig CameraConfig => m_cameraConfig ??=
            Resources.Load<CameraConfig>("Config/CameraConfig");
        
        [SerializeField] 
        private Vector3 m_shakeSpeed = new (1f, 1f, 0f);
        
        [SerializeField] 
        private float m_shakeDuration;
        
        [SerializeField] 
        private int m_vibrato = 10;
        
        [SerializeField] 
        private float m_randomness = 90;
        
        public override void SubscribeToEvents()
        {
            base.SubscribeToEvents();
            
            EventManager.AddListener<SpinEndedEvent>(this);
        }

        public void OnEventReceived(ref SpinEndedEvent evt)
        {
            if (evt.Combination.IsMatch)
            {
                Shake();
            }
        }

        [Button]
        public void Shake()
        {
            transform.DOShakePosition(
                CameraConfig.ShakeDuration, 
                CameraConfig.ShakeStrength, 
                CameraConfig.ShakeVibrato, 
                CameraConfig.ShakeRandomness);
        }
    }

}