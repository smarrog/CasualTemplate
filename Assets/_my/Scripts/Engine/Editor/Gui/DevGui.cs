using System;
using System.Collections.Generic;
using Smr.Extensions;
using UnityEditor;
using UnityEngine;

namespace Smr.Editor {
    public static class DevGui {
        public const float INDENT_STEP = 15;
        public const float ROW_HEIGHT = 18;

        public static string ProjectRootPath => Application.dataPath[..^"/Assets".Length];


        private enum NullableBoolValue {
            Null,
            True,
            False,
        }

        public static bool TryChangeValueBool(string label, TransEditorBool transit, params GUILayoutOption[] options) {
            return TryChangeValueBool(new GUIContent(label), transit, options);
        }

        public static bool TryChangeValueBool(GUIContent label, TransEditorBool transit, params GUILayoutOption[] options) {
            bool currentValue = transit.Value;
            bool newValue = EditorGUILayout.ToggleLeft(label, currentValue, options);

            transit.Value = newValue;
            return newValue != currentValue;
        }

        public static bool TryChangeValueEnum<T>(GUIContent label, TransEditorEnum<T> transit, params GUILayoutOption[] options) where T : Enum {
            var currentValue = transit.Value;
            var newValue = (T)EditorGUILayout.EnumPopup(label, currentValue, options);
            
            transit.Value = newValue;
            return !newValue.Equals(currentValue);
        }

        public static void Foldout(string label, TransEditorBool transit, Action onFoldout) {
            TryChangeFoldingBool(label, transit);

            if (!transit.Value) {
                return;
            }

            EditorGUI.indentLevel++;
            onFoldout();
            EditorGUI.indentLevel--;
        }

        public static void TryChangeFoldingBool(string label, TransEditorBool transit) {
            TryChangeFoldingBool(new GUIContent(label), transit);
        }

        public static void TryChangeFoldingBool(GUIContent label, TransEditorBool transit) {
            bool newValue = EditorGUILayout.Foldout(transit.Value, label, true);
            if (transit.Value != newValue) {
                transit.Value = newValue;
            }
        }

        public static void TryChangeFoldingBool(Rect rect, GUIContent label, TransEditorBool transit) {
            bool newValue = EditorGUI.Foldout(rect, transit.Value, label, true);
            if (transit.Value != newValue) {
                transit.Value = newValue;
            }
        }

        public static bool TryChangeFoldingBool(string label, ref bool value) {
            bool newValue = EditorGUILayout.Foldout(value, label, true);
            if (value != newValue) {
                value = newValue;
                return true;
            }
            return false;
        }

        public static bool ToggleButton(GUIContent content, TransEditorBool transit, params GUILayoutOption[] options) {
            bool currentValue = transit.Value;
            bool newValue = GUILayout.Toggle(currentValue, content, GUI.skin.button, options);
            transit.Value = newValue;

            return newValue != currentValue;
        }

        public static bool ToggleButton(Rect rect, GUIContent content, TransEditorBool transit) {
            bool currentValue = transit.Value;
            bool newValue = GUI.Toggle(rect, currentValue, content, GUI.skin.button);
            transit.Value = newValue;

            return newValue != currentValue;
        }

        public static bool ToggleMenuItem(Rect rect, string label, TransEditorBool transit) {
            return ToggleMenuItem(rect, new GUIContent(label), transit);
        }

        public static bool ToggleMenuItem(Rect rect, GUIContent content, TransEditorBool transit) {
            bool currentValue = transit.Value;
            bool newValue = GUI.Toggle(rect, currentValue, content, DevGuiStyle.MenuItem);
            transit.Value = newValue;

            return newValue != currentValue;
        }


        public static bool TryChangeValueString(GUIContent label, TransEditorString transit, params GUILayoutOption[] options) {
            string currentValue = transit.Value;
            string newValue = EditorGUILayout.TextField(label, currentValue, options);

            transit.Value = newValue;
            return newValue != currentValue;
        }

        public static bool TryChangeValueString(string label, ref string value, params GUILayoutOption[] options) {
            return TryChangeValueString(new GUIContent(label), ref value, options);
        }

        public static bool TryChangeValueString(GUIContent label, ref string value, params GUILayoutOption[] options) {
            EditorGUILayout.BeginHorizontal();
            string newValue = EditorGUILayout.TextField(label, value, options);

            using (new EditorGUI.DisabledScope(value.IsEmpty())) {
                if (GUILayout.Button("X", DevGuiStyle.MicroButton)) {
                    newValue = "";
                    GUIUtility.keyboardControl = 0;
                }
            }

            EditorGUILayout.EndHorizontal();

            bool isChanged = value != newValue;
            if (isChanged) {
                value = newValue;
            }
            return isChanged;
        }

        public static void HorizontalLine(int height = 1,  int padding = 0) {
            Rect rect = EditorGUILayout.GetControlRect(false, GUILayout.Height(padding + height));
            rect.height = height;
            rect.y += padding * 0.5f;
            EditorGUI.DrawRect(rect, new Color(.5f, .5f, .5f, 1));
        }

