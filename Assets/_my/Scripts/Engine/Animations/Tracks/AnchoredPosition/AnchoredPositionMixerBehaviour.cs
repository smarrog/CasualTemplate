using UnityEngine;

namespace Smr.Animations {
    public class AnchoredPositionMixerBehaviour : AbstractMixerBehaviorForVector2<AnchoredPositionBehavior, RectTransform> {
        protected override Vector2 GetValue(RectTransform trackBinding) {
            return trackBinding.anchoredPosition;
        }

        protected override void SetValue(RectTransform trackBinding, Vector2 value) {
            trackBinding.anchoredPosition = value;
        }
    }
}