using Smr.Ui;
using Smr.UI;
using UnityEngine;

namespace Game {
    public class SpawnIndicator : SimpleUiElement {
        [SerializeField] private SimpleProgressBar _progressBar;

        protected override void ConstructInternal() {
            base.ConstructInternal();
            
            App.Scheduler.RegisterUpdate(UpdateHandler);
        }

        public void UpdateHandler(float delta) {
            var value = App.FieldLogic.TimeFromLastSpawn / App.FieldLogic.SpawnInterval;
            _progressBar.SetValue(value);
        }
    }
}