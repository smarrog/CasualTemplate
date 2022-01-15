using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Cysharp.Threading.Tasks;
using Smr.Components;
using Smr.Files;
using UnityEngine;
using VContainer;

namespace Game {
    public class LocalSaveService : ISaveService {
        private static string FilePath => Path.Combine(Application.persistentDataPath, "save.bin");
        
        public bool IsLoaded => _saveData.Data != null;
        
        private readonly IFilesService _filesService;
        private readonly ISaveData _saveData;
        private readonly IScheduler _scheduler;

        [Preserve]
        public LocalSaveService(IFilesService filesService, ISaveData saveData, IScheduler scheduler) {
            _filesService = filesService;
            _saveData = saveData;
        }
        
        public UniTask<bool> LoadAsync() {
            if (IsLoaded) {
                return UniTask.FromResult(IsLoaded);
            }

            // empty save
            if (!_filesService.LoadBytesFile(FilePath, out var bytes)) {
                _saveData.Data.Reset();
                return UniTask.FromResult(IsLoaded);
            }
            
            try {
                using var stream = new MemoryStream(bytes);
                var bf = new BinaryFormatter();
                _saveData.Data = (SaveData)bf.Deserialize(stream);
                return UniTask.FromResult(IsLoaded);
            } catch (Exception) {
                return UniTask.FromResult(IsLoaded);
            }
        }
        
        public UniTask FlushAsync() {
            if (!IsLoaded) {
                return UniTask.FromCanceled();
            }
            
            using var stream = new MemoryStream();
            var bf = new BinaryFormatter();
            bf.Serialize(stream, _saveData.Data);
            _filesService.WriteAllBytesSafe(FilePath, stream.ToArray());
            return UniTask.CompletedTask;
        }
        
        public void ResetSave() {
            _saveData.Data.Reset();
        }
    }
}