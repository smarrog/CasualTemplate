using Smr.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Smr.Ui {
    [RequireComponent(typeof(Button))]
    public abstract class AbstractButton : SimpleUiElement {
        private Button _button;

        protected override void ConstructInternal() {
            base.ConstructInternal();
            
            _button = GetComponent<Button>();
            _button.onClick.AddListener(OnButtonClick);
        }

        protected abstract void OnButtonClick();
    }
}