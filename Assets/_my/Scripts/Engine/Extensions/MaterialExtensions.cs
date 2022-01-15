using UnityEngine;

namespace Smr.Extensions {
    public static class MaterialExtensions {
        public static Color? GetPropColor(this Material mat, string propName) {
            if (mat.HasProperty(propName)) {
                return mat.GetColor(propName);
            }
            return null;
        }

        public static Color? GetPropColor(this Material mat, int propHash) {
            if (mat.HasProperty(propHash)) {
                return mat.GetColor(propHash);
            }
            return null;
        }

        public static float? GetPropAlpha(this Material mat, string propName) {
            return mat.GetPropColor(propName)?.a ?? 0;
        }

        public static float? GetPropAlpha(this Material mat, int propHash) {
            return mat.GetPropColor(propHash)?.a ?? 0;
        }

        public static void SetPropAlpha(this Material mat, string propName, float value) {
            var color = mat.GetPropColor(propName);
            if (color.HasValue) {
                mat.SetColor(propName, color.Value.WithA(value));
            }
        }

        public static void SetPropAlpha(this Material mat, int propHash, float value) {
            var color = mat.GetPropColor(propHash);
            if (color.HasValue) {
                mat.SetColor(propHash, color.Value.WithA(value));
            }
        }
    }
}