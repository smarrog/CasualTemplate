namespace Game {
    // it's just for readonlyu purpose
    public class SlotInfo {
        public int Level { get; }
        public bool IsLocked { get; }

        public bool IsGift => Level == FieldLogic.GIFT_LEVEL;
        public bool IsEmpty => Level == 0;
        public bool IsFilledWithElement => !IsLocked && !IsEmpty && !IsGift;

        public SlotInfo(int level, bool isLocked) {
            Level = level;
            IsLocked = isLocked;
        }
    }
}