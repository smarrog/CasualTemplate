using UnityEngine;
using UnityEngine.Playables;

namespace Smr.Animations {
    public class DirectorStatusCondition : TimeMachineCondition {
        [SerializeField] private PlayableDirector _director;
        [SerializeField] private PlayState _playState;

        public override bool Check() {
            if (!_director) {
                return false;
            }
            return _director.state == _playState;
        }
    }
}