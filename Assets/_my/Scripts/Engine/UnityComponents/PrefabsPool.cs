using System.Collections.Generic;
using UnityEngine;

namespace Smr.Components {
    public class PrefabsPool<T> : MonoBehaviour where T : MonoBehaviour {
        [SerializeField] private int _warmupCount;
        [SerializeField] private T _prefab;
        [SerializeField] private Transform _container;

        private Transform _poolContainer;
        private readonly Queue<T> _pool = new();

        private void Awake() {
            if (!_container) {
                var containerObject = new GameObject($"Pool of {typeof(T)}");
                DontDestroyOnLoad(containerObject);
                _poolContainer = containerObject.transform;
            } else {
                _poolContainer = _container;
            }
            
            for (var i = 0; i < _warmupCount; ++i) {
                var instance = Instantiate(_prefab, _poolContainer);
                _pool.Enqueue(instance);
            }
            
            _poolContainer.gameObject.SetActive(false);
        }

        public T Take(Transform parent = null) {
            if (!parent) {
                parent = _poolContainer;
            }
            if (_pool.Count == 0) {
                return Instantiate(_prefab, parent);
            }
            var result = _pool.Dequeue();
            result.transform.SetParent(parent, false);
            return result;
        }

        public void Return(T instance) {
            instance.transform.SetParent(_poolContainer, false);
            _pool.Enqueue(instance);
        }
    }
}