using SO;
using UnityEngine;
namespace Managers
{
    public class AudioManager : MonoBehaviour
    {
        /*
        *  Используем для управления аудио игры.
        */
        public static AudioManager Instance;
        private void Awake()
        {
            if (Instance != null && Instance != this)
                Destroy(gameObject);
            else
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
        }
        [SerializeField] private AudioSource bgAudioSource;

        [SerializeField] private AgeAudioDataSO ageAudioData;

        public void PlayBackgroundAgeMusic()
        {
            Debug.Log("[AudioManager] Starting Background Audio");
            bgAudioSource.clip = ageAudioData.AncientAgeBGAudio[0];
            bgAudioSource.Play();
        }
    }
}