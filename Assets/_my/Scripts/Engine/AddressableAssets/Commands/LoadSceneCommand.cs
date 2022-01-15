#if SMR_ADDRESSABLES
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using Smr.AddressableAssets.Editor;

#endif

namespace Smr.AddressableAssets {
    public class LoadSceneCommand : AbstractAddressableCommand<SceneInstance> {
        private readonly AddressableKey _key;

        private readonly LoadSceneMode _mode;

        internal LoadSceneCommand(AddressableKey key, LoadSceneMode mode) {
            _key = key;
            _mode = mode;
        }

        protected override void ExecuteInternal() {
#if UNITY_EDITOR
            if (!Application.isPlaying) {
                var sceneAsset = Editor.AddressableAssets.LoadAssetInEditor<SceneAsset>(_key);
                if (sceneAsset) {
                    var scenePath = AssetDatabase.GetAssetPath(sceneAsset);
                    EditorSceneManager.OpenScene(scenePath);
                    NotifyComplete();
                    return;
                }
            }
#endif

            base.ExecuteInternal();
        }

        protected override AsyncOperationHandle<SceneInstance> CreateOperationHandle() {
            return Addressables.LoadSceneAsync(_key, _mode);
        }
    }
}
#endif