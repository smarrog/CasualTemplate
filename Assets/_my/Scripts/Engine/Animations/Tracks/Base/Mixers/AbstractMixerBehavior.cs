using System;
using System.Collections.Generic;
using Smr.Common;
using UnityEngine;
using UnityEngine.Playables;

namespace Smr.Animations {
    public abstract class AbstractMixerBehavior<TBehavior, TTrackBinding> : PlayableBehaviour
        where TBehavior : AbstractBehavior, new()
        where TTrackBinding : Component {
        private bool _wasOneTimeInitializationPerformed;
        private HashSet<AbstractBehavior> _initializedBehaviours = new();

        public sealed override void ProcessFrame(Playable playable, FrameData info, object playerData) {
            if (!(playerData is TTrackBinding trackBinding) || !trackBinding) {
                return;
            }

            if (!_wasOneTimeInitializationPerformed) {
                _wasOneTimeInitializationPerformed = true;
                ProcessFirstFrameInit(trackBinding);
            }
            ProcessFrameInternalInit(trackBinding);

            var inputCount = playable.GetInputCount();
            var inputsData = new List<InputData>();
            InputData wasJustEndedData = null;

            // нашли реальные веса, как их возвращает API
            for (var i = 0; i < inputCount; i++) {
                InputData inputData = null;
                try {
                    inputData = new InputData(playable, i);
                } catch (Exception exception) {
                    EngineDependencies.Logger.GetChannel(LogChannel.Animation).LogError($"Can't create inputData: {exception.Message}");
                    continue;
                }

                if (!inputData.IsValid || !IsValidBehavior(inputData.Behavior)) {
                    continue;
                }
                if (inputData.Behavior.WasJustEnded) {
                    // TODO проверить, возможно будет выбираться не последний клип, а случайный
                    if (wasJustEndedData != null) {
                        wasJustEndedData.Behavior.WasJustEnded = false;
                    }
                    wasJustEndedData = inputData;
                } else if (inputData.Weight > 0) {
                    inputsData.Add(inputData);
                }
                if (!_initializedBehaviours.Contains(inputData.Behavior)) {
                    CorrectBehaviorAfterInitialization(inputData.Behavior);
                    _initializedBehaviours.Add(inputData.Behavior);
                }
            }

            if (wasJustEndedData != null) {
                wasJustEndedData.Behavior.WasJustEnded = false;
                if (inputsData.Count == 0) {
                    wasJustEndedData.Weight = 1;
                    inputsData.Add(wasJustEndedData);
                }
            }

            if (inputsData.Count <= 0) {
                return;
            }

            // запускаем процесс с измененными весами для завершенных клипов
            foreach (var inputData in inputsData) {
                var normalizedTime = inputData.GetNormalizedTime();
                if (normalizedTime < 0) {
                    continue;
                }
                ProcessBehavior(inputData.Behavior, normalizedTime, inputData.Weight);
            }

            ProcessFrameInternalComplete(trackBinding);
        }

        protected virtual void ProcessFirstFrameInit(TTrackBinding trackBinding) {}
        protected abstract void ProcessFrameInternalInit(TTrackBinding trackBinding);
        protected abstract void ProcessFrameInternalComplete(TTrackBinding trackBinding);
        protected abstract void ProcessBehavior(TBehavior behavior, float normalizedTime, float weight);
        protected virtual void CorrectBehaviorAfterInitialization(TBehavior behavior) {}
        protected virtual bool IsValidBehavior(TBehavior behavior) => true;

        private class InputData {
            private ScriptPlayable<TBehavior> _input;

            public bool IsValid => !_input.IsNull();
            public float Weight { get; set; }
            public TBehavior Behavior => _input.GetBehaviour();

            public InputData(Playable playable, int i) {
                _input = (ScriptPlayable<TBehavior>)playable.GetInput(i);
                Weight = playable.GetInputWeight(i);
            }

            public float GetNormalizedTime() {
                var inputTime = (float)_input.GetTime();
                if (inputTime < 0) {
                    return -1f;
                }
                var inputDuration = (float)_input.GetDuration();
                if (inputDuration <= 0) {
                    return -1f;
                }
                return inputTime / inputDuration;
            }
        }
    }
}