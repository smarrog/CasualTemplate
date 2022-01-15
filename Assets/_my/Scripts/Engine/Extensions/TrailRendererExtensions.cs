using UnityEngine;

namespace Smr.Extensions {
    public static class TrailRendererExtensions {
        public static TrailRenderer SetColorWithoutChangingAlpha(this TrailRenderer renderer, Color color) {
            renderer.startColor = color.WithA(renderer.startColor.a);
            renderer.endColor = color.WithA(renderer.endColor.a);
            return renderer;
        }
    }
}