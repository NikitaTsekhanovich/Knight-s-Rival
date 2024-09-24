using System.Collections.Generic;
using UnityEngine;

namespace GameBoard.Figures.Models
{
    public class Pawn : FigureData
    {
        [SerializeField] private List<Points> _attackPoints = new();

        public List<Points> AttackPoints => _attackPoints;
    }
}

