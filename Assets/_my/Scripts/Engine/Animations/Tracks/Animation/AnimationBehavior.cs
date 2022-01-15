using UnityEngine;
using UnityEngine.Playables;

namespace Smr.Animations {
    public class AnimationBehavior : AbstractBehavior {
        protected override bool ExecuteOnce => true;

        protected override void ProcessFrameInternal(Playable playable, FrameData info, object playerData) {
            base.ProcessFrameInternal(playable, info, playerData);

            var animation = playerData as Animation;
            if (animation == null) {
                return;
            }

            animation.Play();
        }
    }
}