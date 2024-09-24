using System.Collections;
using System.Collections.Generic;
using GameBoard.CellTypes;
using GameBoard.Figures;
using GameLogic.MoveQueue;
using Players;
using UnityEngine;

namespace GameBoard
{
    public abstract class GameBoardCreator : MonoBehaviour
    {
        [SerializeField] protected List<GameObject> _cells = new ();
        [SerializeField] protected Transform _gameBoardParent;
        [SerializeField] protected List<SpawnPositionFiguresData> _positionsFiruges = new ();
        [SerializeField] protected MoveQueueHandler _moveQueueHandler;

        protected GameObject[,] _gameBoard = new GameObject[MaxSizeBoard, MaxSizeBoard];

        protected const int MinSizeBoard = 0;
        protected const int MaxSizeBoard = 8;
        protected float _cellSizeX;
        protected float _cellSizeY;

        public readonly static int SizeBoard = MaxSizeBoard;

        private void Start()
        {
            CreateGameBoard();
        }

        protected void CreateGameBoard()
        {
            StartCoroutine(WaitLoadGameBoard());
        }

        protected void PostCells()
        {
            for (var i = 0; i < MaxSizeBoard; i++)
            {
                for (var j = 0; j < MaxSizeBoard; j++)
                {
                    var cell = Instantiate(_cells[(j + i) % 2]);
                    cell.transform.SetParent(_gameBoardParent, false);
                    cell.transform.localScale = Vector3.one;
                    cell.name = $"{CellDataTypes.HorizontalTypes.GetValue(j)} {CellDataTypes.VerticalTypes.GetValue(i)}";
                    cell.GetComponent<Cell>().SetPosition(
                        i, j, FigurePositionHandler.GetPositionOnBoard(i, j));
                    _gameBoard[i, j] = cell;
                }
            }
        }

        protected void ChooseSpawnPositionsFigures()
        {
            var index = Random.Range(0, _positionsFiruges.Count);
            SpawnFirstFigures(
                _positionsFiruges[index].PointsPawnPlayer,
                _positionsFiruges[index].PointKingPlayer,
                PlayersTypes.Human);
            SpawnFirstFigures(
                _positionsFiruges[index].PointsPawnEnemy,
                _positionsFiruges[index].PointKingEnemy,
                PlayersTypes.AI);

            _moveQueueHandler.CreateQueueUI();
        }

        protected void SpawnFirstFigures(
            Points[] pointsPawns, 
            Points pointKing,
            PlayersTypes playerType)
        {
            foreach (var points in pointsPawns)
                GetSpawnFigure(points, FiguresTypes.Pawn, playerType);

            GetSpawnFigure(pointKing, FiguresTypes.King, playerType);
        }

        private IEnumerator WaitLoadGameBoard()
        {
            yield return new WaitForEndOfFrame();
            SetSizeCells();
            PostCells();
            ChooseSpawnPositionsFigures();
        }

        private void SetSizeCells()
        {
            var cellSizes = _gameBoardParent.GetComponent<FlexibleGridLayout>().cellSize;
            _cellSizeX = cellSizes.x;
            _cellSizeY = cellSizes.y;
        }

        protected abstract FigureData GetSpawnFigure(
            Points spawnPoints, 
            FiguresTypes figureType, 
            PlayersTypes playerType);

        protected void RestartGameBoard()
        {
            while (_gameBoardParent.transform.childCount > 0) 
                DestroyImmediate(_gameBoardParent.transform.GetChild(0).gameObject);

            CreateGameBoard();
        }
    }
}
