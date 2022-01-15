using UnityEditor;
using UnityEngine;

namespace Smr.Editor {
    public class TransEditorColor {
#pragma warning disable 67, 414, 649
        private string _prefsKey;

        private Color _defEditorValue;
        private Color _defPlayerValue;

        private Color? _value;
#pragma warning restore 67, 414, 649

        public Color Value {
            get {
                if (_value == null) {
                    var rValue = EditorPrefs.GetFloat($"{_prefsKey}_r", _defEditorValue.r);
                    var gValue = EditorPrefs.GetFloat($"{_prefsKey}_g", _defEditorValue.g);
                    var bValue = EditorPrefs.GetFloat($"{_prefsKey}_b", _defEditorValue.b);
                    var aValue = EditorPrefs.GetFloat($"{_prefsKey}_a", _defEditorValue.a);
                    _value = new Color(rValue, gValue, bValue, aValue);
                }
                return _value.Value;
            }
            set {
                if (_value != value) {
                    _value = value;

                    EditorPrefs.SetFloat($"{_prefsKey}_r", value.r);
                    EditorPrefs.SetFloat($"{_prefsKey}_g", value.g);
                    EditorPrefs.SetFloat($"{_prefsKey}_b", value.b);
                    EditorPrefs.SetFloat($"{_prefsKey}_a", value.a);
                }
            }
        }


        public TransEditorColor(string prefsKey, Color defaultValue, Color? defaultPlayerValue = null) {
            _prefsKey = prefsKey;
            _defEditorValue = defaultValue;
            _defPlayerValue = defaultPlayerValue ?? defaultValue;
        }

        public static implicit operator Color(TransEditorColor transColor) {
            return transColor.Value;
        }
    }
}