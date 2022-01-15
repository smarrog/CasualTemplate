using UnityEngine;

namespace Smr.Animations {
    public class ScaleMixerBehavior : AbstractMixerBehaviorForVector3<ScaleBehavior, Transform> {
        protected override void CorrectBehaviorAfterInitialization(ScaleBehavior behavior) {
            base.CorrectBehaviorAfterInitialization(behavior);

            behavior.StartValue = GetCorrectedValue(behavior, behavior.StartValue);
            behavior.EndValue = GetCorrectedValue(behavior, behavior.EndValue);
        }

        protected override Vector3 GetValue(Transform trackBinding) {
            return trackBinding.localScale;
        }

        protected override void SetValue(Transform trackBinding, Vector3 value) {
            trackBinding.localScale = value;
        }

        private Vector3 GetCorrectedValue(ScaleBehavior behavior, Vector3 value) {
            return new Vector3(
                GetCorrectedValue(behavior, value.x, InitialValue.x),
                GetCorrectedValue(behavior, value.y, InitialValue.y),
                GetCorrectedValue(behavior, value.z, InitialValue.z));
        }

        private float GetCorrectedValue(ScaleBehavior behavior, float value, float initialValue) {
            if (behavior.IsRelative) {
                return initialValue * value;
            }

            return behavior.SaveStartSign
                ? GetValueOfSameSign(value, initialValue)
                : value;
        }

        private Vector3 GetCorrectedValueFor(Vector3 value) {
            return new Vector3(
                GetValueOfSameSign(value.x, InitialValue.x),
                GetValueOfSameSign(value.y, InitialValue.y),
                GetValueOfSameSign(value.z, InitialValue.z));
        }

        private float GetValueOfSameSign(float value, float referenceValue) {
            if (referenceValue > 0) {
                return value > 0 ? value : -value;
            }
            if (referenceValue < 0) {
                return value > 0 ? -value : value;
            }
            return value;
        }
    }
}