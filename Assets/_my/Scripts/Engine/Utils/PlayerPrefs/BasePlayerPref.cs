using UnityEngine;

namespace Smr.Utils {
    public abstract class BasePlayerPref<T> {
        protected readonly string _prefsKey;
        protected readonly T _defaultValue;
        
        private T _value;
        private bool _isValueReceived;

        public T Value {
            get {
                if (!_isValueReceived) {
                    _isValueReceived = true;
                    _value = GetPlayerPrefValue();
                }
                return _value;
            }
            set {
                if (!_isValueReceived || !_value.Equals(value)) {
                    _isValueReceived = true;
                    _value = value;

                    SetPlayerPrefValue(value);
                }
            }
        }

        public virtual void Clear() {
            PlayerPrefs.DeleteKey(_prefsKey);
        }

        protected abstract T GetPlayerPrefValue();
        protected abstract void SetPlayerPrefValue(T value);

        protected BasePlayerPref(string prefsKey, T defaultValue) {
            _prefsKey = prefsKey;
            _defaultValue = defaultValue;
        }

        public static implicit operator T(BasePlayerPref<T> transInt) => transInt.Value;
    }
}