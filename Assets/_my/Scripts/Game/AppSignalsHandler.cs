using System.Collections.Generic;
using Smr.Extensions;
using Smr.Localization;
using Smr.Vibration;
using UnityEngine;

namespace Game {
    public class AppSignalsHandler : MonoBehaviour {
        private readonly object _focusAudioLock = new object();
        private readonly object _pauseAudioLock = new object();
        
        public void Initialize() {
            App.SignalBus.Subscribe<SpawnSignal>(OnSpawnSignal);
            App.SignalBus.Subscribe<MoveSignal>(OnMoveSignal);
            App.SignalBus.Subscribe<UpgradeLevelChangedSignal>(OnUpgradeLevelChangedSignal);
            App.SignalBus.Subscribe<MaxOpenedLevelChangedSignal>(OnMaxOpenedLevelChangedSignal);
            App.SignalBus.Subscribe<GiftStartedSignal>(OnGiftStartedSignal);
            App.SignalBus.Subscribe<ElementUnlockedSignal>(OnElementUnlockedSignal);
            App.SignalBus.Subscribe<DailyBonusTakenSignal>(OnDailyBonusTakenSignal);
            App.SignalBus.Subscribe<OfflineRewardGivenSignal>(OnOfflineRewardGivenSignal);
            App.SignalBus.Subscribe<SettingsChangedSignal>(OnSettingsChangedSignal);
            App.SignalBus.Subscribe<LocalizationChangedSignal>(OnLocalizationChangedSignal);
                
            App.Scheduler.RegisterUpdate(UpdateLastOnlineTime);
        }

        private void UpdateLastOnlineTime(float deltaTime = 0) {
            App.OfflineLogic.UpdateLastOnlineTimestamp();
        }
        
        private void OnApplicationQuit() {
            UpdateLastOnlineTime();
            App.SaveService.Flush();
        }

        private void OnApplicationPause(bool pause) {
            if (pause) {
                App.UiController.HideOfflineBonusWindow();
                App.Scheduler.UnregisterUpdate(UpdateLastOnlineTime);
                UpdateLastOnlineTime();
                App.AudioService.Mute(_pauseAudioLock);
            } else {
                App.OfflineLogic.CheckOfflineReward();
                App.Scheduler.RegisterUpdate(UpdateLastOnlineTime);
                App.AudioService.Unmute(_pauseAudioLock);
            }
        }

        private void OnApplicationFocus(bool hasFocus) {
            if (hasFocus) {
                App.AudioService.Unmute(_focusAudioLock);
            } else {
                App.AudioService.Mute(_focusAudioLock);
            }
        }

        private void OnSpawnSignal(SpawnSignal signal) {
            Vibration.VibrateShortPulse();
            App.PlaySpawn();
            App.SaveService.Flush();
        }

        private void OnMoveSignal(MoveSignal signal) {
            if (signal.MoveResult == MoveResult.Merge) {
                Vibration.VibrateShortPulse();
                App.SaveService.Flush();
            }
            App.PlayMoveResult(signal.MoveResult);
        }
        
        private void OnUpgradeLevelChangedSignal(UpgradeLevelChangedSignal signal) {
            App.TrackingService.Track(TrackingConstants.UPGRADE_LEVEL_CHANGED, new Dictionary<string, object> {
                ["upgrade"] = signal.UpgradeType.ToString(),
                ["level"] = signal.UpgradeLevel.ToString()  
            });
        }

        private void OnMaxOpenedLevelChangedSignal(MaxOpenedLevelChangedSignal signal) {
            if (signal.Level is 10 or 15 or 20) {
                App.AskForRate();
            }

            if (signal.Level > 1) {
                App.UiLogic.ShowAlbumWindow(true);
            }
            
            App.TrackingService.Track(TrackingConstants.LEVEL_REACHED, new Dictionary<string, object> {
                ["level"] = signal.Level
            });
        }

        private void OnGiftStartedSignal(GiftStartedSignal signal) {
            App.TrackingService.Track(TrackingConstants.GIFT_STARTED, new Dictionary<string, object> {
                ["gift"] = signal.GiftType.ToString()
            });
        }

        private void OnElementUnlockedSignal(ElementUnlockedSignal signal) {
            App.TrackingService.Track(TrackingConstants.ELEMENT_UNLOCKED, new Dictionary<string, object> {
                ["index"] = signal.Index
            });
        }

        private void OnDailyBonusTakenSignal(DailyBonusTakenSignal signal) {
            App.TrackingService.Track(TrackingConstants.DAILY_BONUS_TAKEN, new Dictionary<string, object> {
                ["streak"] = signal.CurrentStreak
            });
        }

        private void OnOfflineRewardGivenSignal(OfflineRewardGivenSignal signal) {
            App.TrackingService.Track(TrackingConstants.OFFLINE_REWARD_GIVEN, new Dictionary<string, object> {
                ["time"] = signal.OfflineTime,
                ["reward"] = signal.Reward.ToAbbreviatedString()
            });
        }

        private void OnSettingsChangedSignal(SettingsChangedSignal signal) {
            var data = new Dictionary<string, object> {
                ["settings"] = signal.SettingsType.ToString()
            };
            switch (signal.SettingsType) {
                case SettingsType.Sound:
                case SettingsType.Music:
                case SettingsType.Vibration:
                    data["value"] = App.SettingsLogic.IsAvailable(signal.SettingsType) ? "ON" : "OFF";
                    break;
            }
            
            App.TrackingService.Track(TrackingConstants.SETTINGS_CHANGED, data);
        }

        private void OnLocalizationChangedSignal(LocalizationChangedSignal signal) {
            App.TrackingService.Track(TrackingConstants.LOCALIZATION_CHANGED, new Dictionary<string, object> {
                ["localization"] = signal.Localization.ToString()
            });
        }
    }
}