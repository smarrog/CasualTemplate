using System.Collections.Generic;
using Smr.Common;

namespace Smr.Services {
    public interface IRandomService {
        int Seed { get; }
        
        bool CheckLuck(float chance);
        
        int NextInt(int max);
        int Range(int min, int max);
        
        T WeightedRandom<T>(IList<T> list) where T : IWeighted;
        T WeightedRandom<T>(IList<int> weights, IList<T> collection);
        T ChooseRandom<T>(IList<T> list);
        void Shuffle<T>(IList<T> list);
    }
}