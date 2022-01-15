using UnityEngine;

namespace Smr.Components {
    public class DontDestroyOnLoadComponent : MonoBehaviour {
        private void Awake() {
            DontDestroyOnLoad(gameObject);
        }
    }
}