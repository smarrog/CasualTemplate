using UnityEngine;

namespace Smr.Animations {
    public abstract class AbstractBehaviorWithLerpValue<TValue> : AbstractBehavior {
        public AnimationCurve Curve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

        public bool UseInitialValueAsStart;
        public bool UseInitialValueAsEnd;
        public virtual TValue StartValue { get; set; }
        public virtual TValue EndValue { get; set; }

        public float EvaluateCurve(float time) {
            return Curve.Evaluate(time);
        }
    }
}