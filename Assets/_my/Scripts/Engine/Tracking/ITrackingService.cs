using System.Collections.Generic;

namespace Smr.Tracking {
    public interface ITrackingService {
        void Track(string id, Dictionary<string, object> data = null, bool flush = false);
    }
}