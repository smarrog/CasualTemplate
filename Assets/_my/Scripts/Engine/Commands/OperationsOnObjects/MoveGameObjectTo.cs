using Smr.Components;
using UnityEngine;

namespace Smr.Commands {
    public class MoveGameObjectTo : AbstractCommand {
        private readonly GameObject _target;
        private readonly Transform _targetTransform;
        private readonly Vector3 _destination;
        private readonly float _speed;

        private ObjectMover _mover;

        public MoveGameObjectTo(GameObject target, Vector3 destination, float speed) {
            _target = target;
            _targetTransform = _target.transform;
            _destination = destination;
            _speed = speed;
        }

        protected override void ExecuteInternal() {
            _mover = _target.AddComponent<ObjectMover>();
            _mover.MoveTo(_targetTransform, _destination, _speed, () => {
                Object.Destroy(_mover);
                NotifyComplete();
            });
        }

        protected override void CleanUpInternal(CommandResult result) {
            if (_mover) {
                Object.Destroy(_mover);
            }
        }
    }
}