using Cysharp.Threading.Tasks;
using UnityEngine.Scripting;
using YG;

namespace Game {
    public class Yg2SaveService : ISaveService {
        public bool IsLoaded => _saveData.Data != null;
        
        private readonly ISaveData _saveData;

        [Preserve]
        public Yg2SaveService(ISaveData saveData) {
            _saveData = saveData;
        }
        
        public UniTask<bool> LoadAsync() {
            _saveData.Data = YG2.saves.Data;
            return UniTask.FromResult(true);
        }

        public async UniTask FlushAsync() {
            YG2.SaveProgress();
            await UniTask.Delay(500);
        }
        public void ResetSave() {
            _saveData.Data.Reset();
        }
    }
}