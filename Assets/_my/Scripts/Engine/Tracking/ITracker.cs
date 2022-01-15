using System.Collections.Generic;

namespace Smr.Tracking {
    public interface ITracker {
        void Track(string id, Dictionary<string, object> data);
        void Flush();
    }
}