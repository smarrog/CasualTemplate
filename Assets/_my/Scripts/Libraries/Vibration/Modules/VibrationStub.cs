namespace Smr.Vibration {
    public class VibrationStub : IVibrationModule {
        public bool IsVibrationEnabled => false;
        
        public void Vibrate(int[] pattern) {}
        public void Vibrate(int durationInMs) {}
        public void StopVibration() {}
    }
}