using System;
using Cysharp.Threading.Tasks;
using Smr.Common;
using UnityEngine;

namespace Smr.Ui {
    public abstract class AbstractUiVisibilityComponent : MonoBehaviour, IVisibility { // just for serialization
        public VisibilityState VisualState { get; protected set; } = VisibilityState.Unknown;
        public abstract void SetVisibilityState(VisibilityState state, Action onComplete = null);
        public abstract void SetVisibility(bool isVisible);
        public abstract void SetVisibilityAnimated(bool isVisible, Action onAnimationComplete = null);
        public abstract void Clear();

        public UniTask SetVisibilityAnimatedAsync(bool isVisible) {
            var completionSource = new UniTaskCompletionSource();
            completionSource.Task.Forget(HandleAnimationException);
            
            void OnComplete() {
                completionSource.TrySetResult();
            }
            
            SetVisibilityAnimated(isVisible, OnComplete);
            return completionSource.Task;
        }
        
        private static void HandleAnimationException(Exception exception) {
            EngineDependencies.Logger.LogError(exception);
        }
    }
}