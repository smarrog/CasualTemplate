using Smr.Localization;
using Smr.Ui;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game {
    public class AlbumWindowData : AbstractWindowData {
        public int Level;
        public bool IsNew;
    }
    
    public class AlbumWindow : AbstractWindow<AlbumWindowData> {
        [SerializeField] private SimpleScroller _simpleScroller;
        [SerializeField] private Image _image;
        [SerializeField] private Image _lock;
        [SerializeField] private TMP_Text _newLabel;
        [SerializeField] private TMP_Text _nameLabel;
        [SerializeField] private TMP_Text _progressLabel;
        [SerializeField] private GameObject _backAnimationObject;
        
        protected override void ConstructInternal() {
            base.ConstructInternal();
            
            _simpleScroller.Init(App.FieldLogic.MaxLevel, OnIndexSelected);
            _simpleScroller.OnBtnTapped += OnBtnTapped;
        }
        
        protected override void OnDataApplied() {
            _simpleScroller.SetIndex(Data.Level - 1);
            if (_newLabel) {
                _newLabel.gameObject.SetActive(Data.IsNew);
            }
        }

        private void OnBtnTapped() {
            App.PlayTap();
        }

        private void OnIndexSelected(int index) {
            var level = index + 1;
            var isLocked = App.FieldLogic.MaxOpenedLevel < level;
            _lock.enabled = isLocked;
            _image.enabled = !isLocked;
            
            _backAnimationObject.SetActive(!isLocked);
            var levelData = isLocked ? null : App.FieldLogic.GetLevelData(level);
            _nameLabel.text = GetTitle(isLocked, levelData);
            _progressLabel.text = $"{level} / {App.FieldLogic.MaxLevel}";
            _image.sprite = levelData?.Image;
            
            if (_newLabel) {
                _newLabel.gameObject.SetActive(false);
            }
        }

        private string GetTitle(bool isLocked, LevelData levelData) {
            // подписываться на переключение языка нет смысла, так как его нельзя сменить не закрыв это
            var isRussian = App.SettingsLogic.Localization == Localization.Russian;
            if (isLocked) {
                return isRussian ? "ЗАКРЫТО" : "LOCKED";
            }
            var title = isRussian ? levelData.TitleRu : levelData.Title;
            return title?.ToUpper();
        }
    }
}