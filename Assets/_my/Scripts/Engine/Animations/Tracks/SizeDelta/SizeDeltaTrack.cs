using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Smr.Animations {
    [TrackColor(0.855f, 0.8623f, 0.870f)]
    [TrackClipType(typeof(SizeDeltaClip))]
    [TrackBindingType(typeof(RectTransform))]
    public class SizeDeltaTrack : AbstractTrack {
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount) {
            return ScriptPlayable<SizeDeltaMixerBehavior>.Create(graph, inputCount);
        }
    }
}