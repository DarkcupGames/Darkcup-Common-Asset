using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DarkcupGames
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioSystem : MonoBehaviour
    {
        public const int CHANEL_AMOUNT = 15;
        public static AudioSystem Instance;
        private Dictionary<string, AudioClip> clips;
        private AudioSource[] chanels;
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                gameObject.transform.SetParent(null);
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
                return;
            }
            clips = new Dictionary<string, AudioClip>();
            chanels = new AudioSource[CHANEL_AMOUNT];
            for (int i = 0; i < CHANEL_AMOUNT; i++)
            {
                chanels[i] = new GameObject().AddComponent<AudioSource>();
                chanels[i].transform.SetParent(transform);
            }
        }

        public void PlaySound(string path)
        {
            if (!clips.ContainsKey(path))
            {
                AudioClip clip = Resources.Load<AudioClip>(path);
                clips.Add(path, clip);
            }
            PlaySound(clips[path]);
        }

        public void PlaySound(AudioClip clip, float volumn = 1)
        {
            for (int i = 0; i < chanels.Length; i++)
            {
                if (chanels[i].isPlaying == false)
                {
                    chanels[i].PlayOneShot(clip, volumn);
                    return;
                }
            }
        }
    }
}