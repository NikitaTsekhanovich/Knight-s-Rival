using System;
using System.Collections.Generic;
using System.Linq;
using GameBoard;
using GameBoard.CellTypes;
using GameBoard.Figures;
using GameController;
using Players;
using Players.AI;
using Players.Human;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameLogic.MoveQueue
{
    public class MoveQueueHandler : MonoBehaviour
    {
        [SerializeField] private Transform _queueUI;
        [SerializeField] private GameObject _figureIcon;
        [SerializeField] private Image _firstFigureIcon;
        [SerializeField] private TMP_Text _firstFigurePosition;

        private Queue<FigureData> _queue = new ();
        private FigureData _currentFigure;
        private bool _isFirstFigure = true;
        private bool _gameIsEnd;

        public static Action<int> OnChangeMana;

        private void OnEnable()
        {
            FigureData.OnFigureMove += MoveQueue;
            AIController.OnFigureMove += MoveQueue;
            PlayerController.OnChangeFigures += DeleteFigureOnQueue;
            GameBoardHandler.OnChangeQueue += AddFigureOnQueue;
            GameBoardHandler.GetCurrentFigure += GetCurrentFigure;
            Cell.OnImproveFigure += MoveQueue;
            GameStateController.OnChangeGameState += ChangeGameState;
            PauseController.OnRestartQueue += RestartQueue;
        }

        private void OnDisable()
        {
            FigureData.OnFigureMove -= MoveQueue;
            AIController.OnFigureMove -= MoveQueue;
            PlayerController.OnChangeFigures -= DeleteFigureOnQueue;
            GameBoardHandler.OnChangeQueue -= AddFigureOnQueue;
            GameBoardHandler.GetCurrentFigure -= GetCurrentFigure;
            Cell.OnImproveFigure -= MoveQueue;
            GameStateController.OnChangeGameState -= ChangeGameState;
            PauseController.OnRestartQueue -= RestartQueue;
        }

        private FigureData GetCurrentFigure() => _currentFigure;

        private void MoveQueue()
        {
            if (!_gameIsEnd)
            {
                UpdateQueue();
                CreateQueueUI();
            }
        }

        private void UpdateQueue()
        {
            OnChangeMana?.Invoke(ManaController.ManaIncreasePerRound);

            if (_currentFigure.FigureType == FiguresTypes.Block)
            {
                if (_currentFigure.PlayerType == PlayersTypes.Human)
                    HumanController.Instance.DeleteFigure(_currentFigure.PositionOnBoard);
                else
                    AIController.Instance.DeleteFigure(_currentFigure.PositionOnBoard);
            }
            else
                _queue.Enqueue(_currentFigure);
        }

        public void CreateQueueUI()
        {
            ClearQueueUI();
            _isFirstFigure = true;

            foreach (var figure in GetQueue())
                InstantiateIcon(figure);

            _currentFigure = _queue.Dequeue();

            if (_currentFigure.PlayerType == PlayersTypes.Human)
                HumanController.Instance.CalculateMovePosition(HumanController.Instance.Figures[_currentFigure.PositionOnBoard]);
            else 
                AIController.Instance.CalculateMovePosition(AIController.Instance.Figures[_currentFigure.PositionOnBoard]);
        }

        private Queue<FigureData> GetQueue()
        {
            if (_queue.Count == 0)
            {
                foreach (var figure in HumanController.Instance.Figures.Zip(AIController.Instance.Figures, (human, ai) => (human, ai)))
                {
                    _queue.Enqueue(figure.human.Value);
                    _queue.Enqueue(figure.ai.Value);
                }
            }

            return _queue;
        }

        private void DeleteFigureOnQueue(string position)
        {
            var tempQueue = new List<FigureData>(_queue);
            tempQueue = tempQueue.Where(val => val.PositionOnBoard != position).ToList();
            _queue = new Queue<FigureData>(tempQueue);
        }

        private void AddFigureOnQueue(FigureData figureData)
        {
            _queue.Enqueue(figureData);
            InstantiateIcon(figureData);
        }

        private void InstantiateIcon(FigureData figureData)
        {
            if (_isFirstFigure)
            {
                _firstFigureIcon.sprite = figureData.FigureImage;
                _firstFigurePosition.text = figureData.PositionOnBoard;
                _isFirstFigure = false;
            }
            else
            {
                var figureIcon = Instantiate(_figureIcon).GetComponent<FigureIcon>();
                figureIcon.SetData(figureData.FigureImage, figureData.PositionOnBoard);
                figureIcon.transform.SetParent(_queueUI);
                figureIcon.transform.localScale = Vector3.one;
                figureIcon.transform.localPosition = new Vector3(
                    figureIcon.transform.localPosition.x,
                    figureIcon.transform.localPosition.y,
                    0
                );
            }
        }

        private void ClearQueueUI()
        {
            while (_queueUI.transform.childCount > 0) 
                DestroyImmediate(_queueUI.transform.GetChild(0).gameObject);
        }

        private void ChangeGameState()
        {
            _gameIsEnd = true;
        }

        private void RestartQueue()
        {
            _gameIsEnd = false;
            ClearQueueUI();
            _queue.Clear();
        }
    }
}

