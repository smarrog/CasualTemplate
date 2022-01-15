using System;
using Smr.Localization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game {
    public class GiftWindowData : AbstractWindowData {
        public int ElementIndex;
    }

    public class GiftWindow : AbstractWindow<GiftWindowData> {
        [SerializeField] private TMP_Text _descriptionLabel;
        [SerializeField] private Image _image;
        [SerializeField] private Sprite _spawnSpeedSprite;
        [SerializeField] private Button _refuseButton;
        [SerializeField] private Button _acceptButton;
        
        private GiftType _giftType;

        protected override void ConstructInternal() {
            _refuseButton.onClick.AddListener(OnRefuseButton);
            _acceptButton.onClick.AddListener(OnAcceptButton);
        }

        protected override void OnDataApplied() {
            _giftType = App.GiftLogic.GetRandomGiftType();
            
            _descriptionLabel.text = GetDescriptionText();
            
            _image.sprite = _giftType switch {
                GiftType.SpawnSpeed => _spawnSpeedSprite,
                GiftType.SpawnLevel => App.FieldLogic.GetLevelData(App.GiftLogic.GetSpawnLevelWithModification(App.FieldLogic.SpawnLevel, App.FieldLogic.MaxLevel)).Image,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        protected override void AfterHide() {
            base.AfterHide();
            App.FieldLogic.SetLevel(Data.ElementIndex, 0);
        }

        // пока пусть так будет, пока нет ключей на локализацию
        private string GetDescriptionText() {
            if (App.SettingsLogic.Localization == Localization.Russian) {
                return _giftType switch {
                    GiftType.SpawnSpeed => $"Слаймы будут появляться в {App.Settings.Meta.Gifts.SpawnSpeedMultiplier} раза быстрее в течение {App.Settings.Meta.Gifts.Duration} секунд",
                    GiftType.SpawnLevel => $"Уровень появляющихся слаймов будет выше на {App.Settings.Meta.Gifts.SpawnLevelBonus} в течение {App.Settings.Meta.Gifts.Duration} секунд",
                    _ => throw new ArgumentOutOfRangeException()
                };
            }
            
            return _giftType switch {
                GiftType.SpawnSpeed => $"New items will spawn {App.Settings.Meta.Gifts.SpawnSpeedMultiplier}x faster for {App.Settings.Meta.Gifts.Duration} seconds",
                GiftType.SpawnLevel => $"New items will spawn {App.Settings.Meta.Gifts.SpawnLevelBonus} level higher for {App.Settings.Meta.Gifts.Duration} seconds",
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private void OnRefuseButton() {
            if (App.IsProcessingAds) {
                return;
            }
            
            App.PlayTap();
            Close();
        }

        private void OnAcceptButton() {
            App.PlayTap();
            App.ShowRewardedVideo(AdsType.Gift, () => {
                App.GiftLogic.Apply(_giftType);
                Close();
            });
        }
    }
}