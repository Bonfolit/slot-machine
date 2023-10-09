using System;
using System.Runtime.InteropServices;
using Core.Helpers;
using UnityEngine;

namespace Core.Runtime.Gameplay.Slot
{
    [System.Serializable]
    [StructLayout(LayoutKind.Sequential, Pack=1)]
    public struct SlotCombination : IEquatable<SlotCombination>
    {
        [SerializeField]
        public SlotType[] SlotTypes;

        private bool? m_isMatch;
        
        public bool IsMatch => m_isMatch ??= this.IsMatch();

        public SlotCombination(SlotType type1, SlotType type2, SlotType type3)
        {
            SlotTypes = new [] { type1, type2, type3 };
            
            m_isMatch = null;
        }

        public bool IsEqualTo(SlotCombination other)
        {
            if (SlotTypes.Length != other.SlotTypes.Length)
            {
                return false;
            }

            for (int i = 0; i < SlotTypes.Length; i++)
            {
                if (!SlotTypes[i].Equals(other.SlotTypes[i]))
                {
                    return false;
                }
            }

            return true;
        }
        
        public override bool Equals(object obj)
        {
            if (obj is SlotCombination other)
            {
                return IsEqualTo(other);
            }
            return false;
        }

        public bool Equals(SlotCombination other)
        {
            return IsEqualTo(other);
        }
        
        public static bool operator ==(SlotCombination left, SlotCombination right)
        {
            return left.IsEqualTo(right);
        }

        public static bool operator !=(SlotCombination left, SlotCombination right)
        {
            return !left.IsEqualTo(right);
        }

        public override int GetHashCode()
        {
            var hash = 17;
            foreach (var slotType in SlotTypes)
            {
                hash = hash * 31 + slotType.GetHashCode();
            }
            return hash;
        }
    }
}