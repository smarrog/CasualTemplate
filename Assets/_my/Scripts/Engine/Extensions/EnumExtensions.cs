using System;
using System.Collections.Generic;
using System.Linq;

namespace Smr.Extensions {
    public static class EnumExtensions {
        public static IEnumerable<T> GetAllValues<T>(bool excludeDefault = false) {
            var values = (T[]) Enum.GetValues(typeof(T));
            return excludeDefault ? values.Except(Enumerable.Repeat(default(T), 1)) : values;
        }
        
        public static T Next<T>(this T src) where T : struct {
            if (!typeof(T).IsEnum) {
                throw new ArgumentException($"Argument {typeof(T).FullName} is not an Enum");
            }

            var values = GetAllValues<T>().ToArray();
            var index = Array.IndexOf(values, src) + 1;
            return (index == values.Length) ? values[0] : values[index];
        }

        public static T Prev<T>(this T src) where T : struct {
            if (!typeof(T).IsEnum) {
                throw new ArgumentException($"Argument {typeof(T).FullName} is not an Enum");
            }

            var values = GetAllValues<T>().ToArray();
            var index = Array.IndexOf(values, src) - 1;
            return (index == -1) ? values[^1] : values[index];
        }
    }
}