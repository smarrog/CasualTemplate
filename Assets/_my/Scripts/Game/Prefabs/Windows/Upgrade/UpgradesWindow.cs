namespace Game {
    public class UpgradesWindowData : AbstractWindowData {
        
    }

    public class UpgradesWindow : AbstractWindow<UpgradesWindowData> {
        private UpgradeWindowElement[] _elements;
        
        protected override void OnDataApplied() {
            _elements ??= GetComponentsInChildren<UpgradeWindowElement>();
            
            foreach (var element in _elements) {
                element.Activate();
            }
        }

        protected override void AfterHide() {
            foreach (var element in _elements) {
                element.Deactivate();
            }

            base.AfterHide();
        }
    }
}