using UnityEngine;
using UnityEngine.Playables;

namespace Smr.Components {
    public class PlayableDirectorsContainer : MonoBehaviour {
        private void Awake() {
            foreach (var director in GetComponentsInChildren<PlayableDirector>()) {
                director.playOnAwake = false;
                director.Stop();
            }
        }
    }
}