using System.Numerics;

namespace Game {
    public interface IUiLogic {
        void ShowAdsWindow();
        void ShowUpgradeWindow();
        void ShowDailyBonusWindow();
        void ShowAlbumWindow(bool isNew = false);
        void ShowOfflineBonus(long offlineTime, BigInteger reward);
        void ShowSettingsWindow();
        void ShowGiftWindow(int elementIndex);
    }
}