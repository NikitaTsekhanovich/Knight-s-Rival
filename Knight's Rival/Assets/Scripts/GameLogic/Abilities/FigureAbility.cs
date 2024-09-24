using System;
using GameBoard.Figures;
using Players;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GameLogic.Abilities
{
    public class FigureAbility : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        [SerializeField] private FiguresTypes _figureType;
        [SerializeField] private PlayersTypes _playerType;
        [SerializeField] private int _cost;
        [SerializeField] private AudioSource _improveSound;

        private Canvas _gameCanvas;
        private RectTransform _rectTransform;
        private CanvasGroup _canvasGroup;

        public FiguresTypes FigureType => _figureType;
        public PlayersTypes PlayerType => _playerType;
        public int Cost => _cost;
        public AudioSource ImproveSound => _improveSound;

        public static Action<int> OnUseAbility;

        private void Start()
        {
            _rectTransform = GetComponent<RectTransform>();
            _canvasGroup = GetComponent<CanvasGroup>();
            _gameCanvas = GameObject.FindWithTag("GameCanvas").GetComponent<Canvas>();
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            _canvasGroup.blocksRaycasts = false;
        }

        public void OnDrag(PointerEventData eventData)
        {
            _rectTransform.anchoredPosition += eventData.delta / _gameCanvas.scaleFactor;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            transform.localPosition = Vector3.zero;
            _canvasGroup.blocksRaycasts = true;
        }

        public void DestroyAbility()
        {
            OnUseAbility?.Invoke(-_cost);
            Destroy(gameObject);
        }
    }
}

