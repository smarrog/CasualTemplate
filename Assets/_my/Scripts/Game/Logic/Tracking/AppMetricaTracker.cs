using System.Collections.Generic;
using Smr.Tracking;
using UnityEngine.Scripting;

namespace Game {
    public class AppMetricaTracker : ITracker {
        [Preserve]
        public AppMetricaTracker() {}
        
        public void Track(string id, Dictionary<string, object> data) {
            AppMetrica.Instance.ReportEvent(id, data);
        }
        
        public void Flush() {
            AppMetrica.Instance.SendEventsBuffer();
        }
    }
}