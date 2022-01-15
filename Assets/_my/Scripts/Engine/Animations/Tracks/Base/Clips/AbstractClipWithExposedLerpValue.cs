using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Smr.Animations {
    [Serializable]
    public class AbstractClipWithExposedLerpValue<TBehavior, TExposedValue, TValue> : AbstractClipWithLerpValue<TBehavior, ExposedReference<TExposedValue>, TValue>
        where TBehavior : AbstractBehaviorWithExposedLerpValue<TExposedValue, TValue>, new()
        where TExposedValue : Object {
        public ExposedReference<TExposedValue> ExposedStartValue;
        public ExposedReference<TExposedValue> ExposedEndValue;

        protected override TValue BehaviorStartValue => throw new Exception("Not used");
        protected override TValue BehaviorEndValue => throw new Exception("Not used");

        protected override void FillBehavior(TBehavior behavior, IExposedPropertyTable resolver) {
            behavior.Curve = Curve;
            behavior.UseInitialValueAsStart = UseInitialValueAsStart;
            behavior.UseInitialValueAsEnd = UseInitialValueAsEnd;
            if (!UseInitialValueAsStart) {
                behavior.ExposedStartValue = ExposedStartValue.Resolve(resolver);
            }
            if (!UseInitialValueAsEnd) {
                behavior.ExposedEndValue = ExposedEndValue.Resolve(resolver);
            }
            // !!! do not call base method
        }
    }
}