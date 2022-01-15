using System;
using Smr.Common;

namespace Smr.Signals {
    internal class SubscriptionId : ISignalBusSubscription {
        public Type SignalType { get; }
        private readonly object _callback;

        public SubscriptionId(Type signalType, object callback) {
            SignalType = signalType;
            _callback = callback;
        }

        protected bool Equals(SubscriptionId other) {
            return SignalType == other.SignalType && Equals(_callback, other._callback);
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) {
                return false;
            }

            if (ReferenceEquals(this, obj)) {
                return true;
            }

            if (obj.GetType() != GetType()) {
                return false;
            }

            return Equals((SubscriptionId)obj);
        }

        public override int GetHashCode() {
            unchecked {
                return ((SignalType != null ? SignalType.GetHashCode() : 0) * 397) ^
                    (_callback != null ? _callback.GetHashCode() : 0);
            }
        }
    }
}