using System.Numerics;
using Smr.UI;
using UnityEngine;

namespace Game {
    public class MoneyIndicator : SimpleUiElement {
        [SerializeField] private MoneyElement _moneyElement;

        private bool _needToUpdate;
        private BigInteger _value;

        protected override void ConstructInternal() {
            base.ConstructInternal();
            
            UpdateValue();
            UpdateVisual();

            AddSubscription<MoneyChangedSignal>(OnMoneyChangedSignal);

            App.Scheduler.DoEvery(0.1f, UpdateHandler);
        }

        private void UpdateHandler() {
            if (!_needToUpdate) {
                return;
            }

            _needToUpdate = false;
            UpdateVisual();
        }

        private void UpdateValue() {
            _value = App.MoneyLogic.Money;
            _needToUpdate = true;
        }
        
        private void UpdateVisual() {
            _moneyElement.SetValue(_value);
        }

        private void OnMoneyChangedSignal(MoneyChangedSignal signal) {
            UpdateValue();
        }
    }
}