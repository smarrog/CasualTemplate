using System;
using UnityEngine;

namespace Smr.Animations {
    [Serializable]
    public class BezierTranslationClip : AbstractClipWithLerpValue<BezierTranslationBehavior, TranslationPointData> {
        public TranslationPointData StartControlPoint;
        public TranslationPointData EndControlPoint;

        protected override void FillBehavior(BezierTranslationBehavior behavior, IExposedPropertyTable resolver) {
            base.FillBehavior(behavior, resolver);
            behavior.StartControlPoint = StartControlPoint;
            behavior.EndControlPoint = EndControlPoint;
        }
    }
}