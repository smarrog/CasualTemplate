using System.Numerics;

namespace Game {
    public interface IDailyBonusLogic {
        bool IsCurrentTaken { get; }
        
        bool IsCurrent(int dayStreak);
        bool IsTaken(int dayStreak);

        void TakeDailyBonus(int multiplier = 1);
        int GetSecondsOfIncome(int day);
        BigInteger GetDailyBonus(int day);
    }
}