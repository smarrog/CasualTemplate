using TMPro;
using UnityEngine;

namespace Game {
    public class DiscountAdsBonus : AbstractAdsBonus {
        [SerializeField] private TMP_Text _discountLabel;

        protected override AdsType AdsType => AdsType.Discount;
        protected override bool IsMaxReached => App.FieldLogic.CurrentDiscountPercent >= App.Settings.Meta.Money.MaxDiscount;
        
        protected override void InitInternal() {
            UpdateDiscountLabel();
            SetDescriptionValue(App.Settings.Meta.InstantBonusesForAds.DiscountBonusIncrement.ToString());
        }
        
        protected override void GiveBonus() {
            App.MoneyLogic.AddDiscount(App.Settings.Meta.InstantBonusesForAds.DiscountBonusIncrement);
            UpdateDiscountLabel();
        }

        private void UpdateDiscountLabel() {
            _discountLabel.text = $"{App.FieldLogic.CurrentDiscountPercent}%";
        }
    }
}