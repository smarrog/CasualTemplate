using UnityEngine;
using UnityEngine.Timeline;

namespace Smr.Animations {
    [TrackColor(0.255f, 0.2623f, 0.870f)]
    [TrackClipType(typeof(AnimationClip))]
    [TrackBindingType(typeof(Animation))]
    public class AnimationTrack : AbstractTrack {}
}