using UnityEngine;

namespace Smr.Animations {
    public abstract class AbstractMixerBehaviorForVector3<TBehavior, TTrackBinding> : AbstractMixerBehaviorWithLerpValue<TBehavior, TTrackBinding, Vector3>
        where TBehavior : AbstractBehaviorWithLerpValue<Vector3>, new()
        where TTrackBinding : Component {
        protected override Vector3 GetChange(TBehavior behavior, Vector3 startValue, Vector3 endValue, float curveValue) {
            return startValue + (endValue - startValue) * curveValue;
        }

        protected override Vector3 ModifyBlendedValue(Vector3 current, Vector3 change, float weight) {
            return current + change * weight;
        }
    }
}