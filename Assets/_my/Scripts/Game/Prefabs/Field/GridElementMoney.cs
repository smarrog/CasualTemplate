using System.Numerics;
using Smr.Animations;
using UnityEngine;
using UnityEngine.Playables;

namespace Game {
    public class GridElementMoney : MonoBehaviour {
        [SerializeField] private MoneyElement _moneyElement;
        [SerializeField] private PlayableDirector _showAnimation;
        
        public void Init() {
            gameObject.SetActive(false);
        }

        public void Show(BigInteger amount) {
            gameObject.SetActive(true);
            _moneyElement.SetValue(amount);
            if (_showAnimation) {
                _showAnimation.PlayExt();
            }
        }
    }
}