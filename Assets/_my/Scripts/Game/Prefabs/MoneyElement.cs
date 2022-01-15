using System.Numerics;
using Smr.Extensions;
using Smr.Localization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game {
    public class MoneyElement : MonoBehaviour {
        [SerializeField] private RectTransform _container;
        [SerializeField] private TMP_Text _label;
        [SerializeField] private TextLocalizationComponent _localizationComponent;
        [SerializeField] private Color _unavailableTextColor;

        private Color? _baseTextColor;

        public void SetValue(BigInteger value) {
            if (_localizationComponent) {
                _localizationComponent.SetValue(value.ToAbbreviatedString());
            } else {
                _label.text = value.ToAbbreviatedString();
            }
            LayoutRebuilder.ForceRebuildLayoutImmediate(_container);
        }

        public void SetAvailability(bool value) {
            _baseTextColor ??= _label.color;
            _label.color = value ? _baseTextColor.Value : _unavailableTextColor;
        }
    }
}