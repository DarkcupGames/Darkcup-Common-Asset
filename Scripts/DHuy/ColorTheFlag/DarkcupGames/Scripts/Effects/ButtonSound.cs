using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DarkcupGames;

public class ButtonSound : MonoBehaviour
{
    public AudioClip buttonSound;

    bool addedSound = false;

    private void Start() {
        AddButtonSounds();
    }

    private void OnEnable() {
        AddButtonSounds();
    }

    public void AddButtonSounds() {
        if (addedSound) return;
        addedSound = true;

        Button[] buttons = gameObject.GetComponentsInChildren<Button>(true);
        for (int i = 0; i < buttons.Length; i++) {
            buttons[i].onClick.AddListener(PlayButtonSound);
        }
    }

    public void PlayButtonSound() {
        AudioSystem.Instance.PlaySound(buttonSound);
    }
}