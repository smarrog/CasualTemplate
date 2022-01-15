using System;

namespace Smr.Components {
    public class SchedulerDoAfterWorker : SchedulerWorker {
        public override bool IsCompleted => TimeFromActionStart >= _interval;
        
        private readonly float _interval;
        
        public SchedulerDoAfterWorker(SchedulerInterval interval, Action workerAction)
            : base(interval, workerAction) {
            _interval = Interval.GetNextInterval();
        }
        
        protected override void UpdateInternal() {
            if (IsCompleted) {
                DoAction();
            }
        }
    }
}