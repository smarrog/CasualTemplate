using Smr.Extensions;
using Smr.Localization;
using TMPro;
using UnityEngine;

namespace Game {
    public class DailyBonusWindowElement : MonoBehaviour {
        [SerializeField] private TextLocalizationComponent _dayLocalizationComponent;
        [SerializeField] private TMP_Text _durationLabel;
        [SerializeField] private MoneyElement _moneyElement;
        [SerializeField] private GameObject _isNotTakenObject;
        [SerializeField] private GameObject _isTakenObject;
        [SerializeField] private GameObject _isNotCurrentObject;
        
        public void Init(int dayStreak) { // starts from 1
            var isCurrent = App.DailyBonusLogic.IsCurrent(dayStreak);
            var isTaken = App.DailyBonusLogic.IsTaken(dayStreak);
            var secondsOfIncome = App.DailyBonusLogic.GetSecondsOfIncome(dayStreak);
            var reward = App.DailyBonusLogic.GetDailyBonus(dayStreak); // ignore Income changes during open window
            
            _dayLocalizationComponent.SetValue(dayStreak.ToString());
            _durationLabel.text = secondsOfIncome.ToTime(App.SettingsLogic.Localization);
            _moneyElement.SetValue(reward);
            
            if (_isNotCurrentObject) {
                _isNotCurrentObject.SetActive(!isCurrent);
            }
            if (_isNotTakenObject) {
                _isNotTakenObject.SetActive(!isTaken);
            }
            if (_isTakenObject) {
                _isTakenObject.SetActive(isTaken);
            }
        }
    }
}