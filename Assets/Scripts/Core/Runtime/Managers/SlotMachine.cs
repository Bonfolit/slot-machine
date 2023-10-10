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
using UnityEngine;
using Random = System.Random;

namespace Core.Runtime.Managers
{

    public class SlotMachine : Manager<SlotMachine>,
        IEventHandler<SpinButtonPressedEvent>
    {
        private Random m_random;
        private SlotManager m_slotManager;
        
        private SlotSpriteContainer m_slotSpriteContainer;
        public SlotSpriteContainer SlotSpriteContainer => m_slotSpriteContainer ??=
            Resources.Load<SlotSpriteContainer>("Data/SlotSpriteContainer");
        
        private SlotConfig m_slotConfig;
        public SlotConfig SlotConfig => m_slotConfig ??=
            Resources.Load<SlotConfig>("Config/SlotConfig");
        
        private ColumnAnimationConfig m_columnAnimationConfig;
        public ColumnAnimationConfig ColumnAnimationConfig => m_columnAnimationConfig ??=
            Resources.Load<ColumnAnimationConfig>("Config/ColumnAnimationConfig");

        [SerializeField]
        private PoolObject m_spriteRendererPoolObject;
        
        [SerializeField]
        private SlotColumn[] m_slotColumns;

        private Task m_spinTask;

        public override void ResolveDependencies()
        {
            base.ResolveDependencies();

            m_slotManager = DI.Resolve<SlotManager>();
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

            m_spinTask = null;
        }

        public override void LateInitialize()
        {
            base.LateInitialize();
            
            for (var i = 0; i < m_slotColumns.Length; i++)
            {
                m_slotColumns[i].Initialize(SlotSpriteContainer, SlotConfig, m_spriteRendererPoolObject, SlotConfig.ColumnTotalHeight);
            }
            
            SetCombination(m_slotManager.GetLastCombination(), true);
        }

        public void OnEventReceived(ref SpinButtonPressedEvent evt)
        {
            if (m_spinTask != null && !m_spinTask.IsCompleted)
                return;

            m_spinTask = Spin();
        }

        private async Task Spin()
        {
            var nextCombination = m_slotManager.GetNextCombination();

            await SetCombination(nextCombination);

            var evt = new SpinEndedEvent(nextCombination);
            EventManager.SendEvent(ref evt);
        }
        
        private async Task SetCombination(SlotCombination combination, bool instant = false)
        {
            var firstTwoSlotsEqual = combination.SlotTypes[0].Equals(combination.SlotTypes[1]);

            var spinTasks = new Task[m_slotColumns.Length];

            for (var i = 0; i < m_slotColumns.Length; i++)
            {
                var column = m_slotColumns[i];

                var animationType = ColumnAnimationType.Quick;

                if (i == m_slotColumns.Length - 1 && firstTwoSlotsEqual)
                {
                    animationType = combination.IsMatch ? ColumnAnimationType.Slow : ColumnAnimationType.Normal;
                }

                var animationData = ColumnAnimationConfig.GetAnimationData(animationType);
                
                var slot = combination.SlotTypes[i];
                var slideCount = (SlotConfig.MarkerIndex - (int)slot) % SlotConfig.ColumnSize;
                var slideAmount = SlotConfig.VerticalOffset * slideCount;

                var spinCount = m_random.Next(animationData.LoopSpinRange.x, animationData.LoopSpinRange.y);

                if (instant)
                {
                    m_slotColumns[i].SetSlide(slideAmount);
                    continue;
                }

                if (i > 0)
                {
                    await Task.Delay(ColumnAnimationConfig.StartOffsetPerColumnInMs * i);
                }

                spinTasks[i] = SpinColumn(column, animationData, spinCount, slideAmount);
            }

            await Task.WhenAll(spinTasks);
        }

        private async Task SpinColumn(SlotColumn column, ColumnAnimationData columnAnimationData, int spinCount, float slideAmount)
        {
            DOSetter<float> slideSetter = column.SetSlide;
            
            var sequence = DOTween.Sequence();
                
            sequence.Append(
                DOTween.To(() => (column.SlideAmount), 
                        slideSetter, 
                        column.SlideAmount + SlotConfig.ColumnTotalHeight * columnAnimationData.InitialSlideCount, 
                        columnAnimationData.InitialDuration)
                    .SetEase(columnAnimationData.InitialEase));
                
            column.BlurSlots(columnAnimationData.InitialDuration);
                
            sequence.Append(
                DOTween.To(() => (column.SlideAmount), 
                        slideSetter, 
                        column.SlideAmount + SlotConfig.ColumnTotalHeight * spinCount, 
                        columnAnimationData.LoopDuration)
                    .SetEase(columnAnimationData.LoopEase)
                    .OnComplete(() => column.UnblurSlots(columnAnimationData.StopDuration)));
                
            sequence.Append(
                DOTween.To(() => (column.SlideAmount), 
                        slideSetter, 
                        slideAmount + SlotConfig.ColumnTotalHeight * columnAnimationData.StopSlideCount, 
                        columnAnimationData.StopDuration)
                    .SetEase(columnAnimationData.StopEase));

            var duration = (int)((columnAnimationData.InitialDuration + columnAnimationData.LoopDuration + columnAnimationData.StopDuration) * 1000f);

            await Task.Delay(duration);
        }

    }

}