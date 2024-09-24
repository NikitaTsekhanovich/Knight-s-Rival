using System.Collections;
using System.Collections.Generic;
using GameBoard.CellTypes;
using GameBoard.Figures;
using Players;
using Players.AI;
using Players.Human;
using UnityEngine;
using UnityEngine.UI;
using System;
using GameController;
using GameControllers;

namespace GameBoard
{
    public class GameBoardHandler : GameBoardCreator
    {
        [SerializeField] private GameObject _stepImage;
        [SerializeField] private List<Transform> _figuresParents = new();
        private List<GameObject> _moveSteps = new();

        public static Action<FiguresTypes> OnDropAbility;
        public static Func<FigureData> GetCurrentFigure;
        public static Action<FigureData> OnChangeQueue;

        private void OnEnable()
        {
            HumanController.OnShowSteps += ShowPathCurrentFigure;
            Cell.OnSpawnDropFigure += SpawnDropFigure;
            AIController.OnSpawnDropFigure += SpawnDropFigure;
            PauseController.OnRestartPlayersData += RestartFigures;
            SceneLoader.OnCraeteGameBoard += CreateGameBoard;
            PauseController.OnRestartGameBoard += RestartGameBoard;
            FigureData.OnChangePosition += ChangeParentFigure;
        }

        private void OnDisable()
        {
            HumanController.OnShowSteps -= ShowPathCurrentFigure;
            Cell.OnSpawnDropFigure -= SpawnDropFigure;
            AIController.OnSpawnDropFigure -= SpawnDropFigure;
            PauseController.OnRestartPlayersData -= RestartFigures;
            SceneLoader.OnCraeteGameBoard -= CreateGameBoard;
            PauseController.OnRestartGameBoard -= RestartGameBoard;
            FigureData.OnChangePosition -= ChangeParentFigure;
        }

        private void RestartFigures()
        {
            for (var i = 0; i < _figuresParents.Count - 1; i++) 
            {
                while (_figuresParents[i].transform.childCount > 0) 
                    DestroyImmediate(_figuresParents[i].transform.GetChild(0).gameObject);
            }
        }

        private void ClearMoveSteps()
        {
            foreach (var moveStep in _moveSteps)
                Destroy(moveStep);
            _moveSteps.Clear();
        }

        private void ShowPathCurrentFigure(FigureData figure, List<Points> movePoints, List<Points> attackPoints)
        {
            ClearMoveSteps();
            InstantiateSteps(figure, movePoints, false);
            InstantiateSteps(figure, attackPoints, true);
        }

        private void InstantiateSteps(FigureData figure, List<Points> points, bool _isAttackPoints)
        {
            foreach (var point in points)
            {
                var step = Instantiate(
                    _stepImage,
                    _gameBoard[point.X, point.Y].transform);
                step.transform.SetParent(_gameBoard[point.X, point.Y].transform);
                step.transform.localScale = Vector3.one;

                step.GetComponent<Step>().SetStepData(
                    new Points(point.X, point.Y),
                    figure.PositionOnBoard);

                if (_isAttackPoints)
                    step.GetComponent<Image>().color = new Color(0, 0, 0, 0);

                _moveSteps.Add(step);
            }
        }

        private IEnumerator WaitPositionCell(Transform cellTransform, FigureData newFigure, string positionOnBoard)
        {
            yield return new WaitForEndOfFrame();
            newFigure.transform.SetParent(_figuresParents[^1]);
            newFigure.transform.localPosition = new Vector3(cellTransform.localPosition.x, cellTransform.localPosition.y + 26f, 0);
            newFigure.transform.SetParent(_figuresParents[positionOnBoard[1] - '0' - 1]);
        }

        private void ChangeParentFigure(FigureData newFigure, string positionOnBoard)
        {
            ClearMoveSteps();
            newFigure.transform.SetParent(_figuresParents[positionOnBoard[1] - '0' - 1]);
        }

        protected override FigureData GetSpawnFigure(
            Points spawnPoints, 
            FiguresTypes figureType, 
            PlayersTypes playerType)
        {
            var cellTransform = _gameBoard[spawnPoints.X, spawnPoints.Y].transform;

            var newFigure = Instantiate(
                FigureStorage.Figures[playerType][figureType], 
                cellTransform);

            var positionOnBoard = FigurePositionHandler.GetPositionOnBoard(spawnPoints.X, spawnPoints.Y);;

            StartCoroutine(WaitPositionCell(cellTransform, newFigure, positionOnBoard));
            newFigure.transform.localScale = Vector3.one;

            var poistionOnIndex = new Points (
                    (int)CellDataTypes.HorizontalTypes.GetValue(spawnPoints.X),
                    (int)CellDataTypes.VerticalTypes.GetValue(spawnPoints.Y));

            newFigure.GetComponent<FigureData>().SetFigurePosition(
                positionOnBoard,
                poistionOnIndex);

            newFigure.GetComponent<FigureData>().SetSize(
                _cellSizeX,
                _cellSizeY);

            if (playerType == PlayersTypes.Human)
                HumanController.Instance.AddFigure(positionOnBoard, newFigure);
            else
                AIController.Instance.AddFigure(positionOnBoard, newFigure);

            return newFigure;
        }

        private void SpawnDropFigure<TPlayer>(
            Points spawnPoints, 
            FiguresTypes figureType, 
            PlayersTypes playerType,
            TPlayer player)
            where TPlayer : IPlayer
        {
            var newFigure = GetSpawnFigure(spawnPoints, figureType, playerType);
            OnDropAbility?.Invoke(figureType);
            OnChangeQueue?.Invoke(newFigure);
            player.CalculateMovePosition(GetCurrentFigure());
        }
    }
}

