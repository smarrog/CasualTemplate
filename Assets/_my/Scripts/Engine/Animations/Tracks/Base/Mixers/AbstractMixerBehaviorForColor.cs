using UnityEngine;

namespace Smr.Animations {
    public abstract class AbstractMixerBehaviorForColor<TBehavior, TTrackBinding>
        : AbstractMixerBehaviorWithLerpValue<TBehavior, TTrackBinding, Color>
        where TBehavior : AbstractBehaviorWithLerpValue<Color>, new()
        where TTrackBinding : Component {
        protected override Color GetChange(TBehavior behavior, Color startValue, Color endValue, float curveValue) {
            return startValue + (endValue - startValue) * curveValue;
        }

        protected override Color ModifyBlendedValue(Color current, Color change, float weight) {
            return current + change * weight;
        }
    }
}