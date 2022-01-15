using System;
using System.Collections.Generic;
using Smr.Common;
using VContainer;

namespace Game {
    public class GameLogger : ILogService {
        private readonly Dictionary<LogChannel, IChannelLogger> _channelLoggers = new();
        private readonly IChannelLogger _defaultChannelLogger;

        [Preserve]
        public GameLogger() {
            _defaultChannelLogger = GetChannel(LogChannel.Default);
        }

        public IChannelLogger GetChannel(LogChannel channel) {
            if (!_channelLoggers.TryGetValue(channel, out var channelLogger)) {
                channelLogger = new ChannelLogger(channel);
                _channelLoggers.Add(channel, channelLogger);
            }
            return channelLogger;
        }
        
        public void Log(string message) {
            _defaultChannelLogger.Log(message);
        }
        
        public void LogError(string message) {
            _defaultChannelLogger.LogError(message);
        }
        
        public void LogError(Exception exception, string message = null) {
            _defaultChannelLogger.LogError(exception, message);
        }
    }
}