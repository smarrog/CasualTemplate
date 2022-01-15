using System.Numerics;

namespace Game {
    public class ResetProgressSignal {}
    
    public class SpawnSignal {
        public int Index { get; }
        public int Level { get; }

        public SpawnSignal(int index, int level) {
            Index = index;
            Level = level;
        }
    }
    public class DailyBonusTakenSignal {
        public int CurrentStreak { get; }
        
        public DailyBonusTakenSignal(int currentStreak) {
            CurrentStreak = currentStreak;
        }
    }

    public class MoveSignal {
        public MoveResult MoveResult { get; }
        public MoveSignal(MoveResult moveResult) {
            MoveResult = moveResult;
        }
    }
    
    public class ElementLevelChangedSignal {
        public int Index { get; }
        public int Level { get; }
        
        public ElementLevelChangedSignal(int index, int level) {
            Index = index;
            Level = level;
        }
    }
    
    public class ElementUnlockedSignal {
        public int Index { get; }
        
        public ElementUnlockedSignal(int index) {
            Index = index;
        }
    }
    
    public class GiftStartedSignal {
        public GiftType GiftType { get; }
        
        public GiftStartedSignal(GiftType giftType) {
            GiftType = giftType;
        }
    }
    
    public class IncomeChangedSignal {
        public BigInteger Value { get; }
        
        public IncomeChangedSignal(BigInteger value) {
            Value = value;
        }
    }
    
    public class MaxOpenedLevelChangedSignal {
        public int Level { get; }
        
        public MaxOpenedLevelChangedSignal(int level) {
            Level = level;
        }
    }
    
    public class MoneyChangedSignal {
        public BigInteger Value { get; }
        
        public MoneyChangedSignal(BigInteger value) {
            Value = value;
        }
    }
    
    public class UpgradeLevelChangedSignal {
        public UpgradeType UpgradeType { get; }
        public int UpgradeLevel { get; }
        
        public UpgradeLevelChangedSignal(UpgradeType upgradeType, int upgradeLevel) {
            UpgradeType = upgradeType;
            UpgradeLevel = upgradeLevel;
        }   
    }

    public class OfflineRewardGivenSignal {
        public long OfflineTime { get; }
        public BigInteger Reward { get; }

        public OfflineRewardGivenSignal(long offlineTime, BigInteger reward) {
            OfflineTime = offlineTime;
            Reward = reward;
        }
    }
}