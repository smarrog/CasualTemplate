using System;

namespace Game {
    public class SpawnAdsBonus : AbstractAdsBonus {
        protected override AdsType AdsType => AdsType.Spawn;
        protected override bool IsMaxReached => false;
        
        private int SpawnLevel => Math.Min(App.FieldLogic.SpawnLevel + App.Settings.Meta.InstantBonusesForAds.SpawnLevelBonus, App.FieldLogic.MaxLevel);

        protected override void InitInternal() {
            var levelData = App.FieldLogic.GetLevelData(SpawnLevel);
            SetImage(levelData.Image);
            SetDescriptionValue(App.Settings.Meta.InstantBonusesForAds.SpawnAmount.ToString());
        }
        
        protected override void GiveBonus() {
            App.FieldLogic.TryToSpawnOrReplace(SpawnLevel, App.Settings.Meta.InstantBonusesForAds.SpawnAmount);
        }
    }
}