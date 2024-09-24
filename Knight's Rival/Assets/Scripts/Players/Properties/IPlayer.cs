using System.Collections.Generic;
using GameBoard.Figures;
using UnityEngine;

namespace Players
{
    public interface IPlayer
    {
        public Dictionary<string, FigureData> Figures { get; }
        public void AddFigure(string position, FigureData figureData);
        public void ChangePositionOnBoard(string positionFigureOnBoard, string newPositionOnBoard);
        public abstract void CheckEnemyFigure(string positionOnBoard, AudioSource _destroySound);
        public void CalculateMovePosition(FigureData figure);
    }
}

