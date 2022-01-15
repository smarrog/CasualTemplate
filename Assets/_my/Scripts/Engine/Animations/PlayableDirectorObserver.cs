using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Playables;

namespace Smr.Animations {
    [RequireComponent(typeof(PlayableDirector))]
    public class PlayableDirectorObserver : MonoBehaviour {
        private const double EPSILON = 0.00001;

        public event Action Completed;
        [HideInInspector] public float Speed = 1f;

        public bool IsAtEnd {
            get {
                var timeFromLastUpdateToEnd = _director.duration - _lastTime;
                return Speed > 0 && timeFromLastUpdateToEnd <= Time.deltaTime * Speed;
            }
        }

        private PlayableDirector _director;
        private bool _isCompleteCheckAvailable;
        private Coroutine _watchRoutine;
        private double _lastTime = double.MinValue;

        private void Awake() {
            _director = GetComponent<PlayableDirector>();

            _director.played += DirectorPlayed;
            _director.paused += DirectorPaused;
            _director.stopped += DirectorStopped;
        }

        private void Start() {
            if (_director.playableGraph.IsValid() && _director.playableGraph.IsPlaying()) {
                DirectorPlayed(_director);
            }
        }

        private void OnDisable() {
            if (_watchRoutine != null) {
                StopCoroutine(_watchRoutine);
            }
        }

        private void DirectorPlayed(PlayableDirector director) {
            if (director.extrapolationMode == DirectorWrapMode.Loop) {
                return;
            }

            _isCompleteCheckAvailable = true;
            _watchRoutine = StartCoroutine(WatchDirector());
        }

        private void DirectorPaused(PlayableDirector director) {
            if (_watchRoutine != null) {
                StopCoroutine(_watchRoutine);
            }
        }

        private void DirectorStopped(PlayableDirector director) {
            CheckComplete();

            if (_watchRoutine != null) {
                StopCoroutine(_watchRoutine);
            }
        }

        private IEnumerator WatchDirector() {
            _lastTime = _director.time;

            if (_director.state == PlayState.Paused) {
                yield return null;
            }

            while (_director.time < EPSILON) {
                yield return null;
            }

            while (true) {
                CheckComplete();
                if (!_isCompleteCheckAvailable) {
                    yield break;
                }

                _lastTime = _director.time;
                yield return null;
            }
        }

        private void CheckComplete() {
            if (!_isCompleteCheckAvailable || !IsAtEnd) {
                return;
            }

            _isCompleteCheckAvailable = false;
            _lastTime = double.MinValue;
            Completed?.Invoke();
        }
    }
}