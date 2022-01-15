using System.Threading;
using Cysharp.Threading.Tasks;
using Smr.Common;
using Smr.Extensions;

namespace Smr.Commands {
    public abstract class AbstractAsyncCommand : AbstractCommand {
        private readonly CancellationTokenSource _cancellationTokenSource = new();

        protected CancellationToken CancellationToken => _cancellationTokenSource.Token;

        protected sealed override void ExecuteInternal() {
            ExecuteInternalAsync().HandleException(exception => {
                EngineDependencies.Logger.LogError(exception);
                NotifyFail();
            });
        }

        protected abstract UniTask ExecuteInternalAsync();

        protected override void CleanUpInternal(CommandResult result) {
            base.CleanUpInternal(result);
            _cancellationTokenSource.Cancel();
        }
    }
}