using UnityEngine;

namespace Game {
    public interface ISettings {
        PreloaderSettings Preloader { get; }
        DebugSettings Debug { get; }
        AudioSettings Audio { get; }
        SystemSettings System { get; }
        UiSettings Ui { get; }
        MetaSettings Meta { get; }
    }
    
    [CreateAssetMenu(fileName = "Settings", menuName = "Settings/Settings", order = 0)]
    public class Settings : ScriptableObject, ISettings {
        public PreloaderSettings Preloader;
        public SystemSettings System;
        public DebugSettings Debug;
        public AudioSettings Audio;
        public UiSettings Ui;
        public MetaSettings Meta;
        
        PreloaderSettings ISettings.Preloader => Preloader;
        DebugSettings ISettings.Debug => Debug;
        AudioSettings ISettings.Audio => Audio;
        SystemSettings ISettings.System => System;
        UiSettings ISettings.Ui => Ui;
        MetaSettings ISettings.Meta => Meta;
    }
}