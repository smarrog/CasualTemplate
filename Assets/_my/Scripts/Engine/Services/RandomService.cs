using System;
using System.Collections.Generic;
using System.Linq;
using Smr.Common;

namespace Smr.Services {
    public class RandomService : IRandomService {
        private const int DEFAULT_SEED = 42;
        
        private readonly Random _rng;
        
        public int Seed { get; }
        
        public RandomService(int seed) {
            Seed = seed == 0 ? DEFAULT_SEED : seed;
            _rng = new Random(Seed);
        }

        public bool CheckLuck(float chance) {
            return _rng.NextDouble() < chance;
        }
        
        public int NextInt(int max) {
            return _rng.Next(max);
        }

        public int Range(int min, int max) => 
            _rng.Next(min, max);

        public T ChooseRandom<T>(IList<T> list) {
            return list[NextInt(list.Count)];
        }

        public void Shuffle<T>(IList<T> list) {
            for (var i = list.Count - 1; i > 1; --i) {
                var k = NextInt(i + 1);
                (list[k], list[i]) = (list[i], list[k]);
            }
        }

        public T WeightedRandom<T>(IList<T> list) where T : IWeighted {
            if (list.Count == 0) {
                throw new ArgumentException("The list must not be null or empty.", nameof(list));
            }

            if (list.Count == 1) {
                return list[0];
            }

            var totalWeight = list.Sum(c => c.Weight);
            if (totalWeight == 0) {
                throw new ArgumentException("The total weight of all items must be greater than zero.", nameof(list));
            }
            
            int randomNumber = _rng.Next(totalWeight);
            int cumulativeWeight = 0;
            
            foreach (var item in list) {
                cumulativeWeight += item.Weight;
                if (randomNumber < cumulativeWeight) {
                    return item;
                }
            }

            // In case something goes wrong, return the last item (shouldn't normally reach here)
            return list[^1];
        }

        public T WeightedRandom<T>(IList<int> weights, IList<T> collection) {
            if (collection.Count == 0 || collection.Count != weights.Count) {
                return default;
            }

            int totalWeight = 0;
            
            foreach (var weight in weights) {
                totalWeight += weight;
            }

            int choice = _rng.Next(totalWeight);
            int cumulativeWeight = 0;

            for (int i = 0; i < collection.Count; i++) {
                if (weights[i] == 0) {
                    continue;
                }
                
                cumulativeWeight += weights[i];

                if (cumulativeWeight >= choice) {
                    return collection[i];
                }
            }

            return collection[0];
        }
    }
}