namespace Smr.Common {
    public interface ILogService : IChannelLogger {
        IChannelLogger GetChannel(LogChannel channel);
    }
}