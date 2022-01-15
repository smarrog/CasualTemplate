using UnityEngine;

namespace Smr.Utils {
    public class PlayerPrefInt : BasePlayerPref<int> {
        public PlayerPrefInt(string prefsKey, int defaultValue = 0) : base(prefsKey, defaultValue) { }

        protected override int GetPlayerPrefValue() {
            return PlayerPrefs.GetInt(_prefsKey, _defaultValue);
        }

        protected override void SetPlayerPrefValue(int value) {
            PlayerPrefs.SetInt(_prefsKey, value);
        }

        public static implicit operator int(PlayerPrefInt transInt) => transInt.Value;
    }
}