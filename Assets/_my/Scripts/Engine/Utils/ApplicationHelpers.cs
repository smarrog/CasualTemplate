#if UNITY_EDITOR
using UnityEditor;
#else
using UnityEngine;
#endif
#if UNITY_WEBGL && !UNITY_EDITOR
using System.Runtime.InteropServices;
#endif


namespace Smr.Utils {
    public static class ApplicationHelpers {
#if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void reload_page_js();

        [DllImport("__Internal")]
        private static extern void close_page_js();
#endif        
        
        public static void Close() {
#if UNITY_WEBGL && !UNITY_EDITOR
            close_page_js();
            return;
#endif
            
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif            
        }

        public static void RestartOrClose() {
#if UNITY_WEBGL && !UNITY_EDITOR
            reload_page_js();
            return;
#endif

            Close();
        }
    }
}