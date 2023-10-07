using System;
using System.Linq;
using System.Runtime.InteropServices;
using Core.Runtime.Gameplay.Slot;
using UnityEngine;
using Random = System.Random;

namespace Core.Runtime.Solvers
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

            var iter = 0;

            while (iter < iterationLimit && CalculateLoss(in totalBlockCount, in combinationCounters) > lossThreshold)
            {
                iter++;

                var found = false;
                var target = (combinationIndex: -1, blockIndex: -1);
                
                var randCombinationIndex = random.Next(0, totalCombinationCount);
                var randBlockIndex = random.Next(0, blockCounts[randCombinationIndex]);

                const int EMPTY_BLOCK_FIND_ITER_LIMIT = 500;

                var emptyBlockIter = 0;
                while (emptyBlockIter < EMPTY_BLOCK_FIND_ITER_LIMIT)
                {
                    emptyBlockIter++;
                    
                    if (combinationCounters[randCombinationIndex].BlockCounters[randBlockIndex] == 0)
                    {
                        target = (randCombinationIndex, randBlockIndex);
                        break;
                    }
                    
                    randCombinationIndex = random.Next(0, totalCombinationCount);
                    randBlockIndex = random.Next(0, blockCounts[randCombinationIndex]);
                }

                if (emptyBlockIter == EMPTY_BLOCK_FIND_ITER_LIMIT)
                {
                    continue;
                }

                var width = blockWidths[target.combinationIndex];
                var startPos = Mathf.FloorToInt(target.blockIndex * width);
                var targetIndex = random.Next(startPos, startPos + Mathf.CeilToInt(width));

                var prevCombination = combinationIndices[targetIndex];
                var prevBlockWidth = blockWidths[prevCombination];
                var prevBlock = Mathf.FloorToInt(targetIndex / prevBlockWidth);

                combinationIndices[targetIndex] = target.combinationIndex;
                
                combinationCounters[prevCombination].AddCounter(prevBlock, -1);
                combinationCounters[target.combinationIndex].AddCounter(target.blockIndex, 1);
                
                // TO-DO: There is a bug that causes empty blocks to be overridden
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

            return result;
        }

        public static void QueueNewCombination(SlotCombinationTable table, ref SlotCombination[] current)
        {
            for (int i = 1; i < current.Length; i++)
            {
                current[i - 1] = current[i];
            }

            current[^1] = default(SlotCombination);
            
            var random = new Random();

            var totalCombinationCount = table.SlotCombinations.Count;

            var lastBlockCounts = new float[totalCombinationCount];
            var candidateCombinationIndices = new int[totalCombinationCount];
            var blockWidths = new float[totalCombinationCount];

            for (var i = 0; i < totalCombinationCount; i++)
            {
                var blockWidth = ((float)current.Length / 100f) / table.SlotCombinations[i].Probability;
                blockWidths[i] = blockWidth;

                var combinationCount = 0;
                for (int j = current.Length - 2; j > current.Length - blockWidth - 2; j--)
                {
                    if (current[j].Equals(table.SlotCombinations[i].Combination))
                    {
                        combinationCount++;
                    }
                }

                lastBlockCounts[i] = combinationCount;
            }

            var candidateCombinationIndex = Array.IndexOf(lastBlockCounts, lastBlockCounts.Min());

            current[^1] = table.SlotCombinations[candidateCombinationIndex].Combination;
        }
        
        public static float CalculateLoss(in int totalBlockCount, in CombinationCounter[] combinationCounters)
        {
            var loss = 0;
            for (var i = 0; i < combinationCounters.Length; i++)
            {
                for (int j = 0; j < combinationCounters[i].BlockCounters.Length; j++)
                {
                    loss += Math.Abs(1 - combinationCounters[i].BlockCounters[j]);
                }
            }

            var lossRatio = (float)loss / (float)totalBlockCount;

            return (float)loss / (float)totalBlockCount;
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
                Debug.LogError("This should never happen");
            }
            BlockCounters[blockIndex] += amount;
        }
    }
}