using System.Collections.Generic;
using GameBoard.Figures;
using UnityEngine;

namespace GameLogic.Abilities
{
    public class Improver : FigureAbility
    {
        [SerializeField] private List<Points> _movePoints = new();
        [SerializeField] private Sprite _imageBlue;
        [SerializeField] private Sprite _imageRed;
        private int _numberSlot;

        public List<Points> MovePoints => _movePoints;
        public Sprite ImageBlue => _imageBlue;
        public Sprite ImageRed => _imageRed;
        public int NumberSlot => _numberSlot;

        public void InitNumberSlot(int numberSlot)
        {
            _numberSlot = numberSlot;
        }
    }
}
