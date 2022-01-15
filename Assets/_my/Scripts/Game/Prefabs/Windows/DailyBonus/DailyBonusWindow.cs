using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game {
    public class DailyBonusWindowData : AbstractWindowData {
        
    }
    
    public class DailyBonusWindow : AbstractWindow<DailyBonusWindowData> {
        [SerializeField] private List<DailyBonusWindowElement> _elements;
        [SerializeField] private GameObject _isNotTakenObject;
        [SerializeField] private GameObject _isTakenObject;
        [SerializeField] private Button _exitButton;
        [SerializeField] private Button _takeButton;
        [SerializeField] private Button _adsButton;
        [SerializeField] private TMP_Text _adsMultiplier;
        
        protected override void ConstructInternal() {
            base.ConstructInternal();
            
            _adsMultiplier.text = $"x{App.Settings.Meta.DailyBonus.AdsMultiplier}";
            
            _takeButton.onClick.AddListener(OnTakeButton);
            _exitButton.onClick.AddListener(OnExitButton);
            _adsButton.onClick.AddListener(OnAdsButton);
        }

        protected override void OnDataApplied() {
            var isTaken = App.DailyBonusLogic.IsCurrentTaken;
            _isNotTakenObject.SetActive(!isTaken);
            _isTakenObject.SetActive(isTaken);
            
            for (var day = 1; day <= App.Settings.Meta.DailyBonus.Days.Count; ++day) {
                _elements[day - 1].Init(day);
            }
        }

        private void OnAdsButton() {
            App.PlayTap();
            App.ShowRewardedVideo(AdsType.DoubleDailyBonusReward, () => {
                TakeBonus(App.Settings.Meta.DailyBonus.AdsMultiplier);
            });
        }

        private void OnTakeButton() {
            if (App.IsProcessingAds) {
                return;
            }
            
            App.PlayTap();
            TakeBonus();
        }

        private void OnExitButton() {
            App.PlayTap();
            Close();
        }

        private void TakeBonus(int multiplier = 1) {
            App.DailyBonusLogic.TakeDailyBonus(multiplier);
            App.PlayMoneyReward();
            Close();
        }
    }

}