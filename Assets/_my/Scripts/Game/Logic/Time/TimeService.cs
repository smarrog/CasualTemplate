using System;
using Smr.Common;
using UnityEngine;
using VContainer;

namespace Game {
    public class TimeService : ITimeService {
        public DateTime Now => ServerUtcNow + (_offset ?? TimeSpan.Zero);
        public DateTime ServerUtcNow => _serverTime?.AddSeconds(Time.realtimeSinceStartup - _realtimeOnSync) ?? throw new Exception("Server time was not synced");
        
        private DateTime? _serverTime;
        private TimeSpan? _offset;
        private float _realtimeOnSync;
        
        private readonly ISignalBus _signalBus;

        [Preserve]
        public TimeService(ISignalBus signalBus) {
            _signalBus = signalBus;
        }
        
        public void Sync(DateTime serverTime, TimeSpan? offset = null) {
            _serverTime = serverTime;
            _realtimeOnSync = Time.realtimeSinceStartup;
            _offset = offset;
            
            _signalBus.Fire(new TimeChangedSignal());
        }
    }
}