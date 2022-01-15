using Smr.Ui;

namespace Game {
    public class AccelerateButton : AbstractButton {
        protected override void OnButtonClick() {
            App.PlayTap();
            
            App.FieldLogic.AccelerateSpawn(App.Settings.Meta.Field.AccelerateSpawnByClick);
        }
    }
}