using UnityEditor;

namespace Smr.Editor {
    public class TransEditorFloat {
#pragma warning disable 67, 414, 649

        private string _prefsKey;

        private float _defEditorValue;
        private float _defPlayerValue;

        private float? _value;
#pragma warning restore 67, 414, 649

        public float Value {
            get {
                if (_value == null) {
                    _value = EditorPrefs.GetFloat(_prefsKey, _defEditorValue);
                }
                return _value.Value;
            }
            set {
                if (_value != value) {
                    _value = value;

                    EditorPrefs.SetFloat(_prefsKey, value);
                }
            }
        }


        public TransEditorFloat(string prefsKey, float defaultValue, float? defaultPlayerValue = null) {
            _prefsKey = prefsKey;
            _defEditorValue = defaultValue;
            _defPlayerValue = defaultPlayerValue ?? defaultValue;
        }

        public static implicit operator float(TransEditorFloat transInt) {
            return transInt.Value;
        }

    }
}
