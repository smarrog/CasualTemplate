#if SMR_FINGERS
using DigitalRubyShared;
using Smr.Common;
using UnityEngine;

namespace Smr.Input {
    public class GestureController : MonoBehaviour {
        [SerializeField] private TapGestureRecognizerComponentScript tap;
        [SerializeField] private TapGestureRecognizerComponentScript doubleTap;
        [SerializeField] private PanGestureRecognizerComponentScript pan;
        [SerializeField] private ScaleGestureRecognizerComponentScript scale;
        [SerializeField] private LongPressGestureRecognizerComponentScript longPress;
        
        private Vector3 _panDirection = Vector2.zero;
        
        private void Start() {
            if (tap) {
                tap.Gesture.StateUpdated += TapGestureCallback;
            }
            if (doubleTap) {
                doubleTap.Gesture.StateUpdated += DoubleTapGestureCallback;
            }
            if (pan) {
                pan.Gesture.StateUpdated += PanGestureCallback;
            }
            if (scale) {
                scale.Gesture.StateUpdated += ScaleGestureCallback;
            }
            if (longPress) {
                longPress.Gesture.StateUpdated += LongPressGestureCallback;
            }
        }

        private void OnDestroy() {
            if (tap) {
                tap.Gesture.StateUpdated -= TapGestureCallback;
            }
            if (doubleTap) {
                doubleTap.Gesture.StateUpdated -= DoubleTapGestureCallback;
            }
            if (pan) {
                pan.Gesture.StateUpdated -= PanGestureCallback;
            }
            if (scale) {
                scale.Gesture.StateUpdated -= ScaleGestureCallback;
            }
            if (longPress) {
                longPress.Gesture.StateUpdated -= LongPressGestureCallback;
            }
        }

        private void TapGestureCallback(GestureRecognizer gesture) {
            switch (gesture.State) {
                case GestureRecognizerState.Ended:
                    Publish(new TapGestureSignal {
                        X = gesture.FocusX,
                        Y = gesture.FocusY
                    });
                    break;
            }
        }

        private void DoubleTapGestureCallback(GestureRecognizer gesture) {
            if (gesture.State == GestureRecognizerState.Ended) {
                Publish(new DoubleTapGestureSignal {
                    X = gesture.FocusX,
                    Y = gesture.FocusY
                });
            }
        }

        private void PanGestureCallback(GestureRecognizer gesture) {
            switch (gesture.State) {
                case GestureRecognizerState.Executing:
                    if (gesture.DeltaX != 0 || gesture.DeltaY != 0) {
                        var signal = new PanGestureSignal {
                            X = gesture.DeltaX,
                            Y = gesture.DeltaY
                        };
                        _panDirection.x += signal.X;
                        _panDirection.y += signal.Y;
                        Publish(signal);
                    }
                    break;
                case GestureRecognizerState.Ended:
                    Publish(new PanGestureEndedSignal());
                    ApplyPanDelta();
                    break;
            }
        }

        private void ScaleGestureCallback(GestureRecognizer gesture) {
            switch (gesture.State) {
                case GestureRecognizerState.Executing:
                    Publish(new ScaleGestureSignal {
                        // invert the scale so that smaller scales actually zoom out and larger scales zoom in
                        Scale = 1.0f + (1.0f - scale.Gesture.ScaleMultiplier)
                    });
                    break;
                case GestureRecognizerState.Ended:
                    Publish(new ScaleGestureEndedSignal());
                    break;
            }
        }

        private void LongPressGestureCallback(GestureRecognizer gesture) {
            switch (gesture.State) {
                case GestureRecognizerState.Began:
                    Publish(new LongTapBeganGestureSignal {
                        X = gesture.FocusX,
                        Y = gesture.FocusY
                    });
                    break;
                case GestureRecognizerState.Ended:
                    Publish(new LongTapEndedGestureSignal {
                        X = gesture.FocusX,
                        Y = gesture.FocusY
                    });
                    break;
            }
        }

        private void ApplyPanDelta() {
            _panDirection.Normalize();
            Publish(new SwipeSignal {
                Direction = _panDirection
            });
            _panDirection = Vector3.zero;
        }

        private void Publish<TSignal>(TSignal signal) {
            EngineDependencies.SignalBus.Fire(signal);
        }
    }
}
#endif