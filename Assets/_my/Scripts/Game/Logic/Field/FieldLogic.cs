using System;
using System.Collections.Generic;
using Smr.Common;
using Smr.Extensions;
using VContainer;

namespace Game {
    public class FieldLogic : IFieldLogic {
        public const int GIFT_LEVEL = -1; // level for convenience in save
        
        // settings values
        public int MaxLevel => _settings.Meta.Field.MaxLevel > 0 ? _settings.Meta.Field.MaxLevel : _settings.Meta.Field.Levels.Count;
        public int MaxOpenedLevel => _data.MaxOpenedLevel;
        public int SlotsAmount => _settings.Meta.Field.SlotsAmount;
        public float SpawnInterval => _settings.Meta.Field.SpawnInterval - (_data.GetUpgradeLevel(UpgradeType.SpawnSpeed) * 0.1f);

        // saved value
        public bool CanUnlockSlot => UnlockedElementsAmount < _settings.Meta.Field.SlotsAmount;
        public int SpawnLevel => _data.GetUpgradeLevel(UpgradeType.SpawnLevel) + 1;
        public int CurrentDiscountPercent => _data.DiscountPercent;
        private int UnlockedElementsAmount => _settings.Meta.Field.UnlockedSlotsAmount + _data.UnlockedAmount;

        // runtime values
        public float TimeFromLastSpawn { get; private set; }
        
        // cache
        private bool _isFieldFull;
        
        private readonly IGiftLogic _giftLogic;
        private readonly ISettings _settings;
        private readonly ISignalBus _signalBus;
        private readonly ISaveData _data;

        [Preserve]
        public FieldLogic(IGiftLogic giftLogic, ISettings settings, ISaveData data, ISignalBus signalBus) {
            _giftLogic = giftLogic;
            _settings = settings;
            _signalBus = signalBus;
            _data = data;
        }

        public void Initialize() {
            _signalBus.Subscribe<UpgradeLevelChangedSignal>(OnUpgradeLevelChangedSignal);
            _signalBus.Subscribe<ResetProgressSignal>(OnResetProgressSignal);
        }

        public void SetLevel(int index, int value) {
            if (value != GIFT_LEVEL && (value < 0 || value > MaxLevel)) {
                throw new Exception($"Level {value} is unsupported");
            }
            
            var slotInfo = GetSlotInfo(index);
            if (slotInfo.IsLocked) {
                throw new Exception("Try to change level of locked element");
            }
            
            if (slotInfo.Level == value) {
                return;
            }

            if (slotInfo.Level == GIFT_LEVEL) {
                _giftLogic.SetAtField(false);
            }

            if (value == GIFT_LEVEL) {
                _giftLogic.SetAtField(true);
            }

            if (_data.MaxOpenedLevel < value) {
                _data.MaxOpenedLevel = value;
                _signalBus.Fire(new MaxOpenedLevelChangedSignal(value));
            }

            _isFieldFull = false;
            _data.SetElementLevel(index, value);
            _signalBus.Fire(new ElementLevelChangedSignal(index, value));
        }

        public SlotInfo GetSlotInfo(int index) {
            if (index < 0 || index >= _settings.Meta.Field.SlotsAmount) {
                throw new Exception($"Index {index} is out of range");
            }

            var level = _data.GetElementLevel(index);
            var isLocked = index >= (_settings.Meta.Field.UnlockedSlotsAmount + _data.UnlockedAmount);
            var result = new SlotInfo(level, isLocked);
            return result;
        }

        public void UnlockSlot() {
            if (!CanUnlockSlot) {
                return;
            }

            _isFieldFull = false;
            _data.UnlockedAmount++;
            var unlockedElementIndex = UnlockedElementsAmount - 1;
            SetLevel(unlockedElementIndex, 0);
            _signalBus.Fire(new ElementUnlockedSignal(unlockedElementIndex));
        }

        public LevelData GetLevelData(int level) {
            return _settings.Meta.Field.Levels.GetAtOrDefault(level - 1);
        }
        
        public bool CanMoveTo(int from, int to) {
            if (from == -1 || to == -1 || from == to) {
                return false;
            }

            var fromSlot = GetSlotInfo(from);
            if (!fromSlot.IsFilledWithElement) {
                return false;
            }
            
            var toSlot = GetSlotInfo(to);
            if (toSlot.IsLocked) {
                return false;
            }

            var isMerge = fromSlot.Level == toSlot.Level;
            if (isMerge) {
                return fromSlot.Level != MaxLevel;
            }

            return _settings.Meta.Field.IsSwapEnabled || toSlot.IsEmpty;
        }

