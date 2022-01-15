using System;
using UnityEngine;

namespace Smr.Animations {
    [Serializable]
    public class TimeMachineBehavior : AbstractBehavior {
        public TimeMachineAction action;
        public string markerToJumpTo;
        public float timeToJumpTo;
        public string eventName;
        public float delayToEvent;

        public double clipStartTime;
        public double clipEndTime;

        public TimeMachineConditionType conditionType;
        public GameObject conditionTarget;
        public TimeMachineCondition conditionChecker;
        
        [HideInInspector] public bool wasExecuted;

        public bool IsConditionMet => conditionType switch {
            TimeMachineConditionType.Always => true,
            TimeMachineConditionType.Never => false,
            TimeMachineConditionType.Active => conditionTarget && conditionTarget.activeInHierarchy,
            TimeMachineConditionType.NotActive => !conditionTarget || !conditionTarget.activeInHierarchy,
            TimeMachineConditionType.Custom => conditionChecker != null && conditionChecker.Check(),
            TimeMachineConditionType.CustomInvert => conditionChecker != null && !conditionChecker.Check(),
            _ => throw new ArgumentOutOfRangeException()
        };

        public bool NeedToExecuteOnce => action switch {
            TimeMachineAction.Event => true,
            _ => false
        };
    }
}