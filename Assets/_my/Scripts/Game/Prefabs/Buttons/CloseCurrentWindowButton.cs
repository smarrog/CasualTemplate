using Smr.Ui;

namespace Game {
    public class CloseCurrentWindowButton : AbstractButton {
        protected override void OnButtonClick() {
            App.PlayTap();
            App.UiController.CloseCurrentWindow();
        }
    }
}