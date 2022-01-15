using Smr.Localization;
using UnityEngine;
using UnityEngine.UI;

namespace Game {
    public class SettingsWindowData : AbstractWindowData {

    }

    public class SettingsWindow : AbstractWindow<SettingsWindowData> {
        private enum State {
            Default,
            Reset
        }

        [SerializeField] private GameObject _defaultStateObject;
        [SerializeField] private GameObject _resetStateObject;
        [SerializeField] private Button _saveButton;
        [SerializeField] private Button _resetProgressButton;
        [SerializeField] private TextLocalizationComponent _versionLocalizationComponent;
        [SerializeField] private Button _resetConfirmButton;
        [SerializeField] private Button _resetCancelButton;

        protected override void ConstructInternal() {
            base.ConstructInternal();

            _saveButton.onClick.AddListener(OnSaveButton);
            _resetProgressButton.onClick.AddListener(OnResetProgressButton);
            _resetConfirmButton.onClick.AddListener(OnResetConfirmButton);
            _resetCancelButton.onClick.AddListener(OnResetDenyButton);
            _versionLocalizationComponent.SetValue(App.Settings.System.Version);
        }

        protected override void OnDataApplied() {
            SetState(State.Default);
        }

        protected override void AfterHide() {
            base.AfterHide();
            
            App.SaveService.Flush();
        }

        private void SetState(State state) {
            _defaultStateObject.SetActive(state == State.Default);
            _resetStateObject.SetActive(state == State.Reset);
        }

        private void OnSaveButton() {
            App.PlayTap();
            App.SaveService.Flush();
        }

        private void OnResetProgressButton() {
            App.PlayTap();
            SetState(State.Reset);
        }

        private void OnResetConfirmButton() {
            App.PlayTap();
            App.ResetProgress();
            Close();
        }

        private void OnResetDenyButton() {
            App.PlayTap();
            SetState(State.Default);
        }
    }
}