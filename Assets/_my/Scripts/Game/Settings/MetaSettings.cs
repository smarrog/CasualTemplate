using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game {
    [Serializable]
    public class MetaSettings {
        public DailyBonusSettings DailyBonus;
        public FieldSettings Field;
        public InstantBonusesForAdsSettings InstantBonusesForAds;
        public GiftsSettings Gifts;
        public MoneySettings Money;
        public OfflineSettings Offline;
        public UpgradeSettings Upgrade;
    }

    [Serializable]
    public class LevelData {
        public string Title;
        public string TitleRu;
        public Sprite Image;
    }

    [Serializable]
    public class FieldSettings {
        public List<LevelData> Levels;
        public int MaxLevel; // if 0 then use levels.Count
        public int SlotsAmount;
        public int UnlockedSlotsAmount;
        public bool IsSwapEnabled;
        public float SpawnInterval;
        public float AccelerateSpawnByClick;
    }

    [Serializable]
    public class DailyBonusSettings {
        public int AdsMultiplier;
        public bool HoldAtLastDay;
        public List<int> Days; // seconds of Income
    }

    [Serializable]
    public class GiftsSettings {
        public int Duration;
        public int SpawnSpeedMultiplier;
        public int SpawnLevelBonus;
        public int StartSpawnFromLevel;
        public float SpawnChance;
    }

    [Serializable]
    public class InstantBonusesForAdsSettings {
        public int DiscountBonusIncrement;
        public int SpawnLevelBonus;
        public int SpawnAmount;
        public int MoneyIncomeInSeconds;
    }

    [Serializable]
    public class MoneySettings {
        [NonSerialized] public bool IsEverythingFree;
        public float PayInterval;
        public int MaxDiscount;
    }

    [Serializable]
    public class UpgradeSettings {
        public int a;
        public float r;
        public float s;
        public int MaxSpawnSpeedUpgradeLevel;
        public int MaxIncomeMultiplierUpgradeLevel;
        public int IncomeMultiplierPerUpgrade;
    }

    [Serializable]
    public class OfflineSettings {
        public int AdsMultiplier;
        public int MaxDuration;
        public int MinDurationToShowWindow;
    }
}