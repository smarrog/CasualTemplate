using System;
using System.Collections.Generic;
using System.Linq;
using Smr.Common;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using Object = UnityEngine.Object;

namespace Smr.Animations {
    public static class PlayableDirectorExtensions {
        private static readonly Dictionary<PlayableDirector, PlayableDirectorObserver> _observersMap = new();
        private static readonly Dictionary<PlayableDirector, Action> _completeHandlers = new();

        public static bool IsValid(this PlayableDirector director) {
            return director.isActiveAndEnabled && director.playableGraph.IsValid();
        }

        public static void PlayExt(this PlayableDirector director, AnimationData data, Action onComplete = null) {
            director.SetAsset(data);
            PlayExt(director, onComplete);
        }

        public static void PlayExt(this PlayableDirector director, Action onComplete = null) {
            if (!director.isActiveAndEnabled) {
                return;
            }

            var observer = GetObserver(director);
            if (_completeHandlers.ContainsKey(director)) {
                observer.Completed -= _completeHandlers[director];
                _completeHandlers.Remove(director);
            }

            if (onComplete != null && director.extrapolationMode != DirectorWrapMode.Loop) {
                void onCompleteWrapper() {
                    observer.Completed -= onCompleteWrapper;
                    _completeHandlers.Remove(director);
                    onComplete?.Invoke();
                }

                _completeHandlers[director] = onCompleteWrapper;
                observer.Completed += onCompleteWrapper;
            }

            if (director.extrapolationMode == DirectorWrapMode.Hold) {
                director.Stop(); // Если этого не сделать, то колбек не отработает
            }
            director.Play();
            var speed = GetSpeed(director);
            director.SetSpeed(speed);

            if (!director.IsValid()) {
                return;
            }

            var rootPlayable = director.playableGraph.GetRootPlayable(0);
            rootPlayable.SetTime(0f);
        }

        public static void SetSpeed(this PlayableDirector director, float value) {
            var observer = GetObserver(director);
            observer.Speed = value;

            if (!director.IsValid()) {
                return;
            }

            var rootPlayable = director.playableGraph.GetRootPlayable(0);
            rootPlayable.SetSpeed(value);
        }

        public static float GetSpeed(this PlayableDirector director) {
            var observer = GetObserver(director);
            return observer.Speed;
        }

        public static void SetAsset(this PlayableDirector director, AnimationData data) {
            if (!director) {
                return;
            }

            if (data.Asset != null) {
                director.playableAsset = data.Asset;
            }

            var timelineAsset = (TimelineAsset)director.playableAsset;
            if (timelineAsset == null) {
                return;
            }

            var outputs = timelineAsset.outputs.ToList();
            for (var i = 0; i < outputs.Count; ++i) {
                var playableBinding = outputs[i];
                var genericBinding = data.GetGenericBinding(playableBinding, i);
                var track = (TrackAsset)playableBinding.sourceObject;
                director.SetGenericBinding(track, genericBinding);
            }
        }

        public static void SetAsset(this PlayableDirector director, Object obj) {
            if (!director) {
                return;
            }

            var timelineAsset = (TimelineAsset)director.playableAsset;
            if (timelineAsset == null) {
                return;
            }

            var objectType = obj.GetType();

            var outputs = timelineAsset.outputs.ToList();
            for (var i = 0; i < outputs.Count; ++i) {
                var playableBinding = outputs[i];

                if (playableBinding.outputTargetType == objectType) {
                    var track = (TrackAsset)playableBinding.sourceObject;
                    director.SetGenericBinding(track, obj);

                    return;
                }
            }
        }

        public static void MuteTrack(this PlayableDirector director, string trackName) {
            var timelineAsset = (TimelineAsset)director.playableAsset;
            if (timelineAsset == null) {
                return;
            }

            TrackAsset track = timelineAsset.GetRootTracks().FirstOrDefault(track => track.name == trackName);
            if (track == null) {
                return;
            }

            bool isPlaying = director.state == PlayState.Playing;
            track.muted = true;
            double t0 = director.time;
            director.RebuildGraph();
            director.time = t0;

            if (isPlaying) {
                director.Play();
            }
        }

        public static void UnMuteTrack(this PlayableDirector director, string trackName) {
            var timelineAsset = (TimelineAsset)director.playableAsset;
            if (timelineAsset == null) {
                return;
            }

            TrackAsset track = timelineAsset.GetRootTracks().FirstOrDefault(track => track.name == trackName);
            if (track == null) {
                return;
            }

            bool isPlaying = director.state == PlayState.Playing;
            track.muted = false;
            double t0 = director.time;
            director.Stop();
            director.time = t0;

            if (isPlaying) {
                director.Play();
            }
        }
        public static void SetNormalizedTime(this PlayableDirector director, float value) {
            if (!director.playableGraph.IsValid()) {
                director.RebuildGraph();
            }

            var rootPlayable = director.playableGraph.GetRootPlayable(0);
            rootPlayable.SetSpeed(0f);

            var duration = rootPlayable.GetDuration();
            var time = value * duration;
            rootPlayable.SetTime(time);

            director.Evaluate();
        }

        public static void SkipToTheEnd(this PlayableDirector director) {
            if (!director.IsValid()) {
                return;
            }

            var rootPlayable = director.playableGraph.GetRootPlayable(0);
            rootPlayable.SetTime(rootPlayable.GetDuration());
        }

        public static bool IsPlaying(this PlayableDirector director) {
            return director.IsValid() && director.playableGraph.IsPlaying();
        }

        private static PlayableDirectorObserver GetObserver(PlayableDirector director) {
            if (!_observersMap.ContainsKey(director)) {
                var gameObject = director.gameObject;
                var observer = gameObject.GetComponent<PlayableDirectorObserver>();
                if (!observer) {
                    EngineDependencies.Logger.GetChannel(LogChannel.Animation).LogError($"{gameObject.name} does not contain PlayableDirectorObserver component, it will be added during runtime, but recommended to add it from editor!");
                    observer = gameObject.AddComponent<PlayableDirectorObserver>();
                }
                _observersMap[director] = observer;
            }
            return _observersMap[director];
        }
    }
}