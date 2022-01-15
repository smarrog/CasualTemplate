using System;
using UnityEngine;
using UnityEngine.UI;

namespace Smr.Ui {
    public class SimpleScroller : MonoBehaviour {
        [SerializeField] private Button _btnPrev;
        [SerializeField] private Button _btnNext;

        public event Action OnBtnTapped;

        private int _index;
        private int _elementsAmount;
        private Action<int> _onElementSelected;

        private void Awake() {
            _btnPrev.onClick.AddListener(OnBtnPrev);
            _btnNext.onClick.AddListener(OnBtnNext);
        }

        public void Init(int elementsAmount, Action<int> onElementSelected, int index = 0) {
            _elementsAmount = elementsAmount;
            _onElementSelected = onElementSelected;
            SetIndex(index);
        }

        public void SetIndex(int value) {
            _index = Math.Clamp(value, 0, _elementsAmount);
            _onElementSelected?.Invoke(_index);
            UpdateButtons();
        }

        private void UpdateButtons() {
            _btnPrev.gameObject.SetActive(_index > 0);
            _btnNext.gameObject.SetActive(_index < _elementsAmount - 1);
        }

        private void OnBtnPrev() {
            OnBtnTapped?.Invoke();
            SetIndex(_index - 1);
        }

        private void OnBtnNext() {
            OnBtnTapped?.Invoke();
            SetIndex(_index + 1);
        }
    }
}