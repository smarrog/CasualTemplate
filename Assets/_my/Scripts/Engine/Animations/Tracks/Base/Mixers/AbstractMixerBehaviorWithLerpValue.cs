using UnityEngine;

namespace Smr.Animations {
    public abstract class AbstractMixerBehaviorWithLerpValue<TBehavior, TTrackBinding, TBehaviorValue> :
        AbstractMixerBehavior<TBehavior, TTrackBinding>
        where TBehavior : AbstractBehaviorWithLerpValue<TBehaviorValue>, new()
        where TTrackBinding : Component {
        private TBehaviorValue _blendedValue;
        protected TBehaviorValue InitialValue { get; private set; }


        protected abstract TBehaviorValue GetValue(TTrackBinding trackBinding);
        protected abstract void SetValue(TTrackBinding trackBinding, TBehaviorValue value);

        protected override void ProcessFirstFrameInit(TTrackBinding trackBinding) {
            InitialValue = GetValue(trackBinding);
            base.ProcessFirstFrameInit(trackBinding);
        }

        protected override void ProcessFrameInternalInit(TTrackBinding trackBinding) {
            _blendedValue = default;
        }

        protected override void ProcessFrameInternalComplete(TTrackBinding trackBinding) {
            SetValue(trackBinding, _blendedValue);
        }

        protected override void CorrectBehaviorAfterInitialization(TBehavior behavior) {
            base.CorrectBehaviorAfterInitialization(behavior);

            if (behavior.UseInitialValueAsStart) {
                behavior.StartValue = InitialValue;
            }
            if (behavior.UseInitialValueAsEnd) {
                behavior.EndValue = InitialValue;
            }
        }

        protected override void ProcessBehavior(TBehavior behavior, float normalizedTime, float weight) {
            var curveValue = behavior.EvaluateCurve(normalizedTime);
            var startValue = behavior.StartValue;
            var endValue = behavior.EndValue;
            var change = GetChange(behavior, startValue, endValue, curveValue);
            _blendedValue = ModifyBlendedValue(_blendedValue, change, weight);
        }

        protected abstract TBehaviorValue GetChange(TBehavior behavior, TBehaviorValue startValue, TBehaviorValue endValue, float curveValue);
        protected abstract TBehaviorValue ModifyBlendedValue(TBehaviorValue current, TBehaviorValue change, float weight);
    }
}