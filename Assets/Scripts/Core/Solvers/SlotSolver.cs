using System;
using System.Linq;
using Core.Helpers;
using Core.Misc;
using Core.Runtime.Gameplay.Slot;
using UnityEngine;
using Random = System.Random;

namespace Core.Solvers
{
    public static class SlotSolver
    {
        
        /// <summary>
        /// <para><b>A genetic-inspired knapsack solver for slot combination sequencing.</b></para>
        /// <para>Starts out with generating "count" random slot combinations and calculating block counts for each
        /// combination with respect to its probability, then replaces a random combination on a random block with the
        /// lowest count in each iteration.</para>
        /// <para>Calculates a loss value with blocks that don't have exactly 1 element, and iterates until either
        /// it hits the "lossThreshold", or the "iterationLimit". In every iteration, a fallback combination is cached,
        /// and in the case of hitting the iteration limit, the fallback combination is used. </para>
        /// <para><b>Note:</b> <br></br>This solver only optimizes the probability table starting with index 0.
        /// It does not account the fact that as the game progresses, calculating remaining combination block counts
        /// will not yield optimized results. For example: for count = 100, the calculated loss for the combination
        /// index span [0,99] might be 0.02, but it's probably much higher, let's say for the span [8,107].
        /// Continuous loss calculations are not taken into account when the solver is created.</para>
        /// </summary>
        public static SlotCombination[] Solve(SlotCombinationTable table, int count, 
            int iterationLimit = 1000, float lossThreshold = .05f)
        {
            var random = new Random();

            var totalCombinationCount = table.SlotCombinations.Count;

            var result = new SlotCombination[count];
            var combinationIndices = new int[count];
            var fallbackCombinationIndices = new int[count];
            
            var blockWidths = new float[totalCombinationCount];

            for (var i = 0; i < totalCombinationCount; i++)
            {
                var blockWidth = ((float)count / 100f) / table.SlotCombinations[i].Probability;
                blockWidths[i] = blockWidth;
            }
            
            SlotHelper.FillRandom(random, ref combinationIndices, 0, totalCombinationCount);

            var combinationCounters = SlotHelper.GetCombinationCounters(in combinationIndices, table);

            var totalBlockCount = 0;
            for (int i = 0; i < combinationCounters.Length; i++)
            {
                totalBlockCount += combinationCounters[i].BlockCounters.Length;
            }

            var iter = 0;

            var fallbackLoss = float.MaxValue;
            var loss = float.MaxValue;

            while (iter < iterationLimit && loss > lossThreshold)
            {
                iter++;
                
                if (loss < fallbackLoss)
                {
                    fallbackLoss = loss;
                    
                    Array.Copy(combinationIndices, 0, fallbackCombinationIndices, 0, count);
                }
                
                var target = GetReplacementAddress(random, totalCombinationCount, in combinationCounters);
                var width = blockWidths[target.combinationIndex];
                var targetIndex = GetTargetIndex(random, target.blockIndex, width, count);

                var prevCombination = combinationIndices[targetIndex];
                var prevBlockWidth = blockWidths[prevCombination];
                var prevBlock = Mathf.FloorToInt(targetIndex / prevBlockWidth);

                combinationIndices[targetIndex] = target.combinationIndex;
                
                combinationCounters[prevCombination].AddCounter(prevBlock, -1);
                combinationCounters[target.combinationIndex].AddCounter(target.blockIndex, 1);
                
                loss = CalculateLoss(count, in combinationCounters);
            }

            if (iter == iterationLimit)
            {
                Debug.LogWarning($"Iteration limit reached!");

                Array.Copy(fallbackCombinationIndices, 0, combinationIndices, 0, count);
                loss = fallbackLoss;
            }


            for (var i = 0; i < count; i++)
            {
                result[i] = table.SlotCombinations[combinationIndices[i]].Combination;
            }

            return result;
        }

        private static int GetTargetIndex(Random random, int blockIndex, float width, int rowCount)
        {
            var startPercentage = blockIndex * width;
            var endPercentage = startPercentage + width;
            var startPos = Mathf.FloorToInt(startPercentage) + (blockIndex != 0 ? 1 : 0);
            var endPos = Mathf.CeilToInt(endPercentage);
            var targetIndex = random.Next(startPos, endPos);

            targetIndex = Mathf.Clamp(targetIndex, 0, rowCount - 1);

            return targetIndex;
        }
        
        private static (int combinationIndex, int blockIndex) GetReplacementAddress(
            Random random, int totalCombinationCount, in CombinationCounter[] combinationCounters)
        {
            var totalBlockCount = 0;
            for (int i = 0; i < combinationCounters.Length; i++)
            {
                totalBlockCount += combinationCounters[i].BlockCounters.Length;
            }
            
            var candidateAddressCounter = 0;
            
            var candidateAddresses = new (int combinationIndex, int blockIndex)[totalBlockCount];
            
            var searchAmount = int.MaxValue;

            for (int i = 0; i < totalCombinationCount; i++)
            {
                var minCount = combinationCounters[i].BlockCounters.Min();
                searchAmount = searchAmount > minCount ? minCount : searchAmount;
            }

            for (int i = 0; i < totalCombinationCount; i++)
            {
                for (int j = 0; j < combinationCounters[i].BlockCounters.Length; j++)
                {
                    if (combinationCounters[i].BlockCounters[j] == searchAmount)
                    {
                        candidateAddresses[candidateAddressCounter++] = (i, j);
                    }
                }
            }

            var target = candidateAddresses[random.Next(0, candidateAddressCounter)];

            return target;
        }
        
        public static float CalculateLoss(int rowCount, in CombinationCounter[] combinationCounters)
        {
            var expectedFillRate = (float)rowCount / 100f;
            
            var cumulativeLoss = 0f;
            for (var i = 0; i < combinationCounters.Length; i++)
            {
                for (int j = 0; j < combinationCounters[i].BlockCounters.Length; j++)
                {
                    cumulativeLoss += Math.Abs(expectedFillRate - (float)combinationCounters[i].BlockCounters[j]);
                }
            }

            var loss = (float)cumulativeLoss / 100f;

            return loss;
        }

        public static float CalculateLoss(SlotCombinationTable table, in SlotCombination[] combinations)
        {
            var rowCount = combinations.Length;

            var combinationCount = table.SlotCombinations.Count;

            var totalLoss = 0f;

            for (int i = 0; i < combinationCount; i++)
            {
                var combination = table.SlotCombinations[i].Combination;
                var probability = table.SlotCombinations[i].Probability;
                
                var bucketSize = (float)rowCount / (100f * probability);
                var expectedFillRate = (float)rowCount / 100f;

                var observedCount = 0;
                var upperBound = bucketSize;

                for (int j = 0; j < rowCount; j++)
                {
                    if (j > (int)upperBound)
                    {
                        totalLoss += Math.Abs(expectedFillRate - (float)observedCount);
                        observedCount = 0;
                        upperBound += bucketSize;
                    }
                    if (combinations[j].Equals(combination))
                    {
                        observedCount++;
                    }
                }
            }
            
            return totalLoss / (float)rowCount;
        }
    }

}