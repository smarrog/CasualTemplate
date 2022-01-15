using System;
using UnityEngine;

namespace Smr.Vibration {
    public static class Vibration {
        private static IVibrationModule Module => _module ??= CreateVibrationModule();
        private static IVibrationModule _module;

        public static bool IsVibrationEnabled {
            get {
                try {
                    return Module.IsVibrationEnabled;
                } catch (Exception e) {
                    Debug.LogException(e);
                    return false;
                }
            }
        }

        public static void Vibrate(int[] pattern) {
            if (!IsVibrationEnabled) {
                return;
            }
            
            if (pattern == null || pattern.Length == 0) {
                return;
            }
            
            try {
                Module.Vibrate(pattern);
            } catch (Exception e) {
                Debug.LogException(e);
            }
        }

        public static void Vibrate(int durationInMs) {
            if (!IsVibrationEnabled) {
                return;
            }
            
            try {
                Module.Vibrate(durationInMs);
            } catch (Exception e) {
                Debug.LogException(e);
            }
        }

        public static void StopVibration() {
            if (!IsVibrationEnabled) {
                return;
            }

            try {
                Module.StopVibration();
            } catch (Exception e) {
                Debug.LogException(e);
            }
        }

#region Sugar
        
        public static void VibrateShortPulse() => Vibrate(100);
        public static void VibrateLongPulse() => Vibrate(500);
        
        public static void VibrateDefaultAndroid() => Vibrate(new[] { 0, 250, 250, 250 });
        public static void VibrateTwice() => Vibrate(new[] { 200, 100, 200 });
        public static void VibrateRhythmic() => Vibrate(new[] { 100, 100, 100, 100, 100 });
        
#endregion 

        private static IVibrationModule CreateVibrationModule() {
            if (Application.isEditor) {
                return new VibrationStub();
            }
            
#if UNITY_WEBGL
            return new VibrationWebGL();
#else
            return new VibrationStub();
#endif
        }
    }
}