using Cysharp.Threading.Tasks;

namespace Game {
    public interface ISaveService {
        bool IsLoaded { get; }

        UniTask<bool> LoadAsync();
        UniTask FlushAsync();
        
        void Flush() => FlushAsync().Forget();

        void ResetSave();
    }
}