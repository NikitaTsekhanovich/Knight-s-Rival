using System;
using System.Collections.Generic;
using GameBoard.Figures;
using GameController;
using GameLogic.Abilities;
using Players.AI;
using UnityEngine;

namespace Players.Human
{
    public class HumanController : PlayerController
    {
        [SerializeField] private HumanMana _humanMana;

        public static HumanController Instance;
        public static Action<FigureData, List<Points>, List<Points>> OnShowSteps;

        private void Awake() 
        {           
            if (Instance == null) 
            {
                Instance = this; 
                _humanMana.Init(StartMana);
            }
            else 
                Destroy(this);   
        }

        private void OnEnable()
        {
            FigureAbility.OnUseAbility += _humanMana.ChangeMana;
            PauseController.OnRestartPlayersData += RestartFiguresDict;
        }

        private void OnDisable()
        {
            FigureAbility.OnUseAbility -= _humanMana.ChangeMana;
            PauseController.OnRestartPlayersData -= RestartFiguresDict;
        }

        public int CheckMana() => _humanMana.GetCurrentMana();

        public override void CheckEnemyFigure(string positionOnBoard, AudioSource _destroySound)
        {
            if (AIController.Instance.Figures.ContainsKey(positionOnBoard))
            {
                _destroySound.Play();
                AIController.Instance.DeleteFigure(positionOnBoard);
            }
        }

        protected override void ChooseStrategy(FigureData figure, List<Points> currentMovePoints, List<Points> currentAttackPoints)
        {
            if (currentMovePoints.Count != 0 || currentAttackPoints.Count != 0)
                OnShowSteps?.Invoke(figure, currentMovePoints, currentAttackPoints);
            else
                OnFigureMove?.Invoke();
        }

        protected override void RestartFiguresDict()
        {
            _humanMana.RestartMana(StartMana);
            Figures.Clear();
        }
    }
}

