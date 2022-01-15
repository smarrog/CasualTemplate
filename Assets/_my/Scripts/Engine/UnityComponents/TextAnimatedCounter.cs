using System;
using UnityEngine;
using UnityEngine.UI;

namespace Smr.Components {
    [RequireComponent(typeof(Text))]
    public class TextAnimatedCounter : MonoBehaviour {
        [SerializeField] private float _animationTime;
        private Text _text;
        private int _value;

        private bool _isAnimationInProgress;
        private int _startValue;
        private int _plannedValue;
        private float _timePassed;

        private void Awake() {
            _text = GetComponent<Text>();
            Set(0);
        }

        private void Update() {
            if (!_isAnimationInProgress) {
                return;
            }

            _timePassed += Time.deltaTime;
            if (_timePassed >= _animationTime) {
                Set(_plannedValue);
                _isAnimationInProgress = false;
            } else {
                var value = Mathf.Lerp(_startValue, _plannedValue, _timePassed / _animationTime);
                SetValueInternal(Convert.ToInt32(value), false);
            }
        }

        public void SetAnimationTime(float value) {
            _animationTime = value;
        }

        public void Set(int value) {
            SetValueInternal(value, true);
        }

        public void AnimateTo(int value) {
            if (_animationTime <= 0) {
                Set(value);
                return;
            }

            _isAnimationInProgress = true;
            _timePassed = 0;
            _startValue = _value;
            _plannedValue = value;
        }

        private void SetValueInternal(int value, bool stopAnimation) {
            if (stopAnimation) {
                _isAnimationInProgress = false;
            }
            _value = value;
            _text.text = _value.ToString();
        }
    }
}