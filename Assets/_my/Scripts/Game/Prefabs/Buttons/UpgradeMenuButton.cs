using System.Linq;
using Smr.Extensions;

namespace Game {
    public class UpgradeMenuButton : AbstractButtonWithNotifier {
        protected override bool HasNotification {
            get {
                return EnumExtensions.GetAllValues<UpgradeType>(true).Any(upgradeType => {
                    if (App.UpgradeLogic.IsMaxLevel(upgradeType)) {
                        return false;
                    }
                    var price = App.UpgradeLogic.GetPrice(upgradeType);
                    return App.MoneyLogic.IsEnoughMoney(price);
                });
            }
        }

        protected override void ConstructInternal() {
            base.ConstructInternal();

            AddSubscription<UpgradeLevelChangedSignal>(UpgradeLevelChangedSignal);
            
            App.Scheduler.DoEvery(0.3f, UpdateNotificationBadge);
        }

        protected override void OnButtonClick() {
            App.PlayTap();
            
            App.UiLogic.ShowUpgradeWindow();
        }

        private void UpgradeLevelChangedSignal(UpgradeLevelChangedSignal signal) {
            UpdateNotificationBadge();
        }
    }
}