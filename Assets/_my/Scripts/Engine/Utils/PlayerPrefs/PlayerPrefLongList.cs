using System;
using System.Collections.Generic;
using System.Linq;

namespace Smr.Utils {
    public class PlayerPrefLongList : BasePlayerPref<List<long>> {
        private readonly PlayerPrefInt _sizePref;
        private readonly List<PlayerPrefLong> _valuePrefs = new();

        public int Size => _sizePref.Value;

        private readonly long _defaultElementValue;

        public long this[int index] {
            set => SetValueAt(index, value);
            get => GetValueAt(index);
        }

        public PlayerPrefLongList(string prefsKey, List<long> defaultValue = null, long defaultElementValue = 0) : base(prefsKey, defaultValue) {
            _defaultElementValue = defaultElementValue;
            _sizePref = new PlayerPrefInt($"{_prefsKey}:S", 0);
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
        
        protected override List<long> GetPlayerPrefValue() {
            return _valuePrefs
                .Select(valuePref => valuePref.Value)
                .Take(Size)
                .ToList();
        }
        
        protected override void SetPlayerPrefValue(List<long> value) {
            _sizePref.Value = value?.Count ?? 0;
            for (var i = 0; i < _sizePref.Value; ++i) {
                _valuePrefs[i].Value = value![i];
            }
        }

        private void SetValueAt(int index, long value) {
            if (index < 0) {
                throw new ArgumentException();
            }
            if (index >= Size) {
                SetSize(index + 1);
            }
            _valuePrefs[index].Value = value;
        }

        private long GetValueAt(int index) {
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
                _valuePrefs.Add(new PlayerPrefLong($"{_prefsKey}:{_valuePrefs.Count}", _defaultElementValue));
            }
        }
    }
}