using Smr.Common;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Smr.Animations {
    [TrackColor(0.7366781f, 0.3261246f, 0.8529412f)]
    [TrackClipType(typeof(TimeMachineClip))]
    public class TimeMachineTrack : TrackAsset {
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount) {
            var scriptPlayable = ScriptPlayable<TimeMachineMixerBehavior>.Create(graph, inputCount);

            var b = scriptPlayable.GetBehaviour();
            b.Markers.Clear();

            //This foreach will rename clips based on what they do, and collect the markers and put them into a dictionary
            //Since this happens when you enter Preview or Play mode, the object holding the Timeline must be enabled or you won't see any change in names
            foreach (var c in GetClips()) {
                if (c.asset == null) {
                    EngineDependencies.Logger.LogError("Failed to get clip asset");
                    continue;
                }
                
                var clip = (TimeMachineClip)c.asset;
                var clipName = c.displayName;

                clip.StartTime = c.start;
                clip.EndTime = c.end;

                switch (clip.Action) {
                    case TimeMachineAction.Event:
                        clipName = "! " + clip.EventName;
                        break;
                    case TimeMachineAction.Pause:
                        clipName = "||";
                        break;
                    case TimeMachineAction.Marker:
                        clipName = "● " + clip.MarkerLabel;

                        //Insert the marker clip into the Dictionary of markers
                        if (!b.Markers.ContainsKey(clip.MarkerLabel)) { //happens when you duplicate a clip and it has the same markerLabel
                            b.Markers.Add(clip.MarkerLabel, c.start);
                        }

                        break;
                    case TimeMachineAction.JumpToMarker:
                        clipName = "<- " + clip.MarkerToJumpTo;
                        break;
                    case TimeMachineAction.JumpToTime:
                        clipName = "<- " + clip.TimeToJumpTo;
                        break;
                    case TimeMachineAction.Skip:
                        clipName = "->";
                        break;
                }

                c.displayName = clipName;
            }

            return scriptPlayable;
        }
    }
}