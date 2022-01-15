using Smr.Extensions;
using UnityEngine;

namespace Smr.Animations {
    public class RenderersAlphaMixerBehavior : AbstractMixerBehaviorForFloat<AlphaBehavior, Transform> {
        private static readonly int _colorProp = Shader.PropertyToID("_Color");
        private static readonly int _customColorProp = Shader.PropertyToID("_CustomColorMultiplier");

        private Renderer[] _renderers;

        protected override void ProcessFirstFrameInit(Transform trackBinding) {
            _renderers = trackBinding.GetComponentsInChildren<Renderer>();
            base.ProcessFirstFrameInit(trackBinding);
        }

        protected override float GetValue(Transform trackBinding) {
            if (_renderers.Length != 0) {
                var renderer = _renderers[0];

                if (renderer is SpriteRenderer spriteRenderer) {
                    return spriteRenderer.color.a;
                }
                foreach (var mat in renderer.materials) {
                    var customAlpha = mat.GetPropAlpha(_colorProp) ?? mat.GetPropAlpha(_customColorProp);
                    if (customAlpha.HasValue) {
                        return customAlpha.Value;
                    }
                }
            }
            return 0;
        }

        protected override void SetValue(Transform trackBinding, float alpha) {
            foreach (var render in _renderers) {
                if (render is SpriteRenderer spriteRenderer) {
                    spriteRenderer.color = spriteRenderer.color.WithA(alpha);
                } else {
                    foreach (var mat in render.materials) {
                        mat.SetPropAlpha(_customColorProp, alpha);
                        mat.SetPropAlpha(_colorProp, alpha);
                    }
                }
            }
        }
    }
}