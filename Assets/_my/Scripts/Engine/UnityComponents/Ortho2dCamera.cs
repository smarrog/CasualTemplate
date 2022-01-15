using System;
using UnityEngine;

namespace Smr.Components {
    [ExecuteInEditMode]
    [RequireComponent(typeof(Camera))]
    internal class Ortho2dCamera : MonoBehaviour {
        [SerializeField] private bool _uniform = true;
        [SerializeField] private bool _autoSetUniform = false;

        private Camera _camera;

        private void Awake() {
            _camera = GetComponent<Camera>();
            _camera.orthographic = true;

            if (_uniform) {
                SetUniform();
            }
        }
        private void LateUpdate() {
            if (_autoSetUniform && _uniform) {
                SetUniform();
            }
        }

        private void SetUniform() {
            var orthographicSize = _camera.pixelHeight / 2f;
            if (Math.Abs(orthographicSize - _camera.orthographicSize) > 0.0001f) {
                _camera.orthographicSize = orthographicSize;
            }
        }
    }
}