using UnityEditor;

namespace Smr.Editor {
    public class TransEditorInt {
#pragma warning disable 67, 414, 649

        private string _prefsKey;

        private int _defEditorValue;
        private int _defPlayerValue;

        private int? _value;
#pragma warning restore 67, 414, 649

        public int Value {
            get {
                if (_value == null) {
                    _value = EditorPrefs.GetInt(_prefsKey, _defEditorValue);
                }
                return _value.Value;
            }
            set {
                if (_value != value) {
                    _value = value;

                    EditorPrefs.SetInt(_prefsKey, value);
                }
            }
        }


        public TransEditorInt(string prefsKey, int defaultValue, int? defaultPlayerValue = null) {
            _prefsKey = prefsKey;
            _defEditorValue = defaultValue;
            _defPlayerValue = defaultPlayerValue ?? defaultValue;
        }

        public static implicit operator int(TransEditorInt transInt) {
            return transInt.Value;
        }

    }
}