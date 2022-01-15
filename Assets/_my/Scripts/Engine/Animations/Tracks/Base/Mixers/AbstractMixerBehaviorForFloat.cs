using UnityEngine;

namespace Smr.Animations {
    public abstract class AbstractMixerBehaviorForFloat<TBehavior, TTrackBinding>
        : AbstractMixerBehaviorWithLerpValue<TBehavior, TTrackBinding, float>
        where TBehavior : AbstractBehaviorWithLerpValue<float>, new()
        where TTrackBinding : Component {
        protected override float GetChange(TBehavior behavior, float startValue, float endValue, float curveValue) {
            return startValue + (endValue - startValue) * curveValue;
        }

        protected override float ModifyBlendedValue(float current, float change, float weight) {
            return current + change * weight;
        }
    }
}