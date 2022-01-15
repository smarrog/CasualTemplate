using Smr.Ui;

namespace Game {
    public class SettingsButton : AbstractButton {
        protected override void OnButtonClick() {
            App.PlayTap();

            App.UiLogic.ShowSettingsWindow();
        }
    }
}