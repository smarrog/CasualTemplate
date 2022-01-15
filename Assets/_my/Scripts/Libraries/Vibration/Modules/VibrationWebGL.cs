#if UNITY_WEBGL
using System;
using System.Runtime.InteropServices;

namespace Smr.Vibration {
    public class VibrationWebGL : IVibrationModule {
        [DllImport("__Internal")]
        private static extern void vibrate_pattern_js(IntPtr ptr, int size);
        
        [DllImport("__Internal")]
        private static extern void vibrate_single_js(int durationInMs);
        
        [DllImport("__Internal")]
        private static extern bool is_vibration_enabled_js();
        

        private bool? _isVibrationEnabled;
        public bool IsVibrationEnabled => _isVibrationEnabled ??= is_vibration_enabled_js();
        

        public void Vibrate(int[] pattern) {
            var ptr = Marshal.AllocHGlobal(pattern.Length * sizeof(int));
            Marshal.Copy(pattern, 0, ptr, pattern.Length);
            try {
                vibrate_pattern_js(ptr, pattern.Length);
            } finally {
                Marshal.FreeHGlobal(ptr);
            }
        }

        public void Vibrate(int durationInMs) {
            vibrate_single_js(durationInMs);
        }
        
        public void StopVibration() {
            vibrate_single_js(0);
        }
    }
}
#endif