using UnityEngine;

namespace Smr.Components {
    public class DiamondGradient : RadialGradient {
        protected override Material GradientMaterial => new(Shader.Find("UI/DiamondGradientShader"));
    }
}