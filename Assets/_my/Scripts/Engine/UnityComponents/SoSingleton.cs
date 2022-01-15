using UnityEngine;

namespace Smr.Components {
    public class SoSingleton<T> : ScriptableObject where T : ScriptableObject {
        public static T Instance { get; private set; }
        public static T Get => Instance;

        private void OnEnable() {
            if (Instance != null) {
                throw new UnityException("There are more than one instance of" + typeof(T));
            }
            Instance = Resources.FindObjectsOfTypeAll<T>()[0];
        }

        private void OnDisable() {
            Instance = null;
        }
    }
}