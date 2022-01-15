using System;
using Smr.Extensions;
using UnityEngine;

namespace Smr.Components {
    public class ObjectMover : MonoBehaviour {
        private Transform _target;
        private Vector3 _destination;
        private float _speed;
        private Action _onComplete;

        private void Update() {
            if (_target == null) {
                return;
            }

            _target.position = Vector3.MoveTowards(
                _target.position,
                _destination,
                _speed * Time.deltaTime
            );

            if (_target.position.ApproximatelyEqualsTo(_destination)) {
                Stop(true);
            }
        }

        public void MoveTo(Transform target, Vector3 destination, float speed, Action onComplete) {
            _target = target;
            _destination = destination;
            _speed = speed;
            _onComplete = onComplete;
        }

        public void Stop(bool callAction = false) {
            _target = null;
            var action = _onComplete;
            _onComplete = null;
            if (callAction) {
                action?.Invoke();
            }
        }
    }
}