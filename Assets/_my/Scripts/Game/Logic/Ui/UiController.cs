using Smr.Animations;
using Smr.Ui;
using UnityEngine;
using UnityEngine.Playables;

namespace Game {
    public class UiController : MonoBehaviour {
        [SerializeField] private PlayableDirector _mergeSlimesTutorialDirector;
        [Space]
        [SerializeField] private AdsWindow _adsWindow;
        [SerializeField] private UpgradesWindow _upgradesWindow;
        [SerializeField] private DailyBonusWindow _dailyBonusWindow;
        [SerializeField] private AlbumWindow _albumWindow;
        [SerializeField] private OfflineBonusWindow _offlineBonusWindow;
        [SerializeField] private SettingsWindow _settingsWindow;
        [SerializeField] private GiftWindow _giftWindow;
        
        private IWindow _openedWindow;

        private void Awake() {
            App.SignalBus.Subscribe<WindowHiddenSignal>(OnWindowHiddenSignal);
        }

        public void HideAllWindows() {
            _mergeSlimesTutorialDirector.gameObject.SetActive(false);
            
            _adsWindow.gameObject.SetActive(false);
            _upgradesWindow.gameObject.SetActive(false);
            _dailyBonusWindow.gameObject.SetActive(false);
            _albumWindow.gameObject.SetActive(false);
            _offlineBonusWindow.gameObject.SetActive(false);
            _settingsWindow.gameObject.SetActive(false);
        }

        public void ShowTutorial() {
            _mergeSlimesTutorialDirector.gameObject.SetActive(true);
            
            _mergeSlimesTutorialDirector.PlayExt(() => {
                _mergeSlimesTutorialDirector.gameObject.SetActive(false);
            });
        }

        public void ShowAdsWindow(AdsWindowData data) {
            OpenWindow(_adsWindow, data);
        }

        public void ShowUpgradesWindow(UpgradesWindowData data) {
            OpenWindow(_upgradesWindow, data);
        }

        public void ShowDailyBonusWindow(DailyBonusWindowData dailyBonusWindowData) {
            OpenWindow(_dailyBonusWindow, dailyBonusWindowData);
        }

        public void ShowAlbumWindow(AlbumWindowData data) {
            OpenWindow(_albumWindow, data);
        }

        public void ShowOfflineBonusWindow(OfflineBonusWindowData data) {
            OpenWindow(_offlineBonusWindow, data);
        }

        public void HideOfflineBonusWindow() {
            if (_openedWindow is OfflineBonusWindow) {
                _offlineBonusWindow.SetVisibilityState(VisibilityState.Invisible);
                _openedWindow = null;
            }
        }

        public void ShowSettingsWindow(SettingsWindowData data) {
            OpenWindow(_settingsWindow, data);
        }

        public void ShowGiftWindow(GiftWindowData data) {
            OpenWindow(_giftWindow, data);
        }

        public void CloseCurrentWindow() {
            _openedWindow?.Close();
            _openedWindow = null;
        }

        private void OpenWindow<TData>(AbstractWindow<TData> window, TData data) where TData : AbstractWindowData {
            _mergeSlimesTutorialDirector.gameObject.SetActive(false);
            // TODO add queue

            if (_openedWindow == null) {
                OpenWindowInternal(window, data);
                return;
            }

            if (!ReferenceEquals(_openedWindow, window)) {
                _openedWindow.Close(() => {
                    OpenWindowInternal(window, data);
                });
                return;
            }
            
            if (App.Settings.Ui.CloseOpenedWindowIfOpenedAgain) {
                CloseCurrentWindow();
                return;
            }
            
            // TODO update data
        }

        private void OpenWindowInternal<TData>(AbstractWindow<TData> window, TData data) where TData : AbstractWindowData {
            _openedWindow = window;
            window.gameObject.SetActive(true);
            window.Open(data);
        }

        private void OnWindowHiddenSignal(WindowHiddenSignal signal) {
            signal.Window.gameObject.SetActive(false);
            if (ReferenceEquals(_openedWindow, signal.Window)) {
                _openedWindow = null;
            }
        }
    }
}