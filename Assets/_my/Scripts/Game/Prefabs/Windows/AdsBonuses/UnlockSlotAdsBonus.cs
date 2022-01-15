namespace Game {
    public class UnlockSlotAdsBonus : AbstractAdsBonus {
        protected override AdsType AdsType => AdsType.UnlockSlot;
        protected override bool IsMaxReached => !App.FieldLogic.CanUnlockSlot;
        
        protected override void InitInternal() {
            
        }
        
        protected override void GiveBonus() {
            App.FieldLogic.UnlockSlot();
        }
    }
}