using UnityEngine;
using UnityEngine.UI;

namespace Smr.Components {
    public class RadialGradient : LinearGradient {
        [SerializeField] private Vector2 _center;
        [Range(0.01f, 10)]
        [SerializeField] private float _radius1 = 1;
        [Range(0.01f, 10)]
        [SerializeField] private float _radius2 = 1;

        protected override Material GradientMaterial => new(Shader.Find("UI/RadialGradientShader"));

        protected override void GenerateHelperUvs(VertexHelper vh) {
            var vert = new UIVertex();
            for (var i = 0; i < vh.currentVertCount; i++) {
                vh.PopulateUIVertex(ref vert, i);
                vert.normal = new Vector3(_radius1, _radius2, _angle);
                vert.uv1 = new Vector2(_center.x, 1 - _center.y);
                vh.SetUIVertex(vert, i);
            }
        }
    }
}