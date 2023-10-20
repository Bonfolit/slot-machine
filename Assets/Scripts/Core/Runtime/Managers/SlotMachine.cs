using System.Threading.Tasks;
using BonLib.DependencyInjection;
using BonLib.Events;
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

    public class SlotMachine : MonoBehaviour,
        IEventHandler<SpinButtonPressedEvent>
    {
        private EventManager m_eventManager;
        private SlotManager m_slotManager;
        private Random m_random;
        
        private SlotConfig m_slotConfig;
        public SlotConfig SlotConfig => m_slotConfig ??=
            Resources.Load<SlotConfig>("Config/SlotConfig");
        
        private ColumnAnimationConfig m_columnAnimationConfig;
        public ColumnAnimationConfig ColumnAnimationConfig => m_columnAnimationConfig ??=
            Resources.Load<ColumnAnimationConfig>("Config/ColumnAnimationConfig");
        
        [SerializeField]
        private SlotColumn[] m_slotColumns;

        private Task m_spinTask;

        private Task<Sequence>[] m_columnSequenceTasks;
        private Task<Sequence>[] m_columnEndSequenceTasks;
        private float[] m_slideAmounts;

        public void Init()
        {
            m_eventManager = DI.Resolve<EventManager>();
            m_slotManager = DI.Resolve<SlotManager>();
            m_random = new Random();

            m_spinTask = null;
            
            m_eventManager.AddListener<SpinButtonPressedEvent>(this);
            
            for (var i = 0; i < m_slotColumns.Length; i++)
            {
                m_slotColumns[i].Initialize(SlotConfig);
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
            m_eventManager.SendEvent(ref evt);
        }
        
        private async Task SetCombination(SlotCombination combination, bool instant = false)
        {
            Debug.LogWarning($"Set Combination: {combination.SlotTypes[0]} - {combination.SlotTypes[1]} {combination.SlotTypes[2]}");
            var firstTwoSlotsEqual = combination.SlotTypes[0].Equals(combination.SlotTypes[1]);

            var columnCount = m_slotColumns.Length;

            // var spinTasks = new Task<Sequence>[m_slotColumns.Length];
            m_columnSequenceTasks = new Task<Sequence>[columnCount];
            m_columnEndSequenceTasks = new Task<Sequence>[columnCount];
            m_slideAmounts = new float[columnCount];

            var animDatas = new ColumnAnimationData[columnCount];

            for (var i = 0; i < columnCount; i++)
            {
                var column = m_slotColumns[i];

                var animationType = ColumnAnimationType.Quick;

                if (i == m_slotColumns.Length - 1 && firstTwoSlotsEqual)
                {
                    animationType = combination.IsMatch ? ColumnAnimationType.Slow : ColumnAnimationType.Normal;
                }

                var animationData = ColumnAnimationConfig.GetAnimationData(animationType);
                animDatas[i] = animationData;
                
                var slot = combination.SlotTypes[i];
                var slideCount = (SlotConfig.MarkerIndex - (int)slot) % SlotConfig.ColumnSize;
                var slideAmount = SlotConfig.VerticalOffset * slideCount;
                m_slideAmounts[i] = slideAmount;

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

                m_columnSequenceTasks[i] = SpinColumn(column, animationData, spinCount, slideAmount);
            }

            // await Task.WhenAll(spinTasks);

            await Task.Delay(8000);

            for (int i = 0; i < columnCount; i++)
            {
                m_columnSequenceTasks[i].Result.Kill();
                m_columnEndSequenceTasks[i] = EndSpin(m_slotColumns[i], animDatas[i], m_slideAmounts[i]);
            }
        }

        private async Task<Sequence> SpinColumn(SlotColumn column, ColumnAnimationData columnAnimationData, int spinCount, float slideAmount)
        {
            DOSetter<float> slideSetter = column.SetSlide;
            
            var sequence = DOTween.Sequence();
                
            sequence.Append(
                DOTween.To(() => (column.SlideAmount), 
                        slideSetter, 
                        column.SlideAmount - SlotConfig.ColumnTotalHeight * columnAnimationData.InitialSlideCount, 
                        columnAnimationData.InitialDuration)
                    .SetEase(columnAnimationData.InitialEase));
                
            sequence.AppendCallback(() => column.SetBlur(true));
                
            sequence.Append(
                DOTween.To(() => (column.SlideAmount), 
                        slideSetter, 
                        column.SlideAmount - SlotConfig.ColumnTotalHeight * spinCount, 
                        columnAnimationData.LoopDuration)
                    .SetEase(columnAnimationData.LoopEase)
                    .SetLoops(-1));
            
            // sequence.AppendCallback(() => column.SetBlur(false));
            //
            // sequence.Append(
            //     DOTween.To(() => (column.SlideAmount), 
            //             slideSetter, 
            //             slideAmount - SlotConfig.ColumnTotalHeight * columnAnimationData.StopSlideCount, 
            //             columnAnimationData.StopDuration)
            //         .SetEase(columnAnimationData.StopEase));

            // var duration = (int)((columnAnimationData.InitialDuration + columnAnimationData.LoopDuration + columnAnimationData.StopDuration) * 1000f);

            // await Task.Delay(duration);

            return sequence;
        }

        private async Task<Sequence> EndSpin(SlotColumn column, ColumnAnimationData columnAnimationData, float slideAmount)
        {
            DOSetter<float> slideSetter = column.SetSlide;
            
            var sequence = DOTween.Sequence();
            
            sequence.AppendCallback(() => column.SetBlur(false));
            
            sequence.Append(
                DOTween.To(() => (column.SlideAmount), 
                        slideSetter, 
                        slideAmount - SlotConfig.ColumnTotalHeight * columnAnimationData.StopSlideCount, 
                        columnAnimationData.StopDuration)
                    .SetEase(columnAnimationData.StopEase));

            return sequence;
        }

    }

}