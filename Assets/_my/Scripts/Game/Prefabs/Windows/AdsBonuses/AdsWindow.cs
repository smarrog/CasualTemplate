namespace Game {
    public class AdsWindowData : AbstractWindowData {
        
    }
    
    public class AdsWindow : AbstractWindow<AdsWindowData> {
        private AbstractAdsBonus[] _elements;
        
        protected override void ConstructInternal() {
            base.ConstructInternal();

            _elements = GetComponentsInChildren<AbstractAdsBonus>();
        }

        protected override void OnDataApplied() {
            foreach (var element in _elements) {
                element.Init(OnBonusGiven);
            }
        }

        private void OnBonusGiven() {
            foreach (var element in _elements) {
                element.UpdateVisual();
            }
        }
    }
}