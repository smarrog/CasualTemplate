using UnityEngine;

namespace Smr.Extensions {
    public static class CanvasExtensions {
        public static Vector2 WorldToCanvasAnchoredPosition(this Canvas canvas, Vector3 worldPosition) {
            if (canvas == null) {
                return Vector2.zero;
            }

            var camera = canvas.renderMode == RenderMode.ScreenSpaceCamera ? canvas.worldCamera : Camera.main;
            if (camera == null) {
                return Vector2.zero;
            }

            var viewportPosition = camera.WorldToScreenPoint(worldPosition);
            return canvas.ViewportToCanvasPosition(viewportPosition);
        }

        public static Vector3 ScreenToCanvasPosition(this Canvas canvas, Vector3 screenPosition) {
            var viewportPosition = new Vector3(screenPosition.x / Screen.width,
                screenPosition.y / Screen.height,
                0);
            return canvas.ViewportToCanvasPosition(viewportPosition);
        }

        public static Vector2 ViewportToCanvasPosition(this Canvas canvas, Vector2 viewportPosition) {
            var scaleFactor = canvas.scaleFactor;
            return new Vector2(viewportPosition.x / scaleFactor, viewportPosition.y / scaleFactor);
        }
    }
}