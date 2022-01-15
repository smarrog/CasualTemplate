using Smr.Common;

namespace Smr.Localization {
    public class LocalizationService : ILocalizationService {
        public Localization Localization { get; private set; } = Localization.Default;

        private readonly ISignalBus _signalBus;

        public LocalizationService(ISignalBus signalBus) {
            _signalBus = signalBus;
        }
        
        public void SetLocalization(Localization localization) {
            if (Localization == localization) {
                return;
            }

            Localization = localization;
            _signalBus.Fire(new LocalizationChangedSignal(Localization));
        }
    }
}