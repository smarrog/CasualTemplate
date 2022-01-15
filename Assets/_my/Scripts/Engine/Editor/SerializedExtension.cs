using UnityEditor;
using UnityEngine;

namespace Smr.Editor {
    public static class SerializedExtension {
        public static void Draw(this SerializedObject so, string fieldName) {
            EditorGUILayout.PropertyField(so.FindProperty(fieldName), true);
        }

        public static void Draw(this SerializedProperty so, string fieldName) {
            EditorGUILayout.PropertyField(so.FindPropertyRelative(fieldName), true);
        }

        public static void DrawListResize(this SerializedProperty prop, int minValue, int maxValue) {
            DrawResizeButton(prop, minValue, maxValue, true);
            DrawResizeButton(prop, minValue, maxValue, false);
        }

        private static void DrawResizeButton(SerializedProperty prop, int minValue, int maxValue, bool isInc) {
            var isMin = prop.arraySize <= minValue;
            var isMax = prop.arraySize >= maxValue;
            var text = isInc ? "+" : "-";
            GUI.enabled = isInc ? !isMax : !isMin;
            if (GUILayout.Button(text, DevGuiStyle.MicroButton)) {
                prop.arraySize += isInc ? 1 : -1;
            }
            GUI.enabled = true;
        }
    }
}