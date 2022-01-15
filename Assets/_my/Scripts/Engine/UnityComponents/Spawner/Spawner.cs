using System;
using System.Collections.Generic;
using Smr.Common;
using Smr.Extensions;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Smr.Components {
    public abstract class AbstractSpawner<T> : MonoBehaviour where T : Object {
        [SerializeField] private Transform _container;
        [SerializeField] private Collider _spawnZone;
        [SerializeField] private SpawnRotation _rotationType;
        [SerializeField] private List<T> _prefabs;

        private Transform Container => _container ? _container : transform;

        public void Spawn(Action<T> onSpawn) {
            var prefab = GetPrefab();
            if (!prefab) {
                EngineDependencies.Logger.LogError("Failed to spawn base prefab is null");
                return;
            }
            
            var position = GetPosition();
            var rotation = GetRotation();
            var instance = Instantiate(prefab, position, rotation, Container);
            onSpawn?.Invoke(instance);
        }

        protected virtual T GetPrefab() {
            return _prefabs.GetRandomNotNull();
        }

        protected virtual Vector3 GetPosition() {
            var bounds = _spawnZone.bounds;
            return new Vector3(
                Random.Range(bounds.min.x, bounds.max.x),
                Random.Range(bounds.min.y, bounds.max.y),
                Random.Range(bounds.min.z, bounds.max.z)
            );
        }

        protected virtual Quaternion GetRotation() {
            return _rotationType switch {
                SpawnRotation.Identity => Quaternion.identity,
                SpawnRotation.Container => Container.rotation,
                SpawnRotation.Random => Random.rotation,
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}