using System;
using System.Collections.Generic;

namespace Smr.Utils {
    public class LocksContainer {
        private readonly HashSet<object> _requesters = new();
        public event Action OnUnlock;
        public event Action OnLock;

        public bool IsLocked => _requesters.Count > 0;

        public void Lock(object requester) {
            var wasLocked = IsLocked;
            _requesters.Add(requester);
            if (wasLocked || !IsLocked) {
                return;
            }
            OnLock?.Invoke();
        }

        public void Unlock(object requester) {
            var wasLocked = IsLocked;
            _requesters.Remove(requester);
            if (!wasLocked || IsLocked) {
                return;
            }
            OnUnlock?.Invoke();
        }

        public void Reset() {
            _requesters.Clear();
            OnLock = null;
            OnUnlock = null;
        }
    }
}