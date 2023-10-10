using UnityEngine;

namespace Core.Config
{

    [CreateAssetMenu(fileName = "CameraConfig", menuName = "Config/Camera Config", order = 0)]
    public class CameraConfig : ScriptableObject
    {
        [SerializeField] 
        public Vector3 ShakeStrength = new (1f, 1f, 0f);
        
        [SerializeField] 
        public float ShakeDuration;
        
        [SerializeField] 
        public int ShakeVibrato = 10;
        
        [SerializeField] 
        public float ShakeRandomness = 90;
    }

}