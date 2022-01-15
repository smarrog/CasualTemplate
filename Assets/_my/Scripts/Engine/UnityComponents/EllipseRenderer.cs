using UnityEngine;

namespace Smr.Components {
    [ExecuteInEditMode]
    [RequireComponent(typeof(LineRenderer))]
    public class EllipseRenderer : MonoBehaviour {
        [SerializeField] private Vector2 _radius;
        [SerializeField] private int _segments = 360;

        private LineRenderer _lineRenderer;

        private void Start() {
            _lineRenderer = gameObject.GetComponent<LineRenderer>();
            _lineRenderer.useWorldSpace = false;
        }

        private void OnValidate() {
            _lineRenderer.positionCount = _segments + 1;
            CreatePoints();
        }

        private void CreatePoints() {
            var angle = 0f;
            for (int i = 0; i < _segments + 1; i++) {
                var radians = Mathf.Deg2Rad * angle;
                var x = Mathf.Sin(radians) * _radius.x;
                var y = Mathf.Cos(radians) * _radius.y;

                _lineRenderer.SetPosition(i, new Vector3(x, y, 0f));
                angle += 360f / _segments;
            }
        }
    }
}