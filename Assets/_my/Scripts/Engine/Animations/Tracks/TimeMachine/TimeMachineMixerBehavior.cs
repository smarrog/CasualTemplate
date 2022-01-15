using System;
using System.Collections.Generic;
using Smr.Common;
using UnityEngine;
using UnityEngine.Playables;

namespace Smr.Animations {
    public class TimeMachineMixerBehavior : PlayableBehaviour {

        public readonly Dictionary<string, double> Markers = new();

        private PlayableDirector _director;

        public override void OnPlayableCreate(Playable playable) {
            _director = playable.GetGraph().GetResolver() as PlayableDirector;
        }

        public override void ProcessFrame(Playable playable, FrameData info, object playerData) {
            if (!Application.isPlaying) {
                return;
            }

            var inputCount = playable.GetInputCount();

            for (var i = 0; i < inputCount; i++) {
                var inputWeight = playable.GetInputWeight(i);
                var inputPlayable = (ScriptPlayable<TimeMachineBehavior>)playable.GetInput(i);
                if (inputPlayable.IsNull()) {
                    continue;
                }
                
                var behaviour = inputPlayable.GetBehaviour();
                
                var isExecutionTime = inputWeight > 0;
                if (behaviour.NeedToExecuteOnce && behaviour.wasExecuted) {
                    return;
                }

                behaviour.wasExecuted = isExecutionTime;
                if (!isExecutionTime || !behaviour.IsConditionMet) {
                    continue;
                }
                
                switch (behaviour.action) {
                    case TimeMachineAction.Event:
                        ProcessEvent(behaviour);
                        break;
                    case TimeMachineAction.Pause:
                        _director.Pause();
                        break;
                    case TimeMachineAction.JumpToTime:
                        _director.time = behaviour.timeToJumpTo;
                        break;
                    case TimeMachineAction.JumpToMarker:
                        ProcessJumpToMarker(behaviour);
                        break;
                    case TimeMachineAction.Marker:
                        break;
                    case TimeMachineAction.Skip:
                        ProcessSkip(behaviour);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private static void ProcessEvent(TimeMachineBehavior behavior) {
            void FireSignal() => EngineDependencies.SignalBus.Fire(new TimeMachineSignal { EventName = behavior.eventName });
            if (behavior.delayToEvent > 0) {
                EngineDependencies.Logger.LogError("Event delays are not supported yet");
                FireSignal();
            } else {
                FireSignal();
            }
        }

        private void ProcessJumpToMarker(TimeMachineBehavior behavior) {
            if (Markers.TryGetValue(behavior.markerToJumpTo, out var marker)) {
                _director.time = marker;
            } else {
                EngineDependencies.Logger.GetChannel(LogChannel.Animation).LogError($"Сan not find the specified marker: {behavior.markerToJumpTo}");
            }
        }

        private void ProcessSkip(TimeMachineBehavior behavior) {
            _director.time = behavior.clipEndTime;
        }
    }
}