using Core.Runtime.Gameplay.Slot;
using UnityEngine;
using UnityEngine.Serialization;

namespace Core.Config
{

    [CreateAssetMenu(fileName = "ParticleConfig", menuName = "Config/Particle Config", order = 0)]
    public class ParticleConfig : ScriptableObject
    {
        public ParticleData[] ParticleDatas;

        public int GetBurstCount(SlotType slotType)
        {
            for (var i = 0; i < ParticleDatas.Length; i++)
            {
                if (slotType.Equals(ParticleDatas[i].SlotType))
                {
                    return ParticleDatas[i].BurstCount;
                }
            }

            return 0;
        }
    }

    [System.Serializable]
    public struct ParticleData
    {
        public SlotType SlotType;
        [FormerlySerializedAs("BurstAmount")] public int BurstCount;
    }
}