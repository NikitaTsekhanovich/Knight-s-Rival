using System.Collections.Generic;
using GameBoard.Figures;
using GameController;
using Players.Human;
using UnityEngine;
using System;
using GameBoard;
using GameLogic.Abilities;
using GameBoard.Figures.Models;

namespace Players.AI
{
    public class AIController : PlayerController
    {
        public static AIController Instance;
        public static Action<Points, FiguresTypes, PlayersTypes, IPlayer> OnSpawnDropFigure;
        
        [SerializeField] private AIMana _aIMana;
        private bool _isAttackOnFigure;

        private void Awake() 
        {             
            if (Instance == null) 
            {
                Instance = this; 
                _aIMana.Init(StartMana);
            }
            else 
                Destroy(this);   
        }

        private void OnEnable()
        {
            PauseController.OnRestartPlayersData += RestartFiguresDict;
        }

        private void OnDisable()
        {
            PauseController.OnRestartPlayersData -= RestartFiguresDict;
        }

        protected override void ChooseStrategy(FigureData figure, List<Points> currentMovePoints, List<Points> currentAttackPoints)
        {
            var randomStrategy = UnityEngine.Random.Range(0, 3);
            _isAttackOnFigure = false;

            if (randomStrategy - 2 == (int)StrategyType.UseAbility)
            {
                if (!IsUseAbilitySpawn())
                    ChooseMove(figure, currentMovePoints, currentAttackPoints);
            }
            else if (randomStrategy <= (int)StrategyType.Move)
            {
                ChooseMove(figure, currentMovePoints, currentAttackPoints);
            }                
        }

        private bool IsUseAbilitySpawn()
        {
            if (_aIMana.GetCurrentMana() - 30 >= 0)
            {
                var positionSpawn = GetSpawnPositionPawn();
                if (positionSpawn != null)
                {
                    _aIMana.ChangeMana(-30);
                    OnSpawnDropFigure?.Invoke(
                        positionSpawn, 
                        FiguresTypes.Pawn, 
                        PlayersTypes.AI,
                        Instance);

                    return true;
                }                
            }  

            return false;       
        }

        private Points GetSpawnPositionPawn()
        {
            var movePositionHuman = GetHumanMove();
            var spawnPoints = new List<Points>();

            for (var i = 0; i < GameBoardCreator.SizeBoard; i++)
            {
                for (var j = 0; j < GameBoardCreator.SizeBoard; j++)
                {
                    var positionOnBoard = FigurePositionHandler.GetPositionOnBoard(i, j);

                    if (!AIController.Instance.Figures.ContainsKey(positionOnBoard) &&
                        !HumanController.Instance.Figures.ContainsKey(positionOnBoard) && 
                        !movePositionHuman.Contains(positionOnBoard))
                    {
                        spawnPoints.Add(new Points(i, j));
                    }
                }
            }

            if (spawnPoints.Count != 0)
            {
                var randomPosition = UnityEngine.Random.Range(0, spawnPoints.Count);
                return spawnPoints[randomPosition];
            }
            return null;
        }

        private void ChooseMove(FigureData figureData, List<Points> currentMovePoints, List<Points> currentAttackPoints)
        {
            if (currentAttackPoints.Count != 0)
            {
                var positionBestAttack = ChooseBestAttack(currentAttackPoints);
                DoMove(figureData, positionBestAttack);
            }
            else if (currentMovePoints.Count != 0)
            {                
                var bestMovePoints = ChooseBestMove(currentMovePoints, figureData.PositionOnBoard);

                if (bestMovePoints.Count == 0)
                {
                    if (!IsUseImproveFigure(false))
                        DoMove(figureData, GetRandomMove(currentMovePoints));
                    else 
                        OnFigureMove?.Invoke();
                }
                else
                {
                    if (_isAttackOnFigure)
                        DoMove(figureData, GetRandomMove(bestMovePoints));
                    else if (!IsUseImproveFigure(true))
                        DoMove(figureData, GetRandomMove(bestMovePoints));
                    else
                        OnFigureMove?.Invoke();
                }
            }
            else
            {
                OnFigureMove?.Invoke();
            }
        }

        private bool IsUseImproveFigure(bool isRandom)
        {
            var isImproved = false;
            var randomStrategy = UnityEngine.Random.Range(2, 4);

            if (randomStrategy == (int)StrategyType.ImproveFigure || !isRandom)
            {
                var improvers = AbilitiesController.GetImprovers();
                var index = -1;

                if (improvers[0].Cost >= improvers[1].Cost &&
                    _aIMana.GetCurrentMana() - improvers[0].Cost >= 0)
                    index = 0;
                else if (improvers[1].Cost > improvers[0].Cost &&
                    _aIMana.GetCurrentMana() - improvers[1].Cost >= 0)
                    index = 1;
                else if (_aIMana.GetCurrentMana() - improvers[0].Cost >= 0)
                    index = 0;
                else if (_aIMana.GetCurrentMana() - improvers[1].Cost >= 0)
                    index = 1;

                if (index != -1)
                {
                    foreach (var aiFigure in Instance.Figures)
                    {
                        if (aiFigure.Value.FigureType == FiguresTypes.Pawn)
                        {
                            aiFigure.Value.ImproveFigure(
                                improvers[index].ImageRed,
                                improvers[index].FigureType,
                                improvers[index].MovePoints
                            );
                            _aIMana.ChangeMana(-improvers[index].Cost);
                            isImproved = true;
                            break;
                        }
                    }
                }
            }

            return isImproved;
        }

