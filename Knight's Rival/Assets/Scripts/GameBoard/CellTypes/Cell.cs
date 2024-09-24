using System;
using GameBoard.Figures;
using GameLogic.Abilities;
using Players;
using Players.AI;
using Players.Human;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GameBoard.CellTypes
{
    public class Cell : MonoBehaviour, IDropHandler
    {
        private Points _positionOnIndex;
        private string _positionOnBoard;

        public static Action<Points, FiguresTypes, PlayersTypes, IPlayer> OnSpawnDropFigure;
        public static Action OnImproveFigure;
        public static Action<int> OnRespawnImproverAbility;

        public void SetPosition(int x, int y, string positionOnBoard)
        {
            _positionOnIndex = new Points(x, y);
            _positionOnBoard = positionOnBoard;
        }

        public void OnDrop(PointerEventData eventData)
        {
            eventData.pointerDrag.TryGetComponent<Improver>(out var otherItemImprover);

            if (otherItemImprover != null)
            {
                ImproveFigure(otherItemImprover);
            }
            else
            {
                SpawnFigure(eventData);
            }            
        }

        private void ImproveFigure(Improver otherItemImprover)
        {
            if (HumanController.Instance.CheckMana() - otherItemImprover.Cost < 0)
                return;

            if (HumanController.Instance.Figures.ContainsKey(_positionOnBoard))
            {
                var figureType = HumanController.Instance.Figures[_positionOnBoard].FigureType;

                if (figureType != FiguresTypes.King && 
                    figureType != otherItemImprover.FigureType && 
                    figureType != FiguresTypes.Block)
                {
                    HumanController.Instance.Figures[_positionOnBoard].ImproveFigure(
                        otherItemImprover.ImageBlue,
                        otherItemImprover.FigureType,
                        otherItemImprover.MovePoints);

                    otherItemImprover.ImproveSound.Play();
                        
                    OnImproveFigure?.Invoke();
                    otherItemImprover.DestroyAbility();
                    OnRespawnImproverAbility?.Invoke(otherItemImprover.NumberSlot);
                }
            }
        }

        private void SpawnFigure(PointerEventData eventData)
        {
            if (HumanController.Instance.Figures.ContainsKey(_positionOnBoard) ||
                AIController.Instance.Figures.ContainsKey(_positionOnBoard))
                return;

            var otherItemAbility = eventData.pointerDrag.GetComponent<FigureAbility>();
            
            if (HumanController.Instance.CheckMana() - otherItemAbility.Cost < 0)
                return;

            var otherItemTransform = eventData.pointerDrag.transform;
            var otherItemImage = eventData.pointerDrag.GetComponent<Image>();
            otherItemTransform.SetParent(transform);
            otherItemTransform.localPosition = Vector3.zero;
            otherItemImage.raycastTarget = false;

            otherItemAbility.ImproveSound.Play();

            OnSpawnDropFigure?.Invoke(
                _positionOnIndex, 
                otherItemAbility.FigureType, 
                otherItemAbility.PlayerType,
                HumanController.Instance);
            otherItemAbility.DestroyAbility();
        }
    }
}

