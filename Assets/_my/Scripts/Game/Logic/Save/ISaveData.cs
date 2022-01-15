using System.Numerics;
using Smr.Localization;

namespace Game {
    public interface ISaveData {
        SaveData Data { get; set; }

        Localization Localization { get; set; }

        BigInteger Money { get; set; }
        int MaxOpenedLevel { get; set; }
        int UnlockedAmount { get; set; }
        int DailyBonusStreak { get; set; }
        int DailyBonusLastDay { get; set; }
        long LastOnlineTimestamp { get; set; }
        int DiscountPercent { get; set; }
        GiftType GiftType { get; set; }
        long GiftTimestamp { get; set; }

        void SetElementLevel(int index, int value);
        int GetElementLevel(int index);

        void SetUpgradeLevel(UpgradeType upgradeType, int value);
        int GetUpgradeLevel(UpgradeType upgradeType);
        
        void SetAvailability(SettingsType settingsType, bool value);
        bool IsAvailable(SettingsType settingsType);
    }
}