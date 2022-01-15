using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game {
    [DefaultExecutionOrder(-10)]
    public class Preloader : MonoBehaviour {
        [SerializeField] private Button _continueButton;
        [SerializeField] private GameObject _loadingObject;
        
        public Action OnContinueButtonPressed;

        private bool _isFinalStateShown;

        private void Awake() {
            if (_continueButton) {
                _continueButton.onClick.AddListener(OnContinueButton);
            }

            if (!_isFinalStateShown) {
                ShowLoadingState();
            }
        }

        private void ShowLoadingState() {
            if (_continueButton) {
                _continueButton.gameObject.SetActive(false);
            }
            if (_loadingObject) {
                _loadingObject.SetActive(true);
            }
        }

        public void ShowFinalState() {
            _isFinalStateShown = true;
            
            if (_loadingObject) {
                _loadingObject.SetActive(false);
            }
            if (_continueButton) {
                _continueButton.gameObject.SetActive(true);
            }
        }

        private void OnContinueButton() {
            OnContinueButtonPressed?.Invoke();
        }
    }
}