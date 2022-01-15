using Smr.UI;
using UnityEngine;

namespace Game {
    public class IncomeIndicator : SimpleUiElement {
        [SerializeField] private MoneyElement _moneyElement;

        protected override void ConstructInternal() {
            base.ConstructInternal();
            
            UpdateVisual();
            
            AddSubscription<IncomeChangedSignal>(OnIncomeChangedSignal);
        }

        private void UpdateVisual() {
            _moneyElement.SetValue(App.MoneyLogic.Income);
        }

        private void OnIncomeChangedSignal(IncomeChangedSignal signal) {
            UpdateVisual();
        }
    }
}