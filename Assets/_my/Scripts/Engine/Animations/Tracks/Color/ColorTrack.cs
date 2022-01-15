using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Smr.Animations {
    [TrackColor(0.7f, 0.7f, 0.7f)]
    [TrackClipType(typeof(ColorClip))]
    [TrackBindingType(typeof(Transform))]
    public class ColorTrack : AbstractTrack {
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount) {
            return ScriptPlayable<ColorMixerBehavior>.Create(graph, inputCount);
        }
    }
}