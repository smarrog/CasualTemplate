using Smr.Common;
using Smr.Components;
using Smr.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game {
    public class GiftIndicator : SimpleUiElement {
        [SerializeField] private Image _icon;
        [SerializeField] private Sprite _spawnSpeedSprite;
        [SerializeField] private Sprite _spawnLevelSprite;
        [SerializeField] private TMP_Text _secondsLabel;
        
        private SchedulerKey _schedulerKey;
        private ISignalBusSubscription _updateLevelSubscription;

        protected override void ConstructInternal() {
            base.ConstructInternal();
            
            UpdateStatus();

            AddSubscription<GiftStartedSignal>(OnGiftStartedSignal);
        }

        protected override void OnDestroy() {
            base.OnDestroy();
            
            ClearScheduler();
        }

        private void UpdateStatus() {
            var activeGift = App.GiftLogic.Active;
            if (activeGift == GiftType.Unknown) {
                gameObject.SetActive(false);
                RemoveSubscription(_updateLevelSubscription);
                ClearScheduler();
                return;
            }
            
            _secondsLabel.text = App.GiftLogic.RemainingTime.ToString();

            if (_schedulerKey != null) {
                return;
            }
            
            gameObject.SetActive(true);
            UpdateIcon();

            if (App.GiftLogic.Active == GiftType.SpawnLevel) {
                _updateLevelSubscription = AddSubscription<UpgradeLevelChangedSignal>(OnUpgradeLevelChangedSignal);
            } else {
                RemoveSubscription(_updateLevelSubscription);
            }

            _schedulerKey = App.Scheduler.DoEvery(0.3f, UpdateStatus);
        }

        private void ClearScheduler() {
            App.Scheduler?.Stop(_schedulerKey);
            _schedulerKey = null;
        }

        private void UpdateIcon() {
            _icon.sprite = App.GiftLogic.Active switch {
                GiftType.SpawnSpeed => _spawnSpeedSprite,
                GiftType.SpawnLevel => App.FieldLogic.GetLevelData(App.GiftLogic.GetSpawnLevelWithModification(App.FieldLogic.SpawnLevel, App.FieldLogic.MaxLevel)).Image,
                _ => null
            };
        }

        private void OnGiftStartedSignal(GiftStartedSignal signal) {
            UpdateStatus();
        }

        private void OnUpgradeLevelChangedSignal(UpgradeLevelChangedSignal signal) {
            UpdateIcon();
        }
    }
}