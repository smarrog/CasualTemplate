using System;
using UnityEngine;
using UnityEngine.Playables;

namespace Smr.Animations {
    public class DirectorTimeCondition : TimeMachineCondition {
        private enum CompareMethod {
            EqualTo,
            GreaterThan,
            LessThan,
            GreaterOrEqualTo,
            LessOrEqualTo,
        }

        [SerializeField] private PlayableDirector _director;
        [SerializeField] private CompareMethod _compareMethod;
        [SerializeField] private float _valueNormalized;

        private float CurrentNormalizedTime => _director.duration > 0 ? (float)(_director.time / _director.duration) : 0;

        public override bool Check() {
            if (!_director) {
                return false;
            }
            bool AreEqual() => Math.Abs(CurrentNormalizedTime - _valueNormalized) < 0.001f;
            bool GreaterThan() => CurrentNormalizedTime > _valueNormalized;
            bool LessThan() => CurrentNormalizedTime < _valueNormalized;
            
            return _compareMethod switch {
                CompareMethod.EqualTo => AreEqual(),
                CompareMethod.GreaterThan => !AreEqual() && GreaterThan(),
                CompareMethod.LessThan => !AreEqual() && LessThan(),
                CompareMethod.GreaterOrEqualTo => GreaterThan() || AreEqual(),
                CompareMethod.LessOrEqualTo => LessThan() || AreEqual(),
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}