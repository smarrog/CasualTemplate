using System;
using UnityEngine;

namespace Smr.Animations {
    public class RotationMixerBehavior : AbstractMixerBehaviorWithLerpValue<RotationBehavior, Transform, Quaternion> {
        protected override void SetValue(Transform trackBinding, Quaternion value) {
            trackBinding.rotation = value;
        }

        protected override Quaternion GetValue(Transform trackBinding) {
            return trackBinding.rotation;
        }

        protected override Quaternion GetChange(RotationBehavior behavior, Quaternion startValue, Quaternion endValue, float curveValue) {
            return behavior.InterpolationType switch {
                InterpolationType.Lerp => Quaternion.Lerp(startValue, endValue, curveValue),
                InterpolationType.Slerp => Quaternion.Slerp(startValue, endValue, curveValue),
                InterpolationType.LerpUnclamped => Quaternion.LerpUnclamped(startValue, endValue, curveValue),
                InterpolationType.SlerpUnclamped => Quaternion.SlerpUnclamped(startValue, endValue, curveValue),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        protected override Quaternion ModifyBlendedValue(Quaternion current, Quaternion change, float weight) {
            var currentAngle = current.eulerAngles;
            var changedAngle = Quaternion.Slerp(Quaternion.identity, change, weight).eulerAngles;
            return Quaternion.Euler(currentAngle + changedAngle);
        }
    }
}