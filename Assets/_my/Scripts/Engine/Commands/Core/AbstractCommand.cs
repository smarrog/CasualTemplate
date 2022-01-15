using System;
using Cysharp.Threading.Tasks;
using Smr.Common;
using Smr.Utils;

namespace Smr.Commands {
    public enum CommandResult {
        Completed,
        Failed,
        Timeout,
        Stopped
    }

    public abstract class AbstractCommand {
        private readonly UniTaskCompletionSource<CommandResult> _promise = new();
        private Timer _timeoutTimer;
        
        public event Action<AbstractCommand> OnComplete;
        public event Action<AbstractCommand> OnSucceed;

        protected bool HasResult => _promise.Task.Status != UniTaskStatus.Pending;

        public UniTask<CommandResult> Execute() {
            _timeoutTimer?.Start();
            EngineDependencies.Logger?.Log($"Start command execution: {this}");
            ExecuteInternal();
            return _promise.Task;
        }

        public override string ToString() {
            return GetType().ToString();
        }

        public void Terminate() {
            SetResult(CommandResult.Stopped);
        }

        public AbstractCommand SetTimeout(int milliseconds) {
            _timeoutTimer = milliseconds > 0 ? new Timer(milliseconds, () => {
                SetResult(CommandResult.Timeout);
            }) : null;
            return this;
        }

        protected abstract void ExecuteInternal();
        protected virtual void CleanUpInternal(CommandResult result) {}

        protected void NotifyComplete() {
            SetResult(CommandResult.Completed);
        }

        protected void NotifyFail() {
            SetResult(CommandResult.Failed);
        }

        private void SetResult(CommandResult result) {
            _timeoutTimer?.Stop();
            OnComplete?.Invoke(this);
            if (result == CommandResult.Completed) {
                OnSucceed?.Invoke(this);
            }
            CleanUpInternal(result);
            _promise.TrySetResult(result);
        }
    }
}