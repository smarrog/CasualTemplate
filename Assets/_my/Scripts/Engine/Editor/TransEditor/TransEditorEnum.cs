using System;
using UnityEditor;

namespace Smr.Editor {
    public class TransEditorEnum<T> where T : Enum {
#pragma warning disable 67, 414, 649

        private string _prefsKey;

        private readonly T _defEditorValue;
        private readonly T _defPlayerValue;

        private T _value;
        private bool _isValueReceived;
#pragma warning restore 67, 414, 649

        public T Value {
            get {
                if (!_isValueReceived) {
                    _isValueReceived = true;
                    _value = (T)(object)EditorPrefs.GetInt(_prefsKey, (int)(object)_defEditorValue);
                }
                return _value;
            }
            set {
                if ((int)(object)_value != (int)(object)value) {
                    _value = value;

                    EditorPrefs.SetInt(_prefsKey, (int)(object)value);
                }
            }
        }


        public TransEditorEnum(string prefsKey, T defaultValue) {
            _prefsKey = prefsKey;
            _defEditorValue = defaultValue;
            _defPlayerValue = defaultValue;
        }

        public TransEditorEnum(string prefsKey, T defaultValue, T defaultPlayerValue) {
            _prefsKey = prefsKey;
            _defEditorValue = defaultValue;
            _defPlayerValue = defaultPlayerValue;
        }

        public static implicit operator T(TransEditorEnum<T> transEnum) => transEnum == null ? default : transEnum.Value;
        public static implicit operator int(TransEditorEnum<T> transEnum) => (int)(object)transEnum.Value;

    }
}