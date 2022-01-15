using UnityEngine;

namespace Smr.Extensions {
    public static class VectorExtensions {
        public static float GetRandom(this Vector2 v) {
            return Random.Range(v.x, v.y);
        }
        
        public static Vector2 WithX(this Vector2 v, float value) {
            return new Vector2(value, v.y);
        }

        public static Vector2 WithY(this Vector2 v, float value) {
            return new Vector2(v.x, value);
        }
        
        public static Vector3 WithX(this Vector3 v, float value) {
            return new Vector3(value, v.y, v.z);
        }

        public static Vector3 WithY(this Vector3 v, float value) {
            return new Vector3(v.x, value, v.z);
        }

        public static Vector3 WithZ(this Vector3 v, float value) {
            return new Vector3(v.x, v.y, value);
        }

        public static bool ApproximatelyEqualsTo(this Vector3 lhs, Vector3 rhs) {
            return Mathf.Approximately(lhs.x, rhs.x) &&
                Mathf.Approximately(lhs.y, rhs.y) &&
                Mathf.Approximately(lhs.z, rhs.z);
        }

        public static Vector3 HalfDistanceTo(this Vector3 lhs, Vector3 rhs) {
            return new Vector3(
                lhs.x + (rhs.x - lhs.x) / 2,
                lhs.y + (rhs.y - lhs.y) / 2,
                lhs.y + (rhs.z - lhs.z) / 2
            );
        }
    }
}