using System.Numerics;
using VContainer;

namespace Game {
    public class UiLogic : IUiLogic {
        [Preserve]
        public UiLogic() {}

        public void ShowAdsWindow() {
            var data = new AdsWindowData();
            App.UiController.ShowAdsWindow(data);
        }

        public void ShowUpgradeWindow() {
            var data = new UpgradesWindowData();
            App.UiController.ShowUpgradesWindow(data);
        }

        public void ShowDailyBonusWindow() {
            var data = new DailyBonusWindowData();
            App.UiController.ShowDailyBonusWindow(data);
        }

        public void ShowAlbumWindow(bool isNew) {
            var data = new AlbumWindowData {
                Level = App.FieldLogic.MaxOpenedLevel,
                IsNew = isNew
            };
            App.UiController.ShowAlbumWindow(data);
        }
        
        public void ShowOfflineBonus(long offlineTime, BigInteger reward) {
            var data = new OfflineBonusWindowData {
                OfflineTime = offlineTime,
                Reward = reward
            };
            App.UiController.ShowOfflineBonusWindow(data);
        }
        
        public void ShowSettingsWindow() {
            var data = new SettingsWindowData();
            App.UiController.ShowSettingsWindow(data);
        }
        
        public void ShowGiftWindow(int elementIndex) {
            var data = new GiftWindowData {
                ElementIndex = elementIndex
            };
            App.UiController.ShowGiftWindow(data);
        }
    }
}