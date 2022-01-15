using System;
using System.Collections.Generic;
using Smr.Components;
using Smr.Utils;
using UnityEngine;

namespace Smr.Audio {
    public class AudioService : MonoBehaviorSingleton<AudioService>, IAudioService {
        [SerializeField] private List<AudioChannelComponent> _channels;

        private bool IsMuted => _muteLocksContainer.IsLocked;
        
        private readonly Dictionary<AudioChannelType, AudioChannelComponent> _channelsByType = new();
        private readonly LocksContainer _muteLocksContainer = new();
        
        protected override void AwakeInternal() {
            foreach (var channel in _channels) {
                if (!_channelsByType.TryAdd(channel.ChannelType, channel)) {
                    throw new Exception($"Several audio channels have type: {channel.ChannelType}");
                }
                channel.SetMute(IsMuted);
            }

            _muteLocksContainer.OnLock += UpdateChannelsMute;
            _muteLocksContainer.OnUnlock += UpdateChannelsMute;
        }

        public void Play(AudioEvent audioEvent) {
            if (!audioEvent) {
                return;
            }
            
            var channel = GetChannel(audioEvent.ChannelType);
            if (!channel) {
                return;
            }

            var clip = audioEvent.GetClip();
            channel.Play(clip, audioEvent.Volume);
        }
        
        public void Mute(object requester) {
            _muteLocksContainer.Lock(requester);
        }
        
        public void Unmute(object requester) {
            _muteLocksContainer.Unlock(requester);
        }

        public AudioChannelComponent GetChannel(AudioChannelType channelType) {
            return _channelsByType.GetValueOrDefault(channelType);
        }

        private void UpdateChannelsMute() {
            foreach (var audioChannel in _channelsByType.Values) {
                audioChannel.SetMute(IsMuted);
            }
        }
    }
}