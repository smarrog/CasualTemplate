using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Smr.Animations {
    public abstract class AbstractTrack : TrackAsset {
        public override void GatherProperties(PlayableDirector director, IPropertyCollector driver) {
#if UNITY_EDITOR
            var comp = director.GetGenericBinding(this) as Transform;
            if (comp == null) {
                return;
            }
            var so = new SerializedObject(comp);
            var iter = so.GetIterator();
            while (iter.NextVisible(true)) {
                if (iter.hasVisibleChildren) {
                    continue;
                }
                driver.AddFromName<Transform>(comp.gameObject, iter.propertyPath);
            }
#endif
            base.GatherProperties(director, driver);
        }

        protected override void OnCreateClip(TimelineClip clip) {
            clip.duration = 0.5f;
            if (clip.asset is ITimelineClipHolder timelineClipHolder) {
                timelineClipHolder.Clip = clip;
            }
            base.OnCreateClip(clip);
        }
    }
}