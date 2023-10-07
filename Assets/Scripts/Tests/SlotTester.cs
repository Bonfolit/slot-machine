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
            const int ROW_COUNT = 100;
            const int ITERATION_LIMIT = 10000;
            const float LOSS_THRESHOLD = 0.05f;
            const float LOSS_LIMIT = 0.3f;
            
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

            var loss = SlotSolver.CalculateLoss(in totalBlockCount, in combinationCounters);

            Assert.Less(loss, LOSS_LIMIT);
            
            Debug.LogWarning($"Loss: {loss}");
            
            for (int i = 0; i < result.Length; i++)
            {
                Debug.Log($"Row: {i}, Combination: {result[i].SlotTypes[0]}, {result[i].SlotTypes[1]}, {result[i].SlotTypes[2]}");
            }
        }
        
        
    }

}