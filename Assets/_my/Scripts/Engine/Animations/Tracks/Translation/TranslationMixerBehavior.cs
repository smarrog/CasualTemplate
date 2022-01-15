using UnityEngine;

namespace Smr.Animations {
    public class TranslationMixerBehavior : AbstractMixerBehaviorForVector3<TranslationBehavior, Transform> {
        protected override bool IsValidBehavior(TranslationBehavior behavior) {
            return behavior.ExposedStartValue != null || behavior.ExposedEndValue != null;
        }

        protected override Vector3 GetValue(Transform trackBinding) {
            return trackBinding.position;
        }

        protected override void SetValue(Transform trackBinding, Vector3 value) {
            trackBinding.position = value;
        }
    }
}