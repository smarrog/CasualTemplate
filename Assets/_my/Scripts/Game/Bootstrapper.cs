using System;
using Cysharp.Threading.Tasks;
using Smr.Extensions;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game {
    [DefaultExecutionOrder(int.MinValue)]
    public class Bootstrapper : MonoBehaviour {
        [SerializeField] private string _initialScene = "preload";

        private static bool _isRegistered;

        private void Awake() {
            if (_isRegistered) {
                Destroy(gameObject);
                return;
            }
            
            Bootstrap().HandleException();
        }

        private async UniTask Bootstrap() {
            _isRegistered = true;
            DontDestroyOnLoad(this);

            var activeSceneName = SceneManager.GetActiveScene().name;
            if (!string.Equals(activeSceneName, _initialScene, StringComparison.CurrentCultureIgnoreCase)) {
                await SceneManager.LoadSceneAsync(_initialScene);
            }
        }
    }
}