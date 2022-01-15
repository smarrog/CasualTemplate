using UnityEngine;

namespace Smr.Commands {
    public class MoveCommand : AbstractCommand {
        private readonly Transform _target;
        private readonly Vector3 _worldPosition;
        private readonly float _speed;

        public MoveCommand(Transform target, Vector3 worldPosition, float speed) {
            _target = target;
            _worldPosition = worldPosition;
            _speed = speed;
        }

        protected override void ExecuteInternal() {
            // TODO
            //World.MoveController.Move(_target, _worldPosition, _speed, OnMovementComplete);
        }

        protected virtual void OnMovementComplete() {
            NotifyComplete();
        }
    }
}