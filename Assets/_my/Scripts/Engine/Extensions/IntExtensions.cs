using UnityEngine;

namespace Smr.Extensions {
    public static class IntExtensions {
        public static bool InRange(this int value, Vector2Int range) {
            return value.InRange(range.x, range.y);
        }

        public static bool InRange(this int value, int min, int max) {
            if (min > max) {
                Swap(ref min, ref max);
            }
            return value >= min && value <= max;
        }

        public static void Swap(ref int lhs, ref int rhs) {
            (lhs, rhs) = (rhs, lhs);
        }
    }
}