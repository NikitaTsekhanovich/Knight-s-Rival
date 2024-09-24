using GameData;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using MusicSystem;

namespace Menu
{
    public class MenuController : MonoBehaviour
    {
        [SerializeField] private TMP_Text _matchesText;
        [SerializeField] private TMP_Text _winsText;
        [SerializeField] private GameObject _education;
        [SerializeField] private Image _currentMusicImage;
        [SerializeField] private Image _currentEffectsImage;

        private void Start()
        {
            Screen.orientation = ScreenOrientation.LandscapeRight;
            ShowPlayerData();
        }

        private void ShowPlayerData()
        {
            var matchesStat = PlayerPrefs.GetInt($"{SavedData.MatchesKey}");
            var winsStat = PlayerPrefs.GetInt($"{SavedData.WinsKey}");
            _matchesText.text = $"{matchesStat}";
            _winsText.text = $"{winsStat}";
        }

        public void StartGame()
        {
            LoadingScreenController.Instance.ChangeScene("Game");
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

