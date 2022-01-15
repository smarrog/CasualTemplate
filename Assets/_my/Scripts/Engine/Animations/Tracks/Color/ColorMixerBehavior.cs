using Smr.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace Smr.Animations {
    public class ColorMixerBehavior : AbstractMixerBehaviorForColor<ColorBehavior, Transform> {
        private static readonly int _colorProp = Shader.PropertyToID("_Color");
        private static readonly int _customColorProp = Shader.PropertyToID("_CustomColorMultiplier");

        private Graphic _graphic;
        private Renderer[] _renderers;

        protected override void ProcessFirstFrameInit(Transform trackBinding) {
            _graphic = trackBinding.GetComponent<Graphic>();
            if (_graphic == null) {
                _renderers = trackBinding.GetComponentsInChildren<Renderer>();
            }
            base.ProcessFirstFrameInit(trackBinding);
        }

        protected override Color GetValue(Transform trackBinding) {
            if (_graphic != null) {
                return _graphic.color;
            }
            if (_renderers is { Length: > 0 }) {
                var render = _renderers[0];
                if (render is SpriteRenderer spriteRenderer) {
                    return spriteRenderer.color;
                }
                foreach (var mat in render.sharedMaterials) {
                    var customColor = mat.GetPropColor(_customColorProp) ?? mat.GetPropColor(_colorProp);
                    if (customColor.HasValue) {
                        return customColor.Value;
                    }
                }
            }
            return Color.white;
        }

        protected override void SetValue(Transform trackBinding, Color value) {
            if (_graphic != null) {
                _graphic.color = value;
            }
            _renderers?.ForEach(r => SetValue(r, value));
        }

        private void SetValue(Renderer renderer, Color value) {
            if (renderer is SpriteRenderer spriteRenderer) {
                spriteRenderer.color = value;
                return;
            }
            
            foreach (var mat in renderer.sharedMaterials) {
                mat.SetColor(_customColorProp, value);
                mat.SetColor(_colorProp, value);
            }
        }
    }
}