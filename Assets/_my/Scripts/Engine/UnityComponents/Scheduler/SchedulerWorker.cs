using System;

namespace Smr.Components {
    public abstract class SchedulerWorker {
        public abstract bool IsCompleted { get; }
        
        protected float TimeFromActionStart;
        protected SchedulerInterval Interval;

        private readonly Action _workerAction;

        protected SchedulerWorker(SchedulerInterval interval, Action workerAction) {
            Interval = interval;
            _workerAction = workerAction;
        }

        // returns true if task is fully completed and need to be removed
        public void Update(float deltaTime) {
            TimeFromActionStart += deltaTime;
            UpdateInternal();
        }

        protected abstract void UpdateInternal();

        protected void DoAction() {
            _workerAction?.Invoke();
        }
    }
}