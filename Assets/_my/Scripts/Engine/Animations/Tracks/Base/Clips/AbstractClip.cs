using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Smr.Animations {
    [Serializable]
    public abstract class AbstractClip<TBehavior> : PlayableAsset, ITimelineClipAsset, ITimelineClipHolder
        where TBehavior : AbstractBehavior, new() {
        public TimelineClip Clip { get; set; }
        public abstract ClipCaps clipCaps { get; }

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner) {
            var playable = ScriptPlayable<TBehavior>.Create(graph, (TBehavior)default);
            var behavior = playable.GetBehaviour();
            FillBehavior(behavior, graph.GetResolver());
            return playable;
        }

        protected abstract void FillBehavior(TBehavior behavior, IExposedPropertyTable resolver);
    }

    public interface ITimelineClipHolder {
        TimelineClip Clip { get; set; }
    }
}