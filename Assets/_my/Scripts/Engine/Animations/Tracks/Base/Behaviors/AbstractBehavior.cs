using System;
using UnityEngine.Playables;

namespace Smr.Animations {
    [Serializable]
    public abstract class AbstractBehavior : PlayableBehaviour {
        [NonSerialized] public bool WasJustEnded;

        public double Time => _director ? _director.time : 0;

        private PlayableDirector _director;
        private double _lastDirectorTime = -1;

        // Works only in standalone mode without mixer // TODO Support mixer ??
        protected virtual bool ExecuteOnce => false;
        protected string Name => _director ? _director.gameObject.name : "unidentified";

        // Works only in standalone mode without mixer // TODO Support mixer ??
        protected bool WasExecuted => _director != null && _lastDirectorTime >= 0 && _director.time >= _lastDirectorTime;

        public override void OnPlayableCreate(Playable playable) {
            _director = playable.GetGraph().GetResolver() as PlayableDirector;
        }

        public sealed override void ProcessFrame(Playable playable, FrameData info, object playerData) {
            var needToExecute = !ExecuteOnce || !WasExecuted;
            _lastDirectorTime = _director.time;
            if (needToExecute) {
                ProcessFrameInternal(playable, info, playerData);
            }
        }

        public override void OnBehaviourPause(Playable playable, FrameData info) {
            var duration = playable.GetDuration();
            var time = playable.GetTime();
            var delta = info.deltaTime;

            if (info.evaluationType == FrameData.EvaluationType.Playback) {
                var count = time + delta;

                if (count >= duration) {
                    WasJustEnded = true;
                }
            }

            base.OnBehaviourPause(playable, info);
        }

        protected virtual void ProcessFrameInternal(Playable playable, FrameData info, object playerData) {
            base.ProcessFrame(playable, info, playerData);
        }
    }
}