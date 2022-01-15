using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Smr.Editor {
    public abstract class BaseEditor<T> : UnityEditor.Editor where T : class {
        protected T Data => target as T;

        public override void OnInspectorGUI() {
            if (Data == null) {
                return;
            }

            serializedObject.Update();
            Draw();
            serializedObject.ApplyModifiedProperties();
        }

        protected abstract void Draw();

        protected void WrapWithIndentation(Action action) {
            ++EditorGUI.indentLevel;
            action();
            --EditorGUI.indentLevel;
        }

        protected void WrapWithHorizontalLayout(Action action) {
            GUILayout.BeginHorizontal();
            action();
            GUILayout.EndHorizontal();
        }

        protected bool TryChangeFoldingBool(string label, ref bool value) {
            bool newValue = EditorGUILayout.Foldout(value, label, true, DevGuiStyle.BoldText);
            if (value == newValue) {
                return false;
            }

            value = newValue;
            return true;
        }

        protected void DrawList(string propName, List<T> list, Action<int, SerializedProperty> elementAction, int maxSize = 20) {
            var arrayProp = serializedObject.FindProperty(propName);
            GUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(arrayProp, new GUIContent($"{arrayProp.name} ({list.Count})"), false);

            GUILayout.FlexibleSpace();
            arrayProp.DrawListResize(0, maxSize);
            GUILayout.EndHorizontal();

            if (!arrayProp.isExpanded) {
                return;
            }

            GUILayout.Space(3);
            WrapWithIndentation(() => {
                int visibleCount = Mathf.Min(arrayProp.arraySize, list.Count);
                for (int i = 0; i < visibleCount; i++) {
                    elementAction(i, arrayProp.GetArrayElementAtIndex(i));
                }
            });
        }
    }
}