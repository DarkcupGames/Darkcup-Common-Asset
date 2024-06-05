using UnityEngine;
using UnityEngine.SceneManagement;

namespace DarkcupGames
{
    public class ChangeScene : MonoBehaviour
    {
        public void ChangeToScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }
    }
}