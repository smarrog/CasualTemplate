using UnityEditor;

namespace Smr.Editor {
    public class TransEditorBool {
#pragma warning disable 67, 414, 649
        private string _prefsKey;

        private bool _defEditorValue;
        private bool _defPlayerValue;

        private bool? _value;
#pragma warning restore 67, 414, 649

        public bool Value {
            get {
                if (_value == null) {
                    _value = EditorPrefs.GetBool(_prefsKey, _defEditorValue);
                }
                return _value.Value;
            }
            set {
                if (_value != value) {
                    _value = value;

                    EditorPrefs.SetBool(_prefsKey, value);
                }
            }
        }

        public void Toggle() => Value = !Value;


        public TransEditorBool(string prefsKey, bool defaultValue, bool? defaultPlayerValue = null) {
            _prefsKey = prefsKey;
            _defEditorValue = defaultValue;
            _defPlayerValue = defaultPlayerValue ?? defaultValue;
        }

        public static implicit operator bool(TransEditorBool transBool) {
            return transBool.Value;
        }
    }
}