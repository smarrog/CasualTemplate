using System;

namespace Smr.Common {
    public interface ISignalBusSubscription {
        Type SignalType { get; }
        bool Equals(object obj);
        int GetHashCode();
    }
}