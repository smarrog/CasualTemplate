using System;
using UnityEngine;

namespace Smr.Animations {
    [Serializable]
    public class RotationClip : AbstractClipWithLerpValue<RotationBehavior, Vector3, Quaternion> {
        public InterpolationType InterpolationType = InterpolationType.SlerpUnclamped;

        protected override Quaternion BehaviorStartValue => Quaternion.Euler(StartValue);
        protected override Quaternion BehaviorEndValue => Quaternion.Euler(EndValue);

        protected override void FillBehavior(RotationBehavior behavior, IExposedPropertyTable resolver) {
            base.FillBehavior(behavior, resolver);
            behavior.InterpolationType = InterpolationType;
        }
    }
}