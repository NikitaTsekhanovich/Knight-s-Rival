using UnityEngine;
using UnityEngine.SceneManagement;
using System;

namespace GameControllers
{
    public class SceneLoader : MonoBehaviour
    {
        public static Action OnCraeteGameBoard;

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name == "Game") LoadGame();
        }

        private void LoadGame()
        {
            OnCraeteGameBoard?.Invoke();
        }
    }
}

