using UnityEditor;
using UnityEngine;

namespace Game.Editor {
    [CustomPropertyDrawer(typeof(LevelData))]
    public class LevelDataDrawer : PropertyDrawer {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            EditorGUI.BeginProperty(position, label, property);
            
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
            
            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;
            
            var titleRect = new Rect(position.x, position.y, 100, position.height);
            var titleRuRect = new Rect(titleRect.x + titleRect.width, position.y, 100, position.height);
            var imageRect = new Rect(titleRuRect.x + titleRuRect.width, position.y, 200, position.height);
            
            EditorGUI.PropertyField(titleRect, property.FindPropertyRelative("Title"), GUIContent.none);
            EditorGUI.PropertyField(titleRuRect, property.FindPropertyRelative("TitleRu"), GUIContent.none);
            EditorGUI.PropertyField(imageRect, property.FindPropertyRelative("Image"), GUIContent.none);
            
            EditorGUI.indentLevel = indent;
            
            EditorGUI.EndProperty();
        }
    }
}