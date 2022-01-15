using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Game {
    public class MainScope : LifetimeScope {
        [SerializeField] private UiController _uiController;
        [SerializeField] private AppSignalsHandler _signalsHandler;
        [SerializeField] private Field _field;

        protected override void Configure(IContainerBuilder builder) {
            builder.RegisterEntryPoint<MainEntryPoint>();
            
            builder.RegisterComponent(_uiController).AsSelf();
            builder.RegisterComponent(_signalsHandler).AsSelf();
            builder.RegisterComponent(_field).AsSelf();
        }
    }
}