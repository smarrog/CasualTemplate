using System;
using System.Collections.Generic;
using Smr.Common;
using Smr.Extensions;
using UnityEngine;

namespace Smr.Services {
    public class MoveService : IMoveService {
        private readonly HashSet<MoveTask> _tasks = new();

        public void Reset() {
            _tasks.Clear();
        }

        public void Update(float deltaTime) {
            _tasks.RemoveWhere(task => {
                task.Update(deltaTime);
                return task.IsCompleted;
            });
        }

        public void Move(Transform target, Vector3 worldPosition, float speed, Action onComplete = null) {
            if (speed <= 0) {
                EngineDependencies.Logger.LogError("Can't perform move cause speed is below or equal zero");
                return;
            }

            var task = new MoveTask(target, worldPosition, speed, onComplete);
            _tasks.Add(task);
        }

        private class MoveTask {
            public bool IsCompleted { get; private set; }

            private readonly Transform _target;
            private readonly Vector3 _destination;
            private readonly float _speed;
            private readonly Action _onComplete;

            public MoveTask(Transform target, Vector3 destination, float speed, Action onComplete = null) {
                _target = target;
                _destination = destination;
                _speed = speed;
                _onComplete = onComplete;
            }

            public void Update(float deltaTime) {
                if (IsCompleted) {
                    return;
                }

                var currentPosition = _target.position;
                currentPosition = Vector3.MoveTowards(currentPosition, _destination, deltaTime * _speed);
                _target.position = currentPosition;
                IsCompleted = currentPosition.ApproximatelyEqualsTo(_destination);

                if (IsCompleted) {
                    _onComplete?.Invoke();
                }
            }
        }
    }
}