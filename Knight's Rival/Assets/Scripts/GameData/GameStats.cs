using GameLogic;
using UnityEngine;

namespace GameData
{
    public class GameStats : MonoBehaviour
    {
        private static int _matches;
        private static int _wins;

        public static int Matches => _matches;
        public static int Wins => _wins;

        private void Awake()
        {
            _matches = PlayerPrefs.GetInt(SavedData.MatchesKey);
            _wins = PlayerPrefs.GetInt(SavedData.WinsKey);
        }

        private void OnEnable()
        {
            GameStateController.OnSetWin += SetWin;
            GameStateController.OnSetMatch += SetMatch;
        }

        private void OnDisable()
        {
            GameStateController.OnSetWin -= SetWin;
            GameStateController.OnSetMatch -= SetMatch;
        }

        private static void SetMatch()
        {
            PlayerPrefs.SetInt(SavedData.MatchesKey, _matches + 1);
            _matches = PlayerPrefs.GetInt(SavedData.MatchesKey);
        }

        private static void SetWin()
        {
            PlayerPrefs.SetInt(SavedData.WinsKey, _wins + 1);
            _wins = PlayerPrefs.GetInt(SavedData.WinsKey);
        }
    }
}

