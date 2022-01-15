using System;

namespace Game {
    [Serializable]
    public class SystemSettings {
        public string Version;
        public int TargetFrameRate;
        public int VSyncCount;
        public bool CanSleep;
        public float SaveInterval;
    }
}