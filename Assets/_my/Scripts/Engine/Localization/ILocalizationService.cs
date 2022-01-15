namespace Smr.Localization {
    public interface ILocalizationService {
        public Localization Localization { get; }

        public void SetLocalization(Localization localization);
    }
}