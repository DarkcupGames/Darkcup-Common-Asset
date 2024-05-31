using DarkcupGames;
using UnityEngine;

public class QuitGameShowPopup : MonoBehaviour
{
    [SerializeField] private GameObject popupQuitGame;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("Input.GetKeyDown(KeyCode.Escape)");
            ShowHideChangeSceneLogic.ShowPopup(popupQuitGame);
        }
    }

    private void OnApplicationQuit()
    {
        Debug.Log("You quit game");
        ShowHideChangeSceneLogic.ShowPopup(popupQuitGame);
    }
}