using System;
using UnityEngine;

namespace Smr.Animations {
    public class TranslationBehavior : AbstractBehaviorWithExposedLerpValue<Transform, Vector3> {
        public override Vector3 StartValue {
            get => UseInitialValueAsStart ? base.StartValue : ExposedStartValue ? ExposedStartValue.position : Vector3.zero;
            set {
                if (UseInitialValueAsStart) {
                    base.StartValue = value;
                } else {
                    throw new Exception("Set for Start value in Exposed is restricted");
                }
            }
        }

        public override Vector3 EndValue {
            get => UseInitialValueAsEnd ? base.EndValue : ExposedEndValue ? ExposedEndValue.position : Vector3.zero;
            set {
                if (UseInitialValueAsEnd) {
                    base.EndValue = value;
                } else {
                    throw new Exception("Set for End value in Exposed is restricted");
                }
            }
        }
    }
}