using Smr.Localization;
using Smr.Ui;

namespace Game {
    public class LanguageSwitch : AbstractToggleSwitch {
        protected override bool Value => App.SettingsLogic.Localization == Localization.Russian;
        
        private Localization CurrentLocalization => Value ? Localization.Russian : Localization.English;

        protected override void SubscribeOnChanges() {
            AddSubscription<LocalizationChangedSignal>(OnLocalizationChangedSignal);
        }

        protected override void OnButtonClick() {
            App.PlayTap();
            App.SettingsLogic.SetLocalization(CurrentLocalization == Localization.Russian ? Localization.English : Localization.Russian);
        }

        private void OnLocalizationChangedSignal(LocalizationChangedSignal signal) {
            UpdateVisual();
        }
    }
}