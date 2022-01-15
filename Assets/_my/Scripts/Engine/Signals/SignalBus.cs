using System;
using System.Collections.Generic;
using Smr.Common;
using UnityEngine.Scripting;

namespace Smr.Signals {
    public class SignalBus : ISignalBus {
        private static readonly Dictionary<string, SignalBus> _buses = new();

        public static SignalBus GetBus(string id = null) {
            id ??= DEFAULT;
            if (!_buses.TryGetValue(id, out var bus)) {
                _buses[id] = bus = new SignalBus();
            }
            return bus;
        }

        private const string DEFAULT = "default";
        public static SignalBus Default => GetBus(DEFAULT);

        private readonly Dictionary<Type, List<Action<object>>> _declarationMap = new();
        private readonly Dictionary<ISignalBusSubscription, Action<object>> _subscriptionMap = new();
        
        private SignalBus() { }

        public void Reset() {
            _subscriptionMap.Clear();
            _declarationMap.Clear();
        }

        public void Fire<TSignal>(TSignal signal) {
            var signalType = typeof(TSignal);
            if (!_declarationMap.TryGetValue(signalType, out var value)) {
                return;
            }
            foreach (var action in value.ToArray()) {
                action?.Invoke(signal);
            }
        }

        public ISignalBusSubscription Subscribe<TSignal>(Action<TSignal> callback) {
            if (callback == null) {
                return null;
            }

            var signalType = typeof(TSignal);
            var subscriptionId = new SubscriptionId(signalType, callback);
            if (_subscriptionMap.ContainsKey(subscriptionId)) {
                return subscriptionId;
            }

            void WrapperCallback(object args) => callback((TSignal)args);
            Action<object> wrapperCallBack = WrapperCallback;
            _subscriptionMap[subscriptionId] = wrapperCallBack;

            if (!_declarationMap.ContainsKey(signalType)) {
                _declarationMap[signalType] = new List<Action<object>>();
            }
            _declarationMap[signalType].Add(wrapperCallBack);
            return subscriptionId;
        }

        public void Unsubscribe<TSignal>(Action<TSignal> callback) {
            if (callback == null) {
                return;
            }

            var signalType = typeof(TSignal);
            var subscriptionId = new SubscriptionId(signalType, callback);
            Unsubscribe(subscriptionId);
        }

        public void Unsubscribe(ISignalBusSubscription subscriptionId) {
            if (subscriptionId == null) {
                return;
            }

            var signalType = subscriptionId.SignalType;
            if (!_subscriptionMap.TryGetValue(subscriptionId, out var subscription) || !_declarationMap.TryGetValue(signalType, out var declarations)) {
                return;
            }

            if (!declarations.Remove(subscription)) {
                return;
            }

            _subscriptionMap.Remove(subscriptionId);
            if (declarations.Count == 0) {
                _declarationMap.Remove(signalType);
            }
        }
    }
}