        private Points GetRandomMove(List<Points> points)
        {
            var indexMovePosition = UnityEngine.Random.Range(0, points.Count);
            return new Points(
                points[indexMovePosition].X, 
                points[indexMovePosition].Y);
        }

        private void DoMove(FigureData figureData, Points points)
        {
            var positionOnBoard = figureData.PositionOnBoard;

            figureData.Move(
                Instance,
                points,
                positionOnBoard
            );
        }

        private List<Points> ChooseBestMove(List<Points> points, string currentPositionOnBoard)
        {
            var safeMovePoints = new List<Points>();

            var humanMovePoints = GetHumanMove();

            foreach (var point in points)
            {
                var positionOnBoard = FigurePositionHandler.GetPositionOnBoard(point.X, point.Y);

                if (humanMovePoints.Contains(currentPositionOnBoard))
                    _isAttackOnFigure = true;

                if (!humanMovePoints.Contains(positionOnBoard))
                    safeMovePoints.Add(point);
            }

            return safeMovePoints;
        }

        private HashSet<string> GetHumanMove()
        {
            var humanMovePoints = new HashSet<string>();

            foreach (var humanFigure in HumanController.Instance.Figures)
            {
                var moveAndAttackPoints = GetMovePositions(humanFigure.Value);
                var movePoints = moveAndAttackPoints.Item1;
                var attackPoints = moveAndAttackPoints.Item2;
                CheckMoveAndAttackPoints(movePoints, humanMovePoints);

                humanFigure.Value.TryGetComponent<Pawn>(out var pawn);
                if (pawn != null && humanFigure.Value.FigureType == FiguresTypes.Pawn)
                {
                    var pawnAttackPoints = new List<Points>();

                    foreach (var pawnAttackPoint in pawn.AttackPoints)
                    {
                        var positionStepX = pawnAttackPoint.X + pawn.PoistionOnIndex.X;
                        var positionStepY = pawnAttackPoint.Y + pawn.PoistionOnIndex.Y;
                        var positionStepOnBoard = GetCheckRange(positionStepX, positionStepY);
                        
                        if (positionStepOnBoard != null)
                        {
                            pawnAttackPoints.Add(new Points(positionStepX, positionStepY));
                        }
                    }

                    CheckMoveAndAttackPoints(pawnAttackPoints, humanMovePoints);
                }
                else 
                {
                    CheckMoveAndAttackPoints(attackPoints, humanMovePoints);
                }
            }

            return humanMovePoints;
        }

        private void CheckMoveAndAttackPoints(List<Points> points, HashSet<string> humanMovePoints)
        {
            foreach (var point in points)
            {
                var positionOnBoard = FigurePositionHandler.GetPositionOnBoard(point.X, point.Y);
                // баг в positionOnBoard
                if (!humanMovePoints.Contains(positionOnBoard))
                    humanMovePoints.Add(positionOnBoard);
            }
        }

        private Points ChooseBestAttack(List<Points> points)
        {
            Points pointQueen = null;
            Points pointRook = null;
            Points pointKnight = null;
            Points pointBishop = null;
            Points pointPawn = null;

            foreach (var point in points)
            {
                var positionOnBoard = FigurePositionHandler.GetPositionOnBoard(point.X, point.Y);

                if (!HumanController.Instance.Figures.ContainsKey(positionOnBoard))
                    continue;
                
                var typeEnemyFigure = HumanController.Instance.Figures[positionOnBoard].FigureType;

                switch (typeEnemyFigure)
                {
                    case FiguresTypes.King:
                        return point;
                    case FiguresTypes.Queen:
                        pointQueen = point;
                        break;
                    case FiguresTypes.Rook:
                        pointRook = point;
                        break;
                    case FiguresTypes.Knight:
                        pointKnight = point;
                        break;
                    case FiguresTypes.Bishop:
                        pointBishop = point;
                        break;
                    case FiguresTypes.Pawn:
                        pointPawn = point;
                        break;
                }
            }
            
            if (pointQueen != null)
                return pointQueen;
            else if (pointRook != null)
                return pointRook;
            else if (pointKnight != null)
                return pointKnight;
            else if (pointBishop != null)
                return pointBishop;

            return pointPawn;
        }

        public override void CheckEnemyFigure(string positionOnBoard, AudioSource _destroySound)
        {
            if (HumanController.Instance.Figures.ContainsKey(positionOnBoard))
            {
                HumanController.Instance.DeleteFigure(positionOnBoard);
                _destroySound.Play();
            }
        }

        protected override void RestartFiguresDict()
        {
            _aIMana.RestartMana(StartMana);
            Figures.Clear();
        }
    }
}

