using UnityEngine;
using UnityEngine.UI;

namespace Smr.Components {
    [RequireComponent(typeof(CanvasRenderer))]
    public class LinearGradient : MaskableGraphic {
        [SerializeField] private Gradient _gradient = new();
        [SerializeField] private GradientResolution _gradientResolution = GradientResolution.k256;

        [SerializeField]
        protected float _angle = 180;

        private Texture2D _gradientTexture;
        protected virtual TextureWrapMode WrapMode => TextureWrapMode.Clamp;
        protected virtual Material GradientMaterial => new(Shader.Find("UI/LinearGradientShader"));
        public override Texture mainTexture => _gradientTexture;

#if UNITY_EDITOR
        protected override void OnValidate() {
            base.OnValidate();
            Refresh();
        }
#endif

        protected override void Awake() {
            base.Awake();
            Refresh();
        }

        protected override void OnDestroy() {
            base.OnDestroy();
            if (_gradientTexture != null) {
                DestroyImmediate(_gradientTexture);
            }
        }

        public void SetGradient(Gradient gradient) {
            _gradient = gradient;
            Refresh();
        }

        protected virtual void GenerateHelperUvs(VertexHelper vh) {
            var vert = new UIVertex();
            for (var i = 0; i < vh.currentVertCount; i++) {
                vh.PopulateUIVertex(ref vert, i);
                vert.uv1 = new Vector2(_angle, _angle);
                vh.SetUIVertex(vert, i);
            }
        }

        protected override void OnPopulateMesh(VertexHelper vh) {
            base.OnPopulateMesh(vh);
            GenerateHelperUvs(vh);
        }

        private Texture2D GenerateTexture(bool makeNoLongerReadable = false) {
            var gradientResolutionIndex = (int)_gradientResolution;
            var tex = new Texture2D(1, gradientResolutionIndex, TextureFormat.ARGB32, false, true) {
                wrapMode = WrapMode,
                filterMode = FilterMode.Bilinear,
                anisoLevel = 1
            };
            var colors = new Color[gradientResolutionIndex];
            for (var i = 0; i < gradientResolutionIndex; ++i) {
                var t = i / (float)gradientResolutionIndex;
                colors[i] = _gradient.Evaluate(t);
            }
            tex.SetPixels(colors);
            tex.Apply(false, makeNoLongerReadable);

            return tex;
        }

        private void Refresh() {
            if (_gradientTexture != null) {
                DestroyImmediate(_gradientTexture);
            }

            material = GradientMaterial;
            _gradientTexture = GenerateTexture();
        }
    }
}