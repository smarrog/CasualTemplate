using Smr.Ui;
using UnityEngine;

namespace Game {
    public class SettingsSwitch : AbstractToggleSwitch {
        [SerializeField] private SettingsType _settingsType;
        
        protected override bool Value => App.SettingsLogic.IsAvailable(_settingsType);

        protected override void SubscribeOnChanges() {
            AddSubscription<SettingsChangedSignal>(OnSettingsChangedSignal);
        }
        
        protected override void OnButtonClick() {
            App.PlayTap();
            App.SettingsLogic.SetAvailability(_settingsType, !Value);
        }

        private void OnSettingsChangedSignal(SettingsChangedSignal signal) {
            if (signal.SettingsType == _settingsType) {
                UpdateVisual();
            }
        }
    }
}