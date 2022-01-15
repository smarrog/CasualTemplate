using System.Collections.Generic;
using System.Numerics;
using Smr.Extensions;
using Smr.Localization;
using VContainer;

namespace Game {

    public class SaveDataWrapper : ISaveData {
        public SaveData Data { get; set; }

        public Localization Localization { get => Data.Localization; set => Data.Localization = value; }
        public BigInteger Money { get => BigIntegerExtensions.FromString(Data.Money); set => Data.Money = value.ToString(); }
        public int MaxOpenedLevel { get => Data.MaxOpenedLevel; set => Data.MaxOpenedLevel = value; }
        public int UnlockedAmount { get => Data.UnlockedAmount; set => Data.UnlockedAmount = value; }
        public int DailyBonusStreak { get => Data.DailyBonusStreak; set => Data.DailyBonusStreak = value; }
        public int DailyBonusLastDay { get => Data.DailyBonusLastDay; set => Data.DailyBonusLastDay = value; }
        public long LastOnlineTimestamp { get => Data.LastOnlineTimestamp; set => Data.LastOnlineTimestamp = value; }
        public int DiscountPercent { get => Data.DiscountPercent; set => Data.DiscountPercent = value; }
        public GiftType GiftType { get => Data.GiftType; set => Data.GiftType = value; }
        public long GiftTimestamp { get => Data.GiftTimestamp; set => Data.GiftTimestamp = value; }

        [Preserve]
        public SaveDataWrapper() {}

        public void SetElementLevel(int index, int value) {
            while (Data.Levels.Count <= index) {
                Data.Levels.Add(0);
            }
            Data.Levels[index] = value;
        }
        
        public int GetElementLevel(int index) {
            return Data.Levels.GetAtOrDefault(index);
        }
        
        public void SetUpgradeLevel(UpgradeType upgradeType, int value) {
            Data.UpgradeLevels[upgradeType] = value;
        }
        
        public int GetUpgradeLevel(UpgradeType upgradeType) {
            return Data.UpgradeLevels.GetValueOrDefault(upgradeType);
        }
        
        public void SetAvailability(SettingsType settingsType, bool value) {
            Data.Settings[settingsType] = value;
        }
        
        public bool IsAvailable(SettingsType settingsType) {
            return Data.Settings.GetValueOrDefault(settingsType, true);
        }
    }
}