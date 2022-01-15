using System;
using UnityEngine;

namespace Smr.Ui {
    public class SimpleVisibilityController : IVisibility {
        private readonly GameObject _gameObject;

        public VisibilityState VisualState => !_gameObject
            ? VisibilityState.Unknown
            : _gameObject.activeSelf
                ? VisibilityState.Visible
                : VisibilityState.Invisible;

        public SimpleVisibilityController(GameObject gameObject) {
            _gameObject = gameObject;
        }
        
        public void SetVisibilityState(VisibilityState state, Action onComplete = null) {
            switch (state) {
                case VisibilityState.Appearing:
                case VisibilityState.Visible:
                    SetVisible();
                    break;
                case VisibilityState.Hiding:
                case VisibilityState.Invisible:
                    SetInvisible();
                    break;
                case VisibilityState.Unknown:
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
            
            onComplete?.Invoke();
        }

        private void SetVisible() {
            _gameObject.SetActive(true);
        }

        private void SetInvisible() {
            _gameObject.SetActive(false);
        }
    }
}