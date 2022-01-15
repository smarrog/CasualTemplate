using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Smr.Animations {
    [TrackColor(0.855f, 0.8623f, 0.870f)]
    [TrackClipType(typeof(BezierTranslationClip))]
    [TrackBindingType(typeof(Transform))]
    public class BezierTranslationTrack : AbstractTrack {
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount) {
            return ScriptPlayable<BezierTranslationMixerBehavior>.Create(graph, inputCount);
        }
    }
}