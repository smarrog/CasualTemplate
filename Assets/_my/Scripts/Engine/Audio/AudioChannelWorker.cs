using UnityEngine;

namespace Smr.Audio {
    public class AudioChannelWorker {
        public bool IsFree => !_audioSource.isPlaying;
        
        private float CurrentVolume => _volumeMultiplier * _originalVolume;

        private readonly AudioSource _audioSource;
        private float _originalVolume;
        private float _volumeMultiplier;

        public AudioChannelWorker(AudioSource audioSource) {
            _audioSource = audioSource;
        }

        public void Init(AudioClip clip, float volume) {
            _originalVolume = volume;
            _audioSource.clip = clip;
            UpdateClipVolume();
        }

        public void Play() {
            _audioSource.Play();
        }

        public void Stop() {
            _audioSource.Stop();
        }

        public void SetVolumeMultiplier(float value) {
            _volumeMultiplier = value;
            UpdateClipVolume();
        }

        private void UpdateClipVolume() {
            _audioSource.volume = CurrentVolume;
        }
    }
}