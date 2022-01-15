using Smr.Ui;
using UnityEngine;

namespace Game {
    public abstract class AbstractButtonWithNotifier : AbstractButton {
        [SerializeField] private NotificationBadge _notificationBadge;
        
        protected abstract bool HasNotification { get; }

        private bool _hasNotificationInternal;

        protected override void ConstructInternal() {
            base.ConstructInternal();

            _hasNotificationInternal = HasNotification;
            _notificationBadge.SetVisibility(_hasNotificationInternal);

            App.SignalBus.Subscribe<ResetProgressSignal>(OnResetProgressChangedSignal);
        }

        protected void UpdateNotificationBadge() {
            if (_hasNotificationInternal == HasNotification) {
                return;
            }

            _hasNotificationInternal = HasNotification;
            _notificationBadge.SetVisibilityAnimated(_hasNotificationInternal);
        }

        private void OnResetProgressChangedSignal(ResetProgressSignal signal) {
            UpdateNotificationBadge();
        }
    }
}