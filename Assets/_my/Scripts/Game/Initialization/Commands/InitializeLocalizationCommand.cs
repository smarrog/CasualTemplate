using Smr.Commands;
using Smr.Common;
using Smr.Localization;
using YG;

namespace Game {
    public class InitializeLocalizationCommand : AbstractCommand {
        protected override void ExecuteInternal() {
            if (App.SettingsLogic.Localization != Localization.Default) {
                NotifyComplete();
                return;
            }

            var localization = GetLocalization();
            App.Logger.GetChannel(LogChannel.Initialization).Log($"Identified localization: {localization}");
            App.SettingsLogic.SetLocalization(localization);
            NotifyComplete();
        }

        private Localization GetLocalization() {
            return YG2.envir.language switch {
                "en" => Localization.English,
                "ru" => Localization.Russian,
                _ => Localization.English
            };
        }
    }
}