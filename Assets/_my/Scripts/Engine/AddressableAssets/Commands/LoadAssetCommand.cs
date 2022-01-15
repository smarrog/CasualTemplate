#if SMR_ADDRESSABLES
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
#if UNITY_EDITOR
using Smr.AddressableAssets.Editor;
#endif

namespace Smr.AddressableAssets {
    public class LoadAssetCommand<T> : AbstractAddressableCommand<T> where T : Object {
        private readonly AddressableKey _key;

        public LoadAssetCommand(AddressableKey key) {
            _key = key;
        }

        protected override AsyncOperationHandle<T> CreateOperationHandle() {
            return Addressables.LoadAssetAsync<T>(_key);
        }

        protected override void ExecuteInternal() {
#if UNITY_EDITOR
            if (!Application.isPlaying) {
                var result = Editor.AddressableAssets.LoadAssetInEditor<T>(_key);
                if (result) {
                    SetResultAndComplete(result);
                    return;
                }
            }
#endif
            base.ExecuteInternal();
        }
    }
}
#endif