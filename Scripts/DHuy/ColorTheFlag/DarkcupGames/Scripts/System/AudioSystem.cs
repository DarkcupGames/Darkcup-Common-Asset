using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DarkcupGames {
    [RequireComponent(typeof(AudioSource))]
    public class AudioSystem : MonoBehaviour {
        public static AudioSystem Instance;

        public const int CHANEL_AMOUNT = 5;
        public const int FADE_BACKGROUND_SONG_TIME = 5;

        public List<string> registeredClips;
        public List<AudioClip> fxSounds;
        public AudioClip buttonSound;
        public float backgroundVolumn = 0.6f;

        private Dictionary<string, AudioClip> clips;
        [SerializeField]private AudioSource[] chanels;
        private AudioSource chanelBgSong;
        private AudioSource chanelBgSong2;

        int backgroundChanel = 0;

        void Awake() {
            if (Instance == null) {
                Instance = this;
                gameObject.transform.SetParent(null);
                DontDestroyOnLoad(gameObject);

            } else {
                Destroy(gameObject);
                return;
            }

            clips = new Dictionary<string, AudioClip>();
            chanels = new AudioSource[CHANEL_AMOUNT];

            for (int i = 0; i < CHANEL_AMOUNT; i++) {
                chanels[i] = new GameObject().AddComponent<AudioSource>();
                chanels[i].transform.SetParent(transform);
            }

            chanelBgSong = new GameObject().AddComponent<AudioSource>();
            chanelBgSong.transform.SetParent(transform);
            chanelBgSong.GetComponent<AudioSource>().loop = true;
            chanelBgSong.volume = backgroundVolumn;

            chanelBgSong2 = new GameObject().AddComponent<AudioSource>();
            chanelBgSong2.transform.SetParent(transform);
            chanelBgSong2.GetComponent<AudioSource>().loop = true;
            chanelBgSong2.volume = backgroundVolumn;

            for (int i = 0; i < registeredClips.Count; i++) {
                AudioClip clip = Resources.Load<AudioClip>(registeredClips[i]);
                clips.Add(registeredClips[i], clip);
            }
        }

        public void PlaySound(string path, int chanel_id = 0) {
            if (GameSystem.userdata.dicSetting[SettingKey.Sound] == false) return;

            if (!clips.ContainsKey(path)) {
                AudioClip clip = Resources.Load<AudioClip>(path);
                clips.Add(path, clip);
            }

            chanels[chanel_id].clip = clips[path];
            chanels[chanel_id].Play();
        }

        public void PlaySound(AudioClip clip, int chanel_id = 0) {
            if (GameSystem.userdata.dicSetting[SettingKey.Sound] == false) return;

            chanels[chanel_id].clip = clip;
            chanels[chanel_id].Play();
        }

        public void PlayRandomFxSound() {
            AudioClip clip = fxSounds[Random.Range(0, fxSounds.Count)];
            PlaySound(clip);
        }

        public void PlayButtonSound() {
            chanels[CHANEL_AMOUNT - 1].clip = buttonSound;
            chanels[CHANEL_AMOUNT - 1].Play();
        }

        public void FadeBackgroundSong(AudioClip clip) {
            backgroundChanel = backgroundChanel == 0 ? 1 : 0;
            AudioSource fadeOut = backgroundChanel == 0 ? chanelBgSong : chanelBgSong2;
            AudioSource fadeIn = backgroundChanel == 1 ? chanelBgSong : chanelBgSong2;

            fadeIn.clip = clip;
            fadeIn.Play();

            LeanTween.value(backgroundVolumn, 0f, FADE_BACKGROUND_SONG_TIME).setOnUpdate((float f) => {
                fadeOut.volume = f;
            }).setOnComplete(() => {
                fadeOut.Stop();
            });

            LeanTween.value(0f, backgroundVolumn, FADE_BACKGROUND_SONG_TIME).setOnUpdate((float f) => {
                fadeIn.volume = f;
            });
        }

        public void SetLooping(bool looping, int chanel_id = 0) {
            chanels[chanel_id].loop = looping;
        }

        public void SetBGM(bool play)
        {
            chanelBgSong.mute = !play;
            chanelBgSong2.mute = !play;
        }

        //public void SetFXSound(bool play)
        //{
        //    for (int i = 0; i < chanels.Length; i++)
        //    {
        //        chanels[i].mute = !play;
        //    }
        //}
    }
}