using TMPro;
using UnityEngine;

namespace Smr.Ui {
    public abstract class AbstractToggler : AbstractButton {
        [SerializeField] private TMP_Text _label;

        protected abstract bool Value { get; }

        protected override void ConstructInternal() {
            base.ConstructInternal();
            
            UpdateVisual();

            SubscribeOnChanges();
        }

        protected virtual void UpdateVisual() {
            if (_label) {
                _label.text = GetDescription();
            }
        }

        protected abstract void SubscribeOnChanges();

        protected virtual string GetDescription() => null;
    }
}