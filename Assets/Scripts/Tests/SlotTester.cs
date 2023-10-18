using System;
using System.Diagnostics;
using Core.Helpers;
using Core.Runtime.Gameplay.Slot;
using Core.Solvers;
using NUnit.Framework;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Tests
{

    public class SlotTester
    {
        [Test]
        public void TestSlotCombinationGeneration()
        {
            Debug.LogWarning("--Test Slot Combination Generation--");
            
            const int ROW_COUNT = 100;
            const int ITERATION_LIMIT = 10000;
            const float LOSS_THRESHOLD = 0.01f;
            
            var table = Resources.Load<SlotCombinationTable>("Data/SlotCombinationTable");

            var watch = new Stopwatch();
            watch.Start();
            var result = SlotSolver.Solve(table, ROW_COUNT, ITERATION_LIMIT, LOSS_THRESHOLD);

            Debug.LogWarning($"Time Elapsed(ms): {watch.ElapsedMilliseconds}");
            watch.Stop();

            var combinationCounters = SlotHelper.GetCombinationCounters(in result, table);
            
            var totalBlockCount = 0;

            for (int i = 0; i < combinationCounters.Length; i++)
            {
                totalBlockCount += combinationCounters[i].BlockCounters.Length;
            }

            var loss = SlotSolver.CalculateLoss(ROW_COUNT, in combinationCounters);

            Assert.Less(loss, LOSS_THRESHOLD);
        }

        [Test]
        public void ValidateLossCalculation()
        {
            const int ROW_COUNT = 100;
            const int ITERATION_LIMIT = 10000;
            const float LOSS_THRESHOLD = 0.01f;
            const float LOSS_ERROR_MARGIN = 0.01f;
            
            var table = Resources.Load<SlotCombinationTable>("Data/SlotCombinationTable");
            var result = SlotSolver.Solve(table, ROW_COUNT, ITERATION_LIMIT, LOSS_THRESHOLD);

            var combinationCounters = SlotHelper.GetCombinationCounters(in result, table);
            
            var totalBlockCount = 0;

            for (int i = 0; i < combinationCounters.Length; i++)
            {
                totalBlockCount += combinationCounters[i].BlockCounters.Length;
            }

            var loss = SlotSolver.CalculateLoss(ROW_COUNT, in combinationCounters);

            var validatedLoss = SlotSolver.CalculateLoss(table, in result);
            Debug.LogWarning($"Calculated loss: {loss}, Validated loss: {validatedLoss}");

            Assert.Less(Math.Abs(loss - validatedLoss), LOSS_ERROR_MARGIN);
        }
    }

}