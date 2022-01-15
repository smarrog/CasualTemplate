using System;
using System.Collections.Generic;
using Smr.Common;
using Smr.Ui;
using UnityEngine;

namespace Smr.UI {
    public abstract class AbstractUiElement<TData> : AbstractUiElement where TData : AbstractUiData {
        public TData Data { get; private set; }

        public void ApplyData(AbstractUiData data) {
            Data = data as TData;
            if (Data == null) {
                Logger.LogError($"Try to init element({name}) with incompatible data!");
                return;
            }
            ConstructIfNeed();
            OnDataApplied();
        }
    }
    
    public abstract class AbstractUiElement : MonoBehaviour {
        [SerializeField] private UiVisibilityController _visibilityController;
        
        public VisibilityState VisualState => VisibilityController.VisualState;

        protected IChannelLogger Logger => EngineDependencies.Logger.GetChannel(LogChannel.Ui);

        private bool _isConstructed;
        private readonly HashSet<ISignalBusSubscription> _activeSubscriptions = new();
        private IVisibility _visibility;
        private IVisibility VisibilityController {
            get {
                if (_visibility == null) {
                    if (_visibilityController) {
                        _visibility = _visibilityController;
                    } else {
                        _visibility = new SimpleVisibilityController(gameObject);
                    }
                }
                return _visibility;
            }
        }

        protected virtual void OnDestroy() {
            RemoveAllSubscriptions();
        }

        public void SetVisibility(bool isVisible) {
            SetVisibilityState(isVisible ? VisibilityState.Visible : VisibilityState.Invisible);
        }
        
        public void SetVisibilityAnimated(bool isVisible, Action onComplete = null) {
            SetVisibilityState(isVisible ? VisibilityState.Appearing : VisibilityState.Hiding, onComplete);
        }

        public void SetVisibilityState(VisibilityState state, Action onComplete = null) {
            if (!_isConstructed) {
                ConstructIfNeed();
            }
            
            switch (state) {
                case VisibilityState.Unknown:
                    return;
                case VisibilityState.Appearing or VisibilityState.Visible:
                    OnShowingBegin();
                    break;
                case VisibilityState.Hiding or VisibilityState.Invisible:
                    BeforeHide();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }

            ProceedAnimation(state, onComplete);
        }

        protected void ConstructIfNeed() {
            if (_isConstructed) {
                return;
            }
            
            _isConstructed = true;
            ConstructInternal();
        }

        protected void FireSignal<TSignal>(TSignal signal) {
            EngineDependencies.SignalBus.Fire(signal);
        }

        protected ISignalBusSubscription AddSubscription<TSignal>(Action<TSignal> callback) {
            var subscriptionId = EngineDependencies.SignalBus.Subscribe(callback);
            if (subscriptionId != null) {
                _activeSubscriptions.Add(subscriptionId);
            }
            return subscriptionId;
        }

        protected void RemoveSubscription(ISignalBusSubscription subscriptionId) {
            if (!IsActiveSubscription(subscriptionId)) {
                return;
            }
            
            _activeSubscriptions.Remove(subscriptionId);
            EngineDependencies.SignalBus.Unsubscribe(subscriptionId);
        }

        protected void RemoveAllSubscriptions() {
            foreach (var activeSubscription in _activeSubscriptions) {
                EngineDependencies.SignalBus.Unsubscribe(activeSubscription);
            }
            _activeSubscriptions.Clear();
        }

        protected bool IsActiveSubscription(ISignalBusSubscription subscriptionId) {
            return subscriptionId != null && _activeSubscriptions.Contains(subscriptionId);
        }

        private void ProceedAnimation(VisibilityState state, Action onComplete) {
            void OnComplete() {
                switch (state) {
                    case VisibilityState.Appearing or VisibilityState.Visible:
                        OnShowingEnd();
                        break;
                    case VisibilityState.Hiding or VisibilityState.Invisible:
                        AfterHide();
                        break;
                }
                onComplete?.Invoke();
            }
            
            VisibilityController.SetVisibilityState(state, OnComplete);
        }

        protected virtual void ConstructInternal() {}
        protected virtual void OnDataApplied() {}

        protected virtual void OnShowingBegin() {}
        protected virtual void OnShowingEnd() {}
        protected virtual void BeforeHide() {}
        protected virtual void AfterHide() {}
    }
}