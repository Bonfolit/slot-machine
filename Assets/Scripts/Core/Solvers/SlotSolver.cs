using System;
using System.Linq;
using System.Runtime.InteropServices;
using Core.Runtime.Gameplay.Slot;
using UnityEngine;
using Random = System.Random;

namespace Core.Solvers
{
    public static class SlotSolver
    {
        /// <summary>
        /// A genetic-inspired knapsack solver for slot combination sequencing
        /// </summary>
        public static SlotCombination[] Solve(SlotCombinationTable table, int count, 
            int iterationLimit = 1000, float lossThreshold = .05f)
        {
            var random = new Random();

            var totalCombinationCount = table.SlotCombinations.Count;

            var result = new SlotCombination[count];

            var bestCombinationIndices = new int[count];
            
            var combinationIndices = new int[count];
            var blockWidths = new float[totalCombinationCount];
            var combinationCounters = new CombinationCounter[totalCombinationCount];

            for (var i = 0; i < totalCombinationCount; i++)
            {
                var blockWidth = ((float)count / 100f) / table.SlotCombinations[i].Probability;
                blockWidths[i] = blockWidth;
            }
            
            for (int i = 0; i < count; i++)
            {
                var combinationIndex = random.Next(0, totalCombinationCount);
                
                combinationIndices[i] = combinationIndex;

                var width = blockWidths[combinationIndex];

                var blockIndex = (int)((float)i / width);

                if (combinationCounters[combinationIndex].BlockCounters == null)
                {
                    combinationCounters[combinationIndex].BlockCounters = new int[Mathf.RoundToInt(table.SlotCombinations[combinationIndex].Probability * count)];
                    Array.Fill(combinationCounters[combinationIndex].BlockCounters, 0); 
                }
                
                combinationCounters[combinationIndex].AddCounter(blockIndex, 1);
            }

            var blockCounts = new int[totalCombinationCount];
            for (var i = 0; i < blockCounts.Length; i++)
            {
                blockCounts[i] = combinationCounters[i].BlockCounters.Length;
            }

            var totalBlockCount = 0;

            for (int i = 0; i < combinationCounters.Length; i++)
            {
                totalBlockCount += combinationCounters[i].BlockCounters.Length;
            }

            var candidateAddresses = new (int combinationIndex, int blockIndex)[totalBlockCount];
            var candidateAddressCount = 0;

            var iter = 0;

            var candidateLoss = float.MaxValue;
            var loss = float.MaxValue;

            while (iter < iterationLimit && CalculateLoss(ref loss, in totalBlockCount, in combinationCounters) > lossThreshold)
            {
                iter++;
                
                candidateAddressCount = 0;

                if (loss < candidateLoss)
                {
                    candidateLoss = loss;
                    
                    Array.Copy(combinationIndices, 0, bestCombinationIndices, 0, count);
                }

                var target = (combinationIndex: -1, blockIndex: -1);

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
                            candidateAddresses[candidateAddressCount++] = (i, j);
                        }
                    }
                }

                target = candidateAddresses[random.Next(0, candidateAddressCount)];
                
                var width = blockWidths[target.combinationIndex];
                var startPercentage = target.blockIndex * width;
                var endPercentage = startPercentage + width;
                var startPos = Mathf.FloorToInt(startPercentage) + (target.blockIndex != 0 ? 1 : 0);
                var endPos = Mathf.CeilToInt(endPercentage);
                var targetIndex = random.Next(startPos, endPos);

                targetIndex = Mathf.Clamp(targetIndex, 0, count - 1);

                var prevCombination = combinationIndices[targetIndex];
                var prevBlockWidth = blockWidths[prevCombination];
                var prevBlock = Mathf.FloorToInt(targetIndex / prevBlockWidth);

                combinationIndices[targetIndex] = target.combinationIndex;
                
                combinationCounters[prevCombination].AddCounter(prevBlock, -1);
                combinationCounters[target.combinationIndex].AddCounter(target.blockIndex, 1);
                
                // TO-DO: There is a bug that causes empty blocks to be overridden
            }

            if (iter == iterationLimit)
            {
                Array.Copy(bestCombinationIndices, 0, combinationIndices, 0, count);
                loss = candidateLoss;
            }


            for (var i = 0; i < count; i++)
            {
                result[i] = table.SlotCombinations[combinationIndices[i]].Combination;
            }
            
            for (var i = 0; i < blockWidths.Length; i++)
            {
                Debug.Log($"Block Width {i}: {blockWidths[i]}");
            }
            
            for (var i = 0; i < combinationCounters.Length; i++)
            {
                for (var j = 0; j < combinationCounters[i].BlockCounters.Length; j++)
                {
                    Debug.Log($"Combination: {i}, Block: {j}, Count: {combinationCounters[i].BlockCounters[j]}");
                }
            }

            Debug.LogWarning($"Iteration reached: {iter}");
            Debug.LogWarning($"Loss in Solver: {loss}");

            return result;
        }
        
        public static float CalculateLoss(ref float loss, in int totalBlockCount, in CombinationCounter[] combinationCounters)
        {
            loss = 0f;
            
            var cumulativeLoss = 0;
            for (var i = 0; i < combinationCounters.Length; i++)
            {
                for (int j = 0; j < combinationCounters[i].BlockCounters.Length; j++)
                {
                    cumulativeLoss += Math.Abs(1 - combinationCounters[i].BlockCounters[j]);
                }
            }

            loss = (float)cumulativeLoss / (float)totalBlockCount;

            return loss;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack=1)]
    public struct CombinationCounter
    {
        public int[] BlockCounters;

        public void AddCounter(int blockIndex, int amount)
        {
            if (BlockCounters[blockIndex] == 0 && amount < 0)
            {
                Debug.LogWarning("This should never happen");
            }
            BlockCounters[blockIndex] += amount;
        }

        public void Reset()
        {
            for (var i = 0; i < BlockCounters.Length; i++)
            {
                BlockCounters[i] = 0;
            }
        }
    }
}