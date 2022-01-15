using System;
using UnityEngine;
using UnityEngine.Timeline;

namespace Smr.Animations {
    [Serializable]
    public class TimeMachineClip : AbstractClip<TimeMachineBehavior> {
        public override ClipCaps clipCaps => ClipCaps.None;
        
        public TimeMachineConditionType ConditionType;
        public TimeMachineAction Action;
        public string MarkerToJumpTo = "";
        public string MarkerLabel = "";
        public float TimeToJumpTo;
        public string EventName;
        public float DelayToEvent;

        public double StartTime;
        public double EndTime;

        public ExposedReference<GameObject> ConditionTarget;
        public ExposedReference<TimeMachineCondition> ConditionChecker;

        protected override void FillBehavior(TimeMachineBehavior behavior, IExposedPropertyTable resolver) {
            behavior.wasExecuted = false;

            behavior.clipStartTime = StartTime;
            behavior.clipEndTime = EndTime;

            behavior.markerToJumpTo = MarkerToJumpTo;
            behavior.action = Action;
            behavior.timeToJumpTo = TimeToJumpTo;
            behavior.eventName = EventName;
            behavior.delayToEvent = DelayToEvent;

            behavior.conditionType = ConditionType;
            behavior.conditionTarget = ConditionTarget.Resolve(resolver);
            behavior.conditionChecker = ConditionChecker.Resolve(resolver);
        }
    }
}