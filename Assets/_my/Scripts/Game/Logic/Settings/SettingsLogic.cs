using Smr.Audio;
using Smr.Common;
using Smr.Localization;
using VContainer;

namespace Game {
    public enum SettingsType {
        Unknown = 0,
        Sound = 1,
        Music = 2,
        Vibration = 3
    }
    
    public class SettingsLogic : ISettingsLogic {
        public Localization Localization => _data.Localization;
        
        private readonly AudioService _audioService;
        private readonly ILocalizationService _localizationService;
        private readonly ISettings _settings;
        private readonly ISaveData _data;
        private readonly ISignalBus _signalBus;

        [Preserve]
        public SettingsLogic(AudioService audioService, ILocalizationService localizationService, ISettings settings, ISaveData data, ISignalBus signalBus) {
            _audioService = audioService;
            _localizationService = localizationService;
            _settings = settings;
            _data = data;
            _signalBus = signalBus;
        }
        
        public void Initialize() {
            UpdateSoundChannel();
            UpdateMusicChannel();
            _localizationService.SetLocalization(_data.Localization);
        }
        
        public bool IsAvailable(SettingsType settingsType) {
            return _data.IsAvailable(settingsType);
        }
        
        public void SetAvailability(SettingsType settingsType, bool value) {
            _data.SetAvailability(settingsType, value);
            
            switch (settingsType) {
                case SettingsType.Sound:
                    UpdateSoundChannel();
                    break;
                case SettingsType.Music:
                    UpdateMusicChannel();
                    break;
            }
            
            _signalBus.Fire(new SettingsChangedSignal(settingsType));
        }
        
        public void SetLocalization(Localization language) {
            if (_data.Localization == language) {
                return;
            }
            
            _data.Localization = language;
            _localizationService.SetLocalization(_data.Localization);
        }

        private void UpdateSoundChannel() {
            var isAvailable = _data.IsAvailable(SettingsType.Sound);
            _audioService.GetChannel(AudioChannelType.Sound).SetVolume(isAvailable ? _settings.Audio.Volume : 0);
        }
        
        private void UpdateMusicChannel() {
            var isAvailable = _data.IsAvailable(SettingsType.Music);
            _audioService.GetChannel(AudioChannelType.Music).SetVolume(isAvailable ? _settings.Audio.Volume : 0);
        }
    }
}