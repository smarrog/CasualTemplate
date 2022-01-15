using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Smr.Audio;
using Smr.Common;
using Smr.Components;
using Smr.Extensions;
using Smr.Services;
using Smr.Tracking;
using Smr.Utils;
using YG;

namespace Game {
    public static class App {
        public static bool IsReady { get; set; }
        public static bool IsFirstSession => YG2.isFirstGameSession;

#region PRELOADER SCOPE

        public static ISettings Settings { get; set; }
        public static IAudioService AudioService { get; set; }
        public static IScheduler Scheduler { get; set; }

        public static ILogService Logger { get; set; }
        public static ISignalBus SignalBus { get; set; }
        public static ITrackingService TrackingService { get; set; }
        public static ISaveService SaveService { get; set; }
        public static ISaveData SaveData { get; set; }
        public static IUserService UserService { get; set; }
        public static IRandomService RandomService { get; set; }
        public static ITimeService TimeService { get; set; }
        
        public static ISettingsLogic SettingsLogic { get; set; }
        public static IFieldLogic FieldLogic { get; set; }
        public static IMoneyLogic MoneyLogic { get; set; }
        public static IDailyBonusLogic DailyBonusLogic { get; set; }
        public static IOfflineLogic OfflineLogic { get; set; }
        public static IUpgradeLogic UpgradeLogic { get; set; }
        public static IUiLogic UiLogic { get; set; }
        public static IGiftLogic GiftLogic { get; set; }

#endregion

#region MAIN SCOPE
        public static UiController UiController { get; set; }
        public static AppSignalsHandler SignalsHandler { get; set; }
#endregion

#region AUDIO

        public static void PlayMusic() => Play(Settings.Audio.Music);

        public static void PlayTap() => Play(Settings.Audio.Tap);
        public static void PlayMoneyReward() => Play(Settings.Audio.MoneyReward);
        public static void PlayMoneySpend() => Play(Settings.Audio.MoneySpend);
        public static void PlaySpawn() => Play(Settings.Audio.Spawn);

        public static void PlayMoveResult(MoveResult moveResult) {
            Play(Settings.Audio.Move);
            if (moveResult is MoveResult.Merge) {
                Play(Settings.Audio.Merge);
            }
        }

        private static void Play(AudioEvent audioEvent) {
            if (audioEvent != null) {
                AudioService.Play(audioEvent);
            }
        }

#endregion

#region ADS

        public static bool IsProcessingAds { get; private set; }

        private static bool _isAdsInitialized;

        public static void ShowRewardedVideo(AdsType adsType, Action onSucceed, Action onFailed = null) {
            if (IsProcessingAds) {
                return;
            }

            if (!_isAdsInitialized) {
                _isAdsInitialized = true;
                YG2.onCloseAnyAdv += OnAdvEnd;
                YG2.onErrorAnyAdv += OnAdvEnd;
            }

            IsProcessingAds = true;
            
            TrackingService.Track(TrackingConstants.SHOW_ADS, new Dictionary<string, object> {
                ["ads"] = adsType.ToString()
            });
            YG2.RewardedAdvShow(adsType.ToString(), () => {
                onSucceed?.Invoke();
                SaveService.Flush();
            });
        }

        private static void OnAdvEnd() {
            IsProcessingAds = false;
        } 

#endregion

        public static void TrackTechnicalStep(string stepName, Dictionary<string, object> parameters = null) {
            var trackData = new Dictionary<string, object> {
                [TrackingConstants.STEP_NAME] = stepName
            };
            trackData.Append(parameters);
            TrackingService.Track(TrackingConstants.TECHNICAL, trackData);
        }

        public static void ResetProgress() {
            SaveService.ResetSave();
            SaveService.Flush();
            SignalBus.Fire(new ResetProgressSignal());
        }

        public static void AskForRate() {
            YG2.ReviewShow();
        }
    }
}