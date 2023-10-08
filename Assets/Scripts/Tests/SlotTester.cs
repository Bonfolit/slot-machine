using System;
using System.Diagnostics;
using Core.Runtime.Gameplay;
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
            
            var totalCombinationCount = table.SlotCombinations.Count;

            var combinationCounters = new CombinationCounter[totalCombinationCount];
            var blockWidths = new float[totalCombinationCount];

            for (var i = 0; i < totalCombinationCount; i++)
            {
                var blockWidth = ((float)ROW_COUNT / 100f) / table.SlotCombinations[i].Probability;
                blockWidths[i] = blockWidth;
            }
            
            for (int i = 0; i < ROW_COUNT; i++)
            {
                var combinationIndex = table.GetSlotCombinationIndex(in result[i]);
                var width = blockWidths[combinationIndex];
                var blockIndex = (int)((float)i / width);

                if (combinationCounters[combinationIndex].BlockCounters == null)
                {
                    combinationCounters[combinationIndex].BlockCounters = new int[Mathf.RoundToInt(table.SlotCombinations[combinationIndex].Probability * ROW_COUNT)];
                }
                
                combinationCounters[combinationIndex].AddCounter(blockIndex, 1);
            }
            
            var totalBlockCount = 0;

            for (int i = 0; i < combinationCounters.Length; i++)
            {
                totalBlockCount += combinationCounters[i].BlockCounters.Length;
            }

            var loss = 0f;
            SlotSolver.CalculateLoss(ref loss, in totalBlockCount, in combinationCounters);

            if (loss >= LOSS_LIMIT)
            {
                Debug.LogWarning("Test Failed");
            }

            
            // for (int i = 0; i < result.Length; i++)
            // {
            //     Debug.Log($"Row: {i}, Combination: {result[i].SlotTypes[0]}, {result[i].SlotTypes[1]}, {result[i].SlotTypes[2]}");
            // }

            Debug.LogWarning($"Total Block Count: {totalBlockCount}");
            Debug.LogWarning($"Loss: {loss}");
            Assert.Less(loss, LOSS_LIMIT);
        }

        [Test]
        public void TestSuccessiveCombinationGeneration()
        {
            const int SUCCESSION_COUNT = 200;
            const int ROW_COUNT = 100;
            const int ITERATION_LIMIT = 10000;
            const float LOSS_THRESHOLD = 0.01f;
            const float LOSS_LIMIT = 0.5f;

            var table = Resources.Load<SlotCombinationTable>("Data/SlotCombinationTable");
            var result = SlotSolver.Solve(table, ROW_COUNT, ITERATION_LIMIT, LOSS_THRESHOLD);

            var totalCombinationCount = table.SlotCombinations.Count;

            var combinationCounters = new CombinationCounter[totalCombinationCount];
            var blockWidths = new float[totalCombinationCount];

            for (var i = 0; i < totalCombinationCount; i++)
            {
                var blockWidth = ((float)ROW_COUNT / 100f) / table.SlotCombinations[i].Probability;
                blockWidths[i] = blockWidth;
            }
            
            for (int i = 0; i < ROW_COUNT; i++)
            {
                var combinationIndex = table.GetSlotCombinationIndex(in result[i]);
                var width = blockWidths[combinationIndex];
                var blockIndex = (int)((float)i / width);

                if (combinationCounters[combinationIndex].BlockCounters == null)
                {
                    combinationCounters[combinationIndex].BlockCounters = new int[Mathf.RoundToInt(table.SlotCombinations[combinationIndex].Probability * ROW_COUNT)];
                }
                
                combinationCounters[combinationIndex].AddCounter(blockIndex, 1);
            }
            
            var totalBlockCount = 0;

            for (int i = 0; i < combinationCounters.Length; i++)
            {
                totalBlockCount += combinationCounters[i].BlockCounters.Length;
            }
            
            var testLoss = 0f;
            SlotSolver.CalculateLoss(ref testLoss, in totalBlockCount, in combinationCounters);
            
            

            for (int i = 0; i < SUCCESSION_COUNT; i++)
            {
                SlotSolver.QueueNewCombination(table, ref result);

                for (int j = 0; j < totalCombinationCount; j++)
                {
                    for (var k = 0; k < combinationCounters[j].BlockCounters.Length; k++)
                    {
                        combinationCounters[j].BlockCounters[k] = 0;
                    }
                }
                
                for (int j = 0; j < ROW_COUNT; j++)
                {
                    var combinationIndex = table.GetSlotCombinationIndex(in result[j]);
                    var width = blockWidths[combinationIndex];
                    var blockIndex = (int)((float)j / width);
                
                    combinationCounters[combinationIndex].AddCounter(blockIndex, 1);
                }

                var loss = 0f;
                SlotSolver.CalculateLoss(ref loss, in totalBlockCount, in combinationCounters);

                Assert.Less(loss, LOSS_LIMIT);
            }
        }
        
        
    }

}