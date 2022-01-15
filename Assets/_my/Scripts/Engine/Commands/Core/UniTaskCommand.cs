using Cysharp.Threading.Tasks;

namespace Smr.Commands {
    public class UniTaskCommand : AbstractAsyncCommand {
        private readonly UniTask _task;
        
        public UniTaskCommand(UniTask task) {
            _task = task;
        }
        
        protected override async UniTask ExecuteInternalAsync() {
            await _task;
            NotifyComplete();
        }
    }
}