using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Smr.Animations {
    [TrackColor(0.855f, 0.8623f, 0.870f)]
    [TrackClipType(typeof(AnchoredPositionClip))]
    [TrackBindingType(typeof(RectTransform))]
    public class AnchoredPositionTrack : AbstractTrack {
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount) {
            return ScriptPlayable<AnchoredPositionMixerBehaviour>.Create(graph, inputCount);
        }
    }
}