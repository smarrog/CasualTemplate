using System;

namespace Smr.Components {
    public class SchedulerDoEveryWorker : SchedulerWorker {
        public override bool IsCompleted => false;

        private int _timesCalled;
        private float _interval;

        public SchedulerDoEveryWorker(SchedulerInterval interval, Action workerAction)
            : base(interval, workerAction) {
            _interval = Interval.GetNextInterval();
        }
        
        protected override void UpdateInternal() {
            var unprocessedTime = TimeFromActionStart - _timesCalled * _interval;
            while (unprocessedTime >= _interval) {
                unprocessedTime -= _interval;
                _timesCalled++;
                _interval = Interval.GetNextInterval();
                DoAction();
            }
        }
    }
}