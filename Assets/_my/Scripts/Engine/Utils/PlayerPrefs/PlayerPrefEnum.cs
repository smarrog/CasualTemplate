using System;
using UnityEngine;

namespace Smr.Utils {
    public class PlayerPrefEnum<TEnum> : BasePlayerPref<TEnum> where TEnum : struct, Enum {
        public PlayerPrefEnum(string prefsKey, TEnum defaultValue) : base(prefsKey, defaultValue) { }

        protected override TEnum GetPlayerPrefValue() {
            int storedValue = PlayerPrefs.GetInt(_prefsKey, Convert.ToInt32(_defaultValue));

            return (TEnum)Enum.ToObject(typeof(TEnum), storedValue);
        }

        protected override void SetPlayerPrefValue(TEnum value) {
            PlayerPrefs.SetInt(_prefsKey, Convert.ToInt32(value));
        }

        public static implicit operator TEnum(PlayerPrefEnum<TEnum> transEnum) => transEnum.Value;
    }
}