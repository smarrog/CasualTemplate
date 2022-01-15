using System;
using UnityEngine;

namespace Smr.Animations {
    [Serializable]
    public class ScaleClip : AbstractClipWithLerpValue<ScaleBehavior, Vector3> {
        public bool SaveStartSign;
        public bool IsRelative;

        protected override void FillBehavior(ScaleBehavior behavior, IExposedPropertyTable resolver) {
            base.FillBehavior(behavior, resolver);
            behavior.SaveStartSign = SaveStartSign;
            behavior.IsRelative = IsRelative;
        }
    }
}