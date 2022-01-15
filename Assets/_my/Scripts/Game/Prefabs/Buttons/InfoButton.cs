using Smr.Ui;

namespace Game {
    public class InfoButton : AbstractButton {
        protected override void OnButtonClick() {
            App.PlayTap();

            App.UiController.ShowTutorial();
        }
    }
}