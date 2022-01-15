using System.Globalization;
using TMPro;
using UnityEngine;

namespace Smr.Components {
    public class FpsMeter : MonoBehaviour {
        [SerializeField] private TMP_Text _fpsLabel;

        private float _timer;
        private float _averageFps;

        private void Update() {
            UpdateFpsLabel();
        }

        private void UpdateFpsLabel() {
            var deltaTime = Time.smoothDeltaTime;
            _timer -= deltaTime;
            if (_timer <= 0) {
                _averageFps = Mathf.Ceil(1f / deltaTime);
            }
            _fpsLabel.text = $"Fps: {_averageFps.ToString(CultureInfo.InvariantCulture)}";
        }
    }
}