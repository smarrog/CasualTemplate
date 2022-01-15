using System;
using Smr.Extensions;

namespace Game {
    public interface ITimeService {
        DateTime Now { get; }
        long CurrentTimeStamp => Now.ToUnixLocalTimeStamp();

        void Sync(DateTime serverTime, TimeSpan? offset = null);
    }
}