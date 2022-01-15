#if SMR_ADDRESSABLES
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Smr.Common;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Object = UnityEngine.Object;

namespace Smr.AddressableAssets {
    public class AddressableObjectsPool : IDisposable {
        private const int DEFAULT_START_ELEMENT_COUNT = 8;
        private static readonly Dictionary<AddressableKey, AddressableObjectsPool> _allAvailablePools = new Dictionary<AddressableKey, AddressableObjectsPool>();
        private static GameObject _addressablePoolRootObject;

        private readonly int _elementCount;
        private readonly AddressableKey _keyToInstantiate;
        private GameObject _poolGameObject;

        private Stack<GameObject> _pool;
        private List<GameObject> _allCreatedObjects = new List<GameObject>();

        public bool NeedDetachTransform { get; set; } = true;


        public static AddressableObjectsPool GetPool(AddressableKey key, int? elementCount = null) {
            var exists = _allAvailablePools.TryGetValue(key, out AddressableObjectsPool pool);
            if (!exists) {
                pool = new AddressableObjectsPool(key, elementCount);
                _allAvailablePools[key] = pool;
            }
            return pool;
        }

        public static void ClearPools() {
            _allAvailablePools.Clear();
        }

        private AddressableObjectsPool(AddressableKey key, int? startElementCount) {
            _keyToInstantiate = key;
            _elementCount = startElementCount ?? DEFAULT_START_ELEMENT_COUNT;
            _pool = new Stack<GameObject>(_elementCount);
        }

        public T Take<T>(Transform parent) {
            var obj = TakeGameObject(parent);
            return GetComponent<T>(obj);
        }

        public async Task<T> TakeAsync<T>(Transform parent) {
            var obj = await TakeGameObjectAsync(parent);
            return GetComponent<T>(obj);
        }

        public GameObject TakeGameObject(Transform parent) {
            TryExpandPool();
            return TakeObject(parent);
        }

        public async Task<GameObject> TakeGameObjectAsync(Transform parent) {
            await TryExpandPoolAsync();
            return TakeObject(parent);
        }

        public void Return(GameObject gameObjectToReturn) {
            gameObjectToReturn.SetActive(false);
            if (NeedDetachTransform) {
                gameObjectToReturn.transform.SetParent(null);
            }
            _pool.Push(gameObjectToReturn);
        }

        public void Dispose() {
            if (_pool == null) {
                return;
            }

            if (_keyToInstantiate != null) {
                _allAvailablePools.Remove(_keyToInstantiate);
            }

            foreach (var obj in _allCreatedObjects) {
                if (obj) {
                    Addressables.ReleaseInstance(obj);
                }
            }

            if (_poolGameObject) {
                Object.Destroy(_poolGameObject);
            }

            _poolGameObject = null;
            _allCreatedObjects = null;
            _pool = null;
        }


        private void TryExpandPool() {
            if (_pool.Count > 0) {
                return;
            }

            var expandPoolCommands = GetExpandPoolCommands();
            foreach (var cmd in expandPoolCommands) {
                cmd.Execute();
            }
        }

        private async Task TryExpandPoolAsync() {
            if (_pool.Count > 0) {
                return;
            }

            var expandPoolCommands = GetExpandPoolCommands();
            foreach (var cmd in expandPoolCommands) {
                await cmd.Execute();
            }
        }

        private GameObject TakeObject(Transform parent) {
            if (_pool.Count > 0) {
                var newGameObject = _pool.Pop();
                newGameObject.transform.SetParent(parent, false);
                newGameObject.SetActive(true);
                return newGameObject;
            }

            EngineDependencies.Logger.GetChannel(LogChannel.Addressables).LogError($"Can't take object from pool, size: {_pool.Count} objectAddress:{_keyToInstantiate}");
            return default;
        }

        private T GetComponent<T>(GameObject gameObject) {
            if (!gameObject) {
                return default;
            }

            var result = gameObject.GetComponent<T>();
            if (result == null) {
                EngineDependencies.Logger.GetChannel(LogChannel.Addressables).LogError($"Can't get component with type {typeof(T)} from gameObject loaded by address {_keyToInstantiate}");
            }
            return result;
        }

        private AbstractAddressableCommand<GameObject>[] GetExpandPoolCommands() {
            CreateRootObjectsIfNeeded();

            var result = new AbstractAddressableCommand<GameObject>[_elementCount];
            // TODO
            // for (var i = 0; i < _elementCount; i++) {
            //     var cmd = new InstantiateGameObjectCommand(_keyToInstantiate, null);
            //     cmd.AddCompleteHandler(_ => {
            //         if (cmd.IsSucceed) {
            //             var newGameObject = cmd.Result;
            //             newGameObject.SetActive(false);
            //             newGameObject.transform.SetParent(_poolGameObject.transform);
            //             _pool.Push(newGameObject);
            //             _allCreatedObjects.Add(newGameObject);
            //         }
            //     });
            //     result[i] = cmd;
            // }
            return result;
        }

        private void CreateRootObjectsIfNeeded() {
            if (!_addressablePoolRootObject) {
                _addressablePoolRootObject = new GameObject("addressablePools");
            }
            if (!_poolGameObject) {
                _poolGameObject = new GameObject(_keyToInstantiate.ToString());
                _poolGameObject.transform.SetParent(_addressablePoolRootObject.transform);
            }
        }
    }
}
#endif