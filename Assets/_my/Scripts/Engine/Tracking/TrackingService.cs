using System;
using System.Collections.Generic;
using Smr.Common;
using Smr.Extensions;

namespace Smr.Tracking {
    public class TrackingService : ITrackingService {
        private readonly IEnumerable<ITracker> _trackers;
        private readonly IChannelLogger _logger;
        
        public TrackingService(IEnumerable<ITracker> trackers, ILogService logger) {
            _trackers = trackers;
            _logger = logger.GetChannel(LogChannel.Tracking);
        }
        
        public void Track(string id, Dictionary<string, object> data = null, bool flush = false) {
            _logger.Log($"Track \"{id}\" - {data.ToDebugString()}");
            foreach (var tracker in _trackers) {
                try {
                    tracker.Track(id, data);
                    if (flush) {
                        tracker.Flush();
                    }
                } catch (Exception exception) {
                    _logger.LogError(exception, $"Failed to track {id} for {tracker.GetType()}");
                }
            }
        }
    }
}