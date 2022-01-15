using System;
using System.Threading;

namespace Smr.Utils {
    public class Timer {
        private readonly SynchronizationContext _syncContext;
        private System.Threading.Timer _timer;
        private readonly int _interval;
        private readonly Action _action;

        public Timer(int milliseconds, Action action, SynchronizationContext syncContext = null) {
            _syncContext = syncContext;
            _action = action;
            _interval = milliseconds;
        }

        public Timer Start() {
            if (_interval <= 0) {
                _action?.Invoke();
                return null;
            }

            _timer = new System.Threading.Timer(state => {
                if (_syncContext != null) {
                    _syncContext.Send(_ => {
                        _action?.Invoke();
                    }, null);
                } else {
                    _action?.Invoke();
                }
            }, null, _interval, _interval);

            return this;
        }

        public void Stop() {
            _timer?.Dispose();
            _timer = null;
        }
    }
}