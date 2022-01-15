using UnityEngine;

namespace Smr.Utils {
    public class PlayerPrefString : BasePlayerPref<string> {
        public PlayerPrefString(string prefsKey, string defaultValue = "") : base(prefsKey, defaultValue) { }

        protected override string GetPlayerPrefValue() {
            return PlayerPrefs.GetString(_prefsKey, _defaultValue);
        }

        protected override void SetPlayerPrefValue(string value) {
            PlayerPrefs.SetString(_prefsKey, value);
        }

        public static implicit operator string(PlayerPrefString transInt) => transInt.Value;
    }
}