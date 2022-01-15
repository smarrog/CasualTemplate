using Smr.Commands;

namespace Game {
    public class InitializeGameCommand : CommandsQueue {
        public InitializeGameCommand() {
            AddCommand(new DelayCommand(App.Settings.Preloader.MinTimeInSeconds), false);
            AddCommand(new SetAppSettingsCommand(), false);
            AddCommand(new WaitForSdkEnabledCommand());
            AddCommand(new InitializeServerTimeCommand(), false);
            AddCommand(new LoadSaveCommand());
            AddCommand(new InitializeLocalizationCommand(), false);
        }
    }
}