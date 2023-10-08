using System;
using System.Threading.Tasks;
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

        private bool m_isSpinning = false;

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
            
            m_isSpinning = false;
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

        private async Task SetCombination(SlotCombination combination, bool instant = false)
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

            var spinTasks = new Task[m_slotColumns.Length];

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

                if (i > 0)
                {
                    await Task.Delay(Config.Animation.StartOffsetPerColumnInMs);
                }

                spinTasks[i] = SpinColumn(column, animationData, spinCount, slideAmount);
            }

            await Task.WhenAll(spinTasks);
        }

        private async Task SpinColumn(SlotColumn column, SlotAnimationData animationData, int spinCount, float slideAmount)
        {
            DOSetter<float> slideSetter = column.SetSlide;
            
            var sequence = DOTween.Sequence();
                
            sequence.Append(
                DOTween.To(() => (column.SlideAmount), 
                        slideSetter, 
                        column.SlideAmount + Config.ColumnTotalHeight * animationData.InitialSlideCount, 
                        animationData.InitialDuration)
                    .SetEase(animationData.InitialEase));
                
            column.BlurSlots(animationData.InitialDuration);
                
            sequence.Append(
                DOTween.To(() => (column.SlideAmount), 
                        slideSetter, 
                        column.SlideAmount + Config.ColumnTotalHeight * spinCount, 
                        animationData.LoopDuration)
                    .SetEase(animationData.LoopEase)
                    .OnComplete(() => column.UnblurSlots(animationData.StopDuration)));
                
            sequence.Append(
                DOTween.To(() => (column.SlideAmount), 
                        slideSetter, 
                        slideAmount + Config.ColumnTotalHeight * animationData.StopSlideCount, 
                        animationData.StopDuration)
                    .SetEase(animationData.StopEase));

            var duration = (int)((animationData.InitialDuration + animationData.LoopDuration + animationData.StopDuration) * 1000f);

            await Task.Delay(duration);
        }

        private async Task Spin()
        {
            m_isSpinning = true;
            Debug.LogError("Spin started");
            var nextCombination = m_saveManager.Data.NextCombinations[0];
            
            var spinEvt = new SlotMachineSpinEvent();
            EventManager.SendEvent(ref spinEvt);

            await SetCombination(nextCombination);

            Debug.LogError("Spin ended");
            m_isSpinning = false;

            var stopEvt = new SlotMachineStopEvent();
            EventManager.SendEvent(ref stopEvt);
        }

        public void OnEventReceived(ref SpinButtonPressedEvent evt)
        {
            if (m_isSpinning)
                return;

            Spin();
        }
    }

}