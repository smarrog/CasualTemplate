using System;
using System.Collections.Generic;
using Smr.Extensions;
using UnityEngine;
using Vector2 = System.Numerics.Vector2;

namespace Smr.Components {
    public class Scheduler : MonoBehaviour, IScheduler {
        private readonly HashSet<Action<float>> _updateActions = new(); // TODO add groups to control speed
        private readonly Dictionary<SchedulerKey, SchedulerWorker> _workers = new();
        
        private readonly List<(SchedulerKey, SchedulerWorker)> _workersToAdd = new();
        private readonly List<SchedulerKey> _keysToRemove = new();

        private void Awake() {
            DontDestroyOnLoad(this);
        }

        private void Update() {
            var delta = Time.deltaTime;
            
            foreach (var updateAction in _updateActions) {
                updateAction.Invoke(delta);
            }

            foreach (var kv in _workersToAdd) {
                _workers.Add(kv.Item1, kv.Item2);
            }
            _workersToAdd.Clear();

            _workers.RemoveKeys(_keysToRemove);
            _keysToRemove.Clear();
            
            foreach (var (key, worker) in _workers) {
                if (!worker.IsCompleted) {
                    worker.Update(delta);
                }
                if (worker.IsCompleted) {
                    _keysToRemove.Add(key);
                }
            }
        }

        public void Stop(SchedulerKey key) {
            if (key != null) {
                _keysToRemove.Add(key);
            }
        }

        public void RegisterUpdate(Action<float> updateAction) {
            _updateActions.Add(updateAction);
        }

        public void UnregisterUpdate(Action<float> updateAction) {
            _updateActions.Remove(updateAction);
        }

        public SchedulerKey DoEvery(float interval, Action workerAction) {
            var key = new SchedulerKey();
            var schedulerInterval = new SchedulerInterval(interval);
            var worker = new SchedulerDoEveryWorker(schedulerInterval, workerAction);
            _workersToAdd.Add((key, worker));
            return key;
        }

        public SchedulerKey DoEvery(Vector2 interval, Action workerAction) {
            var key = new SchedulerKey();
            var schedulerInterval = new SchedulerInterval(interval);
            var worker = new SchedulerDoEveryWorker(schedulerInterval, workerAction);
            _workersToAdd.Add((key, worker));
            return key;
        }

        public SchedulerKey DoAfter(float interval, Action workerAction) {
            var key = new SchedulerKey();
            var schedulerInterval = new SchedulerInterval(interval);
            var worker = new SchedulerDoAfterWorker(schedulerInterval, workerAction);
            _workers.Add(key, worker);
            return key;
        }

        public SchedulerKey DoAfter(Vector2 interval, Action workerAction) {
            var key = new SchedulerKey();
            var schedulerInterval = new SchedulerInterval(interval);
            var worker = new SchedulerDoAfterWorker(schedulerInterval, workerAction);
            _workers.Add(key, worker);
            return key;
        }
    }
}