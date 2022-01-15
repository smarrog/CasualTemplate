using System;
using System.Collections.Generic;
using System.Linq;
using Smr.Extensions;
using UnityEngine;

namespace Smr.Animations {
    public class TimeMachineMultipleCondition : TimeMachineCondition {
        private enum ConditionType {
            AND,
            OR
        }

        [SerializeField] private ConditionType _conditionType;
        [SerializeField] private List<TimeMachineCondition> _conditions;
        public override bool Check() {
            if (_conditions.IsEmpty()) {
                return false;
            }
            return _conditionType switch {
                ConditionType.AND => _conditions.All(c => c.Check()),
                ConditionType.OR => _conditions.Any(c => c.Check()),
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}