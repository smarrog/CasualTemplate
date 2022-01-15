using UnityEngine;
using UnityEngine.Playables;

namespace Smr.Animations {
    public class BezierTranslationMixerBehavior : AbstractMixerBehavior<BezierTranslationBehavior, Transform> {
        private Vector3 _blendedPosition;
        private IExposedPropertyTable _resolver;

        protected override void ProcessFrameInternalInit(Transform transform) {
            _blendedPosition = Vector3.zero;
        }

        protected override void ProcessFrameInternalComplete(Transform transform) {
            transform.position = _blendedPosition;
        }

        public override void OnBehaviourPlay(Playable playable, FrameData info) {
            base.OnBehaviourPlay(playable, info);
            _resolver = playable.GetGraph().GetResolver();
        }

        protected override void CorrectBehaviorAfterInitialization(BezierTranslationBehavior behavior) {
            base.CorrectBehaviorAfterInitialization(behavior);

            behavior.StartValue ??= new TranslationPointData(Vector3.zero);
            behavior.EndValue ??= new TranslationPointData(Vector3.zero);
        }

        protected override void ProcessBehavior(BezierTranslationBehavior behavior, float normalizedTime, float weight) {
            SetupResolvers(behavior); //we should do it every frame, because sometimes it gets crazy. Idk why, rly.
            var fraction = behavior.Curve.Evaluate(normalizedTime);
            var tweenProgress = BezierMath.GetPoint(
                behavior.StartValue.Position,
                behavior.StartControlPoint.Position,
                behavior.EndControlPoint.Position,
                behavior.EndValue.Position,
                fraction);

            _blendedPosition += tweenProgress * weight;
        }

        private void SetupResolvers(BezierTranslationBehavior behavior) {
            behavior.StartValue.RuntimeResolver = _resolver;
            behavior.StartControlPoint.RuntimeResolver = _resolver;
            behavior.EndControlPoint.RuntimeResolver = _resolver;
            behavior.EndValue.RuntimeResolver = _resolver;
        }
    }
}