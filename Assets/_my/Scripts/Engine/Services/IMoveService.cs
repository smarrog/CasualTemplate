using System;
using UnityEngine;

namespace Smr.Services {
    public interface IMoveService {
        public void Reset();
        public void Update(float deltaTime);
        public void Move(Transform target, Vector3 worldPosition, float speed, Action onComplete = null);
    }
}