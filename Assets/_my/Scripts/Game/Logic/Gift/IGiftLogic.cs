namespace Game {
    public interface IGiftLogic {
        GiftType Active { get; }
        long RemainingTime { get; }

        void Apply(GiftType giftType);
        GiftType GetRandomGiftType();

        void SetAtField(bool value);
        bool CheckIfGiftCanBeSpawned();

        int GetSpawnLevelWithModification(int spawnLevel, int maxLevel);
    }
}