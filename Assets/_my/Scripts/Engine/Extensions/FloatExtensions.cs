using UnityEngine;

namespace Smr.Extensions {
    public static class FloatExtensions {
        public static bool InRange(this float value, float min, float max) {
            if (min > max) {
                Swap(ref min, ref max);
            }
            return value >= min && value <= max;
        }

        public static void Swap(ref float lhs, ref float rhs) {
            (lhs, rhs) = (rhs, lhs);
        }

        public static int ToMilliSeconds(this float value) {
            return (int)(value * 1000);
        }
        
        public static bool IsApproximatelyZero(this float value) => IsApproximatelyEqual(value, 0);
        public static bool IsApproximatelyEqual(this float value, float secondValue) => Mathf.Approximately(value, secondValue);
    }
}