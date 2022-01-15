using System;
using UnityEngine;

namespace Smr.Animations {
    [Serializable]
    public class TranslationClip : AbstractClipWithExposedLerpValue<TranslationBehavior, Transform, Vector3> {}
}