using Smr.Ui;

namespace Game {
    public class AlbumButton : AbstractButton {
        protected override void OnButtonClick() {
            App.PlayTap();

            App.UiLogic.ShowAlbumWindow();
        }
    }
}