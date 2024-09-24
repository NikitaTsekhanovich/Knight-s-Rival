using DG.Tweening;
using GameLogic.MoveQueue;
using TMPro;
using UnityEngine;

namespace Players
{
    public abstract class ManaController : MonoBehaviour
    {
        [SerializeField] protected TMP_Text _currentHumanManaText;
        protected int _currentMana;

        public static readonly int ManaIncreasePerRound = 2;

        private void OnEnable()
        {
            MoveQueueHandler.OnChangeMana += ChangeMana;
        }

        private void OnDisable()
        {
            MoveQueueHandler.OnChangeMana -= ChangeMana;
        }

        public void Init(int currentMana)
        {
            _currentMana = currentMana;
        }

        public void ChangeMana(int cost)
        {
            _currentMana += cost;
            if (_currentHumanManaText != null)
            {
                if (cost > 0) 
                    AnimationManaText(Color.green);
                else
                    AnimationManaText(Color.red);
                    
                _currentHumanManaText.text = $"{_currentMana}";
            }
        }

        private void AnimationManaText(Color color)
        {
            DOTween.Sequence()
                .Append(_currentHumanManaText.DOColor(color, 0.3f))
                .AppendInterval(0.1f)
                .Append(_currentHumanManaText.DOColor(Color.white, 0.3f));
        }

        public void RestartMana(int currentMana)
        {
            _currentMana = currentMana;
            if (_currentHumanManaText != null)
                _currentHumanManaText.text = $"{_currentMana}";
        }

        public int GetCurrentMana() => _currentMana;
    }
}

