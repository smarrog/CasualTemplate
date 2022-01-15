using UnityEngine;
using UnityEngine.UI;

namespace Smr.Extensions {
    public static class ScrollRectExtensions {
        public static Vector2 GetSnapToPositionToBringChildIntoView(this ScrollRect instance, RectTransform child) {
            Canvas.ForceUpdateCanvases();
            var viewport = instance.viewport;
            var viewportLocalPosition = viewport ? viewport.localPosition : instance.GetComponent<RectTransform>().localPosition;
            var childLocalPosition = child.localPosition;
            return new Vector2(
                0 - (viewportLocalPosition.x + childLocalPosition.x),
                0 - (viewportLocalPosition.y + childLocalPosition.y)
            );
        }
    }
}