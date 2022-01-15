using UnityEngine;

namespace Smr.Animations {
    public abstract class AbstractMixerBehaviorForVector2<TBehavior, TTrackBinding>
        : AbstractMixerBehaviorWithLerpValue<TBehavior, TTrackBinding, Vector2>
        where TBehavior : AbstractBehaviorWithLerpValue<Vector2>, new()
        where TTrackBinding : Component {
        protected override Vector2 GetChange(TBehavior behavior, Vector2 startValue, Vector2 endValue, float curveValue) {
            return startValue + (endValue - startValue) * curveValue;
        }

        protected override Vector2 ModifyBlendedValue(Vector2 current, Vector2 change, float weight) {
            return current + change * weight;
        }
    }
}