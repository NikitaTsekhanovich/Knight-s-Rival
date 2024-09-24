using UnityEngine;

namespace MusicSystem
{
    public class MusicSaver : MonoBehaviour
    {
        private void Start()
        {
            var musicControllers = GameObject.FindGameObjectsWithTag("BackgroundMusic");

            if (musicControllers.Length > 1)
            {
                Destroy(gameObject);
            }

            DontDestroyOnLoad(gameObject);
        }
    }
}

