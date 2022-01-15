using UnityEngine;

namespace Smr.Components {
    public class ParticlesAggregator : MonoBehaviour {
        private ParticleSystem[] _systems;
        
        private ParticleSystem[] Systems => _systems ??= GetComponentsInChildren<ParticleSystem>();

        public void Play() {
            foreach (var system in Systems) {
                system.Play();
            }
        }

        public void Stop() {
            foreach (var system in Systems) {
                system.Stop();
            }
        }
    }
}