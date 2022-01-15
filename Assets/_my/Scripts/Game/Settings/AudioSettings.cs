using System;
using Smr.Audio;
using UnityEngine;

namespace Game {
    [Serializable]
    public class AudioSettings {
        [Range(0, 1)] public float Volume = 1;
        [Header("Events")]
        public AudioEvent Music;
        public AudioEvent Tap;
        public AudioEvent MoneyReward;
        public AudioEvent MoneySpend;
        public AudioEvent Spawn;
        public AudioEvent Merge;
        public AudioEvent Move;
    }
}