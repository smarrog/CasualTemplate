using Smr.Extensions;
using UnityEngine;

namespace Smr.Ui {
    public class SimpleProgressBar : MonoBehaviour {
        [SerializeField] private RectTransform _bar;
        [SerializeField] private bool _isVertical;

        private float _value;

        private void Awake() {
            UpdateVisual();
        }

        public void SetValue(float value) {
            _value = Mathf.Clamp01(value);
            UpdateVisual();
        }

        private void UpdateVisual() {
            _bar.localScale = _isVertical
                ? Vector3.one.WithY(_value)
                : Vector3.one.WithX(_value);
        }
    }
}