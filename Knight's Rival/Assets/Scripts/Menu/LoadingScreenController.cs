using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;
using TMPro;

namespace Menu 
{
    public class LoadingScreenController : MonoBehaviour
    {
        [SerializeField] private Image _background;
        [SerializeField] private TMP_Text _loadingText;
        [SerializeField] private Image _logo;
        [SerializeField] private GameObject _animationBlock;

        public static LoadingScreenController Instance;

        private void Start() 
        {             
            if (Instance == null) 
            { 
                Instance = this; 
                DontDestroyOnLoad(gameObject);
            } 
            else 
            { 
                Destroy(this);  
            } 
        }

        public void ChangeScene(string nameScene)
        {
           StartAnimationFade(nameScene);
        }

        private void StartAnimationFade(string nameScene)
        {
            _loadingText.DOFade(1f, 1f);
            _logo.DOFade(1f, 1f);

            DOTween.Sequence()
                .Append(_background.DOFade(1f, 1f))
                .AppendInterval(3f)
                .OnComplete(() => LoadScene(nameScene));
            _animationBlock.SetActive(true);
        }

        private void LoadScene(string nameScene)
        {
            SceneManager.LoadScene(nameScene);
            EndAnimationFade();
        }

        private void EndAnimationFade()
        {
            _logo.DOFade(0f, 1f);
            _loadingText.DOFade(0f, 1f);
            _background.DOFade(0f, 1f);
            _animationBlock.SetActive(false);
        }
    
    }
}
