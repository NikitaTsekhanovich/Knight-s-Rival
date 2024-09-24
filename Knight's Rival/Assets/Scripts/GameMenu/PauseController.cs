using System;
using Menu;
using UnityEngine;
using UnityEngine.UI;
using MusicSystem;

namespace GameController
{
    public class PauseController : MonoBehaviour
    {
        [SerializeField] private GameObject _education;
        [SerializeField] private Image _currentMusicImage;
        [SerializeField] private Image _currentEffectsImage;
        [SerializeField] private Sprite _musicOffImage;
        [SerializeField] private Sprite _effectsOffImage;

        public static Action OnRestartQueue;
        public static Action OnRestartPlayersData;
        public static Action OnRestartGameBoard;
        public static Action OnRestartAnimMove;

        private void Start()
        {
            if (PlayerPrefs.GetInt("MusicIsOn") == 1)
                _currentMusicImage.sprite = _musicOffImage;
            if (PlayerPrefs.GetInt("EffectsIsOn") == 1)
                _currentEffectsImage.sprite = _effectsOffImage;
        }

        public void BackToMenu()
        {
            LoadingScreenController.Instance.ChangeScene("Menu");
        }

        public void RestartGame()
        {
            OnRestartAnimMove?.Invoke();
            OnRestartPlayersData?.Invoke();
            OnRestartQueue?.Invoke();
            OnRestartGameBoard?.Invoke();
        }

        public void StartEducation()
        {
            _education.SetActive(true);
        }

        public void ChangeMusic()
        {
            MusicController.Instance.ChangeMusicState(_currentMusicImage);
        }

        public void ChangeEffects()
        {
            MusicController.Instance.ChangeEffectsState(_currentEffectsImage);
        }
    }
}

