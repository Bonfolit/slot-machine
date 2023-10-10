﻿using System.Diagnostics;
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
            const int ITERATION_LIMIT = 50000;
            const float LOSS_THRESHOLD = 0.01f;
            const float LOSS_LIMIT = 0.1f;
            
            var table = Resources.Load<SlotCombinationTable>("Data/SlotCombinationTable");

            var watch = new Stopwatch();
            watch.Start();
            var result = SlotSolver.Solve(table, ROW_COUNT, ITERATION_LIMIT, LOSS_THRESHOLD);

            Debug.LogWarning($"Time Elapsed: {watch.ElapsedMilliseconds}");
            watch.Stop();

            var combinationCounters = SlotHelper.GetCombinationCounters(in result, table);
            
            var totalBlockCount = 0;

            for (int i = 0; i < combinationCounters.Length; i++)
            {
                totalBlockCount += combinationCounters[i].BlockCounters.Length;
            }

            var loss = 0f;
            SlotSolver.CalculateLoss(ref loss, in totalBlockCount, in combinationCounters);

            Assert.Less(loss, LOSS_LIMIT);
        }
    }

}