using System;
using System.Numerics;

namespace Smr.Components {
    public interface IScheduler {
        void Stop(SchedulerKey key);

        void RegisterUpdate(Action<float> updateAction);
        void UnregisterUpdate(Action<float> updateAction);

        SchedulerKey DoEvery(float interval, Action workerAction);
        SchedulerKey DoEvery(Vector2 interval, Action workerAction);

        SchedulerKey DoAfter(float interval, Action workerAction);
        SchedulerKey DoAfter(Vector2 interval, Action workerAction);
    }
}