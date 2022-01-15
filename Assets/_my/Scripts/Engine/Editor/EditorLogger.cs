using System;
using Smr.Common;
using UnityEngine;

namespace Smr.Editor {
    public class EditorLogger : IChannelLogger {
        private static EditorLogger _logger;
        public static EditorLogger Instance => _logger ?? new EditorLogger();
        
        public void Log(string message) {
            Debug.Log(message);
        }
        
        public void LogError(string message) {
            Debug.LogError(message);
        }
        
        public void LogError(Exception exception, string message = null) {
            if (!string.IsNullOrEmpty(message)) {
                Debug.LogError(message);
            }
            Debug.LogException(exception);
        }
    }
}