using System;
using System.Collections.Generic;
using System.Linq;

namespace Smr.Utils {
    public class PlayerPrefIntList : BasePlayerPref<List<int>> {
        private readonly PlayerPrefInt _sizePref;
        private readonly List<PlayerPrefInt> _valuePrefs = new();

        public int Size => _sizePref.Value;

        private readonly int _defaultElementValue;

        public int this[int index] {
            set => SetValueAt(index, value);
            get => GetValueAt(index);
        }

        public PlayerPrefIntList(string prefsKey, List<int> defaultValue = null, int defaultElementValue = 0) : base(prefsKey, defaultValue) {
            _defaultElementValue = defaultElementValue;
            _sizePref = new PlayerPrefInt($"{_prefsKey}:S");
            ExpandIfNeed();
        }

        public override void Clear() {
            foreach (var valuePref in _valuePrefs) {
                valuePref.Clear();
            }
            _sizePref.Clear();
        }

        public void SetSize(int value) {
            if (value < 0) {
                throw new ArgumentException();
            }
            
            if (_sizePref.Value == value) {
                return;
            }

            var currentSize = _sizePref.Value;
            _sizePref.Value = value;
            ExpandIfNeed();
            
            // remove unnecessary
            for (var i = value; i < currentSize; ++i) {
                _valuePrefs[i].Clear(); // for optimization they are not removed. Size is calculated by sizePref
            }
        }
        
        protected override List<int> GetPlayerPrefValue() {
            return _valuePrefs
                .Select(valuePref => valuePref.Value)
                .Take(Size)
                .ToList();
        }
        
        protected override void SetPlayerPrefValue(List<int> value) {
            _sizePref.Value = value?.Count ?? 0;
            for (var i = 0; i < _sizePref.Value; ++i) {
                _valuePrefs[i].Value = value![i];
            }
        }

        private void SetValueAt(int index, int value) {
            if (index < 0) {
                throw new ArgumentException();
            }
            if (index >= Size) {
                SetSize(index + 1);
            }
            _valuePrefs[index].Value = value;
        }

        private int GetValueAt(int index) {
            if (index < 0) {
                throw new ArgumentException();
            }
            if (index >= Size) {
                return 0;
            }
            return _valuePrefs[index].Value;
        }

        private void ExpandIfNeed() {
            while (_valuePrefs.Count <= Size) {
                _valuePrefs.Add(new PlayerPrefInt($"{_prefsKey}:{_valuePrefs.Count}", _defaultElementValue));
            }
        }
    }
}