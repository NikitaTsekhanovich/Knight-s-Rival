using System.Collections.Generic;
using GameBoard;
using GameBoard.CellTypes;
using GameBoard.Figures;
using UnityEngine;

namespace GameLogic.Abilities
{
    public class AbilitiesController : MonoBehaviour
    {
        [SerializeField] private GameObject _pawnAbility;
        [SerializeField] private Transform _pawnAbilityParent;
        [SerializeField] private GameObject _blockAbility;
        [SerializeField] private Transform _blockAbilityParent;
        [SerializeField] private List<GameObject> _improverAbilities = new();
        [SerializeField] private Transform _improverAbilityParent1;
        [SerializeField] private Transform _improverAbilityParent2;
        private static Improver _improver1;
        private static Improver _improver2;

        private void OnEnable()
        {
            GameBoardHandler.OnDropAbility += SpawnAbility;
            Cell.OnRespawnImproverAbility += RespawnImproverAbility;
        }   

        private void OnDisable()
        {
            GameBoardHandler.OnDropAbility -= SpawnAbility;
            Cell.OnRespawnImproverAbility -= RespawnImproverAbility;
        }

        private void Start()
        {
            CreateImproverAbility();
        }

        private void CreateImproverAbility()
        {
            _improver1 = GetInstantiateImproverAbility(_improverAbilityParent1, 0);
            _improver2 = GetInstantiateImproverAbility(_improverAbilityParent2, 1);
        }

        private void RespawnImproverAbility(int numberSlot)
        {
            if (numberSlot == 0)
                _improver1 = GetInstantiateImproverAbility(_improverAbilityParent1, numberSlot);
            else
                _improver2 = GetInstantiateImproverAbility(_improverAbilityParent2, numberSlot);
        }

        private Improver GetInstantiateImproverAbility(Transform _improverAbilityParent, int numberSlot)
        {
            var indexAbility = Random.Range(0, _improverAbilities.Count);
            var improver = Instantiate(_improverAbilities[indexAbility], _improverAbilityParent).GetComponent<Improver>();
            improver.InitNumberSlot(numberSlot);
            return improver;
        }

        private void SpawnAbility(FiguresTypes figureType)
        {
            switch (figureType)
            {
                case FiguresTypes.Pawn:
                    Instantiate(_pawnAbility, _pawnAbilityParent);
                    break;
                case FiguresTypes.Block:
                    Instantiate(_blockAbility, _blockAbilityParent);
                    break;
            }
        }

        public static List<Improver> GetImprovers()
        {
            return new List<Improver> () {_improver1, _improver2};
        }
    }
}

