using Smr.Localization;

namespace Smr.Common {
    public class EngineDependencies {
        public static ILogService Logger { get; private set; }
        public static ISignalBus SignalBus { get; private set; }
        public static ILocalizationService Localization { get; private set; }
        
        public EngineDependencies(ISignalBus signalBus, ILogService logger, ILocalizationService localization) {
            Logger = logger;
            SignalBus = signalBus;
            Localization = localization;
        }
    }
}