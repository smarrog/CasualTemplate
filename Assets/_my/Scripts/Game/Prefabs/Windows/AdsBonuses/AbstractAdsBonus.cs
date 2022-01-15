using System;
using Smr.Localization;
using UnityEngine;
using UnityEngine.UI;

namespace Game {
    public abstract class AbstractAdsBonus : MonoBehaviour {
        [SerializeField] private GameObject _maxReachedObject;
        [SerializeField] private Button _adsButton;
        [SerializeField] private TextLocalizationComponent _descriptionLocalizationComponent;
        [SerializeField] private Image _image;

        protected abstract AdsType AdsType { get; }
        protected abstract bool IsMaxReached { get; }
        
        private Action _onBonusGiven;

        private void Awake() {
            _adsButton.onClick.AddListener(OnAdsButton);
        }

        public void Init(Action onBonusGiven) {
            _onBonusGiven = onBonusGiven;
            InitInternal();

            UpdateVisual();
        }

        public void UpdateVisual() {
            _maxReachedObject.SetActive(IsMaxReached);
            _adsButton.gameObject.SetActive(!IsMaxReached);
        }

        protected void SetDescriptionValue(string value) {
            _descriptionLocalizationComponent.SetValue(value);
        }

        protected void SetImage(Sprite value) {
            _image.sprite = value;
        }

        protected abstract void InitInternal();
        protected abstract void GiveBonus();

        private void OnAdsButton() {
            if (IsMaxReached) {
                return;
            }
            
            App.PlayTap();
            App.ShowRewardedVideo(AdsType, OnAdsShownSucceed, OnAdsShownFailed);
        }

        private void OnAdsShownSucceed() {
            GiveBonus();
            _onBonusGiven?.Invoke();
        }

        private void OnAdsShownFailed() {
            
        }
    }
}