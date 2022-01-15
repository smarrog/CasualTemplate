using Cysharp.Threading.Tasks;
using Smr.Commands;

namespace Game {
    public class LoadSaveCommand : AbstractAsyncCommand {
        protected override async UniTask ExecuteInternalAsync() {
            var result = await App.SaveService.LoadAsync();
            if (!result) {
                App.Logger.LogError("Failed to load save");
                NotifyFail();
                return;
            }
            NotifyComplete();
        }
    }
}