        public static bool? NullableBoolPopup(GUIContent label, bool? value) {
            NullableBoolValue enumValue = value switch {
                true  => NullableBoolValue.True,
                false => NullableBoolValue.False,
                null  => NullableBoolValue.Null,
            };

            EditorGUI.showMixedValue = !value.HasValue;
            NullableBoolValue newEnumValue = (NullableBoolValue)EditorGUILayout.EnumPopup(label, enumValue);
            EditorGUI.showMixedValue = false;

            bool? newValue = newEnumValue switch {
                NullableBoolValue.True  => true,
                NullableBoolValue.False => false,
                _                       => null
            };
            return newValue;
        }

        public static void StringPopupWithSearch(Rect position, SerializedProperty property, IEnumerable<string> options) {
            var currentValue = property.stringValue;
            string newValue = StringPopupWithSearch(position, currentValue, options);

            if (newValue != currentValue) {
                property.stringValue = newValue;
            }
        }

        public static void StringPopupWithSearch(Rect position, SerializedProperty property, IDictionary<string, int> options) {
            int currentValue = property.propertyType switch {
                SerializedPropertyType.Enum    => property.enumValueIndex,
                SerializedPropertyType.Integer => property.intValue,
                _                              => throw new ArgumentException($"StringPopupWithSearch with Dictionary<string, int> options not support '{property.propertyType}' property type!")
            };
            string currentKey = options.GetValueKey(currentValue);

            string newKey = StringPopupWithSearch(position, currentKey, options.Keys);
            if (newKey == currentKey || !options.ContainsKey(newKey)) {
                return;
            }
            int newValue = options[newKey];

            switch (property.propertyType) {
                case SerializedPropertyType.Enum:
                    property.enumValueIndex = newValue;
                    break;
                case SerializedPropertyType.Integer:
                    property.intValue = newValue;
                    break;
            }
        }

        public static string StringPopupWithSearch(string value, IEnumerable<string> options, string placeholder = "") {
            Rect rect = EditorGUILayout.GetControlRect(false, 18f, EditorStyles.popup);
            return StringPopupWithSearch(rect, value, options, placeholder);
        }

        public static string StringPopupWithSearch(Rect rect, string value, IEnumerable<string> options, string placeholder = "") {
            int controlId = GUIUtility.GetControlID(nameof(StringSelector).GetHashCode(), FocusType.Keyboard, rect);
            value = StringSelector.GetSelectedValue(value, controlId);

            if (GUI.Button(rect, value ?? placeholder, EditorStyles.popup)) {
                var selector = new StringSelector(options, controlId);
                PopupWindow.Show(rect, selector);
            }

            return value;
        }

        public static Rect TryDrawLabel(Rect position, string label) => TryDrawLabel(position, new GUIContent(label));

        /// Отрисует надпись если она задана и вернёт остаток position
        public static Rect TryDrawLabel(Rect position, GUIContent label) {
            if (label == null) {
                return position;
            }
            // игнорим надписи элементов списка
            if ((label.text.IsEmpty() || label.text.StartsWith("Element ")) && label.image == null) {
                return position;
            }
            float labelWidth = Mathf.Min(EditorGUIUtility.labelWidth, position.width / 2);
            var labelPos = position.WithWidth(labelWidth);

            EditorGUI.LabelField(labelPos, label);

            return position.CropLeft(labelWidth);
        }


        public static void ClearValueButton(ref string value, object owner, string controlName) {
            GUI.SetNextControlName(controlName);
            if (GUILayout.Button("X", DevGuiStyle.MicroButton)) {
                value = "";

                var window = owner as EditorWindow;
                if (window != null) {
                    window.Repaint();
                    window.Focus();
                }
                var inspector = owner as UnityEditor.Editor;
                if (inspector != null) {
                    inspector.Repaint();
                }

                GUI.FocusControl(controlName);
            }
        }
        
        public static bool DrawVector2Field(string label, ref Vector2 vector2, params GUILayoutOption[] options) {
            EditorGUILayout.LabelField(label, options);
            Vector2 newValue = EditorGUILayout.Vector2Field(GUIContent.none, vector2, GUILayout.Width(150));
            if (vector2 != newValue) {
                vector2 = newValue;
                return true;
            }

            return false;
        }

        public static bool DrawBoolField(string label, ref bool value) {
            bool newValue = EditorGUILayout.ToggleLeft(label, value);
            if (newValue != value) {
                value = newValue;
                return true;
            }

            return false;
        }
        
        public static bool DrawFloatField(string label, ref float value) {
            float newValue = EditorGUILayout.FloatField(label, value);
            if (!newValue.IsApproximatelyEqual(value)) {
                value = newValue;
                return true;
            }

            return false;
        }


        public static float GetIndentedLabelWidth() => EditorGUIUtility.labelWidth - GetIndentWidth();
        public static float GetIndentWidth() => EditorGUI.indentLevel * INDENT_STEP;

    }
}