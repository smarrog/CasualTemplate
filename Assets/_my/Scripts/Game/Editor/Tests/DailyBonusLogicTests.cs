using System.Collections.Generic;
using System.Numerics;
using NSubstitute;
using NUnit.Framework;
using Smr.Common;

namespace Game.Tests {
    public class DailyBonusLogicTests {
        private const int DAILY_BONUS_DAY = 5;
        private const int SECONDS_IN_DAY = 86400;

        private DailyBonusLogic _dailyBonusLogic;
        private ISettings _settings;
        private ISaveData _saveData;
        private ITimeService _timeService;
        private ISignalBus _signalBus;
        private IMoneyLogic _moneyLogic;
        
        [SetUp]
        public void Init() {
            _signalBus = Substitute.For<ISignalBus>();
            _saveData = Substitute.For<ISaveData>();
            _timeService = Substitute.For<ITimeService>();
            _moneyLogic = Substitute.For<IMoneyLogic>();
            _moneyLogic.Income.Returns(new BigInteger(10));

            var metaSettings = new MetaSettings {
                DailyBonus = new DailyBonusSettings {
                    Days = new List<int> { 1, 3, 7, 10, 20, 50, 100 }
                }
            };
            _settings = Substitute.For<ISettings>();
            _settings.Meta.Returns(metaSettings);
            _dailyBonusLogic = new DailyBonusLogic(_moneyLogic, _timeService, _settings, _saveData, _signalBus);
            
            _saveData.DailyBonusLastDay.Returns(DAILY_BONUS_DAY);
        }

        [TestCase(3, (DAILY_BONUS_DAY + 0) * SECONDS_IN_DAY, 2, false)]
        [TestCase(3, (DAILY_BONUS_DAY + 0) * SECONDS_IN_DAY, 3, true)]
        [TestCase(3, (DAILY_BONUS_DAY + 0) * SECONDS_IN_DAY, 4, false)]
        [TestCase(3, (DAILY_BONUS_DAY + 1) * SECONDS_IN_DAY, 3, false)]
        [TestCase(3, (DAILY_BONUS_DAY + 1) * SECONDS_IN_DAY, 4, true)]
        [TestCase(3, (DAILY_BONUS_DAY + 1) * SECONDS_IN_DAY, 5, false)]
        public void IsCurrent_Simple(int dailyBonusStreak, long currentTimeStamp, int day, bool expected) {
            _saveData.DailyBonusStreak.Returns(dailyBonusStreak);
            _timeService.CurrentTimeStamp.Returns(currentTimeStamp);

            Assert.AreEqual(expected, _dailyBonusLogic.IsCurrent(day));
        }

        [TestCase(3, (DAILY_BONUS_DAY + 0) * SECONDS_IN_DAY, 2, true)]
        [TestCase(3, (DAILY_BONUS_DAY + 0) * SECONDS_IN_DAY, 3, true)]
        [TestCase(3, (DAILY_BONUS_DAY + 0) * SECONDS_IN_DAY, 4, false)]
        [TestCase(3, (DAILY_BONUS_DAY + 1) * SECONDS_IN_DAY, 2, true)]
        [TestCase(3, (DAILY_BONUS_DAY + 1) * SECONDS_IN_DAY, 3, true)]
        [TestCase(3, (DAILY_BONUS_DAY + 1) * SECONDS_IN_DAY, 4, false)]
        [TestCase(3, (DAILY_BONUS_DAY + 2) * SECONDS_IN_DAY, 2, false)]
        [TestCase(3, (DAILY_BONUS_DAY + 2) * SECONDS_IN_DAY, 3, false)]
        [TestCase(3, (DAILY_BONUS_DAY + 2) * SECONDS_IN_DAY, 4, false)]
        public void IsTaken(int dailyBonusStreak, long currentTimeStamp, int dayToCheck, bool expected) {
            _saveData.DailyBonusStreak.Returns(dailyBonusStreak);
            _timeService.CurrentTimeStamp.Returns(currentTimeStamp);

            var isTaken = _dailyBonusLogic.IsTaken(dayToCheck);
            
            Assert.AreEqual(expected, isTaken);
        }

