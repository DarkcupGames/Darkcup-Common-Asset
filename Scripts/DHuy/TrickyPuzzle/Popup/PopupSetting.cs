using DarkcupGames;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupSetting : MonoBehaviour
{
    public Button btnSoundOn;
    public Button btnSoundOff;

    public Button btnMusicOn;
    public Button btnMusicOff;

    public Button btnVibrateOn;
    public Button btnVibrateOff;

    private void Start()
    {
        UpdateDisplay();
    }

    public void UpdateDisplay()
    {
        btnSoundOn.gameObject.SetActive(GameSystem.userdata.playSound);
        btnSoundOff.gameObject.SetActive(GameSystem.userdata.playSound == false);

        btnMusicOn.gameObject.SetActive(GameSystem.userdata.playBGM);
        btnMusicOff.gameObject.SetActive(GameSystem.userdata.playBGM == false);

        btnVibrateOn.gameObject.SetActive(GameSystem.userdata.virate);
        btnVibrateOff.gameObject.SetActive(GameSystem.userdata.virate == false);
    }

    public void SetSound(bool enable)
    {
        GameSystem.userdata.playSound = enable;
        GameSystem.SaveUserDataToLocal();
        UpdateDisplay();
    }

    public void SetMusic(bool enable)
    {
        GameSystem.userdata.playBGM = enable;
        GameSystem.SaveUserDataToLocal();
        AudioSystem.Instance.SetBGM(GameSystem.userdata.playBGM);
        BackgroundSong.Instance.backgroundSong.mute = !GameSystem.userdata.playBGM;
        UpdateDisplay();
    }

    public void SetVibrate(bool enable)
    {
        GameSystem.userdata.virate = enable;
        GameSystem.SaveUserDataToLocal();
        UpdateDisplay();
    }
}