using Smr.Ui;

namespace Game {
    public class AdsButton : AbstractButton {
        protected override void OnButtonClick() {
            App.PlayTap();
            
            App.UiLogic.ShowAdsWindow();
        }
    }
}