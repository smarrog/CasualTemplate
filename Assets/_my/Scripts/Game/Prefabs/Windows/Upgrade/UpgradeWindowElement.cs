using System.Numerics;
using Smr.Extensions;
using Smr.Localization;
using UnityEngine;
using UnityEngine.UI;

namespace Game {
    public class UpgradeWindowElement : MonoBehaviour {
        [SerializeField] private UpgradeType _upgradeType;
        [SerializeField] private Image _image;
        [SerializeField] private TextLocalizationComponent _descriptionLocalizationComponent;
        [SerializeField] private MoneyElement _moneyElement;
        [SerializeField] private Button _buyButton;
        [SerializeField] private GameObject _maxLevelObject;
        [SerializeField] private GameObject _notMaxLevelObject;

        private BigInteger Price => App.UpgradeLogic.GetPrice(_upgradeType);
        private bool IsEnoughMoney => App.MoneyLogic.IsEnoughMoney(Price);
        
        private bool _needToUpdate;

        private void Awake() {
            _buyButton.onClick.AddListener(OnBuyButton);
        }

        private void OnDestroy() {
            Deactivate();
        }

        public void Activate() {
            UpdateVisual();
            
            App.SignalBus.Subscribe<MoneyChangedSignal>(OnMoneyChangedSignal);
            App.SignalBus.Subscribe<UpgradeLevelChangedSignal>(OnUpgradeLevelChangedSignal);
            
            App.Scheduler.RegisterUpdate(UpdateHandler);
        }

        public void Deactivate() {
            App.SignalBus.Unsubscribe<MoneyChangedSignal>(OnMoneyChangedSignal);
            App.SignalBus.Unsubscribe<UpgradeLevelChangedSignal>(OnUpgradeLevelChangedSignal);
            
            App.Scheduler.UnregisterUpdate(UpdateHandler);
        }

        private void UpdateHandler(float deltaTime) {
            if (!_needToUpdate) {
                return;
            }

            _needToUpdate = false;
            UpdateVisual();
        }

        private void UpdateVisual() {
            var isMaxLevel = App.UpgradeLogic.IsMaxLevel(_upgradeType);
            if (_maxLevelObject) {
                _maxLevelObject.SetActive(isMaxLevel);
            }
            if (_notMaxLevelObject) {
                _notMaxLevelObject.SetActive(!isMaxLevel);
            }
            
            _moneyElement.SetValue(Price);
            _moneyElement.SetAvailability(IsEnoughMoney);
            
            var descriptionValue = GetDescriptionValue();
            _descriptionLocalizationComponent.SetValue(descriptionValue);
            
            _image.sprite = GetSprite();
        }

        private string GetDescriptionValue() {
            return _upgradeType switch {
                UpgradeType.SpawnSpeed => TimeExtensions.SecondsToString(App.FieldLogic.SpawnInterval, App.SettingsLogic.Localization),
                UpgradeType.SpawnLevel => App.FieldLogic.SpawnLevel.ToString(),
                UpgradeType.IncomeMultiplier => App.MoneyLogic.IncomePercentMultiplier.ToString(),
                _ => null
            };
        }

        private void OnBuyButton() {
            if (App.UpgradeLogic.IsMaxLevel(_upgradeType)) {
                return;
            }
            
            App.PlayTap();
            App.PlayMoneySpend();
            App.MoneyLogic.SpendMoney(Price, () => {
                App.UpgradeLogic.PerformUpgrade(_upgradeType);
            });
        }

        private Sprite GetSprite() {
            return _upgradeType switch {
                UpgradeType.SpawnLevel => App.FieldLogic.GetLevelData(App.FieldLogic.SpawnLevel).Image,
                _ => null
            };
        }

        private void OnMoneyChangedSignal(MoneyChangedSignal signal) {
            _needToUpdate = true;
        }

        private void OnUpgradeLevelChangedSignal(UpgradeLevelChangedSignal signal) {
            if (signal.UpgradeType == _upgradeType) {
                _needToUpdate = true;
            }
        }
    }
}