        [TestCase((DAILY_BONUS_DAY - 1) * SECONDS_IN_DAY, true)]
        [TestCase((DAILY_BONUS_DAY - 1) * SECONDS_IN_DAY + 100, true)]
        [TestCase((DAILY_BONUS_DAY + 0) * SECONDS_IN_DAY, true)]
        [TestCase((DAILY_BONUS_DAY + 0) * SECONDS_IN_DAY + 100, true)]
        [TestCase((DAILY_BONUS_DAY + 1) * SECONDS_IN_DAY, false)]
        public void IsDailyBonusTaken_Simple(long currentTimeStamp, bool expected) {
            _timeService.CurrentTimeStamp.Returns(currentTimeStamp);

            Assert.AreEqual(expected, _dailyBonusLogic.IsCurrentTaken);
        }

        [Test]
        public void TakeDailyBonus_CannotBeTaken_NothingChanges() {
            _timeService.CurrentTimeStamp.Returns(DAILY_BONUS_DAY * SECONDS_IN_DAY);

            _dailyBonusLogic.TakeDailyBonus();

            _saveData.DidNotReceive().Money = Arg.Any<int>();
            _saveData.DidNotReceive().DailyBonusLastDay = Arg.Any<int>();
            _saveData.DidNotReceive().DailyBonusStreak = Arg.Any<int>();
            _moneyLogic.DidNotReceive().AddMoney(Arg.Any<BigInteger>());
            _signalBus.DidNotReceive().Fire(Arg.Any<DailyBonusTakenSignal>());
        }
        
        [TestCase((DAILY_BONUS_DAY + 1) * SECONDS_IN_DAY, 0, 1)]
        [TestCase((DAILY_BONUS_DAY + 2) * SECONDS_IN_DAY, 0, 1)]
        [TestCase((DAILY_BONUS_DAY + 1) * SECONDS_IN_DAY, 3, 4)]
        [TestCase((DAILY_BONUS_DAY + 2) * SECONDS_IN_DAY, 3, 1)]
        [TestCase((DAILY_BONUS_DAY + 1) * SECONDS_IN_DAY, 7, 8)]
        [TestCase((DAILY_BONUS_DAY + 2) * SECONDS_IN_DAY, 7, 1)]
        public void TakeDailyBonus_CanBeTaken_CorrectChanges(long currentTimeStamp, int dailyBonusStreak, int expectedDailyBonusStreak) {
            _timeService.CurrentTimeStamp.Returns(currentTimeStamp);
            _saveData.DailyBonusStreak.Returns(dailyBonusStreak);

            _dailyBonusLogic.TakeDailyBonus();
            var expectedReward = _dailyBonusLogic.GetDailyBonus(expectedDailyBonusStreak);

            _saveData.Received(1).DailyBonusLastDay = (int) (currentTimeStamp / SECONDS_IN_DAY);
            _saveData.Received(1).DailyBonusStreak = expectedDailyBonusStreak;
            _moneyLogic.Received(1).AddMoney(expectedReward);
            _signalBus.Received(1).Fire(Arg.Any<DailyBonusTakenSignal>());
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        public void TakeDailyBonus_CheckMultiplier_CorrectReward(int multiplier) {
            _saveData.DailyBonusStreak.Returns(3);
            _timeService.CurrentTimeStamp.Returns((DAILY_BONUS_DAY + 1) * SECONDS_IN_DAY);
            
            _dailyBonusLogic.TakeDailyBonus(multiplier);
            var expectedReward = _dailyBonusLogic.GetDailyBonus(4) * multiplier;
            
            _moneyLogic.Received(1).AddMoney(expectedReward);
        }

        [TestCase(-2, 0)]
        [TestCase(0, 0)]
        [TestCase(1, 1)]
        [TestCase(3, 7)]
        [TestCase(7, 100)]
        [TestCase(10, 100)]
        public void GetSecondsOfIncome_Simple(int day, int expected) {
            var secondsOfIncome = _dailyBonusLogic.GetSecondsOfIncome(day);
            
            Assert.AreEqual(expected, secondsOfIncome);
        }

        [TestCase(-2, 0)]
        [TestCase(0, 0)]
        [TestCase(1, 10)]
        [TestCase(3, 70)]
        [TestCase(7, 1000)]
        [TestCase(10, 1000)]
        public void GetDailyBonus_Simple(int day, int expected) {
            var reward = _dailyBonusLogic.GetDailyBonus(day);
            
            Assert.AreEqual(new BigInteger(expected), reward);
        }
    }
}