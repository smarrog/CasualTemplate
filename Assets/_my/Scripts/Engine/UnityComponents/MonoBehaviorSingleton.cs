using Smr.Common;
using UnityEngine;

namespace Smr.Components {
    public abstract class MonoBehaviorSingleton<T> : MonoBehaviour where T : MonoBehaviour {
        public static T Instance { get; private set; }

        private void Awake() {
            if (Instance) {
                EngineDependencies.Logger.LogError($"Several instances of {typeof(T)} are not supported");
                return;
            }

            Instance = FindFirstObjectByType<T>();
            DontDestroyOnLoad(this);
            
            AwakeInternal();
        }

        private void OnDestroy() {
            Instance = null;
            OnDestroyInternal();
        }

        protected abstract void AwakeInternal();
        protected virtual void OnDestroyInternal() {}
    }
}