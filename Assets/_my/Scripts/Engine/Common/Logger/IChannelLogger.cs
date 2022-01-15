using System;

namespace Smr.Common {
    public interface IChannelLogger {
        void Log(string message);
        void LogError(string message);
        void LogError(Exception exception, string message = null);
    }
}