using System;
using Cysharp.Threading.Tasks;

namespace Smr.Commands {
    public class DelayCommand : AbstractAsyncCommand {
        private readonly float _delayInSeconds;
        private readonly Action _action;

        public DelayCommand(float delayInSeconds, Action action = null) {
            _delayInSeconds = delayInSeconds;
            _action = action;
        }
        
        protected override async UniTask ExecuteInternalAsync() {
            if (_delayInSeconds > 0) {
                var delayInMilliseconds = (int)(_delayInSeconds * 1000);
                await UniTask.Delay(delayInMilliseconds, cancellationToken : CancellationToken);
            }
            
            _action?.Invoke();
            NotifyComplete();
        }
    }
}