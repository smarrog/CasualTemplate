using UnityEngine;

namespace Smr.Utils {
    public class PlayerPrefBool : BasePlayerPref<bool> {
        public PlayerPrefBool(string prefsKey, bool defaultValue = false) : base(prefsKey, defaultValue) { }

        protected override bool GetPlayerPrefValue() {
            return PlayerPrefs.GetInt(_prefsKey, _defaultValue ? 1 : 0) > 0;
        }

        protected override void SetPlayerPrefValue(bool value) {
            PlayerPrefs.SetInt(_prefsKey, value ? 1 : 0);
        }

        public static implicit operator bool(PlayerPrefBool transInt) => transInt.Value;
    }
}