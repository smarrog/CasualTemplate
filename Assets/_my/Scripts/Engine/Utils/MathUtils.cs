using UnityEngine;

namespace Smr.Utils {
    public static class MathUtils {
        public static float SquareDistanceFromPointToSection(
            Vector2 point,
            Vector2 sectionStart,
            Vector2 sectionEnd
        ) {
            float x = point.x;
            float y = point.y;

            float x1 = sectionStart.x;
            float y1 = sectionStart.y;

            float x2 = sectionEnd.x;
            float y2 = sectionEnd.y;

            var A = x - x1;
            var B = y - y1;
            var C = x2 - x1;
            var D = y2 - y1;

            var dot = A * C + B * D;
            var len_sq = C * C + D * D;
            float param = -1;
            if (!Mathf.Approximately(len_sq, 0)) { //in case of 0 length line
                param = dot / len_sq;
            }

            float xx, yy;

            if (param < 0) {
                xx = x1;
                yy = y1;
            } else if (param > 1) {
                xx = x2;
                yy = y2;
            } else {
                xx = x1 + param * C;
                yy = y1 + param * D;
            }

            var dx = x - xx;
            var dy = y - yy;
            return dx * dx + dy * dy;
        }
    }
}