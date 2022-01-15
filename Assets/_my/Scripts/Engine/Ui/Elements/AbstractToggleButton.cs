using UnityEngine;

namespace Smr.Ui {
    public abstract class AbstractToggleButton : AbstractToggler {
        [SerializeField] private GameObject _toggleOnVisual;
        [SerializeField] private GameObject _toggleOffVisual;

        protected override void UpdateVisual() {
            base.UpdateVisual();
            _toggleOnVisual.SetActive(Value);
            _toggleOffVisual.SetActive(!Value);
        }
    }
}