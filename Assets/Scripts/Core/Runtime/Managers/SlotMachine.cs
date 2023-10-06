using BonLib.Managers;
using BonLib.Pooling;
using Core.Runtime.Gameplay.Slot;
using UnityEngine;

namespace Core.Runtime.Managers
{

    public class SlotMachine : Manager<SlotMachine>
    {
        [SerializeField]
        private PoolObject m_spriteRendererPoolObj;

        private PoolObject m_rentedPoolObj;
    }

}