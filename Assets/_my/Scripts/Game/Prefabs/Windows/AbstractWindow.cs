using System;
using Smr.Ui;
using Smr.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Game {
    public interface IWindow {
        void Close(Action onComplete = null);
    }
    
    public abstract class AbstractWindow<TWindowData> : AbstractUiElement<TWindowData>, IWindow where TWindowData : AbstractWindowData {
        public void Open(TWindowData data) {
            ApplyData(data);
            SetVisibilityState(VisibilityState.Appearing);
        }

        public void Close(Action onComplete = null) {
            SetVisibilityState(VisibilityState.Hiding, onComplete);
        }

        protected override void AfterHide() {
            base.AfterHide();
            
            App.SignalBus.Fire(new WindowHiddenSignal(this));
        }
    }
}