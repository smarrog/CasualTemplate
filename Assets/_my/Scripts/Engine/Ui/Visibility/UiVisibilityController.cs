using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Smr.Animations;
using Smr.Extensions;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Playables;
using Object = UnityEngine.Object;

namespace Smr.Ui {
    public class UiVisibilityController : AbstractUiVisibilityComponent {
        [SerializeField] private PlayableDirector _appearDirector;
        [SerializeField] private PlayableDirector _hideDirector;
        [SerializeField] private PlayableDirector _visibleDirector;
        [SerializeField] private PlayableDirector _invisibleDirector;
        [SerializeField] private PlayableDirector _idleDirector;

        private Action _onStateFinalized;

        private void Awake() {
            Assert.IsNotNull(_visibleDirector, $"{nameof(UiVisibilityController)}:: Visible director is null");
            Assert.IsNotNull(_invisibleDirector, $"{nameof(UiVisibilityController)}:: Invisible director is null");

            StopAll();

            if (_appearDirector) {
                _appearDirector.playOnAwake = false;
                _appearDirector.extrapolationMode = DirectorWrapMode.None;
            }
            if (_hideDirector) {
                _hideDirector.playOnAwake = false;
                _hideDirector.extrapolationMode = DirectorWrapMode.None;
            }
            if (_visibleDirector) {
                _visibleDirector.playOnAwake = false;
                _visibleDirector.extrapolationMode = DirectorWrapMode.Hold;
            }
            if (_invisibleDirector) {
                _invisibleDirector.playOnAwake = false;
                _invisibleDirector.extrapolationMode = DirectorWrapMode.Hold;
            }
            if (_idleDirector) {
                _idleDirector.playOnAwake = false;
                _idleDirector.extrapolationMode = DirectorWrapMode.Loop;
            }
        }

        private void OnEnable() {
            switch (VisualState) {
                case VisibilityState.Invisible:
                case VisibilityState.Hiding:
                    SetVisibility(false);
                    break;
                case VisibilityState.Visible:
                case VisibilityState.Appearing:
                    SetVisibilityAnimated(true, _onStateFinalized);
                    break;
            }
        }

        private void OnDisable() {
            var buf = _onStateFinalized;
            _onStateFinalized = null;
            buf?.Invoke();
        }

        public override void Clear() {
            _onStateFinalized = null;
            StopAll();
        }

        public void SetAnimationBindings(IEnumerable<Object> bindings, VisibilityState state) {
            var asset = new AnimationData(null, bindings);

            switch (state) {
                case VisibilityState.Unknown:
                    break;
                case VisibilityState.Appearing:
                    _appearDirector.SetAsset(asset);
                    _idleDirector.SetAsset(asset);
                    break;
                case VisibilityState.Hiding:
                    _hideDirector.SetAsset(asset);
                    break;
                case VisibilityState.Visible:
                    _visibleDirector.SetAsset(asset);
                    break;
                case VisibilityState.Invisible:
                    _invisibleDirector.SetAsset(asset);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }

        public UniTask SetVisibilityStateAsync(VisibilityState state) {
            var completionSource = new UniTaskCompletionSource();
            completionSource.Task.HandleException();

            void OnComplete() {
                completionSource.TrySetResult();
            }

            SetVisibilityState(state, OnComplete);
            return completionSource.Task;
        }

        public override void SetVisibilityState(VisibilityState state, Action onComplete = null) {
            switch (state) {
                case VisibilityState.Unknown:
                    break;
                case VisibilityState.Appearing:
                    SetVisibilityAnimated(true, onComplete);
                    break;
                case VisibilityState.Hiding:
                    SetVisibilityAnimated(false, onComplete);
                    break;
                case VisibilityState.Visible:
                    SetVisibility(true);
                    onComplete?.Invoke();
                    break;
                case VisibilityState.Invisible:
                    SetVisibility(false);
                    onComplete?.Invoke();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }

        public override void SetVisibilityAnimated(bool isVisible, Action onAnimationComplete = null) {
            _onStateFinalized = null;
            var director = isVisible ? _appearDirector : _hideDirector;
            if (!director) {
                SetVisibility(isVisible);
                onAnimationComplete?.Invoke();
                return;
            }

            SetState(isVisible ? VisibilityState.Appearing : VisibilityState.Hiding);
            _onStateFinalized = onAnimationComplete;
            Play(director, () => {
                SetState(isVisible ? VisibilityState.Visible : VisibilityState.Invisible);
                _onStateFinalized = null;
                onAnimationComplete?.Invoke();
            });
        }

        public override void SetVisibility(bool isVisible) {
            _onStateFinalized = null;
            var director = isVisible ? _visibleDirector : _invisibleDirector;
            SetState(isVisible ? VisibilityState.Visible : VisibilityState.Invisible);
            Play(director, null);
        }

        public void StopAll() {
            Stop(_visibleDirector);
            Stop(_invisibleDirector);
            Stop(_appearDirector);
            Stop(_hideDirector);
            Stop(_idleDirector);
        }

        private void Play(PlayableDirector director, Action onComplete) {
            if (!director || !director.playableAsset) {
                onComplete?.Invoke();
                return;
            }

            StopAll();

            if (!director.gameObject.activeSelf) {
                director.gameObject.SetActive(true);
            }
            director.PlayExt(onComplete);
            director.Evaluate(); //we need to start the graph in current frame to avoid visual bugs
        }

        private void Stop(PlayableDirector director) {
            if (director) {
                director.gameObject.SetActive(false);
            }
        }

        private void SetState(VisibilityState state) {
            VisualState = state;
            if (VisualState == VisibilityState.Visible) {
                Play(_idleDirector, null);
            }
        }
    }
}