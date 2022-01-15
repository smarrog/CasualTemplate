using UnityEngine;

namespace Smr.Commands {
    public class DestroyWithDelayCommand : DelayCommand {
        public DestroyWithDelayCommand(GameObject go, float delayInSeconds) : base(delayInSeconds, () => {
            if (go) {
                Object.Destroy(go);
            }
        }) {}
    }
}