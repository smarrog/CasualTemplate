using System.Numerics;
using UnityEngine;

namespace Smr.Utils {
    public class PlayerPrefBigInteger : BasePlayerPref<BigInteger> {
        public PlayerPrefBigInteger(string prefsKey, BigInteger defaultValue = default) : base(prefsKey, defaultValue) {}
        
        protected override BigInteger GetPlayerPrefValue() {
            var strValue = PlayerPrefs.GetString(_prefsKey, _defaultValue.ToString());
            return BigInteger.TryParse(strValue, out BigInteger result) ? result : _defaultValue;
        }
        protected override void SetPlayerPrefValue(BigInteger value) {
            PlayerPrefs.SetString(_prefsKey, value.ToString());
        }
    }
}