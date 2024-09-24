using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace MusicSystem
{
    public class MusicController : MonoBehaviour
    {
        [SerializeField] private Image _currentMusicImage;
        [SerializeField] private Image _currentEffectsImage;
        [SerializeField] private AudioMixer _mixer;
        [SerializeField] private Sprite _musicOnImage;
        [SerializeField] private Sprite _musicOffImage;
        [SerializeField] private Sprite _effectsOnImage;
        [SerializeField] private Sprite _effectsOffImage;
        private string _musicMixerName = "Music";
        private string _effectsMixerName = "SoundEffects";
        private int _musicIsOn; 
        private int _effectsIsOn;

        private const int VolumeOn = 0;
        private const int VolumeOff = -80;

        public static MusicController Instance;

        private void Start()
        {
            if (Instance == null) 
                Instance = this; 
            else 
                Destroy(this);  

            var musicControllers = GameObject.FindGameObjectsWithTag("MusicController");

            if (musicControllers.Length > 1)
                Destroy(gameObject);

            DontDestroyOnLoad(gameObject);

            _musicIsOn = PlayerPrefs.GetInt("MusicIsOn"); 
            _effectsIsOn = PlayerPrefs.GetInt("EffectsIsOn");
            ChangeVolume(_musicIsOn, _musicMixerName, _currentMusicImage, 
                _musicOnImage, _musicOffImage);
            ChangeVolume(_effectsIsOn, _effectsMixerName, _currentEffectsImage,
                _effectsOnImage, _effectsOffImage);
        }

        public void ChangeMusicState(Image currentMusicImage)
        {
            if (_musicIsOn == 0)
            {
                PlayerPrefs.SetInt("MusicIsOn", 1);
                _musicIsOn = 1;
            }
            else
            {
                PlayerPrefs.SetInt("MusicIsOn", 0);
                _musicIsOn = 0;
            }
            ChangeVolume(_musicIsOn, _musicMixerName, currentMusicImage,
                _musicOnImage, _musicOffImage);
        }

        public void ChangeEffectsState(Image currentEffectsImage)
        {
            if (_effectsIsOn == 0)
            {
                PlayerPrefs.SetInt("EffectsIsOn", 1);
                _effectsIsOn = 1;
            }
            else
            {
                PlayerPrefs.SetInt("EffectsIsOn", 0);
                _effectsIsOn = 0;
            }
            ChangeVolume(_effectsIsOn, _effectsMixerName, currentEffectsImage,
                _effectsOnImage, _effectsOffImage);
        }

        private void ChangeVolume(int isOn, string mixerName, Image currentImage,
            Sprite onImage, Sprite offImage)
        {
            if (isOn == 0)
            {
                _mixer.SetFloat(mixerName, VolumeOn);
                currentImage.sprite = onImage;
            }
            else
            {
                _mixer.SetFloat(mixerName, VolumeOff);
                currentImage.sprite = offImage;
            }
        }
    }
}
