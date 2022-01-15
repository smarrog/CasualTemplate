using Smr.Animations;
using Smr.Common;
using Smr.Extensions;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;
#if UNITY_EDITOR
using System;
using NSubstitute;
using Smr.Editor;
#endif

namespace Game {
    public class GridElement : MonoBehaviour {
#if UNITY_EDITOR
        [Header("Test")]
        [SerializeField] private int _levelTest;
        [SerializeField] private bool _isLockedTest;
        [SerializeField] private bool _isHighlightedTest;
        [SerializeField] private bool _isSelectedTest;

        public void OnValidate() {
            if (Application.isPlaying) {
                return;
            }

            if (App.FieldLogic == null) {
                App.Settings = ScriptableObjectUtility.GetFirst<Settings>();
                App.FieldLogic = new FieldLogic(
                    Substitute.For<IGiftLogic>(),
                    App.Settings,
                    Substitute.For<ISaveData>(),
                    Substitute.For<ISignalBus>());
            }

            _level = Math.Min(App.Settings.Meta.Field.Levels.Count, _levelTest);
            _levelData = App.FieldLogic.GetLevelData(_levelTest);
            _isLocked = _isLockedTest;
            _isHighlighted = _isHighlightedTest;
            _isSelected = _isSelectedTest;

            UpdateAllVisual();
        }
#endif

        [Header("Parameters")]
        [SerializeField] private Image _itemImage;
        [SerializeField] private Image _emptyImage;
        [SerializeField] private GameObject _lockObject;
        [SerializeField] private GameObject _giftObject;
        [SerializeField] private GridElementMoney _moneyElement;
        [SerializeField] private PlayableDirector _breathingAnimation;

        [Header("Back")]
        [SerializeField] private Image _backImage;
        [SerializeField] private Color _normalColor;
        [SerializeField] private Color _highlightedColor;
        [SerializeField] private Color _disabledColor;

        public int Index { get; private set; }

        private int _level = -1;
        private LevelData _levelData;
        private bool _isLocked;
        private bool _isSelected;
        private bool _isHighlighted;

        private float _timeFromLastPay; // it is needed only here, so not in logic

        public Sprite CurrentSprite => _itemImage.sprite;

        public void UpdateHandler(float deltaTime) {
            if (_level > 0 && !_isLocked) {
                _timeFromLastPay += deltaTime;
                PayIfNeed();
            }
        }

        public void Init(int index) {
            Index = index;
            UpdateSlotInfo();
            
            UpdateAllVisual();
            _moneyElement.Init();
        }

        public void UpdateSlotInfo() {
            var slotInfo = App.FieldLogic.GetSlotInfo(Index);
            if (slotInfo.Level == _level && slotInfo.IsLocked == _isLocked) {
                return;
            }

            _timeFromLastPay = 0;
            _level = slotInfo.Level;
            _isLocked = slotInfo.IsLocked;
            
            UpdateEmptyVisual();
            UpdateLevelVisual();
            UpdateGiftVisual();
            UpdateIsLockedVisual();
        }

        public void SetSelected(bool value) {
            _isSelected = value;
            UpdateSelectedVisual();
        }

        public void SetIsHighlighted(bool value) {
            _isHighlighted = value;
            UpdateHighlightedVisual();
        }

        private void UpdateAllVisual() {
            UpdateLevelVisual();
            UpdateHighlightedVisual();
            UpdateSelectedVisual();
            UpdateIsLockedVisual();
            UpdateEmptyVisual();
            UpdateGiftVisual();
        }

        private void UpdateLevelVisual() {
            _levelData = _level > 0 && !_isLocked ? App.FieldLogic.GetLevelData(_level) : null;
           
            _itemImage.sprite = _levelData?.Image;
            _itemImage.enabled = _levelData != null;
        }

        private void UpdateEmptyVisual() {
            _emptyImage.enabled = !_isLocked && _level == 0;
        }

        private void UpdateIsLockedVisual() {
            if (_lockObject) {
                _lockObject.SetActive(_isLocked);
            }
            UpdateBackColor();
        }

        private void UpdateGiftVisual() {
            if (_giftObject) {
                _giftObject.SetActive(!_isLocked && _level == FieldLogic.GIFT_LEVEL);
            }
        }

        private void UpdateSelectedVisual() {
            _itemImage.color = _itemImage.color.WithA(_isSelected ? 0.5f : 1f);
            UpdateBackColor();
        }

        private void UpdateHighlightedVisual() {
            UpdateBackColor();
        }

        private void UpdateBackColor() {
            _backImage.color = _isLocked || _isSelected
                ? _disabledColor
                : _isHighlighted
                    ? _highlightedColor
                    : _normalColor;
        }

        private void PayIfNeed() {
            if (_timeFromLastPay < App.MoneyLogic.PayInterval) {
                return;
            }

            _timeFromLastPay = 0;
            var receivedAmount = App.MoneyLogic.ReceivePayFor(_level);
            _moneyElement.Show(receivedAmount);

            _breathingAnimation?.PlayExt();
        }
    }
}