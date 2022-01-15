using System;
using System.Numerics;
using Smr.Common;
using Smr.Extensions;
using VContainer;

namespace Game {
    public class DailyBonusLogic : IDailyBonusLogic {
        public bool IsCurrentTaken => _data.DailyBonusLastDay >= CurrentDay;

        private int MaxStreak => _settings.Meta.DailyBonus.Days.Count;
        private int CurrentDay => _timeService.CurrentTimeStamp.ToUnixDay();
        private int CurrentDayStreak {
            get {
                if (CurrentDay > (_data.DailyBonusLastDay + 1)) {
                    return 0;
                }

                if (_settings.Meta.DailyBonus.HoldAtLastDay) {
                    return _data.DailyBonusStreak;
                }

                return ((_data.DailyBonusStreak - 1) % MaxStreak) + 1;
            }
        }

        private readonly IMoneyLogic _moneyLogic;
        private readonly ITimeService _timeService;
        private readonly ISettings _settings;
        private readonly ISaveData _data;
        private readonly ISignalBus _signalBus;

        [Preserve]
        public DailyBonusLogic(
            IMoneyLogic moneyLogic,
            ITimeService timeService,
            ISettings settings,
            ISaveData data,
            ISignalBus signalBus
        ) {
            _moneyLogic = moneyLogic;
            _timeService = timeService;
            _settings = settings;
            _data = data;
            _signalBus = signalBus;
        }
        
        public bool IsCurrent(int dayStreak) {
            var valueToCheck = Math.Min(CurrentDayStreak, MaxStreak);
            if (!IsCurrentTaken) {
                valueToCheck++;
            }
            return valueToCheck == dayStreak;
        }
        
        public bool IsTaken(int dayStreak) {
            return CurrentDayStreak >= dayStreak;
        }

        public void TakeDailyBonus(int multiplier = 1) {
            if (IsCurrentTaken) {
                return;
            }

            var rewardStreak = CurrentDayStreak + 1;
            var reward = GetDailyBonus(rewardStreak);
            reward *= multiplier;
            _moneyLogic.AddMoney(reward);
            
            _data.DailyBonusStreak = rewardStreak;
            _data.DailyBonusLastDay = _timeService.CurrentTimeStamp.ToUnixDay();
            
            _signalBus.Fire(new DailyBonusTakenSignal(rewardStreak));
        }
        
        public int GetSecondsOfIncome(int day) {
            if (day <= 0) {
                return 0;
            }
            
            var secondsOfIncome = _settings.Meta.DailyBonus.Days.GetAtOrLast(day - 1);
            return secondsOfIncome;
        }

        public BigInteger GetDailyBonus(int day) {
            if (day <= 0) {
                return BigInteger.Zero;
            }
            
            var secondsOfIncome = GetSecondsOfIncome(day);
            var income = _moneyLogic.Income == BigInteger.Zero ? BigInteger.One : _moneyLogic.Income;
            return income * secondsOfIncome;
        }
    }
}