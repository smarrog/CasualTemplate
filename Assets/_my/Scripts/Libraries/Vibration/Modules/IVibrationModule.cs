namespace Smr.Vibration {
    public interface IVibrationModule {
        bool IsVibrationEnabled { get; }

        void Vibrate(int[] pattern);
        void Vibrate(int durationInMs);
        void StopVibration();
    }
}