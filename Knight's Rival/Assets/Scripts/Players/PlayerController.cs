using System;
using System.Collections.Generic;
using GameBoard.Figures;
using UnityEngine;
using Players.AI;
using Players.Human;
using GameBoard.Figures.Models;

namespace Players
{
    public abstract class PlayerController : MonoBehaviour, IPlayer
    {
        protected Dictionary<string, FigureData> _figures = new ();

        protected const int StartMana = 40;

        public Dictionary<string, FigureData> Figures { get => _figures; }

        public static Action<string> OnChangeFigures;
        public static Action OnFigureMove;
        public static Action<FiguresTypes, PlayersTypes> OnCheckGameState;

        public void AddFigure(string position, FigureData figureData)
        {
            _figures[position] = figureData;
        }

        public void ChangePositionOnBoard(string positionFigureOnBoard, string newPositionOnBoard)
        {
            var figure = Figures[positionFigureOnBoard];
            Figures.Remove(positionFigureOnBoard);
            Figures[newPositionOnBoard] = figure;
        }

        public void DeleteFigure(string position)
        {
            OnCheckGameState?.Invoke(Figures[position].FigureType, Figures[position].PlayerType);
            Destroy(Figures[position].gameObject);
            Figures.Remove(position);
            OnChangeFigures?.Invoke(position);
        }

        public void CalculateMovePosition(FigureData figure)
        {
            var movePoints = GetMovePositions(figure);
            ChooseStrategy(figure, movePoints.Item1, movePoints.Item2);
        }

        protected Tuple<List<Points>, List<Points>> GetMovePositions(FigureData figure)
        {
            var currentMovePoints = new List<Points>();
            var currentAttackPoints = new List<Points>();

            var indexAttack = 0;
            figure.TryGetComponent<Pawn>(out var pawn);
            
            foreach (var point in figure.MovePoints)
            {
                var positionStepX = point.X + figure.PoistionOnIndex.X;
                var positionStepY = point.Y + figure.PoistionOnIndex.Y;

                if (pawn != null && pawn.FigureType == FiguresTypes.Pawn)
                {
                    var positionAttackX = pawn.AttackPoints[indexAttack].X + figure.PoistionOnIndex.X;
                    var positionAttackY = pawn.AttackPoints[indexAttack].Y + figure.PoistionOnIndex.Y;

                    var positionStepOnBoard = GetCheckRange(positionStepX, positionStepY);
                    CanAddMovePoint(figure, positionStepX, positionStepY, currentMovePoints, null, positionStepOnBoard);

                    positionStepOnBoard = GetCheckRange(positionAttackX, positionAttackY);
                    CanAddMovePoint(figure, positionAttackX, positionAttackY, null, currentAttackPoints, positionStepOnBoard);

                    indexAttack++;
                }
                else if (figure.FigureType == FiguresTypes.King || figure.FigureType == FiguresTypes.Knight)
                {
                    var positionStepOnBoard = GetCheckRange(positionStepX, positionStepY);
                    CanAddMovePoint(figure, positionStepX, positionStepY, currentMovePoints, currentAttackPoints, positionStepOnBoard);
                }
                else 
                {
                    var canMoveNext = true;
                    var offset = 1;
                    while (canMoveNext)
                    {
                        positionStepX = point.X * offset + figure.PoistionOnIndex.X;
                        positionStepY = point.Y * offset + figure.PoistionOnIndex.Y;
                        var positionStepOnBoard = GetCheckRange(positionStepX, positionStepY);

                        canMoveNext = CanAddMovePoint(figure, positionStepX, positionStepY, currentMovePoints, currentAttackPoints, positionStepOnBoard);
                        
                        offset++;
                    }
                }
            }

            return Tuple.Create(currentMovePoints, currentAttackPoints);
        }

        protected bool CanAddMovePoint(
            FigureData figure,
            int positionX, 
            int positionY,
            List<Points> currentMovePoints,
            List<Points> currentAttackPoints,
            string positionStepOnBoard)
        {
            if (positionStepOnBoard == null)
                return false;

            if (currentMovePoints != null &&
                !AIController.Instance.Figures.ContainsKey(positionStepOnBoard) &&
                !HumanController.Instance.Figures.ContainsKey(positionStepOnBoard))
            {
                currentMovePoints.Add(new Points(positionX, positionY));
                return true;
            }
            else if (currentAttackPoints != null &&
                    ((AIController.Instance.Figures.ContainsKey(positionStepOnBoard) && 
                    figure.PlayerType == PlayersTypes.Human &&
                    AIController.Instance.Figures[positionStepOnBoard].FigureType != FiguresTypes.Block) ||
                    (HumanController.Instance.Figures.ContainsKey(positionStepOnBoard) && 
                    figure.PlayerType == PlayersTypes.AI &&
                    HumanController.Instance.Figures[positionStepOnBoard].FigureType != FiguresTypes.Block)))
            {
                currentAttackPoints.Add(new Points(positionX, positionY));
                return false;
            }

            return false;
        }

        protected string GetCheckRange(
            int positionX, 
            int positionY)
        {
            if (positionX >= 0 && positionX < 8 &&
                positionY >= 0 && positionY < 8)
            {
                var positionStepOnBoard = FigurePositionHandler.GetPositionOnBoard(positionX, positionY);;
                return positionStepOnBoard;
            }
            return null;
        }

        public abstract void CheckEnemyFigure(string positionOnBoard, AudioSource _destroySound);

        protected abstract void RestartFiguresDict();

        protected abstract void ChooseStrategy(FigureData figure, List<Points> currentMovePoints, List<Points> currentAttackPoints);
    }
}

