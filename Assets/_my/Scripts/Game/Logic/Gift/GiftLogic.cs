using System.Linq;
using Smr.Common;
using Smr.Extensions;
using Smr.Services;
using VContainer;
using Math = System.Math;

namespace Game {
    public class GiftLogic : IGiftLogic {
        private bool _hasGiftAtField;
        
        private readonly ITimeService _timeService;
        private readonly IRandomService _randomService;
        private readonly ISettings _settings;
        private readonly ISaveData _data;
        private readonly ISignalBus _signalBus;

        public GiftType Active => RemainingTime > 0 ? _data.GiftType : GiftType.Unknown;
        public long RemainingTime => _data.GiftTimestamp + _settings.Meta.Gifts.Duration - _timeService.CurrentTimeStamp;
        
        [Preserve]
        public GiftLogic(ITimeService timeService, IRandomService randomService, ISettings settings, ISaveData data, ISignalBus signalBus) {
            _timeService = timeService;
            _randomService = randomService;
            _settings = settings;
            _data = data;
            _signalBus = signalBus;
        }
        
        public void Apply(GiftType giftType) {
            _data.GiftType = giftType;
            _data.GiftTimestamp = _timeService.CurrentTimeStamp;
            
            _signalBus.Fire(new GiftStartedSignal(giftType));
        }
        
        public GiftType GetRandomGiftType() {
            // TODO restrict Spawn level when max level
            var values = EnumExtensions.GetAllValues<GiftType>(true).ToList();
            return values.GetRandom();
        }
        
        public void SetAtField(bool value) {
            _hasGiftAtField = value;
        }

        public bool CheckIfGiftCanBeSpawned() {
            if (_hasGiftAtField || _data.MaxOpenedLevel < _settings.Meta.Gifts.StartSpawnFromLevel || Active != GiftType.Unknown) {
                return false;
            }
            return _randomService.CheckLuck(_settings.Meta.Gifts.SpawnChance);
        }
        
        public int GetSpawnLevelWithModification(int spawnLevel, int maxLevel) {
            spawnLevel += _settings.Meta.Gifts.SpawnLevelBonus;
            return Math.Min(spawnLevel, maxLevel);
        }
    }
}