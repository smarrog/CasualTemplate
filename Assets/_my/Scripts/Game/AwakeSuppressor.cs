using UnityEngine;

namespace Game {
    [DefaultExecutionOrder(-5000)]
    public class AwakeSuppressor : MonoBehaviour {
        private void Awake() {
            // если запускаемся не с прелоада, то первоначальные awake нужно игнорировать
            gameObject.SetActive(App.IsReady);
        }
    }
}