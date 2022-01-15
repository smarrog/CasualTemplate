using System.Collections.Generic;
using Smr.Extensions;
using UnityEngine;

namespace Smr.Audio {
    [CreateAssetMenu(fileName = "AudioEvent", menuName = "Audio/Event", order = 0)]
    public class AudioEvent : ScriptableObject {
        [SerializeField] private AudioChannelType _channelType;
        [SerializeField, Range(0, 1)] private float _volume = 1;
        [SerializeField] private List<AudioClip> _clips;
        
        public AudioChannelType ChannelType => _channelType;
        public float Volume => _volume;
        
        public AudioClip GetClip(int index = -1) {
            if (_clips == null || _clips.Count == 0) {
                return null;
            }

            if (index < 0) {
                return _clips.GetRandom();
            }

            return _clips.GetAtOrLast(index);
        }
    }
}