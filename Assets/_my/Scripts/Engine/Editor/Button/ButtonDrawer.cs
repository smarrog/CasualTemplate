using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Smr.Editor {
    [CustomPropertyDrawer(typeof(ButtonAttribute))]
    public class ButtonDrawer : PropertyDrawer {
        private ButtonAttribute _buttonAttribute;
        private Object _obj;
        private Rect _buttonRect;
        private Rect _valueRect;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            _buttonAttribute = attribute as ButtonAttribute;
            if (_buttonAttribute == null) {
                return;
            }

            _obj = property.serializedObject.targetObject;
            MethodInfo method = _obj.GetType().GetMethod(_buttonAttribute._methodName, _buttonAttribute._flags);

            if (method == null) {
                EditorGUI.HelpBox(position, "Method Not Found", MessageType.Error);
                return;
            }

            if (_buttonAttribute._useValue) {
                _valueRect = new Rect(position.x, position.y, position.width / 2f, position.height);
                _buttonRect = new Rect(position.x + position.width / 2f, position.y, position.width / 2f, position.height);

                EditorGUI.PropertyField(_valueRect, property, GUIContent.none);
                if (GUI.Button(_buttonRect, _buttonAttribute._buttonName)) {
                    method.Invoke(_obj, new[] { fieldInfo.GetValue(_obj) });
                }
                return;
            }

            if (GUI.Button(position, _buttonAttribute._buttonName)) {
                method.Invoke(_obj, null);
            }
        }
    }
}