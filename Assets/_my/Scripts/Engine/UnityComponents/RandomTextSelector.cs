using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Smr.Components {
    public class RandomTextSelector : MonoBehaviour {
        [SerializeField] private TMP_Text _label;
        [SerializeField] private List<string> _variants;

        private void Awake() {
            if (_variants.Count == 0) {
                return;
            }

            _label.text = _variants[Random.Range(0, _variants.Count)];
        }
    }
}