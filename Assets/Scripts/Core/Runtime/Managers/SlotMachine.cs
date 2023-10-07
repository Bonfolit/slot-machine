using System;
using BonLib.DependencyInjection;
using BonLib.Events;
using BonLib.Managers;
using BonLib.Pooling;
using Core.Config;
using Core.Data;
using Core.Runtime.Events.Gameplay;
using Core.Runtime.Gameplay.Slot;
using DG.Tweening;
using DG.Tweening.Core;
using NaughtyAttributes;
using UnityEngine;
using Random = System.Random;

namespace Core.Runtime.Managers
{

    public class SlotMachine : Manager<SlotMachine>,
        IEventHandler<SpinButtonPressedEvent>
    {
        private Random m_random;
        private SaveManager m_saveManager;
        
        private SlotSpriteContainer m_slotSpriteContainer;
        public SlotSpriteContainer SlotSpriteContainer => m_slotSpriteContainer ??=
            Resources.Load<SlotSpriteContainer>("Data/SlotSpriteContainer");
        
        private SlotConfig m_config;
        public SlotConfig Config => m_config ??=
            Resources.Load<SlotConfig>("Config/SlotConfig");

        [SerializeField]
        private PoolObject m_spriteRendererPoolObject;
        
        [SerializeField]
        private SlotColumn[] m_slotColumns;

        public override void ResolveDependencies()
        {
            base.ResolveDependencies();

            m_saveManager = DI.Resolve<SaveManager>();
        }

        public override void SubscribeToEvents()
        {
            base.SubscribeToEvents();
            
            EventManager.AddListener<SpinButtonPressedEvent>(this);
        }

        public override void Initialize()
        {
            base.Initialize();

            m_random = new Random();
        }

        public override void LateInitialize()
        {
            base.LateInitialize();
            
            for (var i = 0; i < m_slotColumns.Length; i++)
            {
                m_slotColumns[i].Initialize(SlotSpriteContainer, Config, m_spriteRendererPoolObject, Config.ColumnTotalHeight);
            }
            
            SetCombination(m_saveManager.Data.LastCombination, true);
        }

        private void SetCombination(SlotCombination combination, bool instant = false)
        {
            var firstTwoSlotsEqual = combination.SlotTypes[0].Equals(combination.SlotTypes[1]);
            var isMatch = true;
            var slotType = combination.SlotTypes[0];
            for (int i = 1; i < combination.SlotTypes.Length; i++)
            {
                if (!combination.SlotTypes[i].Equals(slotType))
                {
                    isMatch = false;
                    break;
                }
            }
            

            for (var i = 0; i < m_slotColumns.Length; i++)
            {
                var column = m_slotColumns[i];

                var animationType = SlotAnimationType.Quick;

                if (i == m_slotColumns.Length - 1 && firstTwoSlotsEqual)
                {
                    if (isMatch)
                    {
                        animationType = SlotAnimationType.Slow;
                    }
                    else
                    {
                        animationType = SlotAnimationType.Normal;
                    }
                }

                var animationData = Config.Animation.GetAnimationData(animationType);
                
                var slot = combination.SlotTypes[i];
                var slideCount = (Config.MarkerIndex - (int)slot) % Config.ColumnSize;
                var slideAmount = Config.VerticalOffset * slideCount;

                var spinCount = m_random.Next(animationData.LoopSpinRange.x, animationData.LoopSpinRange.y);

                if (instant)
                {
                    m_slotColumns[i].SetSlide(slideAmount);
                    continue;
                }
                

                DOSetter<float> slideSetter = (val) =>
                {
                    column.SetSlide(val); 
                };

                var offset = (float)i * Config.Animation.StopOffsetPerColumn;

                var sequence = DOTween.Sequence();
                
                sequence.Append(
                    DOTween.To(() => (column.SlideAmount), 
                        slideSetter, 
                        column.SlideAmount + Config.ColumnTotalHeight * animationData.InitialSlideCount, 
                        animationData.InitialDuration)
                        .SetEase(animationData.InitialEase));
                
                sequence.Append(
                    DOTween.To(() => (column.SlideAmount), 
                            slideSetter, 
                            column.SlideAmount + Config.ColumnTotalHeight * spinCount, 
                            animationData.LoopDuration + offset)
                        .SetEase(animationData.LoopEase));
                // sequence.Append(
                //     DOTween.To(() => (column.SlideAmount), 
                //         slideSetter, 
                //         column.SlideAmount + Config.ColumnTotalHeight, 
                //         animationData.LoopDuration)
                //         .SetEase(animationData.LoopEase)
                //         .SetLoops(spinCount));

                sequence.Append(
                    DOTween.To(() => (column.SlideAmount), 
                        slideSetter, 
                        slideAmount + Config.ColumnTotalHeight * animationData.StopSlideCount, 
                        animationData.StopDuration)
                        .SetEase(animationData.StopEase));

                var aniCurve = new AnimationCurve();
                
                sequence.Append(
                    DOTween.To(() => (column.SlideAmount), 
                            slideSetter, 
                            column.SlideAmount + Config.ColumnTotalHeight * animationData.InitialSlideCount, 
                            animationData.InitialDuration)
                        .SetEase(aniCurve));
            }
        }

        [Button]
        public void Spin()
        {
            var nextCombination = m_saveManager.Data.NextCombinations[0];
            
            SetCombination(nextCombination);

            var spunEvt = new SlotMachineSpunEvent();
            EventManager.SendEvent(ref spunEvt);
        }

        public void OnEventReceived(ref SpinButtonPressedEvent evt)
        {
            Spin();
        }
    }

}