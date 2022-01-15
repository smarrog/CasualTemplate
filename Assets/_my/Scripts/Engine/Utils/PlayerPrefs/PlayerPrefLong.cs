using UnityEngine;

namespace Smr.Utils {
    public class PlayerPrefLong : BasePlayerPref<long> {
        public PlayerPrefLong(string prefsKey, long defaultValue = 0) : base(prefsKey, defaultValue) { }

        protected override long GetPlayerPrefValue() {
            var lowerBits = PlayerPrefs.GetInt(_prefsKey + ":L", (int)(_defaultValue & 0xFFFFFFFF));
            var upperBits = PlayerPrefs.GetInt(_prefsKey + ":U", (int)(_defaultValue >> 32));
            return ((long)upperBits << 32) | (uint)lowerBits;
        }

        protected override void SetPlayerPrefValue(long value) {
            var lowerBits = (int)(value & 0xFFFFFFFF);
            var upperBits = (int)(value >> 32);
            
            PlayerPrefs.SetInt(_prefsKey + ":L", lowerBits);
            PlayerPrefs.SetInt(_prefsKey + ":U", upperBits);
        }

        public static implicit operator long(PlayerPrefLong transInt) => transInt.Value;
    }
}