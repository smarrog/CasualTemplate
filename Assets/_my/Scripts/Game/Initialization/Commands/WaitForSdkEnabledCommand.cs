using Cysharp.Threading.Tasks;
using Smr.Commands;
using YG;

namespace Game {
    public class WaitForSdkEnabledCommand : AbstractAsyncCommand {
        protected override async UniTask ExecuteInternalAsync() {
            await UniTask.WaitUntil(() => YG2.isSDKEnabled);
            NotifyComplete();
        }
    }
}