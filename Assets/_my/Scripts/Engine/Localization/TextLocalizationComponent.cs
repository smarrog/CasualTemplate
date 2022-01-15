using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Smr.Common;
using Smr.Extensions;
using TMPro;
using UnityEngine;

namespace Smr.Localization {
    [RequireComponent(typeof(TMP_Text))]
    public class TextLocalizationComponent : MonoBehaviour {
        private const string DEFAULT_VALUE = "v";
        
        [SerializeField] private string _ru;
        [SerializeField] private string _en;
#if UNITY_EDITOR
        [Header("Test")]
        [SerializeField] private Localization _testLocalization = Localization.Default;
        [SerializeField] private List<string> _testValues = new List<string>();

        public void OnValidate() {
            if (Application.isPlaying) {
                return;
            }

            if (!_label) {
                _label = GetComponent<TMP_Text>();
            }

            _localization = _testLocalization;
            _values?.Clear();
            if (_testValues.IsFilled() && _testLocalization != Localization.Default) {
                var matches = new Regex("!(\\S+?)!").Matches(GetLocalizedText());
                for (var i = 0; i < matches.Count; i++) {
                    _values ??= new Dictionary<string, string>();
                    _values[matches[i].Groups[1].Value] = _testValues.GetAtOrDefault(i);
                }
            }
            UpdateLabel();
        }
#endif

        private TMP_Text _label;
        private Localization _localization;
        private Dictionary<string, string> _values;

        private void Awake() {
            _localization = EngineDependencies.Localization.Localization;
            _label = GetComponent<TMP_Text>();
            
            UpdateLabel();

            EngineDependencies.SignalBus.Subscribe<LocalizationChangedSignal>(OnLocalizationChangedSignal);
        }

        private void OnDestroy() {
            EngineDependencies.SignalBus.Unsubscribe<LocalizationChangedSignal>(OnLocalizationChangedSignal);
        }
        
        public void SetValue(string value) {
            SetValue(DEFAULT_VALUE, value);
        }

        public void SetValue(string key, string value) {
            SetValues(new Dictionary<string, string> { { key, value } });
        }

        public void SetValues(Dictionary<string, string> values) {
            _values = values;
            UpdateLabel();
        }

        private void UpdateLabel() {
            if (!_label || _localization == Localization.Default) {
                return;
            }

            var localizedText = GetLocalizedText();
            if (string.IsNullOrEmpty(localizedText)) {
                EngineDependencies.Logger?.LogError($"Localized text is empty!. Current text: {_label.text}");
                return;
            }
            
            _label.text = ApplyValues(localizedText);
        }

        private string ApplyValues(string text) {
            if (string.IsNullOrEmpty(text) || _values == null) {
                return text;
            }

            foreach (var (key, value) in _values) {
                text = text.Replace($"!{key}!", value);
            }

            return text;
        }

        private void OnLocalizationChangedSignal(LocalizationChangedSignal signal) {
            _localization = signal.Localization;
            UpdateLabel();
        }

        private string GetLocalizedText() {
            return _localization switch {
                Localization.English => _en,
                Localization.Russian => _ru,
                _ => throw new ArgumentOutOfRangeException(nameof(_localization), _localization, null)
            };
        }
    }
}