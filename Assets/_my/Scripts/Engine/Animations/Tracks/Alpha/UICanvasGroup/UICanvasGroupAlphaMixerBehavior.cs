using UnityEngine;

namespace Smr.Animations {
    public class UICanvasGroupAlphaMixerBehavior : AbstractMixerBehaviorForFloat<AlphaBehavior, CanvasGroup> {
        protected override float GetValue(CanvasGroup trackBinding) {
            return trackBinding.alpha;
        }
        protected override void SetValue(CanvasGroup trackBinding, float alpha) {
            trackBinding.alpha = alpha;
        }
    }
}