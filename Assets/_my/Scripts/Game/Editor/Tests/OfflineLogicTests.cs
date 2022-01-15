using System.Numerics;
using NSubstitute;
using NUnit.Framework;
using Smr.Common;

namespace Game.Tests {
    public class OfflineLogicTests {
        private OfflineLogic _offlineLogic;
        private IMoneyLogic _moneyLogic;
        private IUiLogic _uiLogic;
        private ITimeService _timeService;
        private MetaSettings _metaSettings;
        private ISaveData _saveData;
        private ISignalBus _signalBus;
        
        [SetUp]
        public void Init() {
            _moneyLogic = Substitute.For<IMoneyLogic>();
            _uiLogic = Substitute.For<IUiLogic>();
            _timeService = Substitute.For<ITimeService>();
            _metaSettings = new MetaSettings {
                Offline = new OfflineSettings {
                    MinDurationToShowWindow = 10,
                    MaxDuration = 100
                }
            };
            var settings = Substitute.For<ISettings>();
            settings.Meta.Returns(_metaSettings);
            _saveData = Substitute.For<ISaveData>();
            _signalBus = Substitute.For<ISignalBus>();
            
            _offlineLogic = new OfflineLogic(_uiLogic, _moneyLogic, _timeService, settings, _saveData, _signalBus);
        }

        [Test]
        public void UpdateLastOnlineTimestamp_Simple() {
            _timeService.CurrentTimeStamp.Returns(100);
            
            _offlineLogic.UpdateLastOnlineTimestamp();

            _saveData.Received(1).LastOnlineTimestamp = 100;
        }

        [Test]
        public void CheckOfflineReward_EnoughTimePassed_ShowWindow() {
            _moneyLogic.Income.Returns(50);
            _saveData.LastOnlineTimestamp.Returns(100);
            _timeService.CurrentTimeStamp.Returns(100L + _metaSettings.Offline.MinDurationToShowWindow + 5);
            
            _offlineLogic.CheckOfflineReward();

            var offlineTime = _metaSettings.Offline.MinDurationToShowWindow + 5;
            var expectedReward = offlineTime * _moneyLogic.Income;
            _uiLogic.Received(1).ShowOfflineBonus(offlineTime, expectedReward);
            _saveData.DidNotReceive().LastOnlineTimestamp = Arg.Any<long>();
            _moneyLogic.DidNotReceive().AddMoney(Arg.Any<BigInteger>());
        }

        [Test]
        public void CheckOfflineReward_RewardIsGreaterThenMax_MaxRewardWasGiven() {
            _moneyLogic.Income.Returns(50);
            _saveData.LastOnlineTimestamp.Returns(100);
            _timeService.CurrentTimeStamp.Returns(100L + _metaSettings.Offline.MaxDuration + 5);
            
            _offlineLogic.CheckOfflineReward();

            var offlineTime = _metaSettings.Offline.MaxDuration;
            var expectedReward = offlineTime * _moneyLogic.Income;
            _uiLogic.Received(1).ShowOfflineBonus(offlineTime, expectedReward);
            _saveData.DidNotReceive().LastOnlineTimestamp = Arg.Any<long>();
            _moneyLogic.DidNotReceive().AddMoney(Arg.Any<BigInteger>());
        }

        [Test]
        public void CheckOfflineReward_NotEnoughTimePassed_RewardWasGiven() {
            _moneyLogic.Income.Returns(50);
            _saveData.LastOnlineTimestamp.Returns(100);
            _timeService.CurrentTimeStamp.Returns(100L + _metaSettings.Offline.MinDurationToShowWindow - 5);
            
            _offlineLogic.CheckOfflineReward();

            var offlineTime = _metaSettings.Offline.MinDurationToShowWindow - 5;
            var expectedReward = offlineTime * _moneyLogic.Income;
            _uiLogic.DidNotReceive().ShowOfflineBonus(offlineTime, Arg.Any<BigInteger>());
            _saveData.Received(1).LastOnlineTimestamp = Arg.Any<long>();
            _moneyLogic.Received(1).AddMoney(expectedReward);
        }
    }
}