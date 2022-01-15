using System.Collections.Generic;
using Smr.Extensions;
using UnityEngine;

namespace Smr.Audio {
    public class AudioChannelComponent : MonoBehaviour {
        [SerializeField] private AudioChannelType _channelType;
        [SerializeField, Range(0, 1)] private float _volume = 1;
        [SerializeField] private int _maxSimultaneous;
        [SerializeField] private AudioSource _audioSource;

        public AudioChannelType ChannelType => _channelType;

        private readonly List<AudioChannelWorker> _workers = new();
        private bool _isMuted;
        
        private void OnValidate() {
            SetVolume(_volume);
        }

        public void Play(AudioClip clip, float volume) {
            if (clip == null) {
                return;
            }

            var worker = GetFreeWorker();
            if (worker == null) {
                return;
            }
            
            worker.Init(clip, volume);
            worker.Play();
        }

        public void SetMute(bool value) {
            _isMuted = value;
            foreach (var worker in _workers) {
                UpdateChannelVolume(worker);
            }
        }

        public void StopAll() {
            foreach (var worker in _workers) {
                worker.Stop();
            }
        }

        public void SetVolume(float value) {
            _volume = value;
            foreach (var worker in _workers) {
                UpdateChannelVolume(worker);
            }
        }

        private AudioChannelWorker GetFreeWorker() {
            return _workers.FirstOr(worker => worker.IsFree, () => {
                if (_maxSimultaneous > 0 && _workers.Count >= _maxSimultaneous) {
                    return null;
                }
                
                var audioSource = Instantiate(_audioSource, transform);
                audioSource.gameObject.name = $"Audio source {_workers.Count}";
                var worker = new AudioChannelWorker(audioSource);
                UpdateChannelVolume(worker);
                _workers.Add(worker);
                return worker;
            });
        }

        private void UpdateChannelVolume(AudioChannelWorker worker) {
            worker.SetVolumeMultiplier(_isMuted ? 0 : _volume);
        }
    }
}