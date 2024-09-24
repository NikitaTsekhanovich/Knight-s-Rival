using System;
using GameBoard.Figures;
using Players;
using UnityEngine;

namespace GameLogic
{
    public class GameStateController : MonoBehaviour
    {
        [SerializeField] private GameObject _winScreen;
        [SerializeField] private GameObject _loseScreen;
        [SerializeField] private AudioSource _winSound;
        [SerializeField] private AudioSource _loseSound;

        public static Action OnChangeGameState;
        public static Action OnSetWin;
        public static Action OnSetMatch;

        private void OnEnable()
        {
            PlayerController.OnCheckGameState += CheckGameState;
        }

        private void OnDisable()
        {
            PlayerController.OnCheckGameState -= CheckGameState;
        }

        private void CheckGameState(FiguresTypes figureType, PlayersTypes playerType)
        {
            if (figureType == FiguresTypes.King)
            {
                if (playerType == PlayersTypes.Human)
                {
                    _loseScreen.SetActive(true);
                    _loseSound.Play();
                }
                else if (playerType == PlayersTypes.AI)
                {
                    _winScreen.SetActive(true);
                    _winSound.Play();
                    OnSetWin?.Invoke();
                }

                OnSetMatch?.Invoke();
                OnChangeGameState?.Invoke();
            }
        }
    }
}

