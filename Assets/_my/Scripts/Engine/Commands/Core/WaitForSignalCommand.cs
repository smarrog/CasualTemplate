using System;
using Smr.Common;

namespace Smr.Commands {
    public class WaitForSignalCommand<T> : AbstractCommand {
        private readonly Func<T, bool> _signalPredicate;
        
        public WaitForSignalCommand(Func<T, bool> signalPredicate = null) {
            _signalPredicate = signalPredicate;
        }
        
        protected override void ExecuteInternal() {
            EngineDependencies.SignalBus.Subscribe<T>(OnSignal);
        }

        protected override void CleanUpInternal(CommandResult result) {
            base.CleanUpInternal(result);

            EngineDependencies.SignalBus.Unsubscribe<T>(OnSignal);
        }

        private void OnSignal(T signal) {
            var isExpectedSignal = _signalPredicate?.Invoke(signal) ?? true;
            if (isExpectedSignal) {
                NotifyComplete();
            }
        }
    }
}