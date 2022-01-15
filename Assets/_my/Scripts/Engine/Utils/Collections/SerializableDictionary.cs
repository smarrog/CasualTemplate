using System;
using System.Collections.Generic;
using UnityEngine;

namespace Smr.Utils {
    [Serializable]
    public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver {
        [SerializeField] private List<SerializableKeyValuePair<TKey, TValue>> _serializedValues = new List<SerializableKeyValuePair<TKey, TValue>>();

        public void OnBeforeSerialize() {
            _serializedValues.Clear();
            
            foreach (var (key, value) in this) {
                _serializedValues.Add(new SerializableKeyValuePair<TKey, TValue>(key, value));
            }
        }

        public void OnAfterDeserialize() {
            Clear();
            foreach (var (key, value) in _serializedValues) {
                Add(key, value);
            }
        }
    }

    [Serializable]
    public class SerializableKeyValuePair<TKey, TValue> {
        public TKey Key;
        public TValue Value;

        public SerializableKeyValuePair(TKey key, TValue value) {
            Key = key;
            Value = value;
        }

        public void Deconstruct(out TKey key, out TValue value) {
            key = Key;
            value = Value;
        }
    }
}