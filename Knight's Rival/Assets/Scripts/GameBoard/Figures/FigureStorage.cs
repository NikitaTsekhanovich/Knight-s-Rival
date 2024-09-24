using System.Collections.Generic;
using Players;
using UnityEngine;

namespace GameBoard.Figures
{
    public class FigureStorage : MonoBehaviour
    {
        [SerializeField] private List<GameObject> _prefabFigures = new ();

        private static Dictionary<PlayersTypes, Dictionary<FiguresTypes, FigureData>> _figures = new ();
        public static Dictionary<PlayersTypes, Dictionary<FiguresTypes, FigureData>> Figures => _figures;

        private void Awake()
        {
            SetFigures();
        }

        private void SetFigures()
        {
            foreach (var prefabFigure in _prefabFigures)
            {
                var figureData = prefabFigure.GetComponent<FigureData>();
                if (!_figures.ContainsKey(figureData.PlayerType))
                    _figures[figureData.PlayerType] = new Dictionary<FiguresTypes, FigureData>();
                _figures[figureData.PlayerType][figureData.FigureType] = figureData;
            }
        }
    }
}

