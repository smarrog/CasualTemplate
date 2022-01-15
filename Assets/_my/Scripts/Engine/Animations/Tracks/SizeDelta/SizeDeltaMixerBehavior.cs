using UnityEngine;

namespace Smr.Animations {
    public class SizeDeltaMixerBehavior : AbstractMixerBehaviorForVector2<SizeDeltaBehavior, RectTransform> {
        protected override Vector2 GetValue(RectTransform trackBinding) {
            return trackBinding.sizeDelta;
        }

        protected override void SetValue(RectTransform trackBinding, Vector2 value) {
            trackBinding.sizeDelta = value;
        }
    }
}