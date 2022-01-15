using System.Numerics;
using UnityEngine;

namespace Game {
    public class MoneyAdsBonus : AbstractAdsBonus {
        [SerializeField] private MoneyElement _moneyElement;

        protected override AdsType AdsType => AdsType.Money;
        protected override bool IsMaxReached => false;

        private BigInteger _reward;
        
        protected override void InitInternal() {
            _reward = App.MoneyLogic.Income * App.Settings.Meta.InstantBonusesForAds.MoneyIncomeInSeconds;
            _moneyElement.SetValue(_reward);
        }
        
        protected override void GiveBonus() {
            App.MoneyLogic.AddMoney(_reward);
            App.PlayMoneyReward();
        }
    }
}