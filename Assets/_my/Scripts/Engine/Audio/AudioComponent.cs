using UnityEngine;

namespace Smr.Audio {
    public class AudioComponent : MonoBehaviour {
        [SerializeField] private bool _playOnEnabled = true;
        [SerializeField] private AudioEvent _event;

        private void OnEnable() {
            if (_playOnEnabled) {
                Play();
            }
        }

        public void Play() {
            if (AudioService.Instance) {
                AudioService.Instance.Play(_event);
            }
        }
    }
}