using UnityEditor;

namespace Smr.Editor {
    public class TransEditorString {
#pragma warning disable 67, 414, 649
        private string _prefsKey;

        private string _defEditorValue;
        private string _defPlayerValue;

        private string _value;
        private bool _isValueReceived;
#pragma warning restore 67, 414, 649

        public string Value {
            get {
                if (!_isValueReceived) {
                    _isValueReceived = true;
                    _value = EditorPrefs.GetString(_prefsKey, _defEditorValue);
                }
                return _value;
            }
            set {
                if (_value != value) {
                    _value = value;

                    EditorPrefs.SetString(_prefsKey, value);
                }
            }
        }


        public TransEditorString(string prefsKey, string defaultValue, string defaultPlayerValue = null) {
            _prefsKey = prefsKey;
            _defEditorValue = defaultValue;
            _defPlayerValue = defaultPlayerValue ?? defaultValue;
        }

        public static implicit operator string(TransEditorString transString) {
            return transString.Value;
        }

        public override string ToString() {
            return Value;
        }
    }
}