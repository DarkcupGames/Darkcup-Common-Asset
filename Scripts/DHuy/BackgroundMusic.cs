using UnityEngine;
using DG.Tweening;

public class BackgroundMusic : MonoBehaviour
{
    public const float FADE_TIME = 3f;

    public static BackgroundMusic Instance;
    [SerializeField] AudioSource audioNormal;
    [SerializeField] AudioSource audioVictory;
    private float normalVolume;
    private float victoryVolume;

    private void Awake()
    {
        Instance = this;
        normalVolume = audioNormal.volume;
        victoryVolume = audioVictory.volume;
        audioNormal.volume = 0;
        audioVictory.volume = 0;
    }

    private void Start()
    {
        CheckSetting();
        PlayNormalMusic();
    }

    public void CheckSetting()
    {
        bool muted = GameSystem.userdata.dicSetting[SettingKey.Sound] == false;
        audioNormal.mute = muted;
        audioVictory.mute = muted;
    }

    public void PlayNormalMusic()
    {
        DOVirtual.Float(audioVictory.volume, 0, FADE_TIME, (float f) =>
        {
            if (audioVictory) audioVictory.volume = f;
        });

        DOVirtual.Float(audioNormal.volume, normalVolume, FADE_TIME, (float f) => {
            if (audioNormal) audioNormal.volume = f;
        });
    }

    public void PlayVictoryMusic()
    {
        audioVictory.Play();
        DOVirtual.Float(audioVictory.volume, victoryVolume, FADE_TIME, (float f) =>
        {
            if (audioVictory) audioVictory.volume = f;
        });

        DOVirtual.Float(audioNormal.volume, 0, FADE_TIME, (float f) => {
            if (audioNormal) audioNormal.volume = f;
        });
    }
}