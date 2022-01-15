using System;
using UnityEngine;
using UnityEngine.Timeline;

namespace Smr.Animations {
    [Serializable]
    public abstract class AbstractClipWithLerpValue<TBehavior, TValue> : AbstractClipWithLerpValue<TBehavior, TValue, TValue>
        where TBehavior : AbstractBehaviorWithLerpValue<TValue>, new() {
        protected override TValue BehaviorStartValue => StartValue;
        protected override TValue BehaviorEndValue => EndValue;
    }

    [Serializable]
    public abstract class AbstractClipWithLerpValue<TBehavior, TClipValue, TBehaviorValue> : AbstractClip<TBehavior>
        where TBehavior : AbstractBehaviorWithLerpValue<TBehaviorValue>, new() {
        public override ClipCaps clipCaps => ClipCaps.Blending;

        public AnimationCurve Curve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

        public bool UseInitialValueAsStart;
        public bool UseInitialValueAsEnd;
        public TClipValue StartValue;
        public TClipValue EndValue;

        protected abstract TBehaviorValue BehaviorStartValue { get; }
        protected abstract TBehaviorValue BehaviorEndValue { get; }

        protected override void FillBehavior(TBehavior behavior, IExposedPropertyTable resolver) {
            behavior.Curve = Curve;
            behavior.UseInitialValueAsStart = UseInitialValueAsStart;
            behavior.UseInitialValueAsEnd = UseInitialValueAsEnd;
            if (!UseInitialValueAsStart) {
                behavior.StartValue = BehaviorStartValue;
            }
            if (!UseInitialValueAsEnd) {
                behavior.EndValue = BehaviorEndValue;
            }
        }
    }
}