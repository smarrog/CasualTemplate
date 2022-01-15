using Smr.Audio;
using Smr.Common;
using Smr.Components;
using Smr.Files;
using Smr.Localization;
using Smr.Services;
using Smr.Signals;
using Smr.Tracking;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Game {
    public class PreloaderScope : LifetimeScope {
        [SerializeField] private Preloader _preloader;
        [SerializeField] private Settings _settings;
        [SerializeField] private AudioService _audioService;
        [SerializeField] private Scheduler _scheduler;
        
        protected override void Configure(IContainerBuilder builder) {
            // Controllers - have unity components
            // Services - does not depend on game
            // Logic - depends on game
            
            builder.RegisterEntryPoint<PreloaderEntryPoint>();

            builder.RegisterComponent(_preloader).AsSelf();
            
            builder.RegisterComponent(_settings).AsImplementedInterfaces();
            builder.RegisterComponent(_audioService).AsImplementedInterfaces();
            builder.RegisterComponent(_scheduler).AsImplementedInterfaces();
            
            // various services
            builder.Register<GameLogger>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.RegisterInstance(SignalBus.GetBus("app")).AsImplementedInterfaces();
            builder.Register<LocalizationService>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<SaveDataWrapper>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<RandomService>(Lifetime.Singleton).AsImplementedInterfaces().WithParameter(42);
            builder.Register<TimeService>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<Yg2SaveService>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<Yg2UserService>(Lifetime.Singleton).AsImplementedInterfaces();
            
            // files
            builder.Register<DiskFiles>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<DiskDirectories>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<FilesService>(Lifetime.Singleton).AsImplementedInterfaces();
            
            // tracking
            builder.Register<TrackingService>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<AppMetricaTracker>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<Yg2Tracker>(Lifetime.Singleton).AsImplementedInterfaces();
            
            // game logic
            builder.Register<UiLogic>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<SettingsLogic>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<GiftLogic>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<FieldLogic>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<MoneyLogic>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<DailyBonusLogic>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<OfflineLogic>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<UpgradeLogic>(Lifetime.Singleton).AsImplementedInterfaces();
            
            // singletons
            builder.Register<EngineDependencies>(Lifetime.Singleton).AsSelf();
            builder.Register<DiFacade>(Lifetime.Singleton).AsSelf();
            builder.RegisterBuildCallback(container => {
                container.Resolve<EngineDependencies>();
                container.Resolve<DiFacade>();
            });
        }
    }
}