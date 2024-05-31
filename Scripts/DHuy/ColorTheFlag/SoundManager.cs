using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;
    [Header("This is a name of prefab audio source")]
    [SerializeField] private string audioSourceName;
    [SerializeField] private AudioSource audioSource;
        
    private void Awake()
    {
        Instance = this;
    }

    public void PlayClipOneShot(AudioClip clip, float volume = 1f)
    {
        if (clip == null) return;
        AudioSource newAudioSource = SimpleObjectPool.Instance.GetObjectFromPool(audioSource);
        if (newAudioSource == null) return;
        newAudioSource.clip = clip;
        newAudioSource.volume = volume;
        newAudioSource.PlayOneShot(clip);

        newAudioSource.mute = false;
        if (!GameSystem.userdata.dicSetting[SettingKey.Sound])
        {
            newAudioSource.mute = true;
        }
        
        StartCoroutine(IDeactivate(clip.length, newAudioSource.gameObject));
    }
    public void PlayClip(AudioClip clip)
    {
        if (clip == null) return;
        AudioSource audioSource = SimpleObjectPool.Instance.GetObjectFromPool(audioSourceName).GetComponent<AudioSource>();
        if (audioSource == null) return;
        audioSource.clip = clip;
        audioSource.loop = true;
        audioSource.Play();
    }
    public AudioSource GetAudioSource(AudioClip clip)
    {
        if (clip == null) return null;
        AudioSource audioSource = SimpleObjectPool.Instance.GetObjectFromPool(audioSourceName).GetComponent<AudioSource>();
        if (audioSource == null) return null;
        audioSource.clip = clip;
        return audioSource;
    }
    IEnumerator IDeactivate(float length, GameObject obj)
    {
        yield return new WaitForSeconds(length);
        obj.SetActive(false);
    }
}