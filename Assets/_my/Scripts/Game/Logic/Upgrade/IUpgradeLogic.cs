using System.Numerics;

namespace Game {
    public interface IUpgradeLogic {
        void PerformUpgrade(UpgradeType upgradeType);

        int GetLevel(UpgradeType upgradeType);
        bool IsMaxLevel(UpgradeType upgradeType);
        BigInteger GetPrice(UpgradeType upgradeType);
        
    }
}