        public MoveResult MoveTo(int from, int to) {
            if (!CanMoveTo(from, to)) {
                return MoveResult.Fail;
            }

            MoveResult result;
            var fromSlot = GetSlotInfo(from);
            var toSlot = GetSlotInfo(to);
            var isMerge = fromSlot.Level == toSlot.Level;
            var resultFromLevel = isMerge ? 0 : toSlot.Level;
            var resultToLevel = isMerge ? fromSlot.Level + 1 : fromSlot.Level;

            if (isMerge) {
                result = MoveResult.Merge;
            } else if (toSlot.IsEmpty) {
                result = MoveResult.Move;
            } else {
                result = MoveResult.Swap;
            }
            
            SetLevel(from, resultFromLevel);
            SetLevel(to, resultToLevel);

            return result;
        }

        public void AccelerateSpawn(float value) {
            TimeFromLastSpawn += value;
        }

        public void TryToSpawnOrReplace(int levelToSpawn, int amount) {
            var levelsMap = new Dictionary<int, List<int>>();
            for (var i = 0; i < SlotsAmount; ++i) {
                var slotInfo = GetSlotInfo(i);
                if (slotInfo.IsLocked) {
                    continue;
                }
                
                if (!levelsMap.TryGetValue(slotInfo.Level, out var indexes)) {
                   levelsMap[slotInfo.Level] = indexes = new List<int>();
                }
                indexes.Add(i);
            }

            var remainingAmount = amount;
            for (var i = 0; i < levelToSpawn; ++i) {
                var indexesForLevel = levelsMap.GetValueOrDefault(i);
                if (indexesForLevel == null) {
                    continue;
                }
                foreach (var index in indexesForLevel) {
                    --remainingAmount;
                    SetLevel(index, levelToSpawn);   
                    _signalBus.Fire(new SpawnSignal(index, levelToSpawn));
                    if (remainingAmount == 0) {
                        return;
                    }
                }
            }
        }

        public bool TryToSpawn(out int spawnIndex) {
            if (_isFieldFull) { // just for optimization
                spawnIndex = -1;
                return false;
            }
            
            var isGift = _giftLogic.CheckIfGiftCanBeSpawned();
            var spawnLevel = isGift ? GIFT_LEVEL : SpawnLevel;
            if (!isGift && _giftLogic.Active == GiftType.SpawnLevel) {
                spawnLevel = _giftLogic.GetSpawnLevelWithModification(spawnLevel, MaxLevel);
            }
            
            var isSpawned = TryToSpawnInternal(spawnLevel, out spawnIndex);
            if (isSpawned) {
                TimeFromLastSpawn = 0;
            }
            return isSpawned;
        }

        private bool TryToSpawnInternal(int levelToSpawn, out int spawnIndex) {
            spawnIndex = -1;
            // there is no need to save spawn time cause interval it is very low and can be started from the beginning
            for (var i = 0; i < SlotsAmount; ++i) {
                var slotInfo = GetSlotInfo(i);
                if (slotInfo.IsLocked) {
                    _isFieldFull = true;
                    return false; // other will be also locked
                }
                
                if (!slotInfo.IsEmpty) {
                    continue; // already have item
                }
                
                // empty space, can spawn
                spawnIndex = i;
                SetLevel(spawnIndex, levelToSpawn);
                _signalBus.Fire(new SpawnSignal(spawnIndex, levelToSpawn));
                return true;
            }
            
            _isFieldFull = true;
            return false;
        }

        private void OnUpgradeLevelChangedSignal(UpgradeLevelChangedSignal signal) {
            if (signal.UpgradeType != UpgradeType.SpawnLevel) {
                return;
            }

            for (var i = 0; i < SlotsAmount; ++i) {
                var slotInfo = GetSlotInfo(i);
                if (slotInfo.IsFilledWithElement && slotInfo.Level < SpawnLevel) {
                    SetLevel(i, SpawnLevel);
                }
            }
        }

        private void OnResetProgressSignal(ResetProgressSignal signal) {
            _isFieldFull = false;
            TimeFromLastSpawn = 0;
        }
    }
}