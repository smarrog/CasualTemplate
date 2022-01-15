using Smr.Extensions;
using VContainer;

namespace Game {
    public class MainEntryPoint : VContainer.Unity.IInitializable {
        private readonly Field _field;
        
        [Preserve]
        public MainEntryPoint(
            UiController uiController,
            AppSignalsHandler signalsHandler,
            Field field
        ) {
            _field = field;
            App.UiController = uiController;
            App.SignalsHandler = signalsHandler;
        }
        
        public void Initialize() {
            App.TrackTechnicalStep("04_main_entry_point");
            
            App.Logger.Log("Build version: " + App.Settings.System.Version);

            App.UiController.HideAllWindows();

            InitLogics();
            
            App.PlayMusic();
        }
        
        private void InitLogics() {
            ApplyDebugSettings();
            
            _field.Init();
            DiFacade.InitializeInstances();
            App.SignalsHandler.Initialize();

            if (App.Settings.System.SaveInterval > 0) {
                App.Scheduler.DoEvery(App.Settings.System.SaveInterval, App.SaveService.Flush);
            }

            // logic at start
            if (!App.IsFirstSession) {
                App.OfflineLogic.CheckOfflineReward();
            } else if (App.FieldLogic.MaxOpenedLevel <= 1) {
                App.UiController.ShowTutorial();
            }
        }

        private void ApplyDebugSettings() {
            var debug = App.Settings.Debug;
            if (debug.ClearAtStart) {
                App.SaveService.ResetSave();
            }
            
            App.Settings.Meta.Money.IsEverythingFree = debug.IsEverythingFree;
            
            if (!debug.UseStabData) {
                return;
            }

            var data = App.SaveData;
            data.Money = debug.Money;
            data.UnlockedAmount = debug.UnlocksUpgradeValue;
            data.SetUpgradeLevel(UpgradeType.SpawnSpeed, debug.SpawnSpeedUpgradeValue);
            data.SetUpgradeLevel(UpgradeType.SpawnLevel, debug.SpawnLevelUpgradeValue);
            data.SetUpgradeLevel(UpgradeType.IncomeMultiplier, debug.IncomeMultiplierUpgradeValue);
            data.DailyBonusStreak = debug.DailyBonusStreak;
            data.DailyBonusLastDay = debug.DailyBonusDay;
            data.LastOnlineTimestamp = debug.LastOnlineTimeStamp;
            data.DiscountPercent = debug.DiscountPercent;
            
            for (var i = 0; i < App.Settings.Meta.Field.SlotsAmount; ++i) {
                data.SetElementLevel(i, debug.Levels.GetAtOrDefault(i));
            }
        }
    }
}