using System;
using UnityEngine;
using UnityEngine.Timeline;

namespace Smr.Animations {
    [Serializable]
    public class AnimationClip : AbstractClip<AnimationBehavior> {
        public override ClipCaps clipCaps => ClipCaps.None;
        protected override void FillBehavior(AnimationBehavior behavior, IExposedPropertyTable resolver) {}
    }
}