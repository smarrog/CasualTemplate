namespace Game {
    public interface IFieldLogic : IInitializable {
        int MaxLevel { get; }
        int MaxOpenedLevel { get; }
        int SlotsAmount { get; }
        float TimeFromLastSpawn { get; }
        float SpawnInterval { get; }
        bool CanUnlockSlot { get; }
        int SpawnLevel { get; }
        int CurrentDiscountPercent { get; }

        void SetLevel(int index, int value);
        SlotInfo GetSlotInfo(int index);
        void UnlockSlot();
        LevelData GetLevelData(int level);
        bool CanMoveTo(int from, int to);
        MoveResult MoveTo(int from, int to);
        void AccelerateSpawn(float value);
        void TryToSpawnOrReplace(int levelToSpawn, int amount);
        bool TryToSpawn(out int spawnIndex);
    }
}