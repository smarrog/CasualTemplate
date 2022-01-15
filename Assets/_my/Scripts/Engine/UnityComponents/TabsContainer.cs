using System;
using System.Collections.Generic;
using Smr.Extensions;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Smr.Components {
    public class TabsContainer : TabsContainer<GameObject> {}

    public class TabsContainer<T> : MonoBehaviour where T : Object {
        [SerializeField] private List<T> _tabs;

        public IReadOnlyList<T> Tabs => _tabs;
        public T Active => _tabs.GetAtOrDefault(_activeIndex);

        public event Action<T> OnSelected;

        private bool IsAnythingSelected => _activeIndex >= 0;

        private int _activeIndex = -1;

        private void Awake() {
            if (!IsAnythingSelected) {
                SetActive(0);
            }
        }

        public void SetActive(T tab) {
            if (_tabs.Count > 0) {
                SetActive(_tabs.IndexOf(tab));
            }
        }

        public void SetActive(int index) {
            if (_activeIndex == index) {
                return;
            }
            
            _activeIndex = index;
            for (var i = 0; i < _tabs.Count; i++) {
                var tabAsGo = _tabs[i] as GameObject;
                if (tabAsGo) {
                    tabAsGo.SetActive(i == _activeIndex);
                }
            }
            OnSelected?.Invoke(_tabs[_activeIndex]);
        }

        public void Reset() {
            _activeIndex = -1;
            OnSelected = null;
        }
    }
}