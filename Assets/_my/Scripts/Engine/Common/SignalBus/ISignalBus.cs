using System;

namespace Smr.Common {
    public interface ISignalBus {
        void Fire<TSignal>(TSignal signal);
        ISignalBusSubscription Subscribe<TSignal>(Action<TSignal> callback);
        void Unsubscribe<TSignal>(Action<TSignal> callback);
        void Unsubscribe(ISignalBusSubscription subscriptionId);
    }
}