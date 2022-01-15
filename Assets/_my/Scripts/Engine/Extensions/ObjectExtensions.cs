using UnityEngine;

namespace Smr.Extensions {
    public static class ObjectExtensions {
        public static bool IsNull(this Object obj) {
            return ReferenceEquals(obj, null);
        }

        public static bool IsNotNull(this Object obj) {
            return !obj.IsNull();
        }
    }
}