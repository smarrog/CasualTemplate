using UnityEngine;

namespace Smr.Ui {
    public abstract class AbstractToggleSwitch : AbstractToggler {
        [SerializeField] private UiVisibilityController _togglerVisibilityController; // Visible = On Invisible = Off
        
        private bool? _visualValue;

        protected override void UpdateVisual() {
            base.UpdateVisual();
            
            if (_visualValue.HasValue && _visualValue == Value) {
                return;
            }
            
            if (_visualValue.HasValue) {
                _togglerVisibilityController.SetVisibilityAnimated(Value);
            } else {
                _togglerVisibilityController.SetVisibility(Value);
            }
            _visualValue = Value;
        }
    }
}