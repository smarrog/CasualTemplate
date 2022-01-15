using UnityEngine;
using UnityEngine.UI;

namespace Smr.Components {
    public class AngularGradient : LinearGradient {
        [SerializeField] protected Vector2 _center;

        protected override Material GradientMaterial => new(Shader.Find("UI/AngularGradientShader"));
        protected override TextureWrapMode WrapMode => TextureWrapMode.Repeat;

        protected override void GenerateHelperUvs(VertexHelper vh) {
            var vert = new UIVertex();
            for (var i = 0; i < vh.currentVertCount; i++) {
                vh.PopulateUIVertex(ref vert, i);
                vert.normal = new Vector3(_center.x, 1 - _center.y, _angle);
                vh.SetUIVertex(vert, i);
            }
        }
    }
}