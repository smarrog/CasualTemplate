using System;
using Smr.Common;
using UnityEngine;

namespace Game {
    public class ChannelLogger : IChannelLogger {
        private readonly LogChannel _channel;
        
        public ChannelLogger(LogChannel channel) {
            _channel = channel;
        }
        
        public void Log(string message) {
            Debug.Log($"[{_channel.ToString()}] {message}");
        }
        
        public void LogError(string message) {
            Debug.LogError($"[{_channel.ToString()}] {message}");
        }
        
        public void LogError(Exception exception, string message = null) {
            if (!string.IsNullOrEmpty(message)) {
                Debug.LogError($"[{_channel.ToString()}] {message}");
            }
            Debug.LogException(exception);
        }
    }
}