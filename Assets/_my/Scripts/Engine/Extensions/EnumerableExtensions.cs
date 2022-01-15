using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Random = UnityEngine.Random;

namespace Smr.Extensions {
    public static class EnumerableExtension {
        public static void ForEach<T>(this IReadOnlyList<T> list, Action<T, int> action) {
            for (int i = 0; i < list.Count; i++) {
                action(list[i], i);
            }
        }

        public static bool IsFilled<T>(this IReadOnlyList<T> list) {
            return list is { Count: > 0 };
        }

        public static bool IsEmpty<T>(this IReadOnlyList<T> list) {
            return list == null || list.Count == 0;
        }
        
        public static void ForEach<T>(this IEnumerable<T> enumeration, Action<T> action) {
            foreach (var element in enumeration) {
                action(element);
            }
        }

        public static T FirstOr<T>(this IEnumerable<T> enumeration, Func<T, bool> predicate, Func<T> factoryMethod) {
            foreach (var element in enumeration) {
                if (predicate(element)) {
                    return element;
                }
            }

            return factoryMethod();
        }

        public static void Shuffle<T>(this IList<T> list) {
            if (list == null) {
                return;
            }

            for (var i = 0; i < list.Count; i++) {
                var temp = list[i];
                var randomIndex = Random.Range(i, list.Count);
                list[i] = list[randomIndex];
                list[randomIndex] = temp;
            }
        }

        public static T GetRandom<T>(this IReadOnlyList<T> list) {
            var wanted = default(T);
            if (list.IsEmpty()) {
                return wanted;
            }

            var count = list.Count;
            if (count == 0) {
                return wanted;
            }

            var i = Random.Range(0, count);
            return list[i];
        }

        public static T GetRandom<T>(this IReadOnlyList<T> list, Func<T, bool> condition) {
            if (list.IsEmpty()) {
                return default;
            }

            var filteredList = list.Where(condition).ToList();
            return filteredList.GetRandom();
        }
        
        public static T GetRandomNotNull<T>(this IReadOnlyList<T> list) {
            return list.GetRandom(element => element != null);
        }

        public static void AddIfNewAndNotNull<T>(this IList<T> list, T value) {
            if (!typeof(T).IsValueType && Equals(value, null)) {
                return;
            }

            if (list == null) {
                return;
            }

            if (!list.Contains(value)) {
                list.Add(value);
            }
        }

        public static void AddRangeNewNotNull<T>(this IList<T> list, IEnumerable<T> collection) {
            if (list == null) {
                return;
            }

            foreach (T element in collection) {
                list.AddIfNewAndNotNull(element);
            }
        }

        public static IEnumerable<T> Clone<T>(this IEnumerable<T> list) where T : ICloneable {
            return list?
                .Select(item => item == null ? default : (T)item.Clone());
        }

        public static void RemoveWhere<T>(this IList<T> list, Func<T, bool> predicate) {
            for (int i = 0; i < list.Count; i++) {
                if (predicate(list[i])) {
                    list.RemoveAt(i);
                    i--;
                }
            }
        }

        public static T GetAndRemove<T>(this IList<T> list, int index) {
            var value = list[index];
            list.RemoveAt(index);
            return value;
        }

        public static void MoveToEnd<T>(this IList<T> list, int index) {
            var value = list.GetAndRemove(index);
            list.Add(value);
        }

        public static bool IndexIsOutOfBounds<T>(this IReadOnlyList<T> list, int index) {
            return list == null || index < 0 || index >= list.Count;
        }

        public static bool UnorderedEqualTo<T>(this IReadOnlyList<T> seq1, List<T> seq2) {
            if (seq1 == null && seq2 == null) {
                return true;
            }

            if (seq1 == null || seq2 == null) {
                return false;
            }

            return seq1.Count == seq2.Count && seq1.All(seq2.Contains);
        }
        
        public static bool EqualTo<T>(this IReadOnlyList<T> lhs, IReadOnlyList<T> rhs) {
            if (lhs == rhs) {
                return true;
            }

            if (lhs == null || rhs == null) {
                return false;
            }

            if (lhs.Count != rhs.Count) {
                return false;
            }

            for (var i = 0; i < rhs.Count; ++i) {
                var lhsValue = lhs[i];
                var rhsValue = rhs[i];
                if (!lhsValue.Equals(rhsValue)) {
                    return false;
                }
            }

            return true;
        }

        public static T GetAtOrDefault<T>(this IReadOnlyList<T> list, int index) {
            return list != null && index >= 0 && index < list.Count ? list[index] : default;
        }

        public static T GetAtOrLast<T>(this IReadOnlyList<T> list, int index) {
            if (list.IsEmpty()) {
                throw new Exception("No last element");
            }

            return index >= list.Count ? list.Last() : list[index];
        }

        public static T GetAtInCircle<T>(this IReadOnlyList<T> list, int index) {
            if (list.IsEmpty() || index < 0) {
                return default;
            }
            index %= list.Count;
            return list[index];
        }

        public static string JoinToString(this IEnumerable seq, string delimiter = null, bool skipEmpty = false) {
            if (seq == null) {
                return "null";
            }

            var sb = new StringBuilder();

            var n = seq.GetEnumerator();
            var i = 0;
            while (n.MoveNext()) {
                if (i > 0 && delimiter.IsFilled()) {
                    sb.Append(delimiter);
                }

                if (skipEmpty && n.Current == null) {
                    continue;
                }
                var value = n.Current;
                if (value == null) {
                    sb.Append("null");
                } else {
                    sb.Append(n.Current);
                }

                i++;
            }

            return sb.ToString();
        }
        
        public static string ToDebugString<T>(this IEnumerable<T> list) {
            const string separator = ",\n"; // почему-то генерик не распознаётся с аргументом по умолчанию

            if (list == null) {
                return "null";
            }

            return "[" + string.Join(separator, list) + "]";
        }
    }
}