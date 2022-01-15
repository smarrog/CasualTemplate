namespace Smr.Localization {
    public class LocalizationChangedSignal {
        public Localization Localization { get; }
        
        public LocalizationChangedSignal(Localization localization) {
            Localization = localization;
        }   
    }
}