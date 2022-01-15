using UnityEngine;
using UnityEngine.UI;

namespace Smr.Animations {
    public class UIGraphicAlphaTweenMixerBehavior : AbstractMixerBehaviorForFloat<AlphaBehavior, Graphic> {
        protected override float GetValue(Graphic trackBinding) {
            return trackBinding.color.a;
        }
        protected override void SetValue(Graphic trackBinding, float alpha) {
            var color = trackBinding.color;
            color = new Color(color.r, color.g, color.b, alpha);
            trackBinding.color = color;
        }
    }
}