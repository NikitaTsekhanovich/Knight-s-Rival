using System;
using System.Collections.Generic;
using DG.Tweening;
using GameController;
using Players;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace GameBoard.Figures
{
    public class FigureData : MonoBehaviour
    {
        [SerializeField] private AudioSource _startMoveSound;
        [SerializeField] private AudioSource _endMoveSound;
        [SerializeField] private AudioSource _destroySound;
        [SerializeField] protected Sprite _figureImage;
        [SerializeField] protected FiguresTypes _figureType;
        [SerializeField] protected PlayersTypes _playerType;
        [SerializeField] protected List<Points> _movePoints = new ();
        protected string _positionOnBoard;
        protected Points _poistionOnIndex;
        protected float _cellSizeX;
        protected float _cellSizeY;
        private Image _imageFigure;
        private DG.Tweening.Sequence _animMove;

        public Sprite FigureImage => _figureImage;
        public FiguresTypes FigureType => _figureType;
        public PlayersTypes PlayerType => _playerType;
        public string PositionOnBoard => _positionOnBoard;
        public List<Points> MovePoints => _movePoints;
        public Points PoistionOnIndex => _poistionOnIndex;

        public static Action<FigureData, string> OnChangePosition;
        public static Action OnFigureMove;

        private void Awake()
        {
            _imageFigure = GetComponent<Image>();
        }

        private void OnEnable()
        {
            PauseController.OnRestartAnimMove += RestartAnim;
        }

        private void OnDisable()
        {   
            PauseController.OnRestartAnimMove -= RestartAnim;
        }

        public void SetFigurePosition(string positionOnBoard, Points poistionOnIndex)
        {
            _positionOnBoard = positionOnBoard;
            _poistionOnIndex = poistionOnIndex;
        }

        public void SetSize(float cellSizeX, float cellSizeY)
        {
            _cellSizeX = cellSizeX;
            _cellSizeY = cellSizeY;
        }

        public void ImproveFigure(
            Sprite figureImage, 
            FiguresTypes figureType, 
            List<Points> movePoints)
        {
            _imageFigure.sprite = figureImage;
            _figureImage = figureImage;
            _figureType = figureType;
            _movePoints = movePoints;
        }

        public void Move<TPlayer>(TPlayer player, Points poistionStep, string positionFigureOnBoard)
            where TPlayer : IPlayer
        {
            var offsetX = _poistionOnIndex.X - poistionStep.X;
            var offsetY = _poistionOnIndex.Y - poistionStep.Y;

            var newLocalPosition = new Vector3(
                transform.localPosition.x - offsetY * _cellSizeX,
                24f,
                0);

            _startMoveSound.Play();

            var newPositionOnBoard = FigurePositionHandler.GetPositionOnBoard(poistionStep.X, poistionStep.Y);
            var newPositionOnIndex = new Points(poistionStep.X, poistionStep.Y);

            OnChangePosition?.Invoke(this, newPositionOnBoard);
            player.ChangePositionOnBoard(positionFigureOnBoard, newPositionOnBoard);
            SetFigurePosition(newPositionOnBoard, newPositionOnIndex);

            _animMove = DOTween.Sequence()
                .Append(transform.DOLocalMove(newLocalPosition, 1f))
                .AppendCallback(() => UpdatePosition(player, newPositionOnBoard));

            // Система координат перевернута
            // перемещение без анимации
            // transform.localPosition = new Vector3(
            //     transform.localPosition.x - offsetY * _cellSizeX,
            //     transform.localPosition.y + offsetX * _cellSizeY,
            //     0);
        }

        private void UpdatePosition<TPlayer>(TPlayer player, string newPositionOnBoard)
            where TPlayer : IPlayer
        {
            player.CheckEnemyFigure(newPositionOnBoard, _destroySound);
            OnFigureMove?.Invoke();
            _endMoveSound.Play();
        }

        private void RestartAnim()
        {
            _animMove.Kill();
        }
    }
}

