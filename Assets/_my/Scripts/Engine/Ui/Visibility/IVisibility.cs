using System;

namespace Smr.Ui {
    public interface IVisibility {
        VisibilityState VisualState { get; }
        void SetVisibilityState(VisibilityState state, Action onComplete = null);
    }
}