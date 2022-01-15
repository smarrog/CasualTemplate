using Smr.Localization;

namespace Game {
    public interface ISettingsLogic : IInitializable {
        Localization Localization { get; }
        
        bool IsAvailable(SettingsType settingsType);
        
        void SetAvailability(SettingsType settingsType, bool value);
        
        void SetLocalization(Localization language);
    }
}