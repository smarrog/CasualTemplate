using Smr.Audio;
using Smr.Common;
using Smr.Components;
using Smr.Services;
using Smr.Tracking;
using UnityEngine.SceneManagement;
using VContainer;

namespace Game {
#if VCONTAINER_UNITASK_INTEGRATION
    public class PreloaderEntryPoint : IAsyncStartable {
#else
    public class PreloaderEntryPoint : VContainer.Unity.IInitializable {
#endif
        private readonly Preloader _preloader;
        
        [Preserve]
        public PreloaderEntryPoint(
            Preloader preloader,
            ISettings settings,
            IAudioService audioService,
            IScheduler scheduler,
            ITrackingService trackingService,
            ILogService logger,
            ISignalBus signalBus,
            IRandomService randomService,
            ISaveService saveService,
            ISaveData saveData,
            IUserService userService,
            IUiLogic uiLogic,
            ITimeService timeService,
            ISettingsLogic settingsLogic,
            IGiftLogic giftLogic,
            IFieldLogic fieldLogic,
            IMoneyLogic moneyLogic,
            IDailyBonusLogic dailyBonusLogic,
            IOfflineLogic offlineLogic,
            IUpgradeLogic upgradeLogic
        ) {
            _preloader = preloader;
            
            App.Settings = settings;
            App.AudioService = audioService;
            App.Scheduler = scheduler;

            App.Logger = logger;
            App.SignalBus = signalBus;
            App.TrackingService = trackingService;
            App.RandomService = randomService;
            App.SaveService = saveService;
            App.SaveData = saveData;
            App.UserService = userService;
            App.TimeService = timeService;
            App.SettingsLogic = settingsLogic;

            App.UiLogic = uiLogic;
            App.GiftLogic = giftLogic;
            App.FieldLogic = fieldLogic;
            App.MoneyLogic = moneyLogic;
            App.DailyBonusLogic = dailyBonusLogic;
            App.OfflineLogic = offlineLogic;
            App.UpgradeLogic = upgradeLogic;
        }

#if VCONTAINER_UNITASK_INTEGRATION
        public async Cysharp.Threading.Tasks.UniTask StartAsync(System.Threading.CancellationToken cancellation) {
#else
        public void Initialize() {
#endif
            App.TrackTechnicalStep("01_start_game_initialization");
            
            var cmd = new InitializeGameCommand();
            cmd.OnComplete += _ => OnGameInitializationFinished();
            cmd.Execute();
        }

        private void OnGameInitializationFinished() {
            App.TrackTechnicalStep("02_game_initialization_finished");

            App.IsReady = true;
            
            if (!App.Settings.Preloader.WaitFotContinue) {
                StartMainScene();
                return;
            }
            
            _preloader.OnContinueButtonPressed += OnContinueButtonPressed;
            _preloader.ShowFinalState();
        }

        private void StartMainScene() {
            App.TrackTechnicalStep("03_start_main_scene");
                
            SceneManager.LoadScene(App.Settings.Preloader.MainSceneIndex);
        }

        private void OnContinueButtonPressed() {
            _preloader.OnContinueButtonPressed -= StartMainScene;
            StartMainScene();
        }
    }
}