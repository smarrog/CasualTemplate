using System;
using System.Numerics;
using Smr.Common;
using VContainer;

namespace Game {
    public class UpgradeLogic : IUpgradeLogic {
        private readonly IFieldLogic _fieldLogic;
        private readonly IMoneyLogic _moneyLogic;
        private readonly ISettings _settings;
        private readonly ISaveData _data;
        private readonly ISignalBus _signalBus;

        [Preserve]
        public UpgradeLogic(IFieldLogic fieldLogic, IMoneyLogic moneyLogic, ISettings settings, ISaveData data, ISignalBus signalBus) {
            _fieldLogic = fieldLogic;
            _moneyLogic = moneyLogic;
            _settings = settings;
            _data = data;
            _signalBus = signalBus;
        }
        public void PerformUpgrade(UpgradeType upgradeType) {
            if (IsMaxLevel(upgradeType)) {
                throw new Exception($"Failed to perform upgrade: {upgradeType} cause it's already max level");
            }
            
            var newLevel = GetLevel(upgradeType) + 1;
            _data.SetUpgradeLevel(upgradeType, newLevel);
            _signalBus.Fire(new UpgradeLevelChangedSignal(upgradeType, newLevel));
        }
        
        public int GetLevel(UpgradeType upgradeType) {
            var maxLevel = GetMaxLevel(upgradeType);
            var savedLevel = _data.GetUpgradeLevel(upgradeType);
            return Math.Min(maxLevel, savedLevel);
        }

        public bool IsMaxLevel(UpgradeType upgradeType) {
            return GetLevel(upgradeType) >= GetMaxLevel(upgradeType);
        }

        public BigInteger GetPrice(UpgradeType upgradeType) {
            var level = GetLevel(upgradeType);
            return GetUpgradePrice(level);
        }

        private int GetMaxLevel(UpgradeType upgradeType) {
            return upgradeType switch {
                UpgradeType.SpawnSpeed => _settings.Meta.Upgrade.MaxSpawnSpeedUpgradeLevel,
                UpgradeType.SpawnLevel => _fieldLogic.MaxLevel - 1,
                UpgradeType.IncomeMultiplier => _settings.Meta.Upgrade.MaxIncomeMultiplierUpgradeLevel,
                _ => throw new ArgumentOutOfRangeException(nameof(upgradeType), upgradeType, null)
            };
        }

        private BigInteger GetUpgradePrice(int n) {
            var value = _settings.Meta.Upgrade.a * Math.Pow(_settings.Meta.Upgrade.r, n) * Math.Pow(n + 1, _settings.Meta.Upgrade.s);
            var result = new BigInteger(Math.Round(value));
            return _moneyLogic.GetPriceWithDiscount(result);
        }
    }
}