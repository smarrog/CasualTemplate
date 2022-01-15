using System.Numerics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game {
    public class OfflineBonusWindowData : AbstractWindowData {
        public long OfflineTime;
        public BigInteger Reward;
    }
    
    public class OfflineBonusWindow : AbstractWindow<OfflineBonusWindowData> {
        [SerializeField] private MoneyElement _moneyElement;
        [SerializeField] private Button _takeRewardButton;
        [SerializeField] private Button _adsButton;
        [SerializeField] private TMP_Text _adsMultiplier; 
        
        private BigInteger _rewardToGive;
        
        protected override void ConstructInternal() {
            base.ConstructInternal();

            _adsMultiplier.text = $"x{App.Settings.Meta.Offline.AdsMultiplier}";
            
            _takeRewardButton.onClick.AddListener(OnTakeRewardButton);
            _adsButton.onClick.AddListener(OnAdsButton);
        }

        protected override void OnDataApplied() {
            _rewardToGive = Data.Reward;
            
            _moneyElement.SetValue(_rewardToGive);
        }

        protected override void AfterHide() {
            App.OfflineLogic.GiveOfflineReward(Data.OfflineTime, _rewardToGive);
            App.PlayMoneyReward();
            
            base.AfterHide();
        }

        private void OnTakeRewardButton() {
            if (App.IsProcessingAds) {
                return;
            }
            
            App.PlayTap();            
            Close();
        }

        private void OnAdsButton() {
            App.PlayTap();            
            App.ShowRewardedVideo(AdsType.DoubleOfflineReward, OnAdsShownSucceed, OnAdsShownFailed);
        }

        private void OnAdsShownSucceed() {
            _rewardToGive *= App.Settings.Meta.Offline.AdsMultiplier;
            Close();
        }

        private void OnAdsShownFailed() {
            Close();
        }
    }
}