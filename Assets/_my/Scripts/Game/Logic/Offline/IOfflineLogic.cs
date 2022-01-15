using System.Numerics;

namespace Game {
    public interface IOfflineLogic {
        void UpdateLastOnlineTimestamp();
        void CheckOfflineReward();
        void GiveOfflineReward(long offlineTime, BigInteger reward);
    }
}