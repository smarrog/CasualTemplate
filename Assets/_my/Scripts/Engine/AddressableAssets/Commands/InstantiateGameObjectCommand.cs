#if SMR_ADDRESSABLES
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
#if UNITY_EDITOR
using Smr.AddressableAssets.Editor;
#endif

namespace Smr.AddressableAssets {
    public class InstantiateGameObjectCommand : AbstractAddressableCommand<GameObject> {
        private readonly AddressableKey _key;
        private readonly Transform _parent;

        public InstantiateGameObjectCommand(AddressableKey key, Transform parent) {
            _key = key;
            _parent = parent;
        }

        protected override void ExecuteInternal() {
#if UNITY_EDITOR
            if (!Application.isPlaying) {
                SetResultAndComplete(Object.Instantiate(Editor.AddressableAssets.LoadAssetInEditor<GameObject>(_key), _parent));
                return;
            }
#endif
            base.ExecuteInternal();
        }

        protected override AsyncOperationHandle<GameObject> CreateOperationHandle() {
            return Addressables.InstantiateAsync(_key, _parent);
        }
    }
}
#endif