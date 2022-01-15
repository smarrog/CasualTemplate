#if SMR_ADDRESSABLES
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;

namespace Smr.AddressableAssets {
    public class UnloadSceneCommand : AbstractAddressableCommand<SceneInstance> {
        private readonly SceneInstance _sceneInstance;

        internal UnloadSceneCommand(SceneInstance sceneInstance) {
            _sceneInstance = sceneInstance;
        }

        protected override AsyncOperationHandle<SceneInstance> CreateOperationHandle() {
            return Addressables.UnloadSceneAsync(_sceneInstance, true);
        }
    }
}
#endif