using System.Collections.Generic;
using System.Linq;
using Smr.Tracking;
using UnityEngine.Scripting;
using YG;

namespace Game {
    public class Yg2Tracker : ITracker {
        [Preserve]
        public Yg2Tracker() {}

        public void Track(string id, Dictionary<string, object> data) {
            var convertedData = data.ToDictionary(kv => kv.Key, kv => kv.Value.ToString());
            YG2.MetricaSend(id, convertedData);
        }
        
        public void Flush() {
            
        }
    }
}