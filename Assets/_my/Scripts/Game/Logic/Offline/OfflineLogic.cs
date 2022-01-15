using System;
using System.Numerics;
using Smr.Common;
using VContainer;

namespace Game {
    public class OfflineLogic : IOfflineLogic {
        private readonly IUiLogic _uiLogic;
        private readonly IMoneyLogic _moneyLogic;
        private readonly ITimeService _timeService;
        private readonly ISettings _settings;
        private readonly ISaveData _data;
        private readonly ISignalBus _signalBus;

        [Preserve]
        public OfflineLogic(
            IUiLogic uiLogic,
            IMoneyLogic moneyLogic,
            ITimeService timeService,
            ISettings settings,
            ISaveData data,
            ISignalBus signalBus
        ) {
            _uiLogic = uiLogic;
            _moneyLogic = moneyLogic;
            _timeService = timeService;
            _settings = settings;
            _data = data;
            _signalBus = signalBus;
        }

        public void UpdateLastOnlineTimestamp() {
            _data.LastOnlineTimestamp = _timeService.CurrentTimeStamp;
        }

        public void CheckOfflineReward() {
            var offlineTime = _timeService.CurrentTimeStamp - _data.LastOnlineTimestamp;
            offlineTime = Math.Clamp(offlineTime, 0, _settings.Meta.Offline.MaxDuration);
            
            var reward = offlineTime * _moneyLogic.Income;
            if (reward == BigInteger.Zero) {
                return;
            }

            // silently give reward
            if (offlineTime <= _settings.Meta.Offline.MinDurationToShowWindow) {
                GiveOfflineReward(offlineTime, reward);
                return;
            }
            
            _uiLogic.ShowOfflineBonus(offlineTime, reward);
        }

        public void GiveOfflineReward(long offlineTime, BigInteger reward) {
            UpdateLastOnlineTimestamp();
            _moneyLogic.AddMoney(reward);
            _signalBus.Fire(new OfflineRewardGivenSignal(offlineTime, reward));
        }
    }
}