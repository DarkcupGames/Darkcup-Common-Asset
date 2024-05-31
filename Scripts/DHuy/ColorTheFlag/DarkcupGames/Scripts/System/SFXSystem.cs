using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DarkcupGames
{
    public class SFXSystem : MonoBehaviour
    {
        public static SFXSystem Instance;
        [SerializeField] private AudioSource audioPlayer;
        [SerializeField] private List<AudioClip> audioClips;
        private Dictionary<string, AudioClip> audioLibrary = new Dictionary<string, AudioClip>();
        private List<AudioSource> enabledAudio = new List<AudioSource>();
        private List<AudioSource> disabledAudio = new List<AudioSource>();
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
            Init();
        }
        public void Play(string audioName)
        {
            var clip = audioLibrary[audioName];
            if (disabledAudio.Count != 0)
            {
                var audioSource = disabledAudio[0];
                audioSource.gameObject.SetActive(true);
                audioSource.clip = clip;
                audioSource.time = 0;
                audioSource.Play();
                enabledAudio.Add(audioSource);
                disabledAudio.Remove(audioSource);
                LeanTween.delayedCall(audioSource.clip.length, () =>
                {
                    audioSource.gameObject.SetActive(false);
                    disabledAudio.Add(audioSource);
                    enabledAudio.Remove(audioSource);
                });
            }
            else
            {
                var newAudio = Instantiate(audioPlayer);
                newAudio.clip = clip;
                newAudio.time = 0;
                newAudio.Play();
                enabledAudio.Add(newAudio);
                disabledAudio.Remove(newAudio);
                LeanTween.delayedCall(newAudio.clip.length, () =>
                {
                    newAudio.gameObject.SetActive(false);
                    enabledAudio.Remove(newAudio);
                    disabledAudio.Add(newAudio);
                });
            }
        }
        private void Init()
        {
            for (int i = 0; i < audioClips.Count; i++)
            {
                string key = audioClips[i].name;
                audioLibrary.Add(key, audioClips[i]);
            }
        }
    }
}