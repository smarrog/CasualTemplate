using System;
using System.Collections.Generic;
using System.Linq;

namespace Smr.Extensions {
    public static class DictionaryExtensions {
        public static string ToDebugString<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dict) {
            const string separator = ",\n"; // почему-то генерик не распознаётся с аргументом по умолчанию

            if (dict == null) {
                return "null";
            }

            return "{" + string.Join(separator, dict.Select(kv => kv.Key + "=" + kv.Value).ToArray()) + "}";
        }
        
        public static bool EqualTo<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> lhs, IReadOnlyDictionary<TKey, TValue> rhs) {
            if (lhs == rhs) {
                return true;
            }

            if (lhs == null || rhs == null) {
                return false;
            }

            if (lhs.Count != rhs.Count) {
                return false;
            }

            foreach (var pair in rhs) {
                if (!lhs.ContainsKey(pair.Key)) {
                    return false;
                }

                if (!pair.Value.Equals(lhs[pair.Key])) {
                    return false;
                }
            }

            return true;
        }

        public static void RemoveByKeyCondition<TKey, TValue>(this IDictionary<TKey, TValue> dict, Func<TKey, bool> predicate) {
            dict.Keys
                .Where(predicate)
                .ToList()
                .ForEach(key => dict.Remove(key));
        }
        
        public static void SetValue<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, TValue value) {
            if (dict == null || key == null) {
                return;
            }

            if (dict.ContainsKey(key)) {
                dict[key] = value;
            } else {
                dict.Add(key, value);
            }
        }

        public static void AddInnerValue<TKey, TInner, TValue>(this IDictionary<TKey, TInner> dict, TKey key, TValue value) where TInner : ICollection<TValue> {
            if (dict == null) {
                return;
            }
            if (!dict.ContainsKey(key)) {
                dict[key] = default;
            }
            dict[key].Add(value);
        }
        
        public static void RemoveInnerValue<TKey, TInner, TValue>(this IDictionary<TKey, TInner> dict, TKey key, TValue value) where TInner : ICollection<TValue> {
            if (dict == null || !dict.ContainsKey(key)) {
                return;
            }
            dict[key].Remove(value);
        }

        public static void RemoveKeys<TKey, TValue>(this IDictionary<TKey, TValue> dict, IEnumerable<TKey> keys) {
            foreach (var key in keys) {
                dict.Remove(key);
            }
        }
        
        public static TKey GetValueKey<TKey, TValue>(this IDictionary<TKey, TValue> dict, TValue searchValue) {
            foreach ((TKey key, TValue value) in dict) {
                if (value.Equals(searchValue)) {
                    return key;
                }
            }
            return default;
        }

        public static void Append<TKey, TValue>(this IDictionary<TKey, TValue> dict, IReadOnlyDictionary<TKey, TValue> rhs) {
            if (rhs == null) {
                return;
            }
            
            foreach (var kv in rhs) {
                dict[kv.Key] = kv.Value;
            }
        }
    }
}