using System;
using System.Numerics;
using Smr.Common;
using VContainer;

namespace Game {
    public class MoneyLogic : IMoneyLogic {
        private readonly IFieldLogic _fieldLogic;
        private readonly ISettings _settings;
        private readonly ISaveData _data;
        private readonly ISignalBus _signalBus;
        public int IncomePercentMultiplier => _data.GetUpgradeLevel(UpgradeType.IncomeMultiplier) * _settings.Meta.Upgrade.IncomeMultiplierPerUpgrade;
        public BigInteger Money => _data.Money;
        public float PayInterval => _settings.Meta.Money.PayInterval;

        public BigInteger Income { get; private set; }

        [Preserve]
        public MoneyLogic(IFieldLogic fieldLogic, ISettings settings, ISaveData data, ISignalBus signalBus) {
            _fieldLogic = fieldLogic;
            _settings = settings;
            _data = data;
            _signalBus = signalBus;
        }

        public void Initialize() {
            RecalculateIncome();

            _signalBus.Subscribe<ElementLevelChangedSignal>(OnElementLevelChangedSignal);
            _signalBus.Subscribe<UpgradeLevelChangedSignal>(OnUpgradeLevelChangedSignal);
            _signalBus.Subscribe<ResetProgressSignal>(OnResetProgressSignal);
        }

        public bool IsEnoughMoney(BigInteger price) {
            return _settings.Meta.Money.IsEverythingFree || Money >= price;
        }
        
        public void AddMoney(BigInteger value) {
            if (value < 0) {
                throw new Exception("Can't add negative amount of money");
            }
            SetMoney(Money + value);
        }

        public void SpendMoney(BigInteger value, Action onSuccess, Action onFail) {
            if (value < 0) {
                throw new Exception("Can't spend negative amount of money");
            }

            var isEnoughMoney = IsEnoughMoney(value);
            if (!isEnoughMoney) {
                onFail?.Invoke();
                return;
            }

            if (!_settings.Meta.Money.IsEverythingFree) {
                SetMoney(Money - value);
            }
            
            onSuccess?.Invoke();
        }

        public BigInteger ReceivePayFor(int level) {
            // there is no need to save pay time cause interval it is very low and can be started from the beginning
            var amount = GetIncomeFor(level);
            if (amount > BigInteger.Zero) {
                AddMoney(amount);
            }
            return amount;
        }
        
        public void AddDiscount(int amount) {
            _data.DiscountPercent += amount;
        }
        
        public BigInteger GetPriceWithDiscount(BigInteger price) {
            if (_data.DiscountPercent == 0) {
                return price;
            }
            return (price * (100 - _data.DiscountPercent) / 100);
        }

        private void SetMoney(BigInteger value) {
            if (value < 0) {
                throw new Exception("Can't set negative amount of money");
            }
            _data.Money = value;
            _signalBus.Fire(new MoneyChangedSignal(value));
        }

        private void RecalculateIncome() {
            Income = BigInteger.Zero;
            for (var i = 0; i < _fieldLogic.SlotsAmount; ++i) {
                var slotInfo = _fieldLogic.GetSlotInfo(i);
                Income += GetIncomeFor(slotInfo.Level);
            }
            
            _signalBus.Fire(new IncomeChangedSignal(Income));
        }

        private BigInteger GetIncomeFor(int level) {
            if (level <= 0) {
                return BigInteger.Zero;
            }
            
            var income = BigInteger.Pow(2, level - 1);
            var bonusPercents = _data.GetUpgradeLevel(UpgradeType.IncomeMultiplier) * _settings.Meta.Upgrade.IncomeMultiplierPerUpgrade; // TODO cache
            income += (bonusPercents * income / 100);
            return income;
        }

        private void OnElementLevelChangedSignal(ElementLevelChangedSignal signal) {
            RecalculateIncome();
        }

        private void OnUpgradeLevelChangedSignal(UpgradeLevelChangedSignal signal) {
            if (signal.UpgradeType == UpgradeType.IncomeMultiplier) {
                RecalculateIncome();
            }
        }

        private void OnResetProgressSignal(ResetProgressSignal signal) {
            SetMoney(_data.Money);
            RecalculateIncome();
        }
    }
}