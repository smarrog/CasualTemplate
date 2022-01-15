using UnityEngine;

namespace Smr.Animations {
    public abstract class AbstractMixerBehaviorForDouble<TBehavior, TTrackBinding>
        : AbstractMixerBehaviorWithLerpValue<TBehavior, TTrackBinding, double>
        where TBehavior : AbstractBehaviorWithLerpValue<double>, new()
        where TTrackBinding : Component {
        protected override double GetChange(TBehavior behavior, double startValue, double endValue, float curveValue) {
            return startValue + (endValue - startValue) * curveValue;
        }

        protected override double ModifyBlendedValue(double current, double change, float weight) {
            return current + change * weight;
        }
    }
}