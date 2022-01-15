using System;
using System.Collections.Generic;
using Smr.Localization;
using Smr.Utils;
using VContainer;

namespace Game {
    [Serializable]
    public class SaveData {
        // app
        public Localization Localization;
        public SerializableDictionary<SettingsType, bool> Settings = new();
        
        // meta
        public string Money;
        public long LastOnlineTimestamp;
        public List<int> Levels = new();
        public int MaxOpenedLevel;
        public int UnlockedAmount;
        public SerializableDictionary<UpgradeType, int> UpgradeLevels = new();
            
        public GiftType GiftType;
        public long GiftTimestamp;
        
        public int DailyBonusStreak;
        public int DailyBonusLastDay;
        public int DiscountPercent;

        [Preserve]
        public SaveData() {
            Reset();
        }

        public void Reset() {
            Money = null;
            LastOnlineTimestamp = 0;
            Levels.Clear();
            MaxOpenedLevel = 1;
            UnlockedAmount = 0;
            UpgradeLevels.Clear();
            GiftType = GiftType.Unknown;
            GiftTimestamp = 0;
            DailyBonusStreak = 0;
            DailyBonusLastDay = 0;
            DiscountPercent = 0;
        }
    }
}