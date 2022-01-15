using UnityEngine;

namespace Smr.Extensions {
    public static class RectExtensions {

        private const float SPLIT_SPACE = 5;

        public static bool IsEmpty(this Rect rect) {
            return Mathf.Approximately(rect.width, 0) && Mathf.Approximately(rect.height, 0);
        }

        public static bool IsZero(this Rect rect) {
            return Mathf.Approximately(rect.x, 0) && Mathf.Approximately(rect.y, 0) &&
                Mathf.Approximately(rect.width, 0) && Mathf.Approximately(rect.height, 0);
        }

        public static (Rect left, Rect right) SplitCenter(this Rect rect) => rect.SplitLeft(rect.width / 2);

        public static (Rect left, Rect right) SplitLeft(this Rect rect, float leftWidth) {
            var leftRect = rect.WithWidth(leftWidth);
            leftRect.width -= SPLIT_SPACE;

            var rightRect = rect.CropLeft(leftWidth);
            return (leftRect, rightRect);
        }

        public static (Rect Left, Rect Right) SplitRight(this Rect rect, float rightWidth) {
            var leftRect = rect.CropRight(rightWidth);
            leftRect.width -= SPLIT_SPACE;

            var rightRect = rect.AlignRight(rightWidth);
            return (leftRect, rightRect);
        }

        /// <summary> На всякий случай: предполагается, что Y идёт сверху вниз, как в гуе </summary>
        public static (Rect top, Rect bottom) SplitTop(this Rect rect, float topWidth) {
            var topRect = rect.WithHeight(topWidth);
            var bottomRect = rect.CropTop(topWidth);
            return (topRect, bottomRect);
        }

        public static Rect WithX(this Rect rect, float x) {
            rect.x = x;
            return rect;
        }

        public static Rect WithY(this Rect rect, float y) {
            rect.y = y;
            return rect;
        }

        public static Rect WithWidth(this Rect rect, float width) {
            rect.width = width;
            return rect;
        }

        public static Rect WithHeight(this Rect rect, float height) {
            rect.height = height;
            return rect;
        }

        public static Rect CropLeft(this Rect rect, float cropWidth) {
            rect.x += cropWidth;
            rect.width -= cropWidth;
            return rect;
        }

        public static Rect CropRight(this Rect rect, float cropWidth) {
            rect.width -= cropWidth;
            return rect;
        }

        public static Rect CropTop(this Rect rect, float cropHeight) {
            rect.y += cropHeight;
            rect.height -= cropHeight;
            return rect;
        }

        public static Rect AlignRight(this Rect rect, float newWidth) {
            rect.x = rect.x + rect.width - newWidth;
            rect.width = newWidth;
            return rect;
        }

        public static Rect Include(this Rect rect, Vector2 point) {
            rect.xMin = Mathf.Min(rect.xMin, point.x);
            rect.xMax = Mathf.Max(rect.xMax, point.x);
            rect.yMin = Mathf.Min(rect.yMin, point.y);
            rect.yMax = Mathf.Max(rect.yMax, point.y);
            return rect;
        }

        public static Rect Include(this Rect rect, Rect other) {
            return Rect.MinMaxRect(
                Mathf.Min(rect.xMin, other.xMin),
                Mathf.Min(rect.yMin, other.yMin),
                Mathf.Max(rect.xMax, other.xMax),
                Mathf.Max(rect.yMax, other.yMax)
            );
        }

        public static Vector2 ClosestPointInside(this Rect rect, Vector2 point) {
            float x = Mathf.Clamp(point.x, rect.xMin, rect.xMax);
            float y = Mathf.Clamp(point.y, rect.yMin, rect.yMax);
            return new Vector2(x, y);
        }

        public static Rect Expand(this Rect rect, float dist) {
            return new Rect(
                rect.xMin - dist,
                rect.yMin - dist,
                rect.width + dist * 2,
                rect.height + dist * 2
            );
        }

        public static Rect Expand(this Rect rect, float toLeft, float toRight, float toTop, float toBottom) {
            float xMin = rect.xMin + toLeft;
            float xMax = rect.xMax + toRight;
            float yMin = rect.yMin + toTop;
            float yMax = rect.yMax + toBottom;
            return new Rect(xMin, yMin, xMax - xMin, yMax - yMin);
        }

        public static Rect ExpandLeft(this Rect rect, float cropWidth) {
            rect.x -= cropWidth;
            rect.width += cropWidth;
            return rect;
        }

        public static void DrawDebug(this Rect rect, Color color, float duration) {
            Debug.DrawLine(new Vector2(rect.xMin, rect.yMin), new Vector2(rect.xMin, rect.yMax), color, duration);
            Debug.DrawLine(new Vector2(rect.xMin, rect.yMax), new Vector2(rect.xMax, rect.yMax), color, duration);
            Debug.DrawLine(new Vector2(rect.xMax, rect.yMax), new Vector2(rect.xMax, rect.yMin), color, duration);
            Debug.DrawLine(new Vector2(rect.xMax, rect.yMin), new Vector2(rect.xMin, rect.yMin), color, duration);
        }

        public static Rect ToGlobal(this Rect rect, Transform transform) {
            var min = transform.TransformPoint(rect.min);
            var max = transform.TransformPoint(rect.max);
            return new Rect(min, max - min);
        }

        public static Vector2 RandomPoint(this Rect rect) {
            return new Vector2(Random.Range(rect.xMin, rect.xMax), Random.Range(rect.yMin, rect.yMax));
        }

        public static bool Contains(this Rect rect, Rect other) {
            return rect.Contains(other.min) && rect.Contains(other.max);
        }
    }
}
