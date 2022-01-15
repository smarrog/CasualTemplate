using Smr.Ui;
using Smr.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Game {
    public class TutorialPanel : SimpleUiElement {
        [SerializeField] private Button _closeButton;

        protected override void ConstructInternal() {
            base.ConstructInternal();
            
            _closeButton.onClick.AddListener(OnCloseButton);
        }

        public void Show() {
            gameObject.SetActive(true);
            SetVisibilityState(VisibilityState.Appearing);
        }

        private void OnCloseButton() {
            SetVisibilityState(VisibilityState.Hiding, () => {
                gameObject.SetActive(false);
            });
        }
    }
}