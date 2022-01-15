namespace Game {
    public class SettingsChangedSignal {
        public SettingsType SettingsType { get; }
        
        public SettingsChangedSignal(SettingsType settingsType) {
            SettingsType = settingsType;
        }
    }
}