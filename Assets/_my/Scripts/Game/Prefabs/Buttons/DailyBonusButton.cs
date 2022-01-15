namespace Game {
    public class DailyBonusButton : AbstractButtonWithNotifier {
        protected override bool HasNotification => !App.DailyBonusLogic.IsCurrentTaken;

        protected override void ConstructInternal() {
            base.ConstructInternal();

            UpdateNotificationBadge();

            AddSubscription<DailyBonusTakenSignal>(OnDailyBonusChangedSignal);
        }

        protected override void OnButtonClick() {
            App.PlayTap();

            App.UiLogic.ShowDailyBonusWindow();
        }

        private void OnDailyBonusChangedSignal(DailyBonusTakenSignal signal) {
            UpdateNotificationBadge();
        }
    }
}