using System;
using System.Collections.Generic;

namespace Game {
    [Serializable]
    public class DebugSettings {
        public bool IsEverythingFree;
        public bool ClearAtStart;
        public bool UseStabData;
        public long Money;
        public List<int> Levels;
        public int UnlocksUpgradeValue;
        public int SpawnSpeedUpgradeValue;
        public int SpawnLevelUpgradeValue;
        public int IncomeMultiplierUpgradeValue;
        public int DailyBonusStreak;
        public int DailyBonusDay;
        public long LastOnlineTimeStamp;
        public int DiscountPercent;
    }
}