using GameBoard.Figures;
using Players.Human;
using UnityEngine;

namespace GameBoard
{
    public class Step : MonoBehaviour
    {
        private Points _currentPositionStep;
        private string _positionFigureOnBoard;

        public void SetStepData(Points currentPositionStep, string positionFigureOnBoard)
        {
            _currentPositionStep = currentPositionStep;
            _positionFigureOnBoard = positionFigureOnBoard;
        }

        public void MoveFigure()
        {
            HumanController.Instance.Figures[_positionFigureOnBoard].Move(
                HumanController.Instance,
                _currentPositionStep,
                _positionFigureOnBoard);
        }
    }
}

