namespace Smr.UI {
    public class SimpleUiElement : AbstractUiElement {
        private void Awake() {
            ConstructIfNeed();
        }

        protected override void ConstructInternal() {
            
        }
        
        protected sealed override void OnDataApplied() {
            
        }
    }
}