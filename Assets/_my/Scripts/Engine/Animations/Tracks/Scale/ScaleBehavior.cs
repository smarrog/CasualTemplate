using UnityEngine;

namespace Smr.Animations {
    public class ScaleBehavior : AbstractBehaviorWithLerpValue<Vector3> {
        public bool SaveStartSign;
        public bool IsRelative;
